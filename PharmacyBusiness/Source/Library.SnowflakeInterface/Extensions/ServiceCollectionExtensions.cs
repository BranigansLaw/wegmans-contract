using Library.SnowflakeInterface.SnowflakeDbConnectionFactory;
using Microsoft.Extensions.DependencyInjection;

namespace Library.SnowflakeInterface.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSnowflakeInterface(this IServiceCollection services, Action<SnowflakeConfig> getConfig)
        {
            services.AddTransient<ISnowflakeInterface, SnowflakeInterfaceImp>();
            services.AddTransient<ISnowflakeDbConnectionFactory, SnowflakeDbConnectionFactoryImp>();
            services.Configure(getConfig);

            return services;
        }
    }
}
