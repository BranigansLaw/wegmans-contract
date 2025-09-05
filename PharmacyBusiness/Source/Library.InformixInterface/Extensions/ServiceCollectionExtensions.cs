using Library.InformixInterface.InformixDatabaseConnection;
using Library.InformixInterface.Mapper;
using Microsoft.Extensions.DependencyInjection;

namespace Library.InformixInterface.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IInformixInterface"/> interface to the <see cref="IServiceCollection"/>
        /// </summary>
        public static IServiceCollection AddInformixInterface(this IServiceCollection services, Action<InformixConfig> getConfig)
        {
            services.Configure(getConfig);
            services.AddTransient<IInformixInterface, InformixInterfaceImp>();
            services.AddTransient<IInformixDatabaseConnection, InformixDatabaseConnectionImp>();
            services.AddTransient<IMapper, MapperImp>();

            return services;
        }
    }
}
