using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wegmans.POS.DataHub.Util;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Azure.Storage.Blobs;
using Wegmans.POS.DataHub.TransactionSubscriberHelper;
using Microsoft.Extensions.Options;

namespace Wegmans.POS.DataHub;

public class TransactionSubscriber
{
    private readonly TransactionController _transactionController;
    private readonly ITransactionSubscriberHelper _transactionSubscriberHelper;

    public TransactionSubscriber(TransactionController transactionController,
        ITransactionSubscriberHelper transactionSubscriberHelper,
        IOptions<TransactionSubscriber> options)
    {
        _transactionController = transactionController ?? throw new ArgumentNullException(nameof(transactionController));
        _transactionSubscriberHelper = transactionSubscriberHelper ?? throw new ArgumentNullException(nameof(transactionSubscriberHelper));
    }

    static async Task<Stream> ReadFixedXmlAsync(Stream stream)
    {
        var data = await BinaryData.FromStreamAsync(stream);
        var xml = data.ToString();
        xml = Regex.Replace(xml, "&(?!(amp|quot|apos|lt|gt);)", "&amp;", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return BinaryData.FromString(xml).ToStream();
    }

    [FunctionName(nameof(TransactionSubscriber))]
    public async Task Run([QueueTrigger(TlogSubscriberQueueBindingSettings.RawTlogTransactionsEventQueueName, Connection = "POSDataHubAccount")]
                            string json,
                            IBinder binder,
                            ILogger log,
                            CancellationToken ct)
    {
        var eventGridEventCollection = EventGridEvent.ParseMany(new BinaryData(json));
        // go thru the collection - collection is possible with deadletters
        foreach (var eventGridEvent in eventGridEventCollection)
        {
            var storageCreatedEvent = eventGridEvent.Data.ToObjectFromJson<StorageBlobCreatedEventData>();
            var blobAttribute = new BlobAttribute(storageCreatedEvent.Url, FileAccess.Read) { Connection = "POSDataHubAccount" };
            using var tlogXMLStream = binder.Bind<Stream>(blobAttribute);

            var fixXMLFormat = await ReadFixedXmlAsync(tlogXMLStream);

            var doc = await XDocument.LoadAsync(fixXMLFormat, LoadOptions.None, ct).ConfigureAwait(false);

            // Get the Mapped transaction data
            var outputTransaction = await _transactionController.GetHubTransactionDataAsync(doc, storageCreatedEvent.Url, ct).ConfigureAwait(false);

            outputTransaction.Total = outputTransaction.TenderExchanges?.Sum(x => x.TenderAmount ?? 0) ?? 0;
            outputTransaction.Total -= outputTransaction.TenderVoids?.Sum(x => x.TenderAmount ?? 0) ?? 0;

            // send sales related data to be uploaded to the JSON hubspace (not sign off transactions)
            if (outputTransaction.TransactionType != TransactionType.OperatorSignOff)
            {
                outputTransaction.HasBeenRepublished = await _transactionSubscriberHelper.DoesBlobExistAsync(outputTransaction, ct);
                await binder.WriteTransactionAsync(outputTransaction, ct).ConfigureAwait(false);
            }
        }
    }
}
