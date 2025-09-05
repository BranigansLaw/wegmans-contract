using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class PrescriberQueryTest
{
    /// <summary>
    /// Tests that <see cref="PrescriberQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(PrescriberQueryTest.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], PrescriberRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, PrescriberRow expectedResult) = testParameters;
        PrescriberQuery query = new()
        {
            RunDate = DateOnly.FromDateTime(DateTime.Now)
        };

        DbDataReader mockedReader = Substitute.For<DbDataReader>();
        mockedReader.GetFieldValue<long>(0).Returns(readerIndex[0]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 1);
        Util.SetupNullableReturn(mockedReader, readerIndex, 2);
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
        Util.SetupNullableReturn(mockedReader, readerIndex, 14);
        mockedReader.GetString(15).Returns(readerIndex[15]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 16);
        Util.SetupNullableReturn(mockedReader, readerIndex, 17);
        mockedReader.GetString(18).Returns(readerIndex[18]);
        mockedReader.GetString(19).Returns(readerIndex[19]);
        mockedReader.GetString(20).Returns(readerIndex[20]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 21);
        Util.SetupNullableReturn(mockedReader, readerIndex, 22);
        Util.SetupNullableReturn(mockedReader, readerIndex, 23);
        Util.SetupNullableReturn(mockedReader, readerIndex, 24);
        mockedReader.GetString(25).Returns(readerIndex[25]);
        mockedReader.GetString(26).Returns(readerIndex[26]);
        mockedReader.GetString(27).Returns(readerIndex[27]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 28);
        mockedReader.GetString(29).Returns(readerIndex[29]);
        mockedReader.GetString(30).Returns(readerIndex[30]);
        mockedReader.GetString(31).Returns(readerIndex[31]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 32);
        mockedReader.GetString(33).Returns(readerIndex[33]);
        mockedReader.GetString(34).Returns(readerIndex[34]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 35);
        mockedReader.GetString(36).Returns(readerIndex[36]);
        mockedReader.GetString(37).Returns(readerIndex[37]);
        mockedReader.GetString(38).Returns(readerIndex[38]);
        mockedReader.GetString(39).Returns(readerIndex[39]);
        mockedReader.GetString(40).Returns(readerIndex[40]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 41);

        // Act
        PrescriberRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], PrescriberRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    5662335694648055808,
                    414235818378563584,
                    5828824191277391872,
                    "14551",
                    "31252",
                    "22834",
                    "89167",
                    "80918",
                    "48451",
                    "52127",
                    "35612",
                    "99483",
                    "71290",
                    "97350",
                    6623578280628560896,
                    "97372",
                    1849623813631333376,
                    5803519916432603136,
                    "59301",
                    "37862",
                    "44983",
                    new DateTime(2016, 11, 6),
                    new DateTime(2035, 8, 23),
                    new DateTime(2026, 11, 18),
                    new DateTime(2024, 1, 3),
                    "95097",
                    "40489",
                    "32995",
                    new DateTime(2037, 10, 15),
                    "54911",
                    "97749",
                    "65587",
                    new DateTime(2022, 10, 23),
                    "68307",
                    "95138",
                    new DateTime(2015, 10, 22),
                    "73734",
                    "10513",
                    "65512",
                    "58018",
                    "43692",
                    new DateTime(2043, 6, 2),
                },
                new PrescriberRow
                {
                    PrescriberKey = 5662335694648055808,
                    PrescriberNum = 414235818378563584,
                    ActivePrescriberNum = 5828824191277391872,
                    TitleAbbr = "14551",
                    FirstName = "31252",
                    MiddleName = "22834",
                    LastName = "89167",
                    SuffixAbbr = "80918",
                    GenderCode = "48451",
                    Status = "52127",
                    AmaActivity = "35612",
                    InactiveCode = "99483",
                    PrefGeneric = "71290",
                    PrefTherSub = "97350",
                    CreateUserKey = 6623578280628560896,
                    AddSource = "97372",
                    SupvPrsNum = 1849623813631333376,
                    UniqPrsNum = 5803519916432603136,
                    PrescriberId = "59301",
                    EsvMatch = "37862",
                    IsEsvValid = "44983",
                    CreateDate = new DateTime(2016, 11, 6),
                    BirthDate = new DateTime(2035, 8, 23),
                    DeceasedDate = new DateTime(2026, 11, 18),
                    InactiveDate = new DateTime(2024, 1, 3),
                    PpiEnabled = "95097",
                    Npi = "40489",
                    NpiBilling = "32995",
                    NpiExpireDate = new DateTime(2037, 10, 15),
                    StateLicNum = "54911",
                    StateLicBilling = "97749",
                    LicenseState = "65587",
                    StateLicExpireDate = new DateTime(2022, 10, 23),
                    DeaNum = "68307",
                    DeaBilling = "95138",
                    DeaExpireDate = new DateTime(2015, 10, 22),
                    MedicaidNum = "73734",
                    FedtaxidNum = "10513",
                    StateissueNum = "65512",
                    NarcdeaNum = "58018",
                    NcpdpNum = "43692",
                    LastUpdate = new DateTime(2043, 6, 2),
                }
            ));

            AddRow((
                new object?[] {
                    5662335694648055808,
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
                    null,
                    null,
                    null,
                    "97372",
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
                    "32995",
                    null,
                    null,
                    "97749",
                    null,
                    null,
                    null,
                    "95138",
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                },
                new PrescriberRow
                {
                    PrescriberKey = 5662335694648055808,
                    PrescriberNum = null,
                    ActivePrescriberNum = null,
                    TitleAbbr = null,
                    FirstName = null,
                    MiddleName = null,
                    LastName = null,
                    SuffixAbbr = null,
                    GenderCode = null,
                    Status = null,
                    AmaActivity = null,
                    InactiveCode = null,
                    PrefGeneric = null,
                    PrefTherSub = null,
                    CreateUserKey = null,
                    AddSource = "97372",
                    SupvPrsNum = null,
                    UniqPrsNum = null,
                    PrescriberId = null,
                    EsvMatch = null,
                    IsEsvValid = null,
                    CreateDate = null,
                    BirthDate = null,
                    DeceasedDate = null,
                    InactiveDate = null,
                    PpiEnabled = null,
                    Npi = null,
                    NpiBilling = "32995",
                    NpiExpireDate = null,
                    StateLicNum = null,
                    StateLicBilling = "97749",
                    LicenseState = null,
                    StateLicExpireDate = null,
                    DeaNum = null,
                    DeaBilling = "95138",
                    DeaExpireDate = null,
                    MedicaidNum = null,
                    FedtaxidNum = null,
                    StateissueNum = null,
                    NarcdeaNum = null,
                    NcpdpNum = null,
                    LastUpdate = null,
                }
            ));
        }
    }
}
