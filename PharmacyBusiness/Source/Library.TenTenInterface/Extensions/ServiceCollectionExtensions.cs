using Library.TenTenInterface.Helper;
using Library.TenTenInterface.ParquetFileGeneration;
using Library.TenTenInterface.TenTenApiCallWrapper;
using Library.TenTenInterface.TenTenResponseParser;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Library.TenTenInterface.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="ITenTenInterface"/> interface to the <see cref="IServiceCollection"/>
        /// </summary>
        public static IServiceCollection AddTenTenDataUpload(this IServiceCollection services, Action<TenTenConfig> getConfig, Func<string> getTenTenAzureConnectionString)
        {
            services.Configure(getConfig);

            services.AddTransient<ITenTenInterface, TenTenInterfaceImp>();
            services.AddTransient<ITenTenApiCallWrapper, TenTenApiCallWrapperImp>();
            services.AddTransient<ITenTenResponseParser, TenTenResponseParserImp>();
            services.AddTransient<IParquetHelper, ParquetHelperImp>();

            services.AddTransient<IHelper, HelperImp>();

            services.AddHttpClient<ITenTenApiCallWrapper, TenTenApiCallWrapperImp>((s, c) =>
            {
                c.BaseAddress = new Uri(s.GetRequiredService<IOptions<TenTenConfig>>().Value.Url);
                c.Timeout = TimeSpan.FromSeconds(120);
            });

            services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddBlobServiceClient(getTenTenAzureConnectionString())
                    .WithName(TenTenInterfaceConstants.TenTenAzureBlobServiceClientName)
                    .ConfigureOptions(opt =>
                    {
                        opt.Retry.Mode = Azure.Core.RetryMode.Exponential;
                        opt.Retry.MaxRetries = 6;
                        opt.Retry.MaxDelay = TimeSpan.FromSeconds(2);
                    });
            });

            return services;
        }
    }
}
