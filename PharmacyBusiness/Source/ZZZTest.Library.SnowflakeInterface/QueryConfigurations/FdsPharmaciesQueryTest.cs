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
    public class FdsPharmaciesQueryTest
    {
        /// <summary>
        /// Tests that <see cref="FdsPharmaciesQuery"/> correctly maps all fields
        /// </summary>
        [Theory]
        [ClassData(typeof(FdsPharmaciesQueryTest.MappingTests))]
        public void MapFromDataReaderToType_Returns_MappedObject((object[], FdsPharmaciesRow) testParameters)
        {
            // Arrange
            (object[] readerIndex, FdsPharmaciesRow expectedResult) = testParameters;
            FdsPharmaciesQuery query = new FdsPharmaciesQuery
            {
                RunDate = DateOnly.FromDateTime(DateTime.Now)
            }; ;

            DbDataReader mockedReader = Substitute.For<DbDataReader>();
            mockedReader.GetString(0).Returns(readerIndex[0]);
            mockedReader.GetString(1).Returns(readerIndex[1]);
            mockedReader.GetString(2).Returns(readerIndex[2]);
            mockedReader.GetString(3).Returns(readerIndex[3]);
            mockedReader.GetString(4).Returns(readerIndex[4]);
            mockedReader.GetString(5).Returns(readerIndex[5]);
            mockedReader.GetString(6).Returns(readerIndex[6]);
            mockedReader.GetString(7).Returns(readerIndex[7]);
            mockedReader.GetString(8).Returns(readerIndex[8]);
            mockedReader.GetString(9).Returns(readerIndex[9]);
            mockedReader.GetString(10).Returns(readerIndex[10]);

            // Act
            FdsPharmaciesRow res = query.MapFromDataReaderToType(mockedReader, s => { });

            // Assert
            Assert.NotNull(res);
            ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
        }

        public class MappingTests : TheoryData<(object[], FdsPharmaciesRow)>
        {
            public MappingTests()
            {
                AddRow((
                new object[] {
                    "NcpdpId", "Npi", "PharmacyName", "AddressLine1", "AddressLine2", "City", "State", "ZipCode", "StorePhoneNumber", "FaxPhoneNumber", "Banner"
                },
                new FdsPharmaciesRow
                {
                    NcpdpId = "NcpdpId",
                    Npi = "Npi",
                    PharmacyName = "PharmacyName",
                    AddressLine1 = "AddressLine1",
                    AddressLine2 = "AddressLine2",
                    City = "City",
                    State = "State",
                    ZipCode = "ZipCode",
                    StorePhoneNumber = "StorePhoneNumber",
                    FaxPhoneNumber = "FaxPhoneNumber",
                    Banner = "Banner",
                }
                ));

                AddRow((
                new object[] {
                    "0010010", "1528042611", "#001 JOHN GLENN", "7519 OSWEGO RD", "null", "LIVERPOOL", "NY", "13090", "3156222100", "3155461298", "null"
                },
                new FdsPharmaciesRow
                {
                    NcpdpId = "0010010",
                    Npi = "1528042611",
                    PharmacyName = "#001 JOHN GLENN",
                    AddressLine1 = "7519 OSWEGO RD",
                    AddressLine2 = "null",
                    City = "LIVERPOOL",
                    State = "NY",
                    ZipCode = "13090",
                    StorePhoneNumber = "3156222100",
                    FaxPhoneNumber = "3155461298",
                    Banner = "null",
                }
                ));
            }
        }
    }
}