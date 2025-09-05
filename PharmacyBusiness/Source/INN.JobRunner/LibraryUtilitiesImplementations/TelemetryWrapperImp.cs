using Library.LibraryUtilities.TelemetryWrapper;
using Microsoft.ApplicationInsights;

namespace INN.JobRunner.LibraryUtilitiesImplementations
{
    public class TelemetryWrapperImp : ITelemetryWrapper
    {
        private readonly TelemetryClient _telemetryClient;

        public TelemetryWrapperImp(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
        }

        /// <inheritdoc />
        public void LogTenTenAzureBlobUploadTelemetry(string message, DateTimeOffset start, TimeSpan duration, bool success)
        {
            LogTelemetry("Azure Blob", "Azure Blob Upload", "TenTen", message, start, duration, null, success);
        }

        /// <inheritdoc />
        public void LogInformixTelemetry(string sql, DateTimeOffset start, TimeSpan duration, bool success)
        {
            LogTelemetry("SQL", "Informix", "Informix DB", sql, start, duration, null, success);
        }

        /// <inheritdoc />
        public void LogMcKessonDwTelemetry(string sql, DateTimeOffset start, TimeSpan duration, bool success)
        {
            LogTelemetry("SQL", "McKessonDw", "McKessonDW Oracle DB", sql, start, duration, null, success);
        }

        /// <inheritdoc />
        public void LogNetezzaTelemetry(string sql, DateTimeOffset start, TimeSpan duration, bool success)
        {
            LogTelemetry("SQL", "Netezza", "Netezza DB", sql, start, duration, null, success);
        }

        /// <inheritdoc />
        public void LogSnowflakeTelemetry(string sql, DateTimeOffset start, TimeSpan duration, bool success)
        {
            LogTelemetry("SQL", "Snowflake", "Snowflake Query", sql, start, duration, null, success);
        }

        /// <inheritdoc />
        public void LogTelemetry(string dependencyType, string target, string name, string? detail, DateTimeOffset start, TimeSpan duration, string? resultCode, bool success)
        {
            _telemetryClient.TrackDependency(dependencyType, target, name, detail, start, duration, resultCode, success);
        }
    }
}
