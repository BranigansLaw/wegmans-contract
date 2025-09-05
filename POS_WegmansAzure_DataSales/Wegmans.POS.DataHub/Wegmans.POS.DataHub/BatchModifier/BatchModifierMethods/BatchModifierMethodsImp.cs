using System;
using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.BatchModifier.BatchModifierMethods
{
    public class BatchModifierMethodsImp : IBatchModifierMethods
    {
        /// <inheritdoc />
        public Task UpdateQuantityAsync(Transaction transaction)
        {
            foreach (Item itemRecord in transaction.Items)
            {
                //only looks at weighted items with a quantity greater than 100 
                if (itemRecord.IsWeightItem == true && itemRecord.Quantity > 100)
                {
                    //if sale price equal to 0.01 then fix transactions between EA fix and penny fix
                    if (itemRecord.SalePrice == 0.01M)
                    {
                        if (itemRecord.SaleQuantity != null)
                        {
                            itemRecord.Quantity = Convert.ToInt32(itemRecord.SaleQuantity);
                        }
                        else
                        {
                            itemRecord.Quantity = 1;
                        }
                    }
                    else
                    {
                        itemRecord.Quantity = Convert.ToInt32(itemRecord.ExtendedPrice / itemRecord.SalePrice);
                    }

                    transaction.HasBeenRepublished = true;
                }
            }

            return Task.CompletedTask;
        }
    }
}
