using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class DailyDrugQueryTests
{
    /// <summary>
    /// Tests that <see cref="DailyDrugQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(DailyDrugQueryTests.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], DailyDrugRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, DailyDrugRow expectedResult) = testParameters;
        DailyDrugQuery query = new();

        DbDataReader mockedReader = Substitute.For<DbDataReader>();
        mockedReader.GetString(0).Returns(readerIndex[0]);
        mockedReader.GetString(1).Returns(readerIndex[1]);
        mockedReader.GetString(2).Returns(readerIndex[2]);
        mockedReader.GetString(3).Returns(readerIndex[3]);
        mockedReader.GetString(4).Returns(readerIndex[4]);
        mockedReader.GetString(5).Returns(readerIndex[5]);
        mockedReader.GetString(6).Returns(readerIndex[6]);
        mockedReader.GetString(7).Returns(readerIndex[7]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 8);
        Util.SetupNullableReturn(mockedReader, readerIndex, 9);
        mockedReader.GetString(10).Returns(readerIndex[10]);
        mockedReader.GetString(11).Returns(readerIndex[11]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 12);
        mockedReader.GetString(13).Returns(readerIndex[13]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 14);
        Util.SetupNullableReturn(mockedReader, readerIndex, 15);
        mockedReader.GetString(16).Returns(readerIndex[16]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 17);
        mockedReader.GetString(18).Returns(readerIndex[18]);
        mockedReader.GetString(19).Returns(readerIndex[19]);
        mockedReader.GetString(20).Returns(readerIndex[20]);
        mockedReader.GetString(21).Returns(readerIndex[21]);
        mockedReader.GetString(22).Returns(readerIndex[22]);
        mockedReader.GetString(23).Returns(readerIndex[23]);
        mockedReader.GetString(24).Returns(readerIndex[24]);
        mockedReader.GetString(25).Returns(readerIndex[25]);
        mockedReader.GetString(26).Returns(readerIndex[26]);
        mockedReader.GetString(27).Returns(readerIndex[27]);
        mockedReader.GetString(28).Returns(readerIndex[28]);
        mockedReader.GetString(29).Returns(readerIndex[29]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 30);
        mockedReader.GetString(31).Returns(readerIndex[31]);
        mockedReader.GetString(32).Returns(readerIndex[32]);
        mockedReader.GetString(33).Returns(readerIndex[33]);
        mockedReader.GetString(34).Returns(readerIndex[34]);
        mockedReader.GetString(35).Returns(readerIndex[35]);
        mockedReader.GetString(36).Returns(readerIndex[36]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 37);
        mockedReader.GetString(38).Returns(readerIndex[38]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 39);
        mockedReader.GetString(40).Returns(readerIndex[40]);
        mockedReader.GetString(41).Returns(readerIndex[41]);
        mockedReader.GetString(42).Returns(readerIndex[42]);
        mockedReader.GetString(43).Returns(readerIndex[43]);
        mockedReader.GetString(44).Returns(readerIndex[44]);
        mockedReader.GetString(45).Returns(readerIndex[45]);
        mockedReader.GetString(46).Returns(readerIndex[46]);
        mockedReader.GetString(47).Returns(readerIndex[47]);
        mockedReader.GetString(48).Returns(readerIndex[48]);
        mockedReader.GetString(49).Returns(readerIndex[49]);
        mockedReader.GetString(50).Returns(readerIndex[50]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 51);
        mockedReader.GetString(52).Returns(readerIndex[52]);
        mockedReader.GetString(53).Returns(readerIndex[53]);
        mockedReader.GetString(54).Returns(readerIndex[54]);
        mockedReader.GetString(55).Returns(readerIndex[55]);
        mockedReader.GetString(56).Returns(readerIndex[56]);
        mockedReader.GetString(57).Returns(readerIndex[57]);
        mockedReader.GetString(58).Returns(readerIndex[58]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 59);
        Util.SetupNullableReturn(mockedReader, readerIndex, 60);
        mockedReader.GetString(61).Returns(readerIndex[61]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 62);
        mockedReader.GetString(63).Returns(readerIndex[63]);
        mockedReader.GetString(64).Returns(readerIndex[64]);
        mockedReader.GetString(65).Returns(readerIndex[65]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 66);
        Util.SetupNullableReturn(mockedReader, readerIndex, 67);
        Util.SetupNullableReturn(mockedReader, readerIndex, 68);
        Util.SetupNullableReturn(mockedReader, readerIndex, 69);
        Util.SetupNullableReturn(mockedReader, readerIndex, 70);
        Util.SetupNullableReturn(mockedReader, readerIndex, 71);
        Util.SetupNullableReturn(mockedReader, readerIndex, 72);
        Util.SetupNullableReturn(mockedReader, readerIndex, 73);
        Util.SetupNullableReturn(mockedReader, readerIndex, 74);
        mockedReader.GetString(75).Returns(readerIndex[75]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 76);
        mockedReader.GetString(77).Returns(readerIndex[77]);
        mockedReader.GetString(78).Returns(readerIndex[78]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 79);

        // Act
        DailyDrugRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], DailyDrugRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    "40717",
                    "86218",
                    "88524",
                    "34013",
                    "74981",
                    "75727",
                    "87295",
                    "28316",
                    17020M,
                    11140M,
                    "22436",
                    "33385",
                    5595600755350197248,
                    "46445",
                    1338969932069342208,
                    968658093915076608,
                    "13251",
                    3805963607271519232,
                    "69457",
                    "87583",
                    "40796",
                    "38835",
                    "78688",
                    "71894",
                    "64423",
                    "57339",
                    "84522",
                    "67659",
                    "89148",
                    "57863",
                    2745155820062556160,
                    "27831",
                    "46758",
                    "69012",
                    "69930",
                    "85660",
                    "33876",
                    2132937799872,
                    "71559",
                    1529400444444,
                    "58082",
                    "75663",
                    "58456",
                    "69956",
                    "56308",
                    "93652",
                    "66009",
                    "61586",
                    "52300",
                    "99115",
                    "78580",
                    127384959287294832,
                    "32650",
                    "34233",
                    "38545",
                    "54923",
                    "79762",
                    "30383",
                    "50023",
                    94164M,
                    71125M,
                    "70025",
                    40489M,
                    "26073",
                    "46620",
                    "94062",
                    29478M,
                    71010M,
                    80462M,
                    new DateTime(2012, 4, 16),
                    new DateTime(2048, 6, 19),
                    new DateTime(2017, 3, 20),
                    new DateTime(2028, 6, 16),
                    21329842343779,
                    98471M,
                    "31148",
                    98267M,
                    "dateTime",
                    "47636",
                    20305M,
                },
                new DailyDrugRow
                {
                    Ndc = "40717",
                    DrugName = "86218",
                    Manufacturer = "88524",
                    MfrNum = "34013",
                    Strength = "74981",
                    StrengthUnits = "75727",
                    UnitDoseFlag = "87295",
                    UnitOfUseFlag = "28316",
                    PackageQty = 17020M,
                    PackSize = 11140M,
                    InnerPack = "22436",
                    OuterPack = "33385",
                    CaseSize = 5595600755350197248,
                    Unit = "46445",
                    DispensingUnits = 1338969932069342208,
                    DrugShipper = 968658093915076608,
                    PackDesc = "13251",
                    OrderNum = 3805963607271519232,
                    PrevNdc = "69457",
                    ReplacementNdc = "87583",
                    ProdSourceInd = "40796",
                    FdbAdded = "38835",
                    DateAdded = "78688",
                    ObsoleteDate = "71894",
                    DeactivateDate = "64423",
                    LastProviderUpdate = "57339",
                    MaintenanceDrugFlag = "84522",
                    GenericName = "67659",
                    Gcn = "89148",
                    GcnSeqNum = "57863",
                    DrugUpc = 2745155820062556160,
                    Sdgi = "27831",
                    SdgiOverride = "46758",
                    DrugSchedule = "69012",
                    DeaClass = "69930",
                    PriceMaintained = "85660",
                    GroupName = "33876",
                    PgPrdProductKey = 2132937799872,
                    PgMemberStatus = "71559",
                    GsGrpGroupNumber = 1529400444444,
                    Decile = "58082",
                    AhfsTherapClass = "75663",
                    AhfsTherClassDesShort = "58456",
                    AhfsTherClassDesLong = "69956",
                    SigVerb = "56308",
                    SigVerbOverride = "93652",
                    SigRoute = "66009",
                    SigRouteOverride = "61586",
                    SigUnit = "52300",
                    SigUnitOverride = "99115",
                    DosageForm = "78580",
                    DesiIndicator = 127384959287294832,
                    OrangeBookCode = "32650",
                    OrangeBookCodeOverride = "34233",
                    DefaultDaw = "38545",
                    WarehouseFlag = "54923",
                    OriginatorInnovator = "79762",
                    EnhancedRefillOptional = "30383",
                    GenSubPackRestriction = "50023",
                    MinDispQty = 94164M,
                    Bbawp = 71125M,
                    Distributor = "70025",
                    BbawpOverride = 40489M,
                    CompoundFlag = "26073",
                    AlternateLabel = "46620",
                    BlockedProductFlag = "94062",
                    RawPcfrCost = 29478M,
                    RawPcfcCost = 71010M,
                    RawPcfnCost = 80462M,
                    UserDeffEffectiveEndDate = new DateTime(2012, 4, 16),
                    NteEffectiveEndDate = new DateTime(2048, 6, 19),
                    ConEffectiveEndDate = new DateTime(2017, 3, 20),
                    RepackConEffectiveEndDate = new DateTime(2028, 6, 16),
                    PPrdProductKey = 21329842343779,
                    Cost = 98471M,
                    CostManuallyMaintained = "31148",
                    PercentModifier = 98267M,
                    CostBasisEffDate = "dateTime",
                    OtcType = "47636",
                    UnitCost = 20305M,
                }
            ));

            AddRow((
                new object?[] {
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
                    null,
                    null,
                    null,
                    1529400444444,
                    null,
                    null,
                    "58082",
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
                    213293774442319,
                    null,
                    null,
                    null,
                    "dateTime",
                    "47636",
                    null,
                },
                new DailyDrugRow
                {
                    Ndc = null,
                    DrugName = null,
                    Manufacturer = null,
                    MfrNum = null,
                    Strength = null,
                    StrengthUnits = null,
                    UnitDoseFlag = null,
                    UnitOfUseFlag = null,
                    PackageQty = null,
                    PackSize = null,
                    InnerPack = null,
                    OuterPack = null,
                    CaseSize = null,
                    Unit = null,
                    DispensingUnits = null,
                    DrugShipper = null,
                    PackDesc = null,
                    OrderNum = null,
                    PrevNdc = null,
                    ReplacementNdc = null,
                    ProdSourceInd = null,
                    FdbAdded = null,
                    DateAdded = null,
                    ObsoleteDate = null,
                    DeactivateDate = null,
                    LastProviderUpdate = null,
                    MaintenanceDrugFlag = null,
                    GenericName = null,
                    Gcn = null,
                    GcnSeqNum = null,
                    DrugUpc = null,
                    Sdgi = null,
                    SdgiOverride = null,
                    DrugSchedule = null,
                    DeaClass = null,
                    PriceMaintained = null,
                    GroupName = null,
                    PgPrdProductKey = 1529400444444,
                    PgMemberStatus = null,
                    GsGrpGroupNumber = null,
                    Decile = "58082",
                    AhfsTherapClass = null,
                    AhfsTherClassDesShort = null,
                    AhfsTherClassDesLong = null,
                    SigVerb = null,
                    SigVerbOverride = null,
                    SigRoute = null,
                    SigRouteOverride = null,
                    SigUnit = null,
                    SigUnitOverride = null,
                    DosageForm = null,
                    DesiIndicator = null,
                    OrangeBookCode = null,
                    OrangeBookCodeOverride = null,
                    DefaultDaw = null,
                    WarehouseFlag = null,
                    OriginatorInnovator = null,
                    EnhancedRefillOptional = null,
                    GenSubPackRestriction = null,
                    MinDispQty = null,
                    Bbawp = null,
                    Distributor = null,
                    BbawpOverride = null,
                    CompoundFlag = null,
                    AlternateLabel = null,
                    BlockedProductFlag = null,
                    RawPcfrCost = null,
                    RawPcfcCost = null,
                    RawPcfnCost = null,
                    UserDeffEffectiveEndDate = null,
                    NteEffectiveEndDate = null,
                    ConEffectiveEndDate = null,
                    RepackConEffectiveEndDate = null,
                    PPrdProductKey = 213293774442319,
                    Cost = null,
                    CostManuallyMaintained = null,
                    PercentModifier = null,
                    CostBasisEffDate = "dateTime",
                    OtcType = "47636",
                    UnitCost = null,
                }
            ));
        }
    }
}
