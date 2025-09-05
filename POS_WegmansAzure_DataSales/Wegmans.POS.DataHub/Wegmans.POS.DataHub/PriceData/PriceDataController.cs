
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.ItemSettings.v1;
using Wegmans.Price.Services;

namespace Wegmans.POS.DataHub.PriceData
{
    public class PriceDataController
    {
        private  IPriceAPI _priceAPI;

        public PriceDataController(IPriceAPI priceAPI)
        {
            _priceAPI = priceAPI ?? throw new ArgumentNullException(nameof(priceAPI));
        }

        [FunctionName("GetPrice")]
        public async Task<IActionResult> GetPriceAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetPrice/{itemNumber}/{storeNumber}")] HttpRequest request,
            string itemNumber, string storeNumber,
            [Table("PriceTable", "{storeNumber}", "{itemNumber}", Connection = "POSDataHubAccount")] PriceData priceData,
            [Table("PriceTable", Connection = "POSDataHubAccount")] IAsyncCollector<PriceData> table,
            CancellationToken c)
        {
            if (priceData is null || priceData.ExpiryDate <= DateTime.Now)
            {
                double oldPrice = 0;
                if (priceData is not null) 
                    { oldPrice = priceData.Price ?? 0; }

                try
                {
                    var priceBySku = await _priceAPI.CurrentPriceBySKUAndStoreAsync(Convert.ToInt32(itemNumber), Convert.ToInt32(storeNumber), c).ConfigureAwait(false);

                    if (priceBySku.Count == 0) {return new NotFoundResult(); }

                    var responseSkuPrice = priceBySku.FirstOrDefault();
                    double defaultPrice = responseSkuPrice.Price ?? 1;
                    double defaultQuantity = responseSkuPrice.Quantity ?? 1;
                    double unitPrice = Math.Round(defaultPrice / defaultQuantity, 2);
                    double defaultAmountChanged = responseSkuPrice.AmountChanged ?? 0;
                    double previousPrice = Math.Round(unitPrice - defaultAmountChanged, 2);
                    double defaultPreviousPrice = previousPrice == 0.01 ? unitPrice : previousPrice;

                    priceData = new PriceData
                    {
                        PartitionKey = responseSkuPrice.StoreNumber.ToString(),
                        Store = responseSkuPrice.StoreNumber.ToString(),
                        RowKey = responseSkuPrice.Sku.ToString(),
                        Item = responseSkuPrice.Sku.ToString(),
                        Price = unitPrice,
                        PreviousPrice = defaultPreviousPrice,
                        ExpiryDate = DateTime.Now.AddDays(30)
                    };
                    await table.AddAsync(priceData, c);
                    return new OkObjectResult(priceData);
                }
                catch (Exception)
                {
                    return new NotFoundResult();
                }
            }
            return new OkObjectResult(priceData);
        }

        /// <summary>
        /// ItemData Table Entity
        /// </summary>
        public class PriceData : Azure.Data.Tables.ITableEntity
        {
            public string Store { get; set; }
            public string Item { get; set; }
            public double? Price { get; set; }
            public double? PreviousPrice { get; set; }
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public DateTime ExpiryDate { get; set; }
            public ETag ETag { get; set; } = new ETag("*");
        }
    }
}
