using Library.LibraryUtilities.TelemetryWrapper;
using Library.NetezzaInterface.DataModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Data;
using System.Data.OleDb;
using System.Net.Sockets;

namespace Library.NetezzaInterface
{
    public class NetezzaInterfaceImp : INetezzaInterface
    {
        private readonly IOptions<NetezzaConfig> _config;
        private readonly ITelemetryWrapper _telemetryWrapper;
        private readonly ILogger<NetezzaInterfaceImp> _logger;

        /// <summary>
        /// Retry logic
        /// </summary>
        private readonly AsyncRetryPolicy RetryPolicy;

        public NetezzaInterfaceImp(
            IOptions<NetezzaConfig> config,
            ITelemetryWrapper telemetryWrapper,
            ILogger<NetezzaInterfaceImp> logger
        )
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _telemetryWrapper = telemetryWrapper ?? throw new ArgumentNullException(nameof(telemetryWrapper));
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
        public async Task<IEnumerable<NetSaleRow>> GetNetSalesAsync(DateOnly runFor, CancellationToken cancellationToken)
        {
            OleDbParameter[] queryParams = [new OleDbParameter("@QueryDateNbr", OleDbType.Integer)];
            queryParams[0].Value = Convert.ToUInt32(runFor.AddDays(-1).ToString("yyyyMMdd"));

            // TODO: use ISqlFileReader from LibraryUtilities library (https://wegmans.visualstudio.com/PointOfSale/_workitems/edit/556065)
            DataSet dataSet = await DownloadQueryWithParamsToDataSetAsync($"{_config.Value.ExecutableBasePath}SQL/GetNetSales.sql", queryParams, cancellationToken).ConfigureAwait(false);

            return dataSet.Tables[0].AsEnumerable().Select(row => new NetSaleRow
            {
                StoreNum = row.Field<int?>("StoreNum"),
                DateSold = row.Field<int?>("DateSold"),
                DisplayTime = row.Field<string?>("DisplayTime"),
                TxNum = row.Field<string?>("TxNum"),
                RegNum = row.Field<string?>("RegNum"),
                NetSalesAmt = row.Field<decimal?>("NetSalesAmt"),
                GpAmt = row.Field<decimal?>("GpAmt"),
                Qty = row.Field<decimal?>("Qty"),
                NetCnt = row.Field<decimal?>("NetCnt"),
                ItemNum = row.Field<string?>("ItemNum"),
                ItemDesc = row.Field<string?>("ItemDesc"),
                DeptNum = row.Field<string?>("DeptNum"),
                DeptName = row.Field<string?>("DeptName"),
                PlName = row.Field<string?>("PlName"),
                StoreName = row.Field<string?>("StoreName"),
                RegDesc = row.Field<string?>("RegDesc"),
                CashNum = row.Field<string?>("CashNum"),
                PlCode = row.Field<string?>("PlCode"),
                CouponDescWp = row.Field<string?>("CouponDescWp"),
                CouponDescMfg = row.Field<string?>("CouponDescMfg"),
                RefundAmt = row.Field<decimal?>("RefundAmt"),
                TenderAmt = row.Field<decimal?>("TenderAmt"),
                TenderType = row.Field<string?>("TenderType"),
                TenderTypeDesc = row.Field<string?>("TenderTypeDesc"),
                TxType = row.Field<string?>("TxType"),
                TxTypeDesc = row.Field<string?>("TxTypeDesc"),
                TenderDesc = row.Field<string?>("TenderDesc")
            });
        }

        public Task<DataSet> DownloadQueryWithParamsToDataSetAsync(
           string sqlFile,
           OleDbParameter[] queryParams,
           CancellationToken cancellationToken
        )
        {
            return RetryPolicy.ExecuteAsync(() =>
            {
                DataSet ds = new DataSet();
                sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);

                _logger.LogInformation("Output results from query file [{0}] with query parameters passed in to DataSet.", sqlFile);
                DateTimeOffset queryStartTime = DateTimeOffset.Now;
                DateTimeOffset queryStopTime;
                string selectSql = string.Empty;
                using (var fileReader = new StreamReader(File.OpenRead(sqlFile)))
                {
                    selectSql = fileReader.ReadToEnd();
                }
                if (selectSql == string.Empty)
                    throw new Exception("Cannot find SQL file [" + sqlFile + "].");

                OleDbConnection? sqlsCon = null;
                bool success = false;
                try
                {
                    sqlsCon = new OleDbConnection(_config.Value.OracleDatabaseConnection);
                    sqlsCon.Open();

                    OleDbCommand cmd = new OleDbCommand(selectSql, sqlsCon);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 900;
                    cmd.Parameters.Clear();

                    foreach (OleDbParameter param in queryParams)
                    {
                        cmd.Parameters.Add(param);
                    }

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(ds);
                    queryStopTime = DateTimeOffset.Now;

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    sqlsCon.Close();

                    success = true;
                }
                finally
                {
                    if (null != sqlsCon)
                        sqlsCon.Close();

                    _telemetryWrapper.LogNetezzaTelemetry(selectSql, queryStartTime, DateTimeOffset.Now - queryStartTime, success);
                }

                TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
                double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
                _logger.LogInformation("Query ran for [{0}] seconds, and returned a DataTable with [{1}] rows and [{2}] cols.", tsSecondsForQuery, ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count);

                return Task.FromResult(ds);
            });
        }
    }
}
