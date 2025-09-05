using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class PatientAddressesQueryTests
{
    /// <summary>
    /// Tests that <see cref="PatientAddressesQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(PatientAddressesQueryTests.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], PatientAddressesRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, PatientAddressesRow expectedResult) = testParameters;
        PatientAddressesQuery query = new()
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
        mockedReader.GetFieldValue<long>(15).Returns(readerIndex[15]);
        mockedReader.GetFieldValue<long>(16).Returns(readerIndex[16]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 17);
        Util.SetupNullableReturn(mockedReader, readerIndex, 18);

        // Act
        PatientAddressesRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], PatientAddressesRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    3106925010907815936,
                    4352863936605522944,
                    "78520",
                    "44615",
                    "11267",
                    "46635",
                    "53402",
                    "17948",
                    "54715",
                    "11501",
                    "73821",
                    "40358",
                    "60249",
                    "14996",
                    "59768",
                    3870717677062018048,
                    2702652165315347456,
                    4326738957791519744,
                    1507930684442115072,
                },
                new PatientAddressesRow
                {
                    PadrKey = 3106925010907815936,
                    PatientNum = 4352863936605522944,
                    AddressOne = "78520",
                    AddressTwo = "44615",
                    AddrCity = "11267",
                    AddrState = "46635",
                    AddrZipcode = "53402",
                    AddressZipext = "17948",
                    County = "54715",
                    AddressType = "11501",
                    AddressUsage = "73821",
                    AddrStatus = "40358",
                    AreaCode = "60249",
                    PhoneNum = "14996",
                    PhoneExt = "59768",
                    AddressUpdated = 3870717677062018048,
                    PhoneUpdated = 2702652165315347456,
                    Dwaddressnum = 4326738957791519744,
                    Dwphonenum = 1507930684442115072,
                }
            ));

            AddRow((
                new object?[] {
                    3106925010907815936,
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
                    3870717677062018048,
                    2702652165315347456,
                    null,
                    null,
                },
                new PatientAddressesRow
                {
                    PadrKey = 3106925010907815936,
                    PatientNum = null,
                    AddressOne = null,
                    AddressTwo = null,
                    AddrCity = null,
                    AddrState = null,
                    AddrZipcode = null,
                    AddressZipext = null,
                    County = null,
                    AddressType = null,
                    AddressUsage = null,
                    AddrStatus = null,
                    AreaCode = null,
                    PhoneNum = null,
                    PhoneExt = null,
                    AddressUpdated = 3870717677062018048,
                    PhoneUpdated = 2702652165315347456,
                    Dwaddressnum = null,
                    Dwphonenum = null,
                }
            ));
        }
    }
}
