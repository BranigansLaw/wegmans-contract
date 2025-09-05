using AutoMapper;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub
{
    public class StoreCloseSubscriber
    {
        private readonly IMapper _mapper;

        public StoreCloseSubscriber(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        static async Task<Stream> ReadFixedXmlAsync(Stream stream)
        {
            var data = await BinaryData.FromStreamAsync(stream);
            var xml = data.ToString();
            xml = Regex.Replace(xml, "&(?!(amp|quot|apos|lt|gt);)", "&amp;", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return BinaryData.FromString(xml).ToStream();
        }

        [FunctionName(nameof(StoreCloseSubscriber))]
        public async Task Run([QueueTrigger(TlogSubscriberQueueBindingSettings.RawTlogStoreCloseEventQueueName, Connection = "POSDataHubAccount")]
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
                StoreCloseController localVar = new StoreCloseController(_mapper);
                var outputStoreClose = localVar.GetHubStoreCloseData(doc, storageCreatedEvent.Url);

                // send the data to be uploaded to the JSON hubspace
                await binder.WriteStoreCloseAsync(outputStoreClose, ct).ConfigureAwait(false);
            }
        }
    }
}
