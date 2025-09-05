using Microsoft.Azure.WebJobs;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.Blobs;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.Customer;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.ReprocessTransactions;

public class ReprocessTransactionsSubscriber
{
    private const string Connection = "POSDataHubAccount";
    private const string ReprocessQueueName = "reprocess-transactions";

    private readonly BlobReader _blobReader;
    private readonly ICustomerLookup _customerLookup;

    public ReprocessTransactionsSubscriber(BlobReader blobReader, ICustomerLookup customerLookup)
    {
        _blobReader = blobReader ?? throw new ArgumentNullException(nameof(blobReader));
        _customerLookup = customerLookup ?? throw new ArgumentNullException(nameof(customerLookup));
    }

    [FunctionName(nameof(ReprocessTransactionsSubscriber))]
    public async Task RunAsync([QueueTrigger(ReprocessQueueName, Connection = Connection)] ReprocessTransaction reprocessTransaction,
        IBinder binder,
        CancellationToken cancellationToken)
    {
        var transaction = await _blobReader.ReadAsync(
            reprocessTransaction.Uri.ToString(), POSTransactionV1Json.Default.Transaction, cancellationToken).ConfigureAwait(false);

        // Skip any transactions with no CustomerIdentification section or where the transaction is from the pharmacy, which would already have customer info removed
        if (transaction.CustomerIdentification is null || (transaction.IsPharmacyTransaction ?? false == true))
        {
            return;
        }

        // If both LoyaltyNumber and CustomerNumberAsEntered are blank, no update is needed
        if (string.IsNullOrEmpty(transaction.CustomerIdentification.LoyaltyNumber) && string.IsNullOrEmpty(transaction.CustomerIdentification.CustomerNumberAsEntered))
        {
            return;
        }

        // Add CUID, clear LoyaltyNumber and CustomerNumberAsEntered, set HasBeenRepublished to true
        transaction = await _customerLookup.AddCuid(transaction, cancellationToken).ConfigureAwait(false);
        transaction.CustomerIdentification.CustomerNumberAsEntered = null;
        transaction.HasBeenRepublished = true;

        // Write transaction back to blob storage
        await binder.WriteTransactionAsync(transaction, cancellationToken).ConfigureAwait(false);
    }
}
