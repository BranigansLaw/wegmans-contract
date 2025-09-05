using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using ZZZZTest.TestHelpers;
using log4net;
using Microsoft.VisualBasic;
using NSubstitute;
using System;
using System.Data.Common;
using Library.LibraryUtilities.Extensions;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations
{
    public class GetSmartOrderMinMaxQueryTests
    {
        /// <summary>
        /// Tests that <see cref="GetSmartOrderMinMaxQuery"/> correctly maps all fields
        /// </summary>
        [Theory]
        [ClassData(typeof(GetSmartOrderMinMaxQueryTests.MappingTests))]
        public void MapFromDataReaderToType_Returns_MappedObject((object[], GetSmartOrderMinMaxRow) testParameters)
        {
            // Arrange
            (object[] readerIndex, GetSmartOrderMinMaxRow expectedResult) = testParameters;
            GetSmartOrderMinMaxQuery query = new(); 

            DbDataReader mockedReader = Substitute.For<DbDataReader>();
            mockedReader.GetString(0).Returns(readerIndex[0]);
            mockedReader.GetString(1).Returns(readerIndex[1]);
            Util.SetupNullableReturn(mockedReader, readerIndex, 2);
            Util.SetupNullableReturn(mockedReader, readerIndex, 3);
            mockedReader.GetString(4).Returns(readerIndex[4]);
            Util.SetupNullableReturn(mockedReader, readerIndex, 5);


            // Act
            GetSmartOrderMinMaxRow res = query.MapFromDataReaderToType(mockedReader, s => { });

            // Assert
            Assert.NotNull(res);
            ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
        }

        public class MappingTests : TheoryData<(object[], GetSmartOrderMinMaxRow)>
        {
            public MappingTests()
            {
                AddRow((
                new object[] {
                    "1", "00002143480", (long)3, (long)4, "Converted Purchasing Plan", (long)20240901
                },
                new GetSmartOrderMinMaxRow
                {
                    StoreNumber = "1",
                    NdcWo = "00002143480",
                    MinQtyOverride = 3,
                    MaxQtyOverride = 4,
                    PurchasePlan = "Converted Purchasing Plan",
                    LastUpdated = 20240901
                }
                ));

                AddRow((
                new object[] {
                    "275", "40000563", (long)2, (long)1, "Converted Purc Plan", (long)20240822
                },
                new GetSmartOrderMinMaxRow
                {
                    StoreNumber = "275",
                    NdcWo = "40000563",
                    MinQtyOverride = 2,
                    MaxQtyOverride = 1,
                    PurchasePlan = "Converted Purc Plan",
                    LastUpdated = 20240822
                }
                ));
            }
        }
    }
}