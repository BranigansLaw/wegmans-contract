namespace Library.LibraryUtilities.TelemetryWrapper
{
    public interface ITelemetryWrapper
    {
        void LogTelemetry(string dependencyType, string target, string name, string? detail, DateTimeOffset start, TimeSpan duration, string? resultCode, bool success);

        void LogMcKessonDwTelemetry(string sql, DateTimeOffset start, TimeSpan duration, bool success);

        void LogInformixTelemetry(string sql, DateTimeOffset start, TimeSpan duration, bool success);

        void LogNetezzaTelemetry(string sql, DateTimeOffset start, TimeSpan duration, bool success);

        void LogSnowflakeTelemetry(string sql, DateTimeOffset start, TimeSpan duration, bool success);

        void LogTenTenAzureBlobUploadTelemetry(string message, DateTimeOffset start, TimeSpan duration, bool success);
    }
}
