using Library.DataFileInterface.DataFileReader;
using Microsoft.Extensions.DependencyInjection;

namespace Library.DataFileInterface.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IDataFileInterface"/> interface to the <see cref="IServiceCollection"/>
        /// </summary>
        public static IServiceCollection AddDataFileInterface(this IServiceCollection services, Action<DataFileConfig> getConfig)
        {
            services.Configure(getConfig);
            services.AddTransient<IDataFileReader, DataFileReaderImp>();
            services.AddTransient<IDataFileInterface, DataFileInterfaceImp>();

            return services;
        }
    }
}
