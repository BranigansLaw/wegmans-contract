using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;

namespace INN.JobRunner.ApplicationInsights
{
    /// <summary>
    /// TenTen Azure Uploads and Snowflake dependencies are especially noisy to application insights. This processor removes them from the logs
    /// </summary>
    public class RemoveNoisyDependencyTelemetry : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor Next;
        private readonly IOptions<ApplicationInsightsConfig> _options;

        public RemoveNoisyDependencyTelemetry(
            ITelemetryProcessor next,
            IOptions<ApplicationInsightsConfig> options
        )
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Process(ITelemetry item)
        {
            if (item is DependencyTelemetry dependencyTelemetry)
            {
                if (dependencyTelemetry.Target == "1010saieu2pwegmansupload.blob.core.windows.net" && !_options.Value.LogTenTenAzureUploadDependencies)
                {
                    return;
                }

                if (dependencyTelemetry.Name.Contains("BlockBlobClient") && !_options.Value.LogTenTenAzureUploadDependencies)
                {
                    return;
                }

                if (dependencyTelemetry.Target == "hslkegu-gua47883.snowflakecomputing.com" && !_options.Value.LogSnowflakeDependencies)
                {
                    return;
                }
            }

            Next.Process(item);
        }
    }
}
