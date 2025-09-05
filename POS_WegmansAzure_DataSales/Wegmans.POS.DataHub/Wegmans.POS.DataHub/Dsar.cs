using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Wegmans.POS.DataHub.Util;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub
{


    public class Dsar
    {
        private const string _jsonUrisContainerName = "json-uris";
        private const string _jsonUrisFileName = "list-of-jsons-to-deidentify.csv";
        private const string _jsonContainerName = "transactions";
        private const string _rawTlogContainerName = "raw-tlog";
        private static string _jsonData = "";
        private static string _tlogData = "";
        private readonly IOptions<DsarOptions> _options;
        private readonly CancellationToken ct;
        private string _fileBeingProcessed;
        private List<string> _jsonUris;

        public Dsar(IOptions<DsarOptions> options)
        {
            _options = options;
        }


        [FunctionName(nameof(Dsar))]
        public async Task Run(
        [BlobTrigger(_jsonUrisContainerName + "/" + _jsonUrisFileName, Connection = TlogSubscriberStorageSettings.POSDataHubConnectionStringKey)] Stream myBlob,
        ILogger log)
        {

            int jsonCount = 0;

            int xmlCount = 0;

            //Getting list of json blobs
            _jsonUris = myBlob.GetJsonUrisFromBlobContent().ToList();

            // Processing each json blob
            foreach (var jsonUri in _jsonUris)
            {

                try
                {

                    _fileBeingProcessed = jsonUri;

                    var jsonBlobContainerClient = new BlobContainerClient(_options.Value.DsarStorageConnection, _jsonContainerName);

                    var jsonBlobClient = jsonBlobContainerClient.GetBlobClient(jsonUri);

                    var jsonContent = await jsonBlobClient.DownloadContentAsync(ct);

                    var jsonDataStream = jsonContent.Value.Content.ToStream();

                    // Read all text from the json blob using a StreamReader
                    using (StreamReader streamReader = new StreamReader(jsonDataStream))
                    {
                        _jsonData = await streamReader.ReadToEndAsync();
                    }

                    var transactionJson = JsonConvert.DeserializeObject<Transaction>(_jsonData);

                    jsonCount++;

                    // Determine which tlog blob generated this json
                    var tlogUriPath = transactionJson.Uri;

                    _fileBeingProcessed = tlogUriPath;

                    var tlogUri = tlogUriPath.GetTlogUriFromBlobContent();

                    var tlogBlobContainerClient = new BlobContainerClient(_options.Value.DsarStorageConnection, _rawTlogContainerName);

                    var tlogBlobClient = tlogBlobContainerClient.GetBlobClient(tlogUri);

                    var tlogContent = await tlogBlobClient.DownloadContentAsync();

                    var tlogDataStream = tlogContent.Value.Content.ToStream();

                    // Read all text from the xml blob using a StreamReader
                    using (StreamReader streamReader = new StreamReader(tlogDataStream))
                    {
                        _tlogData = await streamReader.ReadToEndAsync();
                    }

                    XmlDocument doc = new();

                    doc.LoadXml(_tlogData);

                    XmlDocument editedDoc = new();

                    editedDoc = doc.DeidentifyCustomerIdFromTlog();

                    System.IO.Stream writeStream = new System.IO.MemoryStream();

                    editedDoc.Save(writeStream);

                    writeStream.Position = 0;

                    // Overwrite existing tlog blob with updated xml, so that json blob will be automatically updated as well. 
                    await tlogBlobClient.UploadAsync(writeStream, true, ct);

                    xmlCount++;

                }
                catch (Exception ex)
                {
                    log.LogError($" Blob not exist or reading error:  ==>  {_fileBeingProcessed}");
                    log.LogError(ex, " DSAR Error ");
                }
            }
            log.LogInformation($"{xmlCount} xml files updated from {jsonCount} json files deserialized.");
        }
    }
}