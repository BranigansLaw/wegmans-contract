using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.Enterprise.Services;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.Customer;

public class CustomerLookup : ICustomerLookup
{
    private readonly ICustomerClient _customerClient;

    public CustomerLookup(ICustomerClient customerClient)
    {
        _customerClient = customerClient ?? throw new ArgumentNullException(nameof(customerClient)); ;
    }

    public async Task<Transaction> AddCuid(Transaction transaction, CancellationToken cancellationToken)
    {
        if (transaction.CustomerIdentification is not null)
        {
            var loyaltyNumber = ParseLoyaltyNumber(transaction.CustomerIdentification.LoyaltyNumber);

            if (loyaltyNumber.All(char.IsDigit) && loyaltyNumber.Length >= 5 && loyaltyNumber.Length <= 9)
            {
                var customerCollection = await _customerClient.FindCustomersAsync(
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    loyaltyNumber,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (customerCollection is not null &&
                    customerCollection.Values is not null &&
                    customerCollection.Values.Any())
                {
                    transaction.CustomerIdentification.Cuid = customerCollection.Values.First().Id.ToString();
                }
            }

            transaction.CustomerIdentification.LoyaltyNumber = null;
        }

        return transaction;
    }

    public string ParseLoyaltyNumber(string loyaltyNumber)
    {
        if (loyaltyNumber is null)
        {
            return string.Empty;
        }

        return loyaltyNumber.TrimStart('0');
    }
}
