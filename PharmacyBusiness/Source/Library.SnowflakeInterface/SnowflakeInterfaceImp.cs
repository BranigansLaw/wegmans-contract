using Library.LibraryUtilities.Extensions;
using Library.LibraryUtilities.SqlFileReader;
using Library.LibraryUtilities.TelemetryWrapper;
using Library.SnowflakeInterface.Exceptions;
using Library.SnowflakeInterface.QueryConfigurations;
using Library.SnowflakeInterface.SnowflakeDbConnectionFactory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Snowflake.Data.Client;
using System.Data.Common;
using System.Net.Sockets;

namespace Library.SnowflakeInterface
{
    public class SnowflakeInterfaceImp : ISnowflakeInterface
    {
        private const string DataWarehouseToken = "{DW}";
        private const string AuditDatabaseToken = "{AUD}";

        private readonly ISnowflakeDbConnectionFactory _snowflakeDbConnectionFactory;
        private readonly ISqlFileReader _sqlFileReader;
        private readonly ITelemetryWrapper _telemetryWrapper;
        private readonly IOptions<SnowflakeConfig> _options;
        private readonly ILogger<SnowflakeInterfaceImp> _logger;

        /// <summary>
        /// Retry logic
        /// </summary>
        private readonly AsyncRetryPolicy RetryPolicy;

        public SnowflakeInterfaceImp(
            ISnowflakeDbConnectionFactory snowflakeDbConnectionFactory,
            ISqlFileReader sqlFileReader,
            ITelemetryWrapper telemetryWrapper,
            IOptions<SnowflakeConfig> options,
            ILogger<SnowflakeInterfaceImp> logger
        )
        {
            _snowflakeDbConnectionFactory = snowflakeDbConnectionFactory ?? throw new ArgumentNullException(nameof(snowflakeDbConnectionFactory));
            _sqlFileReader = sqlFileReader ?? throw new ArgumentNullException(nameof(sqlFileReader));
            _telemetryWrapper = telemetryWrapper ?? throw new ArgumentNullException(nameof(telemetryWrapper));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            RetryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<SocketException>() // Network error
                .WaitAndRetryAsync(
                    retryCount: 10,
                    retryAttempt => TimeSpan.FromSeconds(30 * retryAttempt), // retry after 30s, 60s, ... 5 minutes
                    onRetry: (ex, waitTime) => _logger.LogWarning(ex, $"Retrying in {waitTime.Seconds} seconds")
                );
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> QuerySnowflakeAsync<T>(AbstractQueryConfiguration<T> queryConfiguration, CancellationToken c) where T : class
        {
            ICollection<T> toReturn = [];

            string sql = await _sqlFileReader.GetSqlFileContentsAsync(queryConfiguration.QueryFilePath, c).ConfigureAwait(false);
            _logger.LogDebug($"Read SQL: {sql}");

            _logger.LogDebug("Swapping in configured database");
            sql = sql.Replace(DataWarehouseToken, _options.Value.SnowflakeDWDatabase);
            sql = sql.Replace(AuditDatabaseToken, _options.Value.SnowflakeAUDDatabase);
            _logger.LogDebug($"Converted SQL: {sql}");

            bool success = false;
            DateTimeOffset startTime = DateTimeOffset.UtcNow;
            try
            {
                await RetryPolicy.ExecuteAsync(async () =>
                {
                    toReturn = [];

                    using (SnowflakeDbConnection conn = await _snowflakeDbConnectionFactory.CreateAsync(c).ConfigureAwait(false))
                    {
                        try
                        {
                            _logger.LogDebug("Opening snowflake connection");
                            await conn.OpenAsync(c).ConfigureAwait(false);

                            DbCommand cmd = conn.CreateCommand();
                            cmd.CommandText = sql;
                            queryConfiguration.AddParameters(cmd, l => _logger.LogDebug(l));
                            _logger.LogDebug($"Added parameters: {cmd.Parameters}");
                            DbDataReader reader = await cmd.ExecuteReaderAsync(c).ConfigureAwait(false);
                            success = true;

                            int counter = 0;
                            try
                            {
                                while (await reader.ReadAsync(c).ConfigureAwait(false))
                                {
                                    toReturn.Add(queryConfiguration.MapFromDataReaderToType(reader, l => _logger.LogDebug(l)));
                                    counter++;
                                }
                            }
                            catch (InvalidMappingException e)
                            {
                                throw new RowMapperFailedException(counter, e);
                            }
                        }
                        catch (Exception)
                        {
                            _logger.LogError($"Error running {typeof(T)}");
                            throw;
                        }
                        finally
                        {
                            _logger.LogDebug("Closing snowflake connection");
                            conn.Close();
                        }
                    }
                });
            }
            finally
            {
                _telemetryWrapper.LogSnowflakeTelemetry(sql, startTime, DateTimeOffset.Now - startTime, success);
            }

            return toReturn;
        }
    }
}
