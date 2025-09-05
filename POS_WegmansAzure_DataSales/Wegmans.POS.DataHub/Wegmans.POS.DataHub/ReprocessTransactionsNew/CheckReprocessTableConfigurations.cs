using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.POS.DataHub.ReprocessTransactionsNew.ReprocessTransactionNewHelper;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.ReprocessTransactionsNew
{
    public class CheckReprocessTableConfigurations
    {
        private const string ProcessSettingsTableName = "ReprocessSettingsTable";
        private const string TransactionsContainerName = "raw-tlog";
        private const string RawTLogQueueName = "raw-tlog-transactions";

        private readonly TableClient _reprocessSettingsTableClient;
        private readonly QueueClient _rawTlogQueueClient;
        private readonly IReprocessingTransactionHelper _reprocessingTransactionHelper;
        private readonly IOptions<ReprocessTransactionsConfig> _reprocessTransactionOptions;
        private readonly IOptions<DsarOptions> _dsarOptions;

        public CheckReprocessTableConfigurations(
            TableServiceClient tableServiceClient,
            QueueServiceClient queueServiceClient,
            IReprocessingTransactionHelper reprocessingTransactionHelper,
            IOptions<ReprocessTransactionsConfig> options,
            IOptions<DsarOptions> dsarOptions
        )
        {
            _reprocessSettingsTableClient = tableServiceClient.GetTableClient(ProcessSettingsTableName);
            _rawTlogQueueClient = queueServiceClient.GetQueueClient(RawTLogQueueName);
            _reprocessingTransactionHelper = reprocessingTransactionHelper ?? throw new ArgumentNullException(nameof(reprocessingTransactionHelper));
            _reprocessTransactionOptions = options ?? throw new ArgumentNullException(nameof(options));
            _dsarOptions = dsarOptions ?? throw new ArgumentNullException(nameof(dsarOptions));
        }

        /// <summary>
        /// Check the reprocess settings table for any reprocess configurations, and execute it if one is found
        /// </summary>
        [FunctionName(nameof(CheckReprocessTableConfigurations))]
        public async Task RunAsync(
            [TimerTrigger("%ReprocessTransactions:CronSchedule%", RunOnStartup = Constants.RunOnStartup)] TimerInfo myTimer,
            [Queue(queueName: RawTLogQueueName, Connection = "POSDataHubAccount")] IAsyncCollector<string> reprocessQueue,
            ILogger log,
            CancellationToken c
        )
        {
            BlobContainerClient transactionContainerClient = new(_dsarOptions.Value.DsarStorageConnection, TransactionsContainerName);
            AsyncPageable<ReprocessSetting> allRecordsQuery = _reprocessSettingsTableClient.QueryAsync<ReprocessSetting>(maxPerPage: 10);

            int allRecordsQueryCount = await allRecordsQuery.CountAsync(c).ConfigureAwait(false);
            if (allRecordsQueryCount > 1)
            {
                throw new Exception($"Table {ProcessSettingsTableName} should contain no more than 1 record");
            }

            if (allRecordsQueryCount == 0)
            {
                return;
            }

            // Calculate how many records to add to queue
            int currentQueueCount = (await _rawTlogQueueClient.GetPropertiesAsync().ConfigureAwait(false)).Value.ApproximateMessagesCount;
            int numToQueue = _reprocessTransactionOptions.Value.MaxRawTLogQueueSize - currentQueueCount;

            ReprocessSetting config = await allRecordsQuery.SingleAsync(c).ConfigureAwait(false);

            // Get all the URIs from StartDate's date
            DateOnly startDate = DateOnly.Parse(config.StartDate);
            DateOnly endDate = DateOnly.Parse(config.EndDate);

            ReprocessingCursor cursor = new(startDate);
            if (config.Cursor != null)
            {
                cursor = JsonConvert.DeserializeObject<ReprocessingCursor>(config.Cursor);
            }

            ReprocessingSchedule schedule = ReprocessingSchedule.Default;
            if (config.Schedule != null)
            {
                schedule = JsonConvert.DeserializeObject<ReprocessingSchedule>(config.Schedule);
            }
            else
            {
                config.Schedule = JsonConvert.SerializeObject(schedule);
            }
            numToQueue = Math.Min(numToQueue, _reprocessingTransactionHelper.GetMaxTransactionsToQueue(schedule));

            if (startDate < endDate)
            {
                throw new Exception("startDate must occur after endDate");
            }

            List<string> urlsToQueue = new List<string>();
            while (cursor.Date >= endDate && urlsToQueue.Count < numToQueue)
            {
                AsyncPageable<BlobHierarchyItem> blobHierarchyItems =
                    transactionContainerClient.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", cursor.GetPath(), c);

                urlsToQueue.AddRange(await blobHierarchyItems
                    .Where(i => i.Blob != null && i.Blob.Name.Contains("_transaction"))
                    .Select(i => $"{transactionContainerClient.Uri}/{i.Blob.Name}")
                    .ToListAsync(cancellationToken: c));

                cursor.Decrement();
            }

            await Parallel.ForEachAsync(urlsToQueue,
                new ParallelOptions
                {
                    CancellationToken = c,
                    MaxDegreeOfParallelism = _reprocessTransactionOptions.Value.MaxDegreesOfParallelism,
                },
                async (url, c) =>
                {
                    await reprocessQueue.AddAsync(
                        JsonConvert.SerializeObject(new
                        {
                            subject = url,
                            eventType = "Microsoft.Storage.BlobCreated",
                            id = Guid.NewGuid().ToString(),
                            data = new
                            {
                                url
                            },
                            dataVersion = "",
                            metaDataVersion = "1",
                            eventTime = DateTime.Now.ToString(),
                        }),
                    c).ConfigureAwait(false);
                }).ConfigureAwait(false);

            config.Cursor = JsonConvert.SerializeObject(cursor);

            config.TotalQueued += urlsToQueue.Count;
            log.LogInformation("{0} transactions requeued", urlsToQueue.Count);
            await _reprocessSettingsTableClient.UpdateEntityAsync(config, ETag.All, mode: TableUpdateMode.Replace, c);
        }

        /// <summary>
        /// ItemData Table Entity
        /// </summary>
        public class ReprocessSetting : ITableEntity
        {
            public string StartDate { get; set; }

            public string EndDate { get; set; }

            public string Cursor { get; set; }

            public string Schedule { get; set; }

            public long TotalQueued { get; set; }

            public string PartitionKey { get; set; }

            public string RowKey { get; set; }

            public DateTimeOffset? Timestamp { get; set; }

            public ETag ETag { get; set; } = new ETag("*");
        }
    }
}
