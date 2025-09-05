using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.TransactionSubscriberHelper
{
    public class TransactionSubscriberHelperImp  : ITransactionSubscriberHelper
    {
        private const string _transactionContainerName = "transactions";
        private readonly IOptions<DsarOptions> _dsarOptions;

        public TransactionSubscriberHelperImp(IOptions<DsarOptions> dsarOptions)
        {
           _dsarOptions = dsarOptions ?? throw new ArgumentNullException(nameof(dsarOptions));
        }

       
        /// <inheritdoc/>
        public async Task<bool?> DoesBlobExistAsync(Transaction transaction,
            CancellationToken ct)
        {
            var easternEventDateTime = transaction.Timestamp;
            string path = $"v1/{easternEventDateTime:yyyy}/{easternEventDateTime:MM}/{easternEventDateTime:dd}/{easternEventDateTime:HH}/{transaction.StoreNumber}_{transaction.TerminalId}_{transaction.TransactionId}.json";

            BlobContainerClient blobContainerClient = new BlobContainerClient(_dsarOptions.Value.DsarStorageConnection, _transactionContainerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(path);

            return await blobClient.ExistsAsync().ConfigureAwait(false) ? true : null!;
        }
    }
}
