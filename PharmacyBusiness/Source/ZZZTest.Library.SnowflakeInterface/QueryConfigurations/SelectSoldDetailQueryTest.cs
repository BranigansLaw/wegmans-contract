using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using ZZZZTest.TestHelpers;
using log4net;
using Microsoft.VisualBasic;
using NSubstitute;
using System;
using System.Data.Common;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations
{
    public class SelectSoldDetailQueryTest
    {
        /// <summary>
        /// Tests that <see cref="SelectSoldDetailQuery"/> correctly maps all fields
        /// </summary>
        [Theory]
        [ClassData(typeof(SelectSoldDetailQueryTest.MappingTests))]
        public void MapFromDataReaderToType_Returns_MappedObject((object[], SelectSoldDetailRow) testParameters)
        {
            // Arrange
            (object[] readerIndex, SelectSoldDetailRow expectedResult) = testParameters;
            SelectSoldDetailQuery query = new SelectSoldDetailQuery
            {
                RunDate = DateOnly.FromDateTime(DateTime.Now)
            };
            DbDataReader mockedReader = Substitute.For<DbDataReader>();
            mockedReader.GetString(0).Returns(readerIndex[0]);
            mockedReader.GetString(1).Returns(readerIndex[1]);
            mockedReader.GetFieldValue<int?>(2).Returns(readerIndex[2]);
            mockedReader.GetFieldValue<int?>(3).Returns(readerIndex[3]);
            mockedReader.GetDateTime(4).Returns(readerIndex[4]);
            mockedReader.GetString(5).Returns(readerIndex[5]);
            mockedReader.GetFieldValue<decimal?>(6).Returns(readerIndex[6]);
            mockedReader.GetString(7).Returns(readerIndex[7]);
            mockedReader.GetFieldValue<decimal?>(8).Returns(readerIndex[8]);
            mockedReader.GetFieldValue<decimal?>(9).Returns(readerIndex[9]);
            mockedReader.GetFieldValue<decimal?>(10).Returns(readerIndex[10]);
            mockedReader.GetFieldValue<decimal?>(11).Returns(readerIndex[11]);

            // Act
            SelectSoldDetailRow res = query.MapFromDataReaderToType(mockedReader, s => { });

            // Assert
            Assert.NotNull(res);
            ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
        }

        public class MappingTests : TheoryData<(object[], SelectSoldDetailRow)>
        {
            public MappingTests()
            {
                AddRow((
                new object[] {
                    "1", "2", 3, 4, new DateTime(2024, 6, 5), "7", 8.27M, "michael", 1.20M, 1.21M, 1.23M, 1.24M
                },
                new SelectSoldDetailRow
                {
                    StoreNumber = "1",
                    RxNumber = "2",
                    RefillNumber = 3,
                    PartSequenceNumber = 4,
                    SoldDate = new DateTime(2024, 6, 5),
                    OrderNumber = "7",
                    QtyDispensed = 8.27M,
                    NdcWo = "michael",
                    AdqCost = 1.20M,
                    TpPay = 1.21M,
                    PatientPay = 1.23M,
                    TxPrice = 1.24M,
                }
                ));

                AddRow((
                new object[] {
                    "275", "40000563", 2, 1, new DateTime(2024, 6, 5), "100", 10.01M, "bauer", 2.02M, 2.01M, 2.00M, 1.99M
                },
                new SelectSoldDetailRow
                {
                    StoreNumber = "275",
                    RxNumber = "40000563",
                    RefillNumber = 2,
                    PartSequenceNumber = 1,
                    SoldDate = new DateTime(2024, 6, 5),
                    OrderNumber = "100",
                    QtyDispensed = 10.01M,
                    NdcWo = "bauer",
                    AdqCost = 2.02M,
                    TpPay = 2.01M,
                    PatientPay = 2.00M,
                    TxPrice = 1.99M,
                }
                ));
            }
        }
    }
}