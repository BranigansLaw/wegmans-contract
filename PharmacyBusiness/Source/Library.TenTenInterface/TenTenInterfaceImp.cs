using Library.LibraryUtilities.EmailSender;
using Library.LibraryUtilities.Extensions;
using Library.LibraryUtilities.GetNow;
using Library.TenTenInterface.DataModel.UploadRow;
using Library.TenTenInterface.DownloadsFromTenTen;
using Library.TenTenInterface.Exceptions;
using Library.TenTenInterface.Helper;
using Library.TenTenInterface.ParquetFileGeneration;
using Library.TenTenInterface.Response;
using Library.TenTenInterface.TenTenApiCallWrapper;
using Library.TenTenInterface.UploadsToTenTen;
using Library.TenTenInterface.XmlTemplateHandlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System.Text;

namespace Library.TenTenInterface
{
    public class TenTenInterfaceImp : ITenTenInterface
    {
        private readonly ITenTenApiCallWrapper _tenTenApiCallWrapper;
        private readonly IGetNow _getNow;
        private readonly IParquetHelper _parquetHelper;
        private readonly IEmailNotificationHelper _emailHelper;
        private readonly IHelper _helper;
        private readonly IOptions<TenTenConfig> _config;
        private readonly ILogger<TenTenInterfaceImp> _logger;

        /// <summary>
        /// Retry logic for HTTP Exceptions
        /// </summary>
        private static readonly AsyncRetryPolicy RowMismatchRetryPolicy = Policy
            .Handle<RowCountMismatchException>()
            .RetryAsync(5);

        public TenTenInterfaceImp(
            IParquetHelper parquetHelper,
            IEmailNotificationHelper emailHelper,
            IHelper helper,
            IOptions<TenTenConfig> config,
            ITenTenApiCallWrapper tenTenApiCallWrapper,
            IGetNow getNow,
            ILogger<TenTenInterfaceImp> logger)
        {
            _parquetHelper = parquetHelper ?? throw new ArgumentNullException(nameof(parquetHelper));
            _emailHelper = emailHelper ?? throw new ArgumentNullException(nameof(emailHelper));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tenTenApiCallWrapper = tenTenApiCallWrapper ?? throw new ArgumentNullException(nameof(tenTenApiCallWrapper));
            _getNow = getNow ?? throw new ArgumentNullException(nameof(getNow));
        }

