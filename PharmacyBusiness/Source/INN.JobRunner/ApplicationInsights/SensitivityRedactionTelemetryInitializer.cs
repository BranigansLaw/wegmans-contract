using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace INN.JobRunner.ApplicationInsights
{
    /// <summary>
    /// Redacts standardized sensitive information from the trace messages.
    /// </summary>
    public class SensitivityRedactionTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry t)
        {
            if (t is TraceTelemetry traceTelemetry)
            {
                traceTelemetry.Message = SensitiveDataRedactionUtilities.RedactSensitiveData(traceTelemetry.Message);

                // If we don't remove this CustomDimension, the telemetry message will still contain the PII in the "OriginalFormat" property.
                traceTelemetry.Properties.Remove("OriginalFormat");
            }

            if (t is DependencyTelemetry dependencyTelemetry)
            {
                if (dependencyTelemetry.Type.Equals("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    dependencyTelemetry.Data = SensitiveDataRedactionUtilities.RedactSensitiveData(dependencyTelemetry.Data);

                    dependencyTelemetry.Properties.Remove("OriginalFormat");
                }
            }
        }
    }
}
