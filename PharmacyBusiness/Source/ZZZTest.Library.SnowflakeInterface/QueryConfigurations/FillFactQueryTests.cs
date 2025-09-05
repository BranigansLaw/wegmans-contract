using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations
{
    public class FillFactQueryTests
    {
        /// <summary>
        /// Tests that <see cref="FillFactQuery"/> correctly maps all fields
        /// </summary>
        [Theory]
        [ClassData(typeof(FillFactQueryTests.MappingTests))]
        public void MapFromDataReaderToType_Returns_MappedObject((object[], FillFactRow) testParameters)
        {
            // Arrange
            (object[] readerIndex, FillFactRow expectedResult) = testParameters;
            FillFactQuery query = new FillFactQuery
            {
                ReadyDateKey = 1,
                DispensedItemExpirationDate = new DateOnly(2024, 8, 27),
                LocalTransactionDate = new DateTime(2024, 8, 27),
                RxNumber = "dfgf78df9gf789f",
                TotalPricePaid = 19.99M,
            };
            DbDataReader mockedReader = Substitute.For<DbDataReader>();
            mockedReader.GetFieldValue<long>(0).Returns(readerIndex[0]);
            mockedReader.GetString(1).Returns(readerIndex[1]);
            mockedReader.GetFieldValue<long>(2).Returns(readerIndex[2]);
            mockedReader.GetFieldValue<decimal>(3).Returns(readerIndex[3]);
            Util.SetupNullableReturn(mockedReader, readerIndex, 4);

            // Act
            FillFactRow res = query.MapFromDataReaderToType(mockedReader, s => { });

            // Assert
            ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
        }

        public class MappingTests : TheoryData<(object?[], FillFactRow)>
        {
            public MappingTests()
            {
                AddRow((
                    new object[] {
                        345838934854395,
                        "sdfjkdjfghfdg",
                        (long) 4395895,
                        37.21M,
                        12.3593M,
                    },
                    new FillFactRow
                    {
                        FillFactKey = 345838934854395,
                        Source = "sdfjkdjfghfdg",
                        OrderDateKey = 4395895,
                        RefillQuantity = 37.21M,
                        FullPackageUnc = 12.3593M,
                    }
                ));

                AddRow((
                    new object?[] {
                        98456098456890,
                        "kjhgkfgh",
                        (long) 435908,
                        93.12M,
                        null,
                    },
                    new FillFactRow
                    {
                        FillFactKey = 98456098456890,
                        Source = "kjhgkfgh",
                        OrderDateKey = 435908,
                        RefillQuantity = 93.12M,
                        FullPackageUnc = null,
                    }
                ));
            }
        }
    }
}