using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class WegmansHPOnePrescriptionsQueryTest
{
    /// <summary>
    /// Tests that <see cref="WegmansHPOnePrescriptionsQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(WegmansHPOnePrescriptionsQueryTest.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], WegmansHPOnePrescriptionsRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, WegmansHPOnePrescriptionsRow expectedResult) = testParameters;
        WegmansHPOnePrescriptionsQuery query = new()
        {
            RunDate = DateOnly.FromDateTime(DateTime.Now)
        };

        DbDataReader mockedReader = Substitute.For<DbDataReader>();
        mockedReader.GetFieldValue<long>(0).Returns(readerIndex[0]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 1);
        mockedReader.GetString(2).Returns(readerIndex[2]);
        mockedReader.GetString(3).Returns(readerIndex[3]);
        mockedReader.GetString(4).Returns(readerIndex[4]);
        mockedReader.GetString(5).Returns(readerIndex[5]);
        mockedReader.GetString(6).Returns(readerIndex[6]);
        mockedReader.GetString(7).Returns(readerIndex[7]);
        mockedReader.GetString(8).Returns(readerIndex[8]);
        mockedReader.GetString(9).Returns(readerIndex[9]);
        mockedReader.GetString(10).Returns(readerIndex[10]);
        mockedReader.GetString(11).Returns(readerIndex[11]);
        mockedReader.GetString(12).Returns(readerIndex[12]);
        mockedReader.GetString(13).Returns(readerIndex[13]);
        mockedReader.GetString(14).Returns(readerIndex[14]);
        mockedReader.GetString(15).Returns(readerIndex[15]);
        mockedReader.GetFieldValue<decimal>(16).Returns(readerIndex[16]);
        mockedReader.GetFieldValue<decimal>(17).Returns(readerIndex[17]);
        mockedReader.GetString(18).Returns(readerIndex[18]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 19);
        Util.SetupNullableReturn(mockedReader, readerIndex, 20);
        mockedReader.GetFieldValue<decimal>(21).Returns(readerIndex[21]);
        mockedReader.GetString(22).Returns(readerIndex[22]);
        mockedReader.GetString(23).Returns(readerIndex[23]);
        mockedReader.GetString(24).Returns(readerIndex[24]);
        mockedReader.GetString(25).Returns(readerIndex[25]);
        mockedReader.GetString(26).Returns(readerIndex[26]);
        mockedReader.GetString(27).Returns(readerIndex[27]);
        mockedReader.GetString(28).Returns(readerIndex[28]);
        mockedReader.GetString(29).Returns(readerIndex[29]);
        mockedReader.GetString(30).Returns(readerIndex[30]);
        mockedReader.GetString(31).Returns(readerIndex[31]);

        // Act
        WegmansHPOnePrescriptionsRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], WegmansHPOnePrescriptionsRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    7812658723957784576,
                    2109330039493164032,
                    "78844",
                    "17912",
                    "95621",
                    "82432",
                    "83366",
                    "23769",
                    "92950",
                    "29237",
                    "59550",
                    "87890",
                    "72250",
                    "80101",
                    "78004",
                    "51359",
                    91840M,
                    22716M,
                    "72924",
                    7857315151763823616,
                    220973461295411200,
                    20845M,
                    "88402",
                    "69025",
                    "69788",
                    "46553",
                    "46518",
                    "81689",
                    "46069",
                    "60445",
                    "14211",
                    "23827",
                },
                new WegmansHPOnePrescriptionsRow
                {
                    Version = 7812658723957784576,
                    ClientPatientId = 2109330039493164032,
                    LastName = "78844",
                    FirstName = "17912",
                    Gender = "95621",
                    AddressLine1 = "82432",
                    AddressLine2 = "83366",
                    City = "23769",
                    State = "92950",
                    Zipcode = "29237",
                    Dob = "59550",
                    Email = "87890",
                    Phone = "72250",
                    PhoneType = "80101",
                    Language = "78004",
                    Ndc = "51359",
                    Qty = 91840M,
                    DaysSupply = 22716M,
                    ProductName = "72924",
                    FillDate = 7857315151763823616,
                    FillNum = 220973461295411200,
                    AuthorizedRefills = 20845M,
                    Npi = "88402",
                    MedicareContractId = "69025",
                    MedicarePlanId = "69788",
                    Bin = "46553",
                    Pcn = "46518",
                    GroupId = "81689",
                    Plan = "46069",
                    PrescriberNpi = "60445",
                    Medicared = "14211",
                    MedicareId = "23827",
                }
            ));

            AddRow((
                new object?[] {
                    7812658723957784576,
                    null,
                    null,
                    null,
                    "95621",
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    91840M,
                    22716M,
                    null,
                    null,
                    null,
                    20845M,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                },
                new WegmansHPOnePrescriptionsRow
                {
                    Version = 7812658723957784576,
                    ClientPatientId = null,
                    LastName = null,
                    FirstName = null,
                    Gender = "95621",
                    AddressLine1 = null,
                    AddressLine2 = null,
                    City = null,
                    State = null,
                    Zipcode = null,
                    Dob = null,
                    Email = null,
                    Phone = null,
                    PhoneType = null,
                    Language = null,
                    Ndc = null,
                    Qty = 91840M,
                    DaysSupply = 22716M,
                    ProductName = null,
                    FillDate = null,
                    FillNum = null,
                    AuthorizedRefills = 20845M,
                    Npi = null,
                    MedicareContractId = null,
                    MedicarePlanId = null,
                    Bin = null,
                    Pcn = null,
                    GroupId = null,
                    Plan = null,
                    PrescriberNpi = null,
                    Medicared = null,
                    MedicareId = null,
                }
            ));
        }
    }
}
