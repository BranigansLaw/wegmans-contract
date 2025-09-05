using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class PrescriberAddressQueryTest
{
    /// <summary>
    /// Tests that <see cref="PrescriberAddressQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(PrescriberAddressQueryTest.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], PrescriberAddressRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, PrescriberAddressRow expectedResult) = testParameters;
        PrescriberAddressQuery query = new()
        {
            RunDate = DateOnly.FromDateTime(DateTime.Now)
        };

        DbDataReader mockedReader = Substitute.For<DbDataReader>();
        Util.SetupNullableReturn(mockedReader, readerIndex, 0);
        mockedReader.GetFieldValue<long>(1).Returns(readerIndex[1]);
        mockedReader.GetFieldValue<long>(2).Returns(readerIndex[2]);
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
        mockedReader.GetString(16).Returns(readerIndex[16]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 17);
        mockedReader.GetString(18).Returns(readerIndex[18]);
        mockedReader.GetString(19).Returns(readerIndex[19]);
        mockedReader.GetString(20).Returns(readerIndex[20]);
        mockedReader.GetString(21).Returns(readerIndex[21]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 22);
        mockedReader.GetString(23).Returns(readerIndex[23]);
        mockedReader.GetString(24).Returns(readerIndex[24]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 25);
        mockedReader.GetString(26).Returns(readerIndex[26]);
        mockedReader.GetString(27).Returns(readerIndex[27]);
        mockedReader.GetString(28).Returns(readerIndex[28]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 29);
        mockedReader.GetString(30).Returns(readerIndex[30]);
        mockedReader.GetString(31).Returns(readerIndex[31]);
        mockedReader.GetString(32).Returns(readerIndex[32]);
        mockedReader.GetString(33).Returns(readerIndex[33]);
        mockedReader.GetString(34).Returns(readerIndex[34]);
        mockedReader.GetString(35).Returns(readerIndex[35]);
        mockedReader.GetString(36).Returns(readerIndex[36]);
        mockedReader.GetString(37).Returns(readerIndex[37]);
        mockedReader.GetString(38).Returns(readerIndex[38]);
        mockedReader.GetString(39).Returns(readerIndex[39]);
        mockedReader.GetString(40).Returns(readerIndex[40]);
        mockedReader.GetString(41).Returns(readerIndex[41]);
        mockedReader.GetString(42).Returns(readerIndex[42]);
        mockedReader.GetString(43).Returns(readerIndex[43]);
        mockedReader.GetString(44).Returns(readerIndex[44]);
        mockedReader.GetString(45).Returns(readerIndex[45]);
        mockedReader.GetFieldValue<DateTime>(46).Returns(readerIndex[46]);

        // Act
        PrescriberAddressRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], PrescriberAddressRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    859764942213154816,
                    4690601834349719552,
                    1354626351663533056,
                    "41396",
                    "83925",
                    "60815",
                    "61793",
                    "89458",
                    "69122",
                    "36679",
                    "21450",
                    "28348",
                    "63640",
                    "38772",
                    "57945",
                    "84720",
                    "95569",
                    5568290508952514560,
                    "70243",
                    "17813",
                    "18716",
                    "46820",
                    new DateTime(2015, 4, 12),
                    "95582",
                    "95032",
                    new DateTime(2018, 3, 10),
                    "74035",
                    "28699",
                    "12849",
                    new DateTime(2013, 1, 16),
                    "76898",
                    "79864",
                    "21663",
                    "73050",
                    "27995",
                    "40561",
                    "21582",
                    "96458",
                    "31790",
                    "90580",
                    "95191",
                    "84915",
                    "97347",
                    "68295",
                    "21386",
                    "34790",
                    new DateTime(2023, 11, 24),
                },
                new PrescriberAddressRow
                {
                    PrescriberKey = 859764942213154816,
                    PadrKey = 4690601834349719552,
                    PrescribAddrNum = 1354626351663533056,
                    AddrType = "41396",
                    AddressOne = "83925",
                    AddressTwo = "60815",
                    AddrCity = "61793",
                    Email = "89458",
                    WebAddr = "69122",
                    State = "36679",
                    Zipcode = "21450",
                    ZipExt = "28348",
                    County = "63640",
                    IsDefault = "38772",
                    AddSource = "57945",
                    DrugSched = "84720",
                    PracticeName = "95569",
                    AddrIdNum = 5568290508952514560,
                    PrefContact = "70243",
                    AddrStatus = "17813",
                    Npi = "18716",
                    NpiBilling = "46820",
                    NpiExpireDate = new DateTime(2015, 4, 12),
                    StateLicNum = "95582",
                    StateLicBilling = "95032",
                    StateLicExpireDate = new DateTime(2018, 3, 10),
                    LicenseState = "74035",
                    DeaNum = "28699",
                    DeaBilling = "12849",
                    DeaExpireDate = new DateTime(2013, 1, 16),
                    AreaCodePrim = "76898",
                    PhoneNumPrim = "79864",
                    ExtPrim = "21663",
                    AreaCodeSec = "73050",
                    PhoneNumSec = "27995",
                    ExtSec = "40561",
                    AreaCodeFax = "21582",
                    PhoneNumFax = "96458",
                    ExtFax = "31790",
                    AreaCodeOther = "90580",
                    PhoneNumOther = "95191",
                    ExtOther = "84915",
                    PhoneTypeOther2 = "97347",
                    AreaCodeOther2 = "68295",
                    PhoneNumOther2 = "21386",
                    ExtOther2 = "34790",
                    LastUpdate = new DateTime(2023, 11, 24),
                }
            ));

            AddRow((
                new object?[] {
                    null,
                    4690601834349719552,
                    1354626351663533056,
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
                    "38772",
                    "57945",
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    "46820",
                    null,
                    null,
                    "95032",
                    null,
                    null,
                    null,
                    "12849",
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
                    null,
                    null,
                    null,
                    new DateTime(2023, 11, 24),
                },
                new PrescriberAddressRow
                {
                    PrescriberKey = null,
                    PadrKey = 4690601834349719552,
                    PrescribAddrNum = 1354626351663533056,
                    AddrType = null,
                    AddressOne = null,
                    AddressTwo = null,
                    AddrCity = null,
                    Email = null,
                    WebAddr = null,
                    State = null,
                    Zipcode = null,
                    ZipExt = null,
                    County = null,
                    IsDefault = "38772",
                    AddSource = "57945",
                    DrugSched = null,
                    PracticeName = null,
                    AddrIdNum = null,
                    PrefContact = null,
                    AddrStatus = null,
                    Npi = null,
                    NpiBilling = "46820",
                    NpiExpireDate = null,
                    StateLicNum = null,
                    StateLicBilling = "95032",
                    StateLicExpireDate = null,
                    LicenseState = null,
                    DeaNum = null,
                    DeaBilling = "12849",
                    DeaExpireDate = null,
                    AreaCodePrim = null,
                    PhoneNumPrim = null,
                    ExtPrim = null,
                    AreaCodeSec = null,
                    PhoneNumSec = null,
                    ExtSec = null,
                    AreaCodeFax = null,
                    PhoneNumFax = null,
                    ExtFax = null,
                    AreaCodeOther = null,
                    PhoneNumOther = null,
                    ExtOther = null,
                    PhoneTypeOther2 = null,
                    AreaCodeOther2 = null,
                    PhoneNumOther2 = null,
                    ExtOther2 = null,
                    LastUpdate = new DateTime(2023, 11, 24),
                }
            ));
        }
    }
}
