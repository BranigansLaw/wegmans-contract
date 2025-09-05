using Library.LibraryUtilities.SqlFileReader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Data;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace Library.McKessonCPSInterface.McKessonSqlServerInterface
{
    public class McKessonSqlServerInterfaceImp : IMcKessonSqlServerInterface
    {
        private readonly ISqlFileReader _sqlFileReader;
        private readonly IOptions<McKessonCPSConfig> _options;
        private readonly ILogger<McKessonCPSInterfaceImp> _logger;

        /// <summary>
        /// Retry logic
        /// </summary>
        private readonly AsyncRetryPolicy RetryPolicy;

        public McKessonSqlServerInterfaceImp(
            ISqlFileReader sqlFileReader,
            IOptions<McKessonCPSConfig> options,
            ILogger<McKessonCPSInterfaceImp> logger
        )
        {
            _sqlFileReader = sqlFileReader ?? throw new ArgumentNullException(nameof(sqlFileReader));
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
        public async Task<DataSet> RunQueryFileWithParamsToDataSetAsync(
            string sqlFileName, 
            SqlParameter[] queryParams, 
            CancellationToken cancellationToken)
        {
            string sql = await _sqlFileReader.GetSqlFileContentsAsync(
                $"SQL/{sqlFileName}.sql", cancellationToken).ConfigureAwait(false);

            _logger.LogDebug($"SQL file {sqlFileName}.sql successfully loaded");

            return await DownloadQueryWithParamsToDataSetAsync(
                sql,
                queryParams,
                cancellationToken).ConfigureAwait(false);
        }

        public Task<DataSet> DownloadQueryWithParamsToDataSetAsync(
            string sql,
            SqlParameter[] queryParams,
            CancellationToken cancellationToken)
        {
            return RetryPolicy.ExecuteAsync(() =>
            {
                DataSet ds = new DataSet();
                DateTime queryStartTime;
                DateTime queryStopTime;
                SqlConnection? sqlsCon = null;

                try
                {
                    sqlsCon = new SqlConnection(_options.Value.SqlServerDatabaseConnection);
                    sqlsCon.Open();

                    SqlCommand cmd = new SqlCommand(sql, sqlsCon);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();

                    foreach (SqlParameter param in queryParams)
                    {
                        cmd.Parameters.Add(param);
                    }

                    queryStartTime = DateTime.Now;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    queryStopTime = DateTime.Now;

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    sqlsCon.Close();
                }
                finally
                {
                    if (null != sqlsCon)
                        sqlsCon.Close();
                }

                TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
                double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
                _logger.LogInformation("Query ran for [{0}] seconds, and returned a DataTable with [{1}] rows and [{2}] cols.", tsSecondsForQuery, ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count);

                return Task.FromResult(ds);
            });
        }
    }
}
