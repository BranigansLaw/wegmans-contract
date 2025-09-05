using Library.LibraryUtilities.TelemetryWrapper;
using Microsoft.Extensions.Options;

namespace Library.LibraryUtilities.SqlFileReader
{
    public class SqlFileReaderImp : ISqlFileReader
    {
        private readonly ITelemetryWrapper _telemetryWrapper;
        private readonly IOptions<LibraryUtilitiesConfig> _config;

        public SqlFileReaderImp(
            ITelemetryWrapper telemetryWrapper,
            IOptions<LibraryUtilitiesConfig> config
        )
        {
            _telemetryWrapper = telemetryWrapper ?? throw new ArgumentNullException(nameof(telemetryWrapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <inheritdoc />
        public async Task<string> GetSqlFileContentsAsync(string sqlFilePath, CancellationToken c)
        {
            sqlFilePath = $"{_config.Value.ExecutableRootDirectory}/{sqlFilePath}";
            string sql = string.Empty;

            DateTimeOffset readFileStartTime = DateTimeOffset.Now;
            bool success = false;
            try
            {
                using (StreamReader fileReader = new(File.OpenRead(sqlFilePath)))
                {
                    sql = await fileReader.ReadToEndAsync(c);
                }
                success = true;
            }
            finally
            {
                _telemetryWrapper.LogTelemetry("File", "SQL File", sqlFilePath, null, readFileStartTime, DateTimeOffset.Now - readFileStartTime, null, success);
            }

            if (string.IsNullOrEmpty(sql))
            {
                throw new Exception("Cannot find SQL file [" + sqlFilePath + "].");
            }

            return sql;
        }
    }
}
