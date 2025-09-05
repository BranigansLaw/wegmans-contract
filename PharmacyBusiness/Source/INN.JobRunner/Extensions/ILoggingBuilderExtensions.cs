using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace INN.JobRunner.Extensions
{
    public static class ILoggingBuilderExtensions
    {
        private const string DefaultHostingCategory = "Microsoft.Extensions.Hosting.Internal.Host";

        private const LogLevel DefaultLogLevel = LogLevel.Warning;

        public static ILoggingBuilder HideDefaultHostingLogs(this ILoggingBuilder builder) =>
            builder.AddFilter(DefaultHostingCategory, DefaultLogLevel);

        public static ILoggingBuilder HideDefaultHostingLogsForApplicationInsights(this ILoggingBuilder builder) =>
            builder.AddFilter<ApplicationInsightsLoggerProvider>(DefaultHostingCategory, DefaultLogLevel);
    }
}
