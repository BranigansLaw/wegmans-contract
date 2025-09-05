using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace INN.JobRunner.ApplicationInsights
{
    /// <summary>
    /// Application Insights processor that ignores the Azure Vault dependency telemetry related to retrieving the applications secrets.
    /// The number of requests creates a lot of noise in Application Insights and can be ignored.
    /// If disabling this, note the Vault telemetry will not appear under the corresponding Request telemetry because the secrets are retrieved before the application starts running. This is due to the dependency injection running when then Command is created, and THEN the CoconaRunner being executed which contains the Request telemetry using wrapper.
    /// </summary>
    public class ChangeWarningExceptionsToEvents : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; }

        public ChangeWarningExceptionsToEvents(ITelemetryProcessor next)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public void Process(ITelemetry item)
        {
            if (item is ExceptionTelemetry exceptionTelemetry)
            {
                if (exceptionTelemetry.SeverityLevel == SeverityLevel.Warning)
                {
                    item = ToEventTelemetry(exceptionTelemetry);
                }
            }

            Next.Process(item);
        }

        private ITelemetry ToEventTelemetry(ExceptionTelemetry exceptionTelemetry)
        {
            TraceTelemetry eventTelemetry = new($"Handled Exception: {exceptionTelemetry.Message}");
            foreach (var telemetryProperty in exceptionTelemetry.Properties)
            {
                eventTelemetry.Properties.Add(telemetryProperty);
            }

            eventTelemetry.Timestamp = exceptionTelemetry.Timestamp;
            eventTelemetry.Extension = exceptionTelemetry.Extension;
            eventTelemetry.ProactiveSamplingDecision = exceptionTelemetry.ProactiveSamplingDecision;
            eventTelemetry.Sequence = eventTelemetry.Sequence;

            eventTelemetry.Context.InstrumentationKey = exceptionTelemetry.Context.InstrumentationKey;
            eventTelemetry.Context.Flags = exceptionTelemetry.Context.Flags;

            eventTelemetry.Context.Location.Ip = exceptionTelemetry.Context.Location.Ip;
            eventTelemetry.Context.Cloud.RoleInstance = exceptionTelemetry.Context.Cloud.RoleInstance;
            eventTelemetry.Context.Cloud.RoleName = exceptionTelemetry.Context.Cloud.RoleName;
            eventTelemetry.Context.Component.Version = exceptionTelemetry.Context.Component.Version;
            eventTelemetry.Context.Device.Id = exceptionTelemetry.Context.Device.Id;
            eventTelemetry.Context.Device.Model = exceptionTelemetry.Context.Device.Model;
            eventTelemetry.Context.Device.OemName = exceptionTelemetry.Context.Device.OemName;
            eventTelemetry.Context.Device.OperatingSystem = exceptionTelemetry.Context.Device.OperatingSystem;
            eventTelemetry.Context.Device.Type = exceptionTelemetry.Context.Device.Type;
            eventTelemetry.Context.Operation.Id = exceptionTelemetry.Context.Operation.Id;
            eventTelemetry.Context.Operation.CorrelationVector = exceptionTelemetry.Context.Operation.CorrelationVector;
            eventTelemetry.Context.Operation.Name = exceptionTelemetry.Context.Operation.Name;
            eventTelemetry.Context.Operation.ParentId = exceptionTelemetry.Context.Operation.ParentId;
            eventTelemetry.Context.Operation.SyntheticSource = exceptionTelemetry.Context.Operation.SyntheticSource;
            eventTelemetry.Context.Session.Id = exceptionTelemetry.Context.Session.Id;
            eventTelemetry.Context.Session.IsFirst = exceptionTelemetry.Context.Session.IsFirst;
            eventTelemetry.Context.User.Id = exceptionTelemetry.Context.User.Id;
            eventTelemetry.Context.User.AccountId = exceptionTelemetry.Context.User.AccountId;
            eventTelemetry.Context.User.AuthenticatedUserId = exceptionTelemetry.Context.User.AuthenticatedUserId;
            eventTelemetry.Context.User.UserAgent = exceptionTelemetry.Context.User.UserAgent;

            eventTelemetry.Properties.Add("Call Stack", exceptionTelemetry.Exception.StackTrace);

            return eventTelemetry;
        }
    }
}
