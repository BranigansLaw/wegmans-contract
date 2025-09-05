using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Wegmans.Enterprise.Services;
using Wegmans.POS.DataHub.ItemData;
using Product = Wegmans.Enterprise.Services.Product;

namespace Wegmans.POS.DataHub.UnitTests.ItemData
{
    public class ItemDataControllerTest
    {
        private ItemDataController _sut;

        private IProductAPI _productApiMock = Substitute.For<IProductAPI>();

        public ItemDataControllerTest()
        {
            _sut = new ItemDataController(_productApiMock);
        }

        /// <summary>
        /// Tests that <see cref="ItemDataController.GetItemAsync"/> returns the found <see cref="ItemDataController.ItemData"/> from the local data if it's
        /// passed in as a paramtere without accessing the <see cref="IAsyncCollector{T}"/>
        /// </summary>
        [Fact]
        public async Task GetItemAsync_ReturnsOkWithProduct_WhenFoundInTable()
        {
            // Arrange
            string itemNumber = "12345";
            ItemDataController.ItemData tableItem = new()
            {
                ExpiryDate = DateTime.Now.AddDays(1),
            };
            IAsyncCollector<ItemDataController.ItemData> asyncCollector = Substitute.For<IAsyncCollector<ItemDataController.ItemData>>();
            CancellationToken c = new();

            // Act
            IActionResult response = await _sut.GetItemAsync(null, itemNumber, tableItem, asyncCollector, c);

            // Assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(tableItem, okObjectResult.Value);

            Assert.Empty(asyncCollector.ReceivedCalls());
        }

        /// <summary>
        ///// Tests that <see cref="ItemDataController.GetItemAsync"/> returns the <see cref="ItemDataController.ItemData"/> from the <see cref="IProductAPI"/>
        ///// when not found in the function apps table, and then returns the newly created table item after adding it to the <see cref="IAsyncCollector{T}"/>
        ///// </summary>
        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public async Task GetItemAsync_ReturnsOkWithProduct_WhenFoundInProductApi(bool expiredItemFoundOrNull)
        //{
        //    // Arrange
        //    string itemNumber = "12345";
        //    ItemDataController.ItemData? tableItem = expiredItemFoundOrNull ? new ItemDataController.ItemData
        //    {
        //        ExpiryDate = DateTime.Now.AddDays(-1),
        //    } : null;
        //    IAsyncCollector<ItemDataController.ItemData> asyncCollector = Substitute.For<IAsyncCollector<ItemDataController.ItemData>>();
        //    CancellationToken c = new();
        //    Product apiReturnedProduct = new()
        //    {
        //        Sku = 92685,
        //        ScaleUnitOfMeasure = "EA",
        //    };
        //    _productApiMock.ProductBySKUAsync(Convert.ToInt32(itemNumber), c).Returns(apiReturnedProduct);
        //    ItemDataController.ItemData? createdItem = null;
        //    asyncCollector.AddAsync(Arg.Do<ItemDataController.ItemData>(x => createdItem = x), c).Returns(Task.CompletedTask);

        //    // Act
        //    IActionResult response = await _sut.GetItemAsync(null, itemNumber, tableItem, asyncCollector, c);

        //    // Assert
        //    OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(response);
        //    ItemDataController.ItemData returnedValue = Assert.IsType<ItemDataController.ItemData>(okObjectResult.Value);

        //    Assert.Equal(apiReturnedProduct.Sku.ToString(), returnedValue.RowKey);
        //    Assert.Equal(apiReturnedProduct.Sku.ToString(), returnedValue.Item);
        //    Assert.Equal("EA", apiReturnedProduct.ScaleUnitOfMeasure);
        //    Assert.Equal("EA", returnedValue.PartitionKey);
        //    Assert.Equal(DateTime.Now.AddDays(20), returnedValue.ExpiryDate, new TimeSpan(0, 5, 0));

        //    Assert.NotNull(createdItem);
        //    Assert.Equal(returnedValue, createdItem);
        //}

        /// <summary>
        /// Tests that <see cref="ItemDataController.GetItemAsync"/> returns the <see cref="ItemDataController.ItemData"/> from the <see cref="IProductAPI"/>
        /// when not found in the function apps table, and then returns the newly created table item after adding it to the <see cref="IAsyncCollector{T}"/>
        /// </summary>
        [Theory]
        [InlineData(false, "NotEA")]
        [InlineData(true, "EA")]
        public async Task GetItemAsync_ReturnsNotFound_UnderVariousConditions(bool productApiThrowsException, string scaleOfMeasure)
        {
            // Arrange
            string itemNumber = "12345";
            IAsyncCollector<ItemDataController.ItemData> asyncCollector = Substitute.For<IAsyncCollector<ItemDataController.ItemData>>();
            CancellationToken c = new();
            Product apiReturnedProduct = new()
            {
                Sku = 3409583,
                ScaleUnitOfMeasure = scaleOfMeasure,
            };

            if (productApiThrowsException)
            {
                _productApiMock.ProductBySKUAsync(Convert.ToInt32(itemNumber), c).Throws(new Exception("Exception"));
            }
            else
            {
                _productApiMock.ProductBySKUAsync(Convert.ToInt32(itemNumber), c).Returns(apiReturnedProduct);
            }

            // Act
            IActionResult response = await _sut.GetItemAsync(null, itemNumber, null, asyncCollector, c);

            // Assert
            Assert.IsType<NotFoundResult>(response);

            Assert.Empty(asyncCollector.ReceivedCalls());
        }
    }
}
