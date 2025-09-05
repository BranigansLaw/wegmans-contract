using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class WorkersCompMonthlyQueryTests
{
    /// <summary>
    /// Tests that <see cref="WorkersCompMonthlyQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(WorkersCompMonthlyQueryTests.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], WorkersCompMonthly) testParameters)
    {
        // Arrange
        (object[] readerIndex, WorkersCompMonthly expectedResult) = testParameters;
        WorkersCompMonthlyQuery query = new()
        {
            RunDate = DateOnly.FromDateTime(DateTime.Now)
        };

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
        Util.SetupNullableReturn(mockedReader, readerIndex, 9);
        mockedReader.GetFieldValue<decimal>(10).Returns(readerIndex[10]);
        mockedReader.GetFieldValue<decimal>(11).Returns(readerIndex[11]);
        mockedReader.GetFieldValue<decimal>(12).Returns(readerIndex[12]);
        mockedReader.GetString(13).Returns(readerIndex[13]);
        mockedReader.GetString(14).Returns(readerIndex[14]);
        mockedReader.GetString(15).Returns(readerIndex[15]);
        mockedReader.GetString(16).Returns(readerIndex[16]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 17);
        mockedReader.GetFieldValue<long>(18).Returns(readerIndex[18]);

        // Act
        WorkersCompMonthly res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], WorkersCompMonthly)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    "46386",
                    "31597",
                    "47623",
                    "44520",
                    "51677",
                    "91533",
                    "37625",
                    "27103",
                    "89427",
                    7855312232636797952,
                    14518M,
                    28795M,
                    78603M,
                    "53011",
                    "22562",
                    "36442",
                    "77890",
                    new DateTime(2035, 3, 9),
                    6857025941889024,
                },
                new WorkersCompMonthly
                {
                    FacilityIdNumber = "46386",
                    RxNumber = "31597",
                    DateFilled = "47623",
                    PatientLastName = "44520",
                    PatientFirstName = "51677",
                    CardholderId = "91533",
                    PatientDob = "37625",
                    DrugNdcNumber = "27103",
                    DrugName = "89427",
                    QtyDrugDispensed = 7855312232636797952,
                    TxPrice = 14518M,
                    PatientPay = 28795M,
                    AdjAmt = 78603M,
                    ThirdPartyName = "53011",
                    ThirdPartyCode = "22562",
                    SplitBillIndicator = "36442",
                    PrescriberName = "77890",
                    DateSold = new DateTime(2035, 3, 9),
                    ClaimNumber = 6857025941889024,
                }
            ));

            AddRow((
                new object[] {
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    14518M,
                    28795M,
                    78603M,
                    null!,
                    null!,
                    "36442",
                    null!,
                    null!,
                    6857025941889024,
                },
                new WorkersCompMonthly
                {
                    FacilityIdNumber = null,
                    RxNumber = null,
                    DateFilled = null,
                    PatientLastName = null,
                    PatientFirstName = null,
                    CardholderId = null,
                    PatientDob = null,
                    DrugNdcNumber = null,
                    DrugName = null,
                    QtyDrugDispensed = null,
                    TxPrice = 14518M,
                    PatientPay = 28795M,
                    AdjAmt = 78603M,
                    ThirdPartyName = null,
                    ThirdPartyCode = null,
                    SplitBillIndicator = "36442",
                    PrescriberName = null,
                    DateSold = null,
                    ClaimNumber = 6857025941889024,
                }
            ));
        }
    }
}
