using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Moq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wegmans.Enterprise.Services;
using Wegmans.POS.DataHub.ItemData;
using Wegmans.POS.DataHub.PriceData;
using Wegmans.Price.Services;

namespace Wegmans.POS.DataHub.UnitTests.PriceData
{
    public class PriceDataControllerTest
    {
        private PriceDataController _sut;

        private IPriceAPI _priceApiMock = Substitute.For<IPriceAPI>();
        private bool expiredItemFoundOrNull;

        public PriceDataControllerTest()
        {
            _sut = new PriceDataController(_priceApiMock);
        }

        /// <summary>
        /// Tests that <see cref="PriceDataController.GetPriceAsync(HttpRequest, string, string, PriceDataController.PriceData, IAsyncCollector{PriceDataController.PriceData}, CancellationToken)"/> returns the found <see cref="PriceDataController.PriceData"/> from the local data if it's
        /// passed in as a paramtere without accessing the <see cref="IAsyncCollector{T}"/>
        /// </summary>
        [Fact]
        public async Task GetPriceAsyncReturnsOkWithPriceWhenFoundInTable()
        {
            // Arrange
            string itemNumber = "12345";
            string storeNumber = "25";

            PriceDataController.PriceData tableItem = new()
            {
                ExpiryDate = DateTime.Now.AddDays(1),
            };
            IAsyncCollector<PriceDataController.PriceData> asyncCollector = Substitute.For<IAsyncCollector<PriceDataController.PriceData>>();
            CancellationToken c = new();

            // Act
            IActionResult response = await _sut.GetPriceAsync(null, itemNumber,storeNumber, tableItem, asyncCollector, c);

            // Assert
            OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(tableItem, okObjectResult.Value);

            Assert.Empty(asyncCollector.ReceivedCalls());
        }
        


    }
}
