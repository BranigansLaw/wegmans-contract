using Azure.Identity;
using Azure.Storage.Files.DataLake;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Wegmans.Enterprise.Services;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.BatchModifier;
using Wegmans.POS.DataHub.BatchModifier.BatchModifierMethods;
using Wegmans.POS.DataHub.Customer;
using Wegmans.POS.DataHub.ItemData;
using Wegmans.POS.DataHub.PriceData;
using Wegmans.POS.DataHub.ReprocessTransactionsNew;
using Wegmans.POS.DataHub.ReprocessTransactionsNew.ReprocessTransactionNewHelper;
using Wegmans.POS.DataHub.TransactionControllerHelper;
using Wegmans.POS.DataHub.TransactionSubscriberHelper;
using Wegmans.POS.DataHub.Util.EasternStandardTimeGenerator;
using Wegmans.Price.Services;


[assembly: FunctionsStartup(typeof(Wegmans.POS.DataHub.Startup))]
namespace Wegmans.POS.DataHub;

/// <Summary>
/// This class is used for registering services of AutoMapper Profiles
/// with depenpendency injection during runtime.
/// See: https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
/// </Summary>
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
        builder.Services.AddCloudEventsPublisher();

        using var serviceProvider = builder.Services.BuildServiceProvider();
        static string GetConnectionString(IConfiguration configuration) => configuration.GetValue<string>(TlogSubscriberStorageSettings.POSDataHubConnectionStringKey);
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        _ = builder.Services.AddOptions<DsarOptions>().Configure<IConfiguration>((options, configuration) => configuration.GetSection(DsarOptions.Category).Bind(options));
        _ = builder.Services.AddOptions<BatchModifierOptions>().Configure<IConfiguration>((options, configuration) => configuration.GetSection(BatchModifierOptions.Category).Bind(options));
        _ = builder.Services.AddOptions<ProductApiConfig>().Configure<IConfiguration>((options, configuration) => configuration.GetSection(ProductApiConfig.Category).Bind(options));
        _ = builder.Services.AddOptions<PriceApiConfig>().Configure<IConfiguration>((options, configuration) => configuration.GetSection(PriceApiConfig.Category).Bind(options));
        _ = builder.Services.AddOptions<ReprocessTransactionsConfig>().Configure<IConfiguration>((options, configuration) => configuration.GetSection(ReprocessTransactionsConfig.Category).Bind(options));


        _ = builder.Services.AddOptions<CustomerClientOptions>().Configure<IConfiguration>((options, configuration) => configuration.GetSection(CustomerClientOptions.Category).Bind(options));
        _ = builder.Services.AddHttpClient<ICustomerClient, CustomerClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<CustomerClientOptions>>().Value;
            client.BaseAddress = new Uri(options.ServerAddress, UriKind.Absolute);
        })
       .AddApiVersionQueryStringHandler("2022-08-01").AddPolicyHandler(GetRetryPolicy())
       .AddAzureCredentials(credentialFactory: serviceProvider =>
       {
           var options = serviceProvider.GetRequiredService<IOptions<DefaultAzureCredentialOptions>>().Value;
           var credentialOptions = new DefaultAzureCredentialOptions
           {
               ManagedIdentityClientId = options.ManagedIdentityClientId
           };
           return new DefaultAzureCredential(credentialOptions);
       });


        var hostConfiguration = serviceProvider.GetRequiredService<IConfiguration>();
        var productAPISettings = hostConfiguration.GetSection(nameof(ProductApiConfig)).Get<ProductApiConfig>();
        var priceAPISettings = hostConfiguration.GetSection(nameof(PriceApiConfig)).Get<PriceApiConfig>();


        // product API
        _ = builder.Services.AddHttpClient<IProductAPI, ProductAPI>((provider, client) =>
        {
            client.BaseAddress = new Uri(productAPISettings.BaseAddress, UriKind.Absolute);
            client.DefaultRequestHeaders.Add("Product-Subscription-Key", productAPISettings.ApiKey);
        })
       .AddAzureCredentials(credentialFactory: serviceProvider =>
       {
           var options = serviceProvider.GetRequiredService<IOptions<DefaultAzureCredentialOptions>>().Value;
           var credentialOptions = new DefaultAzureCredentialOptions
           {
               ManagedIdentityClientId = options.ManagedIdentityClientId
           };
           return new DefaultAzureCredential(credentialOptions);
       });


        // price API
        _ = builder.Services.AddHttpClient<IPriceAPI, PriceAPI>((provider, client) =>
        {
            client.BaseAddress = new Uri(priceAPISettings.BaseAddress, UriKind.Absolute);
            client.DefaultRequestHeaders.Add("Price-Subscription-Key", productAPISettings.ApiKey);
        })
       .AddAzureCredentials(credentialFactory: serviceProvider =>
       {
           var options = serviceProvider.GetRequiredService<IOptions<DefaultAzureCredentialOptions>>().Value;
           var credentialOptions = new DefaultAzureCredentialOptions
           {
               ManagedIdentityClientId = options.ManagedIdentityClientId
           };
           return new DefaultAzureCredential(credentialOptions);
       });


        _ = builder.Services.AddTransient<ICustomerLookup, CustomerLookup>();
        _ = builder.Services.AddTransient<ItemDataController>();
        _ = builder.Services.AddTransient<TransactionController>();
        _ = builder.Services.AddTransient<ITransactionControllerHelper, TransactionControllerHelperImp>();
        _ = builder.Services.AddTransient<ITransactionSubscriberHelper, TransactionSubscriberHelperImp>();
        _ = builder.Services.AddTransient<IBatchModifierMethods, BatchModifierMethodsImp>();
        _ = builder.Services.AddTransient<IReprocessingTransactionHelper, ReprocessTransactionHelperImp>();
        _ = builder.Services.AddTransient<IEasternStandardTimeGenerator, EasternStandardTimeGeneratorImp>();

        builder.Services.AddDataHubBlobReader(POSTransactionV1Json.Default.Transaction);
        builder.Services.AddAzureClients(builder =>
        {
            builder.AddTableServiceClient(config.GetValue<string>("POSDataHubAccount"));
            builder.AddQueueServiceClient(config.GetValue<string>("POSDataHubAccount"));
        });

        //Add health checks
        builder.Services
            .AddHealthChecks()
            .AddAzureEventGridDeadLetterHealthCheck<IConfiguration>(config.GetValue<string>(nameof(TlogSubscriberStorageSettings.RawTlogDeadletter)),
                                                                     c => GetConnectionString(c),
                                                                     c => c.GetValue<string>(nameof(TlogSubscriberStorageSettings.RawTlogDeadletter)))
            .AddAzureEventGridDeadLetterHealthCheck<IConfiguration>(config.GetValue<string>(nameof(TlogSubscriberStorageSettings.EventPublisherDeadletter)),
                                                                     c => GetConnectionString(c),
                                                                     c => c.GetValue<string>(nameof(TlogSubscriberStorageSettings.EventPublisherDeadletter)))
            .AddAzureBlobStorage(config.GetValue<string>(TlogSubscriberStorageSettings.POSDataHubConnectionStringKey),
                                 name: nameof(TlogSubscriberStorageSettings.Transactions_container))
            .AddAzureBlobStorage(config.GetValue<string>(TlogSubscriberStorageSettings.POSDataHubConnectionStringKey),
                                 name: nameof(TlogSubscriberStorageSettings.RawTlog_container))
            .AddAzureStorageQueueDepthHealthCheck<IConfiguration>(config.GetValue<string>(nameof(TlogSubscriberQueueBindingSettings.RawTlogTransactionsEventQueueName)),
                                                                  c => GetConnectionString(c),
                                                                  c => c.GetValue<string>(nameof(TlogSubscriberQueueBindingSettings.RawTlogTransactionsEventQueueName)),
                                                                  1000)
            .AddAzureStorageQueueDepthHealthCheck<IConfiguration>(config.GetValue<string>(nameof(TlogSubscriberQueueBindingSettings.RawTlogTransactionsPoisonQueueName)),
                                                                  c => GetConnectionString(c),
                                                                  c => c.GetValue<string>(nameof(TlogSubscriberQueueBindingSettings.RawTlogTransactionsPoisonQueueName)),
                                                                  ignoreQueueNotExists: true)
            .AddAzureStorageQueueDepthHealthCheck<IConfiguration>(config.GetValue<string>(nameof(TlogSubscriberQueueBindingSettings.TlogEventSubscriberQueueName)),
                                                                  c => GetConnectionString(c),
                                                                  c => c.GetValue<string>(nameof(TlogSubscriberQueueBindingSettings.TlogEventSubscriberQueueName)),
                                                                  1000)
            .AddAzureStorageQueueDepthHealthCheck<IConfiguration>(config.GetValue<string>(nameof(TlogSubscriberQueueBindingSettings.RawTlogStoreCloseEventQueueName)),
                                                                  c => GetConnectionString(c),
                                                                  c => c.GetValue<string>(nameof(TlogSubscriberQueueBindingSettings.RawTlogStoreCloseEventQueueName)),
                                                                  100)
            .AddAzureStorageQueueDepthHealthCheck<IConfiguration>(config.GetValue<string>(nameof(TlogSubscriberQueueBindingSettings.RawTlogStoreCloseEventQueueName)) + "-poison",
                                                                  c => GetConnectionString(c),
                                                                  c => c.GetValue<string>(nameof(TlogSubscriberQueueBindingSettings.RawTlogStoreCloseEventQueueName)) + "-poison",
                                                                  ignoreQueueNotExists: true)
            .AddApplicationInsightsPublisher(options => options.ApplicationName = "TlogSubscriberHub");

        var healthCheckPublisherHostedService = builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IHostedService) && x.ImplementationType?.Name == "HealthCheckPublisherHostedService");

        if (!(healthCheckPublisherHostedService is null))
        {
            builder.Services.Remove(healthCheckPublisherHostedService);
        }

        builder.Services.AddSingleton(
            c =>
            {
                var config = c.GetRequiredService<IConfiguration>();
                return new DataLakeServiceClient(config.GetValue<string>("POSDataHubAccount"));
            }
        );
    }

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(5, retryAttempt)));
    }
}