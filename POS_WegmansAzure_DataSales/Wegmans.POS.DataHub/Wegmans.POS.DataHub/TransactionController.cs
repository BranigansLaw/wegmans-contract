using AutoMapper.Configuration.Conventions;
using Microsoft.Azure.Cosmos;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.Customer;
using Wegmans.POS.DataHub.TransactionControllerHelper;
using Wegmans.POS.DataHub.Util;
using Wegmans.POS.DataHub.PriceData;
using System.Numerics;
using Wegmans.EnterpriseLibrary.Events;


namespace Wegmans.POS.DataHub;

public class TransactionController
{
    private readonly ITransactionControllerHelper _transactionControllerHelper;
    private readonly ICustomerLookup _customerLookup;

    public TransactionController(
        ITransactionControllerHelper transactionControllerHelper,
        ICustomerLookup customerLookup)
    {
        _transactionControllerHelper = transactionControllerHelper ?? throw new ArgumentNullException(nameof(transactionControllerHelper));
        _customerLookup = customerLookup ?? throw new ArgumentNullException(nameof(customerLookup));
    }

    /// <Summary>
    ///     creates HubTransaction record from XDocument
    /// </Summary>
    /// <param name="doc">  <see cref="XDocument"/> ACE data serialized as XDocument</param>
    /// <param name="storageCreatedEventUrl">  <see cref="string"/> EventGrid Data Url</param>
    public async Task<Transaction> GetHubTransactionDataAsync(XDocument doc, string storageCreatedEventUrl, CancellationToken cancellationToken)
    {
        //get the name of the XML file from eventGridEvent
        string fileName = Path.GetFileNameWithoutExtension(storageCreatedEventUrl);

        var outputTransaction = new Transaction
        {
            Uri = storageCreatedEventUrl,
            StoreNumber = fileName.getStoreNumber()
        };

        var transactionRecords = doc.Root.Elements();
        int storeNumber = Convert.ToInt32(outputTransaction.StoreNumber);



        foreach (var record in transactionRecords)
        {
            _transactionControllerHelper.UpdateTransactionData(record, outputTransaction);
        }

        if (outputTransaction.Items is not null)
        {
            //The following is business logic that is trying to replicate what had been done in EDW to handle Type 2 UPCs
            foreach (var itemrecord in outputTransaction.Items?.Where(t => t.Weight is not null ||
                                                                     (t.Weight is null && t.UniversalProductCode.StartsWith('2') &&
                                                                     t.UniversalProductCode.Length > 9 &&
                                                                     (t.UniversalProductCode.Substring(t.UniversalProductCode.Length - 5) == "00000")
                                                                     )))
            {
                if (itemrecord.ItemType != "DepositReturn") //Deposit Returns have a price of .01 so the calculation would not work and we will leave it as is  
                {
                    //Items with a scale UOM of LB need to be treated differently so we cache these items in the Items table
                    bool isSoldByLB = await _transactionControllerHelper.CheckItemSoldByLB(itemrecord.ItemNumber);
                    //If the item is sold by LB
                    if (isSoldByLB)
                    {
                        //Then it is already reporting properly
                    }
                    else //If the item has any other Scale UOM then weight should be null and quantity should be calculated 
                    {
                        if ((itemrecord.WasQuantityKeyPressed == true) || (itemrecord.HasPriceRequired == true) || (itemrecord.SalePrice == (decimal)0.01))
                        {
                            //In these cases the quantity does NOT need to be updated because it already took the value entered or defaulted to 1    
                        }
                        else
                        {
                            //If the weight came from a scale and is an integer then the weight represents the quantity
                            if (itemrecord.HasScaleWeight == true && (itemrecord.Weight % 1) == 0)
                            {
                                itemrecord.Quantity = Convert.ToInt32(itemrecord.Weight);
                            }
                            //Otherwise, calculate quantity    
                            else
                            {
                                //Cache the unit price for this item, for this store, in the Price table
                                var itemprice = await _transactionControllerHelper.GetPriceByItemStore(itemrecord.ItemNumber, Convert.ToInt32(outputTransaction.StoreNumber)).ConfigureAwait(false);
                    
                                //If the item has a no price in the Price table/API, then set the quantity to 1
                                if (itemprice is null)
                                {
                                    itemrecord.Quantity = 1;
                                }
                                // If the extended price is null then the quantity should be zero
                                else if (itemrecord.ExtendedPrice == null)
                                {
                                    itemrecord.Quantity = 0;
                                }
                                else
                                {
                                    //Set default values for the current and previous unit prices
                                    decimal? temp_currentSalesPrice = 0.01M;
                                    decimal? temp_PreviousSalesPrice = 0.01M;

                                    //If the item has a price in the Price table/API, then calculate qty
                                    temp_currentSalesPrice = (decimal?)itemprice.Price;
                                    temp_PreviousSalesPrice = (decimal?)itemprice.PreviousPrice;
                                    
                                    // Try to calculate the quantity based on the current unit price
                                    int qty = (int)(itemrecord.ExtendedPrice / temp_currentSalesPrice);
                                    if (itemrecord.ExtendedPrice % temp_currentSalesPrice == 0)
                                    {
                                        itemrecord.Quantity = qty;
                                    }
                                    //If that doesn't result in a whole number, try to calculate the quantity based on the previous unit price
                                    else
                                    {
                                        qty = (int)(itemrecord.ExtendedPrice / temp_PreviousSalesPrice);
                                        if (itemrecord.ExtendedPrice % temp_PreviousSalesPrice == 0)
                                        {
                                            itemrecord.Quantity = qty;
                                        }
                                        //And if that doesn't result in a whole number, then just divide by the current unit price and round
                                        else
                                        {
                                            int temp_qty = Convert.ToInt32(itemrecord.ExtendedPrice / temp_currentSalesPrice);
                                            itemrecord.Quantity = (temp_qty > 0) ? temp_qty : 1;
                                        }
                                    }
                                }
                            }
                        }
                        itemrecord.Weight = null;
                    }
                }
            }
        }

        outputTransaction = (outputTransaction.IsPharmacyTransaction ?? false) ? outputTransaction.removeCustomerData() :
            await _customerLookup.AddCuid(outputTransaction, cancellationToken).ConfigureAwait(false);

        return outputTransaction;
    }
}
