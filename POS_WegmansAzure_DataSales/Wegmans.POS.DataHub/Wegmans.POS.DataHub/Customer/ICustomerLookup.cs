using System.Threading;
using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.Customer
{
    public interface ICustomerLookup
    {
        Task<Transaction> AddCuid(Transaction transaction, CancellationToken cancellationToken);

        string ParseLoyaltyNumber(string loyaltyNumber);
    }
}
