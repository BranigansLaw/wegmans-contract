using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;

namespace Wegmans.POS.DataHub
{
    public class TlogProcessDeadLetters
    {
        [FunctionName(nameof(TlogProcessDeadLetters))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest request,
            [Blob("raw-tlog-deadletter", Connection = "POSDataHubAccount")] IEnumerable<BlobClient> deadLetterFiles,
            [Queue(TlogSubscriberQueueBindingSettings.RawTlogTransactionsEventQueueName, Connection = "POSDataHubAccount")] IAsyncCollector<string> rawTlogTransactionsOrderQueue,
            CancellationToken ct,
            ILogger log)
        {
            foreach (var deadLetterfile in deadLetterFiles)
            {
                var fileContents = (await deadLetterfile.DownloadContentAsync().ConfigureAwait(false)).Value.Content.ToString();
                await rawTlogTransactionsOrderQueue.AddAsync(fileContents, ct).ConfigureAwait(false);

                _ = await deadLetterfile.DeleteAsync().ConfigureAwait(false);
            }
            return new OkObjectResult(new            {
                deadLetterFiles = deadLetterFiles.Count(),
            });
        }
    }
}