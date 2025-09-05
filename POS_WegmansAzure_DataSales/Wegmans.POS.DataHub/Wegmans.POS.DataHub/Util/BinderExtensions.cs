using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using System;
using System.IO;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.StoreClose.v1;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.Util
{
    public static class BinderExtensions
    {
        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions 
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <Summary>
        /// Binder Extension - Writes the serialized JSON object to Blob Container
        /// </Summary>
        /// <param name="transaction">  <see cref="Transaction"/> JSON data that needs to be written out</param>
        /// <param name="blobConnection">  <see cref="String"/> Name of HubStorage - Defaults to "POSDataHubAccount"</param>
        public static async Task WriteTransactionAsync(this IBinder binder,
            Transaction transaction, CancellationToken ct, string blobConnection = "POSDataHubAccount")
        {
            var easternEventDateTime = transaction.Timestamp;
            var writer = await binder.BindAsync<BlobClient>(new BlobAttribute(
                $"transactions/v1/{easternEventDateTime:yyyy}/{easternEventDateTime:MM}/{easternEventDateTime:dd}/{easternEventDateTime:HH}/{transaction.StoreNumber}_{transaction.TerminalId}_{transaction.TransactionId}.json", FileAccess.Write)
            { 
                Connection = blobConnection 
            }).ConfigureAwait(false);
            
            await writer.UploadAsync(new BinaryData(JsonSerializer.Serialize(transaction, _serializerOptions))
                ,new BlobUploadOptions() {HttpHeaders = new BlobHttpHeaders() { ContentType = MediaTypeNames.Application.Json }}
                ,ct
            ).ConfigureAwait(false);
        }

        /// <Summary>
        /// Binder Extension - Writes the serialized JSON object to Blob Container
        /// </Summary>
        /// <param name="storeClose">  <see cref="StoreClose"/> JSON data that needs to be written out</param>
        /// <param name="blobConnection">  <see cref="String"/> Name of HubStorage - Defaults to "POSDataHubAccount"</param>
        public static async Task WriteStoreCloseAsync(this IBinder binder,
            StoreClose storeClose, CancellationToken ct, string blobConnection = "POSDataHubAccount")
        {
            var easternEventDateTime = storeClose.Timestamp;
            var writer = await binder.BindAsync<BlobClient>(new BlobAttribute(
                $"store-close-events/v1/{easternEventDateTime:yyyy}/{easternEventDateTime:MM}/{easternEventDateTime:dd}/{storeClose.StoreNumber}_{Guid.NewGuid()}.json", FileAccess.Write)
            {
                Connection = blobConnection
            }).ConfigureAwait(false);

            await writer.UploadAsync(new BinaryData(JsonSerializer.Serialize(storeClose, _serializerOptions))
                , new BlobUploadOptions() { HttpHeaders = new BlobHttpHeaders() { ContentType = MediaTypeNames.Application.Json } }
                , ct
            ).ConfigureAwait(false);
        }
    }
}