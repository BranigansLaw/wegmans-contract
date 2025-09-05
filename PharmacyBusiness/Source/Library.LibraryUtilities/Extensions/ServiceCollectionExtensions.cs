using Library.LibraryUtilities.DataFileWriter;
using Library.LibraryUtilities.EmailSender;
using Library.LibraryUtilities.GetNow;
using Library.LibraryUtilities.SqlFileReader;
using Library.LibraryUtilities.TelemetryWrapper;
using Microsoft.Extensions.DependencyInjection;

namespace Library.LibraryUtilities.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="ILibraryUtilitiesInterface"/> interface to the <see cref="IServiceCollection"/>
        /// </summary>
        public static IServiceCollection AddLibraryInterface<T>(this IServiceCollection services, Action<LibraryUtilitiesConfig> getConfig) where T : class, ITelemetryWrapper
        {
            services.Configure(getConfig);
            services.AddTransient<ISqlFileReader, SqlFileReaderImp>();
            services.AddTransient<ILibraryUtilitiesInterface, LibraryUtilitiesInterfaceImp>();
            services.AddTransient<ITelemetryWrapper, T>();
            services.AddTransient<IDataFileWriter, DataFileWriterImp>();
            services.AddTransient<ILibraryUtilitiesFileCheckInterface, LibraryUtilitiesFileCheckImp>();
            services.AddTransient<IGetNow, GetNowImp>();
            services.AddTransient<IEmailSenderInterface, EmailSenderInterfaceImp>();
            services.AddTransient<IEmailNotificationHelper, EmailNotificationHelperImp>();

            return services;
        }
    }
}
