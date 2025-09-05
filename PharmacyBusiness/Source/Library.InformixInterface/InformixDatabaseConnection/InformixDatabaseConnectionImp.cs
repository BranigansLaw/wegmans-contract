using Informix.Net.Core;
using Library.LibraryUtilities.TelemetryWrapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Data;
using System.Net.Sockets;

namespace Library.InformixInterface.InformixDatabaseConnection
{
    public class InformixDatabaseConnectionImp : IInformixDatabaseConnection
    {
        private readonly IOptions<InformixConfig> _options;
        private readonly ILogger<InformixDatabaseConnectionImp> _logger;
        private readonly ITelemetryWrapper _telemetryWrapper;

        /// <summary>
        /// Retry logic
        /// </summary>
        private readonly AsyncRetryPolicy RetryPolicy;

        public InformixDatabaseConnectionImp(
            IOptions<InformixConfig> config,
            ILogger<InformixDatabaseConnectionImp> logger,
            ITelemetryWrapper telemetryWrapper
        )
        {
            _options = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetryWrapper = telemetryWrapper ?? throw new ArgumentNullException(nameof(telemetryWrapper));

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
        public async Task<IEnumerable<object[]>> RunQueryAsync(
            string sql,
            IDictionary<string, string> parameters,
            CancellationToken c
        )
        {
            _logger.LogInformation($"Informix Query started:\n{sql}");
            ICollection<object[]> ret = [];

            foreach (string key in parameters.Keys)
            {
                if (sql.Contains(key))
                {
                    sql = sql.Replace(key, parameters[key]);
                }
                else
                {
                    throw new ArgumentException($"Parameter {key} not found in SQL");
                }
            }

            DateTimeOffset startRuntime = DateTimeOffset.Now;
            _logger.LogInformation($"Formatted query:\n{sql}");
            await RetryPolicy.ExecuteAsync(async () =>
            {
                bool success = false;
                ret = [];
                try
                {
                    using (IfxConnection conn = new(_options.Value.ConnectionString))
                    {
                        using (IfxCommand cmd = new(sql, conn))
                        {
                            conn.Open();
                            cmd.CommandType = CommandType.Text;

                            using (IfxDataReader dr = cmd.ExecuteReader())
                            {
                                int rowCount = 0;
                                while (await dr.ReadAsync())
                                {
                                    c.ThrowIfCancellationRequested();

                                    object[] arr = new object[dr.FieldCount];
                                    dr.GetValues(arr);
                                    ret.Add(arr);

                                    rowCount++;
                                }
                            }
                        }
                    }

                    success = true;
                }
                finally
                {
                    _telemetryWrapper.LogInformixTelemetry(sql, startRuntime, DateTimeOffset.Now - startRuntime, success);
                }
            });

            _logger.LogInformation("Informix Connection Async Completed");

            return ret;
        }
    }
}
