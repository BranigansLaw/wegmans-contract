using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.BatchModifier.BatchModifierMethods;

namespace Wegmans.POS.DataHub.UnitTests.BatchModifier.BatchModifierMethods
{
    public class BatchModifierMethodsImpTest
    {
        private readonly BatchModifierMethodsImp _sut;

        public BatchModifierMethodsImpTest()
        {
            _sut = new BatchModifierMethodsImp();
        }

        [Theory]
        [ClassData(typeof(UpdateQuantityTestCases))]
        public async Task UpdateQuantityAsync_TestCases(
            bool isWeightItem,
            decimal salePrice,
            int quantity,
            int? salesQuantity,
            decimal? extendedPrice,
            int expectedSalesQuantity,
            bool? expectedHasBeenRepublished)
        {
            // Arrange
            Transaction transaction = new()
            {
                Items = new List<Item>
                {
                    new() {
                        IsWeightItem = isWeightItem,
                        Quantity = quantity,
                        SalePrice = salePrice,
                        SaleQuantity = salesQuantity,
                        ExtendedPrice = extendedPrice,
                    }
                }
            };

            // Act
            await _sut.UpdateQuantityAsync(transaction);

            // Assert
            Assert.Equal(expectedSalesQuantity, transaction.Items.First().Quantity);
            Assert.Equal(expectedHasBeenRepublished, transaction.HasBeenRepublished);
        }
    }

    public class UpdateQuantityTestCases : TheoryData<
        bool,
        decimal,
        int,
        int?,
        decimal?,
        int,
        bool?>
    {
        // isWeightItem, salePrice, quantity, salesQuantity, extendedPrice, expectedSalesQuantity, expectedHasBeenRepublished
        public UpdateQuantityTestCases()
        {
            // When SalesPrice is 0.01, use SalesQuantity
            AddRow(true, 0.01, 150, 100, null, 100, true);
            // When SalesPrice is 0.01, but SalesQuantity is null, use 1
            AddRow(true, 0.01, 150, null, null, 1, true);
            // When SalesPrice is not 0.01, use ExtendedPrice / SalesPrice
            AddRow(true, 0.02, 150, 100, 300.0M, 15000, true);
            AddRow(true, 0.02, 150, 100, 350.0M, 17500, true);
            AddRow(true, 3, 150, 100, 300.0M, 100, true);
            AddRow(true, 3, 200, 50, 100.0M, 33, true);
            AddRow(true, 3, 200, 100, 200.0M, 67, true);
            // When IsWeightItem is false, do nothing
            AddRow(false, 0.01, 150, 100, 300.0M, 150, null);
            // When Quantity is less than 100, do nothing
            AddRow(true, 0.01, 50, 100, 300.0M, 50, null);
        }
    }
}