        /// <summary>
        /// Get row counts for a table in 1010data and for a given Run For Date
        /// </summary>
        /// <param name="tableSpecs"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public async Task<int> RowCountQueryTenTenAsync<T>(
            TenTenEtl<T> etl,
            CancellationToken c) where T : class, new()
        {
            _logger.LogDebug($"Query row count for table name={etl.TenTenTableSpecs.QueryableTableNameAndPath}");

            string xmlDataFor1010 = _helper.GenerateRowCountQueryReturningCsvResults(etl.TenTenTableSpecs.QueryableTableNameAndPath, (etl.TenTenTableSpecs.ColumnNameContainingDataDate ?? string.Empty).Split(","));

            QueryResponse queryResponse = await _tenTenApiCallWrapper.CallTenTenApiAsync<QueryResponse>((sessionDetails, client) =>
                client.PostAsync(
                    requestUri: $"gw.k?api=querydata&apiversion=3&uid={sessionDetails.Username}&sid={sessionDetails.SessionId}&pswd={sessionDetails.Password}{_config.Value.KillSettings}",
                    content: new StringContent(xmlDataFor1010, Encoding.UTF8, "text/xml"),
                    cancellationToken: c),
                c);
            _logger.LogDebug("queryResponse: {0}", JsonConvert.SerializeObject(queryResponse));

            int rowsActuallyUpdate = _helper.GetRowCountFromQueryResponse(queryResponse, etl.RunFor);
            _logger.LogDebug($"rowsActuallyUpdate: {rowsActuallyUpdate}");

            return rowsActuallyUpdate;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetQueryResultsForTransformingToCollectionsAsync<T>(TenTenDataQuery query, CancellationToken c)
        {
            string xmlQueryFor1010 = _helper.GenerateDataExtractQueryReturningDelimitedResultsForTransformingToCollections(
                query.TenTenDownloadQueryXml,
                query.ColumnNames);

            QueryResponse queryResponse = await _tenTenApiCallWrapper.CallTenTenApiAsync<QueryResponse>((sessionDetails, client) =>
                client.PostAsync(
                    requestUri: $"gw.k?api=querydata&apiversion=3&uid={sessionDetails.Username}&sid={sessionDetails.SessionId}&pswd={sessionDetails.Password}",
                    content: new StringContent(xmlQueryFor1010, Encoding.UTF8, "text/xml"),
                    cancellationToken: c),
                c);

            _logger.LogDebug("queryResponse: {0}", JsonConvert.SerializeObject(queryResponse));

            string delimitedQueryResults = queryResponse?.Csv ?? string.Empty;

            return _helper.ConvertStringToCollection<T>(delimitedQueryResults);
        }

        /// <inheritdoc />
        public async Task<string> GetQueryResultsAsCsvAsync(TenTenDataQuery query, CancellationToken c)
        {
            string xmlQueryFor1010 = _helper.GenerateDataExtractQueryReturningCsvResults(
                query.TenTenDownloadQueryXml,
                query.ColumnNames,
                ",");

            QueryResponse queryResponse = await _tenTenApiCallWrapper.CallTenTenApiAsync<QueryResponse>((sessionDetails, client) =>
                client.PostAsync(
                    requestUri: $"gw.k?api=querydata&apiversion=3&uid={sessionDetails.Username}&sid={sessionDetails.SessionId}&pswd={sessionDetails.Password}",
                    content: new StringContent(xmlQueryFor1010, Encoding.UTF8, "text/xml"),
                    cancellationToken: c),
                c);

            _logger.LogDebug("queryResponse: {0}", JsonConvert.SerializeObject(queryResponse));

            return queryResponse?.Csv ?? "(no results were returned)";
        }

        public async Task OutputDataExtractQueryResultsTenTenAsync(
            TenTenDataExtracts dataExtract,
            CancellationToken c
        )
        {
            string xmlDataFor1010 = _helper.GenerateDataExtractQueryReturningCsvResults(
                dataExtract.TenTenDownloadQueryXml,
                dataExtract.ColumnNames,
                dataExtract.OutputFileSpecifications.FieldDelimiter);

            QueryResponse queryResponse = await _tenTenApiCallWrapper.CallTenTenApiAsync<QueryResponse>((sessionDetails, client) =>
                client.PostAsync(
                    requestUri: $"gw.k?api=querydata&apiversion=3&uid={sessionDetails.Username}&sid={sessionDetails.SessionId}&pswd={sessionDetails.Password}{_config.Value.KillSettings}",
                    content: new StringContent(xmlDataFor1010, Encoding.UTF8, "text/xml"),
                    cancellationToken: c),
                c);

            _logger.LogDebug("queryResponse: {0}", JsonConvert.SerializeObject(queryResponse));

            _helper.WriteResultsToFile(
                queryResponse,
                dataExtract.OutputFileSpecifications);
        }

        /// <inheritdoc />
        public Task CreateOrAppendTenTenDataAsync<T>(DateOnly runFor, IEnumerable<T> rows, CancellationToken c) where T : ITenTenUploadConvertible
        {
            if (!rows.Any())
            {
                return Task.CompletedTask;
            }

            return RowMismatchRetryPolicy.ExecuteAsync(async () =>
            {
                // Step 1: Check the rollup directory exists, or create it
                string xmlMakeDir = _helper.GenerateMakeDirectoryXml(_config.Value.TenTenFolderPath, rows.First().FolderNameOfDatedTables, rows.First().FolderTitleOfDatedTables);
                try
                {
                    _logger.LogInformation($"Creating rollup directory '{rows.First().FolderNameOfDatedTables}'");
                    QueryResponse mkdirResponse = await _tenTenApiCallWrapper.CallTenTenApiAsync<QueryResponse>((sessionCreds, client) =>
                        client.PostAsync(
                            requestUri: $"gw.k?api=mkdir&apiversion=3&uid={_config.Value.Username}&sid={sessionCreds.SessionId}&pswd={sessionCreds.Password}{_config.Value.KillSettings}",
                            content: new StringContent(xmlMakeDir, Encoding.UTF8, "text/xml"),
                            cancellationToken: c),
                        c);

                    _logger.LogInformation($"Created rollup directory '{rows.First().FolderNameOfDatedTables}'");
                }
                catch (TenTenDirectoryAlreadyExistsException)
                {
                    _logger.LogInformation($"Directory '{rows.First().FolderNameOfDatedTables}' already exists");
                }

                // Step 2: Define rollup paths
                string xmlBody = await _helper.ReadXmlTemplateAsync(rows.First().BaseTemplatePath).ConfigureAwait(false);
                string folderOfIndividualTablesByDates = $"{_config.Value.TenTenFolderPath}.{rows.First().FolderNameOfDatedTables}";
                string uploadTableFullPath = $"{_config.Value.TenTenFolderPath}.{rows.First().FolderNameOfDatedTables}.d{runFor:yyyyMMdd}";
                string rollupTableFullPath = $"{_config.Value.TenTenFolderPath}.{rows.First().FolderNameOfDatedTables}_rollup";

                // Do not bypass these conditions in your testing (let the config override the path) as this could cause issues with production data being lost permanently.
                if (!string.IsNullOrWhiteSpace(rows.First().TenTenGoLiveRollupTableFullPathOverride) && _config.Value.OverrideTenTenFullTablePath)
                {
                    rollupTableFullPath = rows.First().TenTenGoLiveRollupTableFullPathOverride;
                }

                // Step 3: Generate 5 MB batches for upload
                string modePlaceholder = "%mode%";
                string xmlUploadBody = string.Format(xmlBody, "{0}", modePlaceholder, _config.Value.TenTenFolderPath, runFor, rows.First().FolderNameOfDatedTables);
                IEnumerable<string> xmlUploadBatchedBodies = _helper.CreateXmlBatches(
                    body: xmlUploadBody,
                    data: rows,
                    convertToTableLine: x => x.ToTenTenUploadXml(),
                    maxSizeMb: 5);

                // Step 4: Upload batches to TenTen
                int count = 1;
                _logger.LogDebug("Uploading {0} batches to 1010", xmlUploadBatchedBodies.Count());
                foreach (string xmlDataFor1010 in xmlUploadBatchedBodies)
                {
                    if (c.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    string currXmlBody = xmlDataFor1010;
                    if (count == 1 && rows.First().ReplaceExistingDataOnUpload)
                    {
                        currXmlBody = xmlDataFor1010.Replace(modePlaceholder, "replace");
                    }
                    else
                    {
                        currXmlBody = xmlDataFor1010.Replace(modePlaceholder, "append");
                    }

                    UploadResponse uploadResponse = await _tenTenApiCallWrapper.CallTenTenApiAsync<UploadResponse>((sessionDetails, client) =>
                        client.PostAsync(
                            requestUri: $"gw.k?api=upload&apiversion=3&uid={sessionDetails.Username}&sid={sessionDetails.SessionId}&pswd={sessionDetails.Password}{_config.Value.KillSettings}",
                            content: new StringContent(currXmlBody, Encoding.UTF8, "text/xml"),
                            cancellationToken: c),
                        c);

                    _logger.LogDebug("uploadResponse: {0}", JsonConvert.SerializeObject(uploadResponse));

                    count++;
                }

                // Step 5: Confirm table has the expected number of records
                _logger.LogDebug("Confirming number of rows uploaded to 1010 table {0}", uploadTableFullPath);
                string xmlRowCountQueryFor1010 = _helper.GenerateRowCountQueryReturningCsvResults(
                    uploadTableFullPath, //The date-specific table name.
                    [] // No date-specific columns needed for row count query of dated tables.
                    );

                QueryResponse rowCountResponse = await _tenTenApiCallWrapper.CallTenTenApiAsync<QueryResponse>((sessionCreds, client) =>
                    client.PostAsync(
                        requestUri: $"gw.k?api=querydata&apiversion=3&uid={_config.Value.Username}&sid={sessionCreds.SessionId}&pswd={sessionCreds.Password}{_config.Value.KillSettings}",
                        content: new StringContent(xmlRowCountQueryFor1010, Encoding.UTF8, "text/xml"),
                        cancellationToken: c),
                    c);
                _logger.LogDebug("rowCountResponse: {0}", JsonConvert.SerializeObject(rowCountResponse));

                // Step 5a: If TenTen row count mismatches the passed in rows, throw and Exception and try again
                int queryRowsUploaded = _helper.GetRowCountFromQueryResponse(rowCountResponse, runFor);
                if (queryRowsUploaded == rows.Count())
                {
                    _logger.LogInformation($"Successfully confirmed that {queryRowsUploaded} rows of Store Inventory History were uploaded.");
                }
                else
                {
                    throw new RowCountMismatchException(expected: rows.Count(), actual: queryRowsUploaded);
                }

                // Step 6: Rollup all dated tables into a rollup table
                _logger.LogDebug("Rolling up individual dated tables to one rollup table named {0}", rollupTableFullPath);
                string xmlRollupFor1010 = _helper.GenerateRollupQuery(
                    folderOfIndividualTablesByDates,
                    rollupTableFullPath,
                    rows.First().TenTenRollupTableTitle);
                QueryResponse rollupResponse = await _tenTenApiCallWrapper.CallTenTenApiAsync<QueryResponse>((sessionCreds, client) =>
                    client.PostAsync(
                        requestUri: $"gw.k?api=querydata&apiversion=3&uid={sessionCreds.Username}&sid={sessionCreds.SessionId}&pswd={sessionCreds.Password}{_config.Value.KillSettings}",
                        content: new StringContent(xmlRollupFor1010, Encoding.UTF8, "text/xml"),
                        cancellationToken: c),
                    c);

                // Step 7: Additional post-processing
                _logger.LogDebug($"Additional postprocessing steps: {rows.First().AdditionalPostprocessingSteps.Count()}");
                foreach (string postprocessingStep in rows.First().AdditionalPostprocessingSteps)
                {
                    _logger.LogDebug($"Send the following XML to TenTen API: {postprocessingStep}");
                    await _tenTenApiCallWrapper.CallTenTenApiAsync<QueryResponse>((sessionCreds, client) =>
                        client.PostAsync(
                            requestUri: $"gw.k?api=querydata&apiversion=3&uid={sessionCreds.Username}&sid={sessionCreds.SessionId}&pswd={sessionCreds.Password}{_config.Value.KillSettings}",
                            content: new StringContent(postprocessingStep, Encoding.UTF8, "text/xml"),
                            cancellationToken: c),
                        c);
                }

                _logger.LogDebug("rollupResponse: {0}", JsonConvert.SerializeObject(rollupResponse));
            });
        }

        /// <inheritdoc />
        public async Task UploadDataAsync<T>(IEnumerable<T> toUpload, CancellationToken c, DateOnly? runDate = null) where T : IAzureBlobUploadRow
        {
            if (!toUpload.Any())
            {
                return;
            }

            string feedName = toUpload.First().FeedName;

            DateTimeOffset now = _getNow.GetNowEasternStandardTime();

            if (runDate == null)
            {
                runDate = now.ToDateOnly();
            }

            string fileName = $"{feedName}_{runDate:yyyyMMdd}_{now:yyyyMMdd}_{now:HHmmss}.parquet";

            // Retry policy is defined in ServiceCollectionExtensions
            using (Stream azureBlobWriteStream = await _parquetHelper.GetParquetUploadStreamAsync(feedName, fileName, c).ConfigureAwait(false))
            {
                await _parquetHelper.WriteParquetToStreamAsync(toUpload, azureBlobWriteStream, c).ConfigureAwait(false);
            }

            await _emailHelper.NotifyOfParquetUploadAsync(
                _config.Value.AzureBlobEnvironmentFolderName,
                feedName,
                runDate.Value,
                toUpload.Count(),
                _config.Value.ParquetUploadNotificationEmailTo,
                c).ConfigureAwait(false);
        }
    }
}
