using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.BatchModifier.BatchModifierMethods
{
    public interface IBatchModifierMethods
    {
        /// <summary>
        /// Updates <paramref name="transaction"/> items quantity if <see cref="Item.IsWeightItem"/> is true and <see cref="Item.Quantity"/> is greater than 100.
        /// </summary>
        /// <param name="transaction">The transaction to modify</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        Task UpdateQuantityAsync(Transaction transaction);
    }
}
