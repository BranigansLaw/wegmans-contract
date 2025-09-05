using Library.LibraryUtilities.SqlFileReader;
using Library.LibraryUtilities.TelemetryWrapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using Polly;
using Polly.Retry;
using System.Data;
using System.Net.Sockets;

namespace Library.McKessonDWInterface.McKessonOracleInterface
{
    public class McKessonOracleInterfaceImp : IMcKessonOracleInterface
    {
        private readonly ISqlFileReader _sqlFileReader;
        private readonly IOptions<McKessonDWConfig> _options;
        private readonly ITelemetryWrapper _telemetryWrapper;
        private readonly ILogger<McKessonDWInterfaceImp> _logger;

        /// <summary>
        /// Retry logic
        /// </summary>
        private readonly AsyncRetryPolicy RetryPolicy;

        public McKessonOracleInterfaceImp(
            ISqlFileReader sqlFileReader,
            IOptions<McKessonDWConfig> options,
            ITelemetryWrapper telemetryWrapper,
            ILogger<McKessonDWInterfaceImp> logger
        )
        {
            _sqlFileReader = sqlFileReader ?? throw new ArgumentNullException(nameof(sqlFileReader));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _telemetryWrapper = telemetryWrapper ?? throw new ArgumentNullException(nameof(telemetryWrapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            OracleConfiguration.OracleDataSources.Add(options.Value.TnsName, options.Value.TnsDescriptor);
            OracleConfiguration.SelfTuning = options.Value.SelfTuning;
            OracleConfiguration.BindByName = options.Value.BindByName;
            OracleConfiguration.CommandTimeout = options.Value.CommandTimeout;
            OracleConfiguration.FetchSize = options.Value.FetchSize;
            OracleConfiguration.DisableOOB = options.Value.DisableOOB;

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
        public async Task<DataSet> RunQueryFileWithParamsToDataSetAsync(
            CommandType commandType,
            string sqlFileName,
            OracleParameter[] queryParams,
            CancellationToken cancellationToken
        )
        {
            string sql = await _sqlFileReader.GetSqlFileContentsAsync(
                $"SQL/{sqlFileName}.sql", cancellationToken).ConfigureAwait(false);

            _logger.LogDebug($"SQL file {sqlFileName}.sql successfully loaded");

            return await DownloadQueryWithParamsToDataSetAsync(
                commandType,
                sql,
                queryParams,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<DataSet> RunQueryFileWithLiteralsToDataSetAsync(
            CommandType commandType,
            string sqlFileName,
            Dictionary<string, string> findReplaceLiteralParams,
            CancellationToken cancellationToken
        )
        {
            string sql = await _sqlFileReader.GetSqlFileContentsAsync(
                $"SQL/{sqlFileName}.sql",
                cancellationToken).ConfigureAwait(false);

            foreach (var findReplaceLiteralParam in findReplaceLiteralParams)
            {
                sql = sql.Replace(findReplaceLiteralParam.Key, findReplaceLiteralParam.Value);
            }

            _logger.LogDebug($"SQL file {sqlFileName}.sql successfully loaded");

            return await DownloadQueryWithParamsToDataSetAsync(
                commandType,
                sql,
                [],
                cancellationToken).ConfigureAwait(false);
        }

        public Task<DataSet> DownloadQueryWithParamsToDataSetAsync(
            string sql,
            OracleParameter[] oracleParameters,
            CancellationToken cancellationToken)
        {
            return DownloadQueryWithParamsToDataSetAsync(
                CommandType.Text,
                sql,
                oracleParameters,
                cancellationToken);
        }

        public Task<DataSet> DownloadQueryWithParamsToDataSetAsync(
            CommandType commandType,
            string sql,
            OracleParameter[] queryParams,
            CancellationToken cancellationToken)
        {
            return RetryPolicy.ExecuteAsync(() =>
            {
                DataSet ds = new DataSet();
                DateTimeOffset queryStartTime = DateTimeOffset.Now;
                DateTimeOffset queryStopTime;
                OracleConnection? orclCon = null;
                bool success = false;

                try
                {
                    orclCon = new OracleConnection(_options.Value.OracleDatabaseConnection);
                    orclCon.Open();

                    _logger.LogDebug("Oracle connection opened");

                    OracleCommand orclCmd = orclCon.CreateCommand();
                    orclCmd.CommandType = commandType;
                    orclCmd.CommandText = sql;
                    orclCmd.AddToStatementCache = false;
                    orclCmd.BindByName = true;
                    orclCmd.Parameters.Clear();

                    foreach (OracleParameter param in queryParams)
                    {
                        orclCmd.Parameters.Add(param);
                    }

                    _logger.LogDebug($"{queryParams.Length} params added");

                    OracleDataAdapter da = new OracleDataAdapter(orclCmd);
                    _logger.LogDebug($"Run 'Fill' stating at {DateTime.Now.ToString("MM/dd HH:mm:ss")}");
                    da.Fill(ds);
                    _logger.LogDebug($"Run 'Fill' ending at {DateTime.Now.ToString("MM/dd HH:mm:ss")}");
                    queryStopTime = DateTimeOffset.Now;

                    orclCmd.Parameters.Clear();
                    orclCmd.Dispose();
                    orclCon.Close();

                    success = true;
                }
                finally
                {
                    if (null != orclCon)
                        orclCon.Close();

                    _telemetryWrapper.LogMcKessonDwTelemetry(
                        sql: sql,
                        start: queryStartTime,
                        duration: DateTimeOffset.Now - queryStartTime,
                        success: success);
                }

                TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
                double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
                _logger.LogInformation("Query ran for [{0}] seconds, and returned a DataTable with [{1}] rows and [{2}] cols.", tsSecondsForQuery, ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count);

                return Task.FromResult(ds);
            });
        }
    }
}
