using System.Threading;
using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.TransactionSubscriberHelper
{
    /// <summary>
    /// Returns true to set the republish flag otherwise null (so the IsRepublished field doesn't populate in the JSON)
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public interface ITransactionSubscriberHelper
    {
        Task<bool?> DoesBlobExistAsync(Transaction transaction,
            CancellationToken ct);
    }
}
