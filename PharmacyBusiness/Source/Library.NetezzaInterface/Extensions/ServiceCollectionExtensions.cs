using Microsoft.Extensions.DependencyInjection;

namespace Library.NetezzaInterface.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="INetezzaInterface"/> interface to the <see cref="IServiceCollection"/>
        /// </summary>
        public static IServiceCollection AddNetezzaInterface(this IServiceCollection services, Action<NetezzaConfig> getConfig)
        {
            services.Configure(getConfig);
            services.AddTransient<INetezzaInterface, NetezzaInterfaceImp>();

            return services;
        }
    }
}
