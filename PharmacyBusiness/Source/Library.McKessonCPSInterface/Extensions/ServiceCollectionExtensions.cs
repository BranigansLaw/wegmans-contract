using Library.McKessonCPSInterface.DataSetMapper;
using Library.McKessonCPSInterface.McKessonSqlServerInterface;
using Microsoft.Extensions.DependencyInjection;

namespace Library.McKessonCPSInterface.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IMcKessonCPSInterface"/> interface to the <see cref="IServiceCollection"/>
        /// </summary>
        public static IServiceCollection AddMcKessonCPSInterface(this IServiceCollection services, Action<McKessonCPSConfig> getConfig)
        {
            services.Configure(getConfig);
            services.AddTransient<IMcKessonCPSInterface, McKessonCPSInterfaceImp>();
            services.AddTransient<IMcKessonSqlServerInterface, McKessonSqlServerInterfaceImp>();
            services.AddTransient<IDataSetMapper, DataSetMapperImp>();

            return services;
        }
    }
}
