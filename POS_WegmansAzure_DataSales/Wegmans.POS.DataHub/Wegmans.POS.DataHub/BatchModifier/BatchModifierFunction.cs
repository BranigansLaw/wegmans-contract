using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.BatchModifier.BatchModifierMethods;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.BatchModifier
{
    public class BatchModifierFunction
    {
        private const string AzureStorageConnectionStringSettingName = "POSDataHubAccount";
        private const string BatchModifierQueueName = "batch-modifier";
        private readonly IBatchModifierMethods _batchModifierMethods;
        private readonly IOptions<BatchModifierOptions> _options;

        public BatchModifierFunction(
            IBatchModifierMethods batchModifierMethods,
            IOptions<BatchModifierOptions> options
        )
        {
            _batchModifierMethods = batchModifierMethods ?? throw new ArgumentNullException(nameof(batchModifierMethods));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Step 1: Receive request list of item paths to modify as well as a list of modifiers to run which are added to the queue
        /// </summary>
        /// <returns>204 No Content</returns>
        [FunctionName(nameof(BatchModifierHttpEndpoint))]
        public async Task<IActionResult> BatchModifierHttpEndpoint(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue(queueName: BatchModifierQueueName, Connection = AzureStorageConnectionStringSettingName)] IAsyncCollector<BatchModifierQueueItem> batchModifierQueue,
            ILogger log,
            CancellationToken c)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            BatchModifierHttpRequest data = JsonConvert.DeserializeObject<BatchModifierHttpRequest>(requestBody);

            ISet<string> queuedItems = new HashSet<string>();
            ICollection<BatchModifierQueueItem> itemsToQueue = new List<BatchModifierQueueItem>();
            foreach (Uri item in data.ItemPaths)
            {
                string batchModifierKey = item.ToString();
                if (!queuedItems.Contains(batchModifierKey))
                {
                    itemsToQueue.Add(new BatchModifierQueueItem
                    {
                        ItemPath = item,
                        Plan = new HashSet<ModificationPlan>(data.ModifiersToRun),
                    });
                    queuedItems.Add(batchModifierKey);
                }
            }

            await Parallel.ForEachAsync(
                itemsToQueue,
                new ParallelOptions
                {
                    CancellationToken = c,
                    MaxDegreeOfParallelism = _options.Value.MaxDegreeOfParallelismForQueueing,
                },
                async (itemToQueue, cancellationToken) =>
                {
                    await batchModifierQueue.AddAsync(itemToQueue, cancellationToken).ConfigureAwait(false);
                });

            return new NoContentResult();
        }

        [FunctionName(nameof(RunBatchModifierQueuedItem))]
        public async Task RunBatchModifierQueuedItem(
            [QueueTrigger(BatchModifierQueueName, Connection = AzureStorageConnectionStringSettingName)] BatchModifierQueueItem toProcess,
            IBinder binder,
            ILogger log,
            CancellationToken c)
        {
            BlobAttribute blobAttribute = new(toProcess.ItemPath.ToString(), FileAccess.Read) { Connection = AzureStorageConnectionStringSettingName };
            Transaction transaction = null;
            using (Stream transactionReaderStream = await binder.BindAsync<Stream>(blobAttribute, c))
            {
                using (StreamReader reader = new(transactionReaderStream))
                {
                    string transactionData = await reader.ReadToEndAsync().ConfigureAwait(false);
                    transaction = JsonConvert.DeserializeObject<Transaction>(transactionData);
                }
            }

            if (transaction == null)
            {
                throw new InvalidOperationException("Transaction data is null");
            }

            foreach (ModificationPlan modifier in toProcess.Plan)
            {
                switch (modifier)
                {
                    case ModificationPlan.UpdateQuantity:
                        await _batchModifierMethods.UpdateQuantityAsync(transaction);
                        break;
                    default:
                        throw new NotImplementedException($"Unimplemented modifier plan: {modifier}");
                }
            }

            await binder.WriteTransactionAsync(transaction, c);
        }
    }
}
