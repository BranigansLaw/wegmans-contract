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
    public class OmnisysClaimQueryTest
    {
        /// <summary>
        /// Tests that <see cref="OmnisysClaimQuery"/> correctly maps all fields
        /// </summary>
        [Theory]
        [ClassData(typeof(OmnisysClaimQueryTest.MappingTests))]
        public void MapFromDataReaderToType_Returns_MappedObject((object[], OmnisysClaimRow) testParameters)
        {
            // Arrange
            (object[] readerIndex, OmnisysClaimRow expectedResult) = testParameters;
            OmnisysClaimQuery query = new OmnisysClaimQuery
            {
                RunDate = DateOnly.FromDateTime(DateTime.Now)
            };
            DbDataReader mockedReader = Substitute.For<DbDataReader>();
            mockedReader.GetString(0).Returns(readerIndex[0]);
            mockedReader.GetString(1).Returns(readerIndex[1]);
            mockedReader.GetString(2).Returns(readerIndex[2]);
            mockedReader.GetDateTime(3).Returns(readerIndex[3]);
            mockedReader.GetDateTime(4).Returns(readerIndex[4]);
            mockedReader.GetString(5).Returns(readerIndex[5]);
            mockedReader.GetString(6).Returns(readerIndex[6]);
            mockedReader.GetString(7).Returns(readerIndex[7]);
            mockedReader.GetString(8).Returns(readerIndex[8]);

            // Act
            OmnisysClaimRow res = query.MapFromDataReaderToType(mockedReader, s => { });

            // Assert
            Assert.NotNull(res);
            ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
        }

        public class MappingTests : TheoryData<(object[], OmnisysClaimRow)>
        {
            public MappingTests()
            {
                AddRow((
                new object[] {
                     "782819372", "Michael", "Some string", new DateTime(2024, 6, 5), new DateTime(2024, 6, 5),"2024927818329", "iiw8sj193ruhO", "100103", "182738292010384"
                },
                new OmnisysClaimRow
                {
                    PharmacyNpi = "782819372",
                    PrescriptionNbr = "Michael",
                    RefillNumber = "Some string",
                    SoldDate = DateOnly.FromDateTime(new DateTime(2024, 6, 5)),
                    DateOfService = DateOnly.FromDateTime(new DateTime(2024, 6, 5)),
                    NdcNumber = "2024927818329",
                    CardholderId = "iiw8sj193ruhO",
                    AuthorizationNumber = "100103",
                    ReservedForFutureUse = "182738292010384",
                }
                ));

                AddRow((
                new object[] {
                    "PharmacyNpi", "PrescriptionNbr", "RefillNumber", new DateTime(2024, 6, 5), new DateTime(2024, 6, 5), "NdcNumber", "CardholderId", "AuthorizationNumber", "ReservedForFutureUse"
                },
                new OmnisysClaimRow
                {
                    PharmacyNpi = "PharmacyNpi",
                    PrescriptionNbr = "PrescriptionNbr",
                    RefillNumber = "RefillNumber",
                    SoldDate = DateOnly.FromDateTime(new DateTime(2024, 6, 5)),
                    DateOfService = DateOnly.FromDateTime(new DateTime(2024, 6, 5)),
                    NdcNumber = "NdcNumber",
                    CardholderId = "CardholderId",
                    AuthorizationNumber = "AuthorizationNumber",
                    ReservedForFutureUse = "ReservedForFutureUse",
                }
                ));
            }
        }
    }
}