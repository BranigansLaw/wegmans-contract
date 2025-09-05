using AstuteAttachmentService;
using AstuteCaseService;
using AstutePatientService;
using FluentValidation;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MtomEncoder;
using Polly;
using Polly.Extensions.Http;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wegmans.RX.Cmm.AzureFunctions.Astute;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Configuration;
using Wegmans.RX.Cmm.AzureFunctions.Cmm;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Configuration;

[assembly: FunctionsStartup(typeof(Wegmans.RX.Cmm.AzureFunctions.Startup))]
namespace Wegmans.RX.Cmm.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            using var provider = builder.Services.BuildServiceProvider();
            var settings = provider.GetRequiredService<IConfiguration>();

            builder.Services.AddOptions();
            builder.Services.Configure<AuthorizationOptions>(settings.GetSection(nameof(AuthorizationOptions)));
            builder.Services.Configure<AppCmmClient>(settings.GetSection(nameof(AppCmmClient)));
            builder.Services.Configure<AppAstuteClientSecurity>(settings.GetSection(nameof(AppAstuteClientSecurity)));

            var astuteClientSettings = settings.GetSection(nameof(AppAstuteClient)).Get<AppAstuteClient>();

            builder.Services.AddSingleton<ICaseService>(new CaseServiceClient(this.GetMtomCustomBinding(), new EndpointAddress(astuteClientSettings.CaseUrl)));
            builder.Services.AddSingleton<IAddressService>(new AddressServiceClient(AddressServiceClient.EndpointConfiguration.WSHttpBinding_IAddressService, astuteClientSettings.AddressUrl));
            builder.Services.AddSingleton<ICaseStreamService>(new CaseStreamServiceClient(this.GetMtomCustomBinding(), new EndpointAddress(astuteClientSettings.CaseStreamUrl)));
            builder.Services.AddTransient(typeof(IAstuteSoapProxy), typeof(AstuteSoapProxy));

            builder.Services.AddHttpClient<CmmHttpClient>()
                .AddPolicyHandler((services, request) => GetRetryPolicy(services, request));

            var configDescriptor = builder.Services.SingleOrDefault(tc => tc.ServiceType == typeof(TelemetryConfiguration));
            if (configDescriptor?.ImplementationFactory == null)
                return;

            var implFactory = configDescriptor.ImplementationFactory;

            builder.Services.Remove(configDescriptor);
            builder.Services.AddSingleton(provider =>
            {
                if (!(implFactory.Invoke(provider) is TelemetryConfiguration config))
                    return null;

                config.TelemetryProcessorChainBuilder.Use(next => new CustomTelemetryProcessor(next));
                config.TelemetryProcessorChainBuilder.Build();

                return config;
            });

            ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
            {
                if (member != null)
                {
                    JsonPropertyNameAttribute jsonPropertyName = member.GetCustomAttribute<JsonPropertyNameAttribute>();

                    if (!(jsonPropertyName is null))
                    {
                        return jsonPropertyName.Name;
                    }

                    return member.Name;
                }
                return null;
            };
        }

        private CustomBinding GetMtomCustomBinding()
        {
            var enc = new TextMessageEncodingBindingElement
            {
                MessageVersion = MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap11, AddressingVersion.None),
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                {
                    MaxBytesPerRead = 20000000
                }
            };
            var encoding = new MtomMessageEncoderBindingElement(enc);
            var transport = new HttpsTransportBindingElement
            {
                MaxReceivedMessageSize = 20000000,
                MaxBufferSize = 20000000
            };
            return new CustomBinding(encoding, transport);
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider services, HttpRequestMessage request)
        {
            Random jitterer = new Random();
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(response => ShouldRetry404(services, request, response))
                .WaitAndRetryAsync(6,    // exponential back-off plus some jitter
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                  + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                );
        }

        static bool ShouldRetry404(IServiceProvider services, HttpRequestMessage request, HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);
                if (dict.ContainsKey("errors"))
                {
                    Dictionary<string, object> logProperties = new Dictionary<string, object>
                    {
                        { "RequestBody", request.Content.ReadAsStringAsync().Result},
                        { "ResponseBody",  response.Content.ReadAsStringAsync().Result}
                    };
                    ILogger logger = services.GetService<ILogger<CmmHttpClient>>();
                    using (logger.BeginScope(logProperties))
                    {
                        logger.LogWarning("404 Error with errors in body calling CMM");
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}
