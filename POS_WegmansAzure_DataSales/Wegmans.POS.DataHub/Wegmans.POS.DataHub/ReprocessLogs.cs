using Azure.Messaging.EventGrid;
using Azure.Storage.Files.DataLake;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Wegmans.POS.DataHub
{
    [DataContract(Name = nameof(ReprocessTlogRequest), Namespace = "http://pos.wegmans.com/hub")]
    public class ReprocessTlogRequest
    {
        [DataMember]
        public DateTime Date { get; set; }

    }

    public class ReprocessLogs
    {
        private const string _rawTlogContainerName = "raw-tlog";
        private readonly DataLakeServiceClient _datalakeServiceClient;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static readonly ParallelOptions _parallelOptions = new()
        {
            MaxDegreeOfParallelism = 100
        };

        public ReprocessLogs(DataLakeServiceClient dataLakeServiceClient)
        {
            _datalakeServiceClient = dataLakeServiceClient ?? throw new ArgumentNullException(nameof(dataLakeServiceClient));
        }


        [FunctionName(nameof(ReprocessLogs))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] ReprocessTlogRequest req,
            [Queue(TlogSubscriberQueueBindingSettings.RawTlogTransactionsEventQueueName, Connection = TlogSubscriberStorageSettings.POSDataHubConnectionStringKey)] IAsyncCollector<string> asyncCollector,
            ILogger log,
            CancellationToken ct)
        {
            DataLakeDirectoryClient dirClient = _datalakeServiceClient.GetFileSystemClient(_rawTlogContainerName)
                                                                      .GetDirectoryClient(req.Date.ToString("yyyy/MM/dd"));
            var tLogsProcessedCount = 0;
            Stopwatch watch = Stopwatch.StartNew();
            await Parallel.ForEachAsync(dirClient.GetPathsAsync(cancellationToken: ct), _parallelOptions, async (pathItem, cancellationToken) =>
            {
                WegmansStorageBlobCreatedEventData data = new("PutBlob",
                                                        "BlockBlob",
                                                        pathItem.ContentLength,
                                                        $"{_datalakeServiceClient.Uri}{_rawTlogContainerName}/{pathItem.Name}");
                List<EventGridEvent> eventGridEvent = new()
                {
                    new EventGridEvent(pathItem.Name,
                                                    "Microsoft.Storage.BlobCreated",
                                                    string.Empty,
                                                    BinaryData.FromObjectAsJson(data, _jsonSerializerOptions))
                };
                //We are serializing because passing the EventGridEvent Object was producing the proper Json. 
                //This allows us to control the output better and allowst he queueTrigger processing to work as if this was a BlobCreated Event.
                await asyncCollector.AddAsync(JsonSerializer.Serialize(eventGridEvent, _jsonSerializerOptions), cancellationToken).ConfigureAwait(false);
                tLogsProcessedCount++;
            }).ConfigureAwait(false);

            watch.Stop();
            return new OkObjectResult(new
            {
                tLogsProcessedCount,
                elapsedMilliseconds = watch.ElapsedMilliseconds
            });

        }
    }
    /// <summary>
    /// This exists because MS does not have implementation for serialization of this object. We need it so we can "fake" the event for processing
    /// </summary>
    public record WegmansStorageBlobCreatedEventData(string Api, string BlobType, long? ContentLength, string Url);
}
