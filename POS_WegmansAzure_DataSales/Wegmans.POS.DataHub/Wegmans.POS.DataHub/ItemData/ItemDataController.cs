
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.Enterprise.Services;
using Wegmans.EnterpriseLibrary.Data.Hubs.Products.v1;

namespace Wegmans.POS.DataHub.ItemData
{
    public class ItemDataController
    {
        private IProductAPI _productAPI;

        public ItemDataController(IProductAPI productAPI)
        {
            _productAPI = productAPI ?? throw new ArgumentNullException(nameof(productAPI));
        }

        [FunctionName("GetItem")]
        public async Task<IActionResult> GetItemAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetItem/{itemNumber}")] HttpRequest request, 
            string itemNumber,
            [Table("ItemsTable", "LB", "{itemNumber}", Connection = "POSDataHubAccount")] ItemData itemData,
            [Table("ItemsTable", Connection = "POSDataHubAccount")] IAsyncCollector<ItemData> table,
            CancellationToken c)
        {
            if (itemData is null || itemData.ExpiryDate <= DateTime.Now)
            {
                try
                {
                    var productBySku = await _productAPI.ProductBySKUAsync(Convert.ToInt32(itemNumber), c).ConfigureAwait(false);
                    if (productBySku is { ScaleUnitOfMeasure: "LB" })
                    {
                        itemData = new ItemData
                        {
                            PartitionKey = "LB",
                            RowKey = productBySku.Sku.ToString(),
                            Item = productBySku.Sku.ToString(),
                            UOM = productBySku.ScaleUnitOfMeasure,
                            ExpiryDate = DateTime.Now.AddDays(20)
                        };
                        await table.AddAsync(itemData, c);

                        return new OkObjectResult(itemData);
                    }
                    else
                    {
                        return new NotFoundResult();
                    }
                }
                catch (Exception)
                {
                    return new NotFoundResult();
                }
            }
            return new OkObjectResult(itemData);
        }

        /// <summary>
        /// ItemData Table Entity
        /// </summary>
        public class ItemData : Azure.Data.Tables.ITableEntity
        {
            public string Item { get; set; }
            public string UOM { get; set; }
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public DateTime ExpiryDate { get; set; }
            public ETag ETag { get; set; } = new ETag("*");
        }
    }
}
