using Library.McKessonDWInterface.DataSetMapper;
using Library.McKessonDWInterface.Helper;
using Library.McKessonDWInterface.McKessonOracleInterface;
using Microsoft.Extensions.DependencyInjection;

namespace Library.McKessonDWInterface.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IMcKessonDWInterface"/> interface to the <see cref="IServiceCollection"/>
        /// </summary>
        public static IServiceCollection AddMcKessonDWInterface(this IServiceCollection services, Action<McKessonDWConfig> getConfig)
        {
            services.Configure(getConfig);
            services.AddTransient<IMcKessonDWInterface, McKessonDWInterfaceImp>();
            services.AddTransient<IMcKessonOracleInterface, McKessonOracleInterfaceImp>();
            services.AddTransient<IDataSetMapper, DataSetMapperImp>();
            services.AddTransient<ITurnaroundTimeHelper, TurnaroundTimeHelperImp>();
            services.AddTransient<ISureScriptsHelper, SureScriptsHelperImp>();
            services.AddTransient<IAstuteAdherenceDispenseHelper, AstuteAdherenceDispenseHelperImp>();

            return services;
        }
    }
}
