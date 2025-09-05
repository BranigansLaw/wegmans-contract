using NSubstitute;
using System.Xml.Linq;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.Customer;
using Wegmans.POS.DataHub.PriceData;
using Wegmans.POS.DataHub.TransactionControllerHelper;

namespace Wegmans.POS.DataHub.UnitTests
{
    public class TransactionControllerTest
    {
        private readonly TransactionController _sut;

        private readonly ITransactionControllerHelper _transactionControllerHelperMock = Substitute.For<ITransactionControllerHelper>();
        private readonly ICustomerLookup _customerLookupMock = Substitute.For<ICustomerLookup>();

        public TransactionControllerTest()
        {
            _sut = new TransactionController(_transactionControllerHelperMock, _customerLookupMock);
        }

        /// <summary>
        /// Tests that <see cref="TransactionController.GetHubTransactionDataAsync(XDocument, string, CancellationToken)"/> returns a <see cref="Transaction"/> and calls all related dependencies to build that transaction, and modifies weight when the Weight is specified, and <see cref="ITransactionControllerHelper.CheckItemSoldByEach(int?)"/> returns true for that item number
        /// </summary>
        [Fact]
        public async Task GetHubTransactionDataAsync_ResetsItemQuantity_WhenConditionsMet()
        {
            // Arrange
            string[] recs = new string[] { "<rec>1</rec>", "<rec>2</rec>", "<rec>3</rec>" };
            XDocument xDocument = XDocument.Parse($"<root>{string.Join("", recs)}</root>");
            IEnumerable<Item> outputTransactionItems = new List<Item> {
                new Item
                {
                    ItemNumber = 1,
                    Weight = 5
                }, 
                new Item{
                    ItemNumber = 2,
                    Quantity = 3,
                    Weight = 6
                },
                new Item{
                    ItemNumber = 3,                    
                    UniversalProductCode = "200000000000"
                },
            };
            int storeNumber = 9438;
            string productPath = $"https://fake/{storeNumber}50.json";
            CancellationToken c = new();

            ICollection<XElement> elementsPassedToUpdateTransactionData = new List<XElement>();
            _transactionControllerHelperMock.UpdateTransactionData(
                Arg.Do<XElement>(x => elementsPassedToUpdateTransactionData.Add(x)),
                Arg.Do<Transaction>(t =>
                {
                    Assert.Equal(productPath, t.Uri);
                    Assert.Equal(storeNumber, t.StoreNumber);
                    t.Items = outputTransactionItems;
                    t.IsPharmacyTransaction = true;
                })
            );

            var expectedItemPrice = new PriceDataController.PriceData
            {
                Item = "3",
                Store = "9438",
                Price = 10.49,
                PreviousPrice = 9.49
            };
            _transactionControllerHelperMock.CheckItemSoldByLB(1).Returns(true);
            _transactionControllerHelperMock.CheckItemSoldByLB(2).Returns(false);
            _transactionControllerHelperMock.GetPriceByItemStore(3,storeNumber).Returns(expectedItemPrice);

            // Act
            Transaction result = await _sut.GetHubTransactionDataAsync(xDocument, productPath, c);

            // Assert
            Assert.NotNull(result);

            Item zeroQuantityItem = Assert.Single(result.Items?.Where(i => i.ItemNumber == 1));
            Assert.Equal(0, zeroQuantityItem.Quantity);
            Assert.NotNull(zeroQuantityItem.Weight);

            Item nullWeightItem = Assert.Single(result.Items?.Where(i => i.ItemNumber == 2));
            Assert.Equal(1, nullWeightItem.Quantity);
            Assert.Null(nullWeightItem.Weight);

            Item zeroQuantityAndNullWeightItem = Assert.Single(result.Items?.Where(i => i.ItemNumber == 3));
            Assert.Equal(0, zeroQuantityAndNullWeightItem.Quantity);
            Assert.Null(zeroQuantityAndNullWeightItem.Weight);

            Assert.Equal(recs.Length, elementsPassedToUpdateTransactionData.Count);
            Assert.Contains(recs[0], elementsPassedToUpdateTransactionData.Select(x => x.ToString()));
            Assert.Contains(recs[1], elementsPassedToUpdateTransactionData.Select(x => x.ToString()));
            Assert.Contains(recs[2], elementsPassedToUpdateTransactionData.Select(x => x.ToString()));

            await _transactionControllerHelperMock.Received(1).CheckItemSoldByLB(1);
            await _transactionControllerHelperMock.Received(1).CheckItemSoldByLB(2);
            await _transactionControllerHelperMock.Received(1).CheckItemSoldByLB(3);
        }
    }
}
