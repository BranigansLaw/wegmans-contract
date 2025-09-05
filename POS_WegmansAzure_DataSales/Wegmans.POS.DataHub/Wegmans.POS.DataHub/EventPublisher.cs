using System;
using System.IO;
using System.Text.Json;
using System.Net.Mime;
using System.Threading.Tasks;

using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;

using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.EnterpriseLibrary.Events;
using Wegmans.EnterpriseLibrary.Events.Models.POS;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.StoreClose.v1;
using System.Threading;

namespace Wegmans.POS.DataHub
{
    public class PublishTransactionV1BlobCreatedEvent
    {
        private static JsonSerializerOptions _options = new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public const string QueueName = TlogSubscriberQueueBindingSettings.TlogEventSubscriberQueueName;
        public const string Connection = "POSDataHubAccount";
        private readonly EventPublisher _publisher;
        public PublishTransactionV1BlobCreatedEvent(EventPublisher publisher)
            => _publisher = publisher;
        [FunctionName(nameof(PublishTransactionV1BlobCreatedEvent))]
        public async Task Run([QueueTrigger(QueueName, Connection = Connection)] string input, IBinder binder, CancellationToken cancellationToken)
        {
            var json = BinaryData.FromString(input);
            var events = EventGridEvent.ParseMany(json);
            foreach (var eventGridEvent in events)
            {
                if (!(eventGridEvent.TryGetSystemEventData(out var systemEvent) && systemEvent is StorageBlobCreatedEventData blobCreated))
                {
                    throw new NotSupportedException($"The event type '{eventGridEvent.EventType}' is not supported.");
                }
                var storageCreatedEvent = eventGridEvent.Data.ToObjectFromJson<StorageBlobCreatedEventData>();
                var blobAttribute = new BlobAttribute(storageCreatedEvent.Url, FileAccess.Read) { Connection = "POSDataHubAccount" };
                using var tlogStream = binder.Bind<Stream>(blobAttribute);


                var blobUriBuilder = new BlobUriBuilder(new Uri(storageCreatedEvent.Url));

                var blobName = blobUriBuilder.BlobName;

                var containerName = blobUriBuilder.BlobContainerName;

                IEvent data = containerName switch
                {
                    ContainerNames.TransactionContainer when blobName.StartsWith("v1/", StringComparison.InvariantCultureIgnoreCase) => await ConvertToTransactionV1BlobCreated(tlogStream, storageCreatedEvent.Url).ConfigureAwait(false),
                    ContainerNames.StoreCloseContainer when blobName.StartsWith("v1/", StringComparison.InvariantCultureIgnoreCase) => await ConvertToStoreCloseV1BlobCreated(tlogStream, storageCreatedEvent.Url).ConfigureAwait(false),
                    _ => default
                };
                if (data is not null)
                {
                    var subject = $"/data/{blobUriBuilder.BlobContainerName}/{blobUriBuilder.BlobName}";
                    await _publisher.SendEventAsync(subject, data, cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task<IEvent> ConvertToStoreCloseV1BlobCreated(Stream tlogStream, string source)
        {
            var incomingStoreClose = await JsonSerializer.DeserializeAsync<StoreClose>(tlogStream, _options).ConfigureAwait(false);
            return new StoreClosedV1BlobCreated
            {
                Source = source
            };
        }

        private async Task<IEvent> ConvertToTransactionV1BlobCreated(Stream tlogStream, string source) 
        {
            var incomingTransaction = await JsonSerializer.DeserializeAsync<Transaction>(tlogStream, _options).ConfigureAwait(false);
            return new TransactionV1BlobCreated
            {
                Source = source,
                IsPharmacy = incomingTransaction.IsPharmacyTransaction ?? false,
                IsInstacart = incomingTransaction.IsInstacartTransaction ?? false,
                IsInstacartBypass = incomingTransaction.IsInstacartBypassTransaction ?? false,
                IsMeals2Go = incomingTransaction.IsMeals2GoTransaction ?? false,
                IsShopic = incomingTransaction.IsShopicTransaction ?? false,
                IsAmazonDashCart = incomingTransaction.IsAmazonDashCartTransaction ?? false,
                IsAceTransaction = incomingTransaction.IsAceTransaction ?? false,
                IsRepublished = incomingTransaction.HasBeenRepublished ?? false,
                IsGiftCardSold = incomingTransaction.IsGiftCardSoldTransaction ?? false,
                IsFsa = incomingTransaction.IsFsaTransaction ?? false,
                Timestamp = incomingTransaction.Timestamp.ToUniversalTime()
            };
        }
    }

    public static class ContainerNames
    {
        public const string TransactionContainer = "transactions";
        public const string ExceptionContainer = "exceptions";
        public const string StoreCloseContainer = "store-close-events";
    }
}