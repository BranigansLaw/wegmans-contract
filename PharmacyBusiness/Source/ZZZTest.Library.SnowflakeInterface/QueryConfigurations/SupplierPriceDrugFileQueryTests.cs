using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class SupplierPriceDrugFileQueryTests
{
    /// <summary>
    /// Tests that <see cref="SupplierPriceDrugFileQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(SupplierPriceDrugFileQueryTests.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], SupplierPriceDrugFileRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, SupplierPriceDrugFileRow expectedResult) = testParameters;
        SupplierPriceDrugFileQuery query = new()
        {
            RunDate = DateOnly.FromDateTime(DateTime.Now)
        };

        DbDataReader mockedReader = Substitute.For<DbDataReader>();
        mockedReader.GetFieldValue<DateTime>(0).Returns(readerIndex[0]);
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
        Util.SetupNullableReturn(mockedReader, readerIndex, 11);
        mockedReader.GetString(12).Returns(readerIndex[12]);
        mockedReader.GetString(13).Returns(readerIndex[13]);
        mockedReader.GetString(14).Returns(readerIndex[14]);
        mockedReader.GetString(15).Returns(readerIndex[15]);
        mockedReader.GetString(16).Returns(readerIndex[16]);
        mockedReader.GetString(17).Returns(readerIndex[17]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 18);
        Util.SetupNullableReturn(mockedReader, readerIndex, 19);
        Util.SetupNullableReturn(mockedReader, readerIndex, 20);
        Util.SetupNullableReturn(mockedReader, readerIndex, 21);
        Util.SetupNullableReturn(mockedReader, readerIndex, 22);
        mockedReader.GetString(23).Returns(readerIndex[23]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 24);

        // Act
        SupplierPriceDrugFileRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], SupplierPriceDrugFileRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    new DateTime(2015, 5, 13),
                    "34597",
                    "15284",
                    "58490",
                    "28930",
                    "85071",
                    "68972",
                    "76861",
                    "61137",
                    "90077",
                    "60736",
                    56997M,
                    "63963",
                    "54055",
                    "17485",
                    "26854",
                    "46373",
                    "12047",
                    15710M,
                    69252M,
                    new DateTime(2011, 1, 2),
                    8983423947424181248,
                    85756M,
                    "17731",
                    new DateTime(2038, 3, 12),
                },
                new SupplierPriceDrugFileRow
                {
                    DateOfService = new DateTime(2015, 5, 13),
                    SupplierName = "34597",
                    VendorItemNumber = "15284",
                    Ndc = "58490",
                    NdcWo = "28930",
                    DrugName = "85071",
                    DrugForm = "68972",
                    DeaClass = "76861",
                    Strength = "61137",
                    StrengthUnit = "90077",
                    GenericName = "60736",
                    PackSize = 56997M,
                    IsMaintDrug = "63963",
                    Sdgi = "54055",
                    Gcn = "17485",
                    GcnSeqNumber = "26854",
                    DrugManufacturer = "46373",
                    OrangeBookCode = "12047",
                    PackPrice = 15710M,
                    PricePerUnit = 69252M,
                    UnitPriceDate = new DateTime(2011, 1, 2),
                    PurchIncr = 8983423947424181248,
                    PkgSizeIncr = 85756M,
                    Status = "17731",
                    EffStartDate = new DateTime(2038, 3, 12),
                }
            ));

            AddRow((
                new object?[] {
                    new DateTime(2015, 5, 13),
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
                },
                new SupplierPriceDrugFileRow
                {
                    DateOfService = new DateTime(2015, 5, 13),
                    SupplierName = null,
                    VendorItemNumber = null,
                    Ndc = null,
                    NdcWo = null,
                    DrugName = null,
                    DrugForm = null,
                    DeaClass = null,
                    Strength = null,
                    StrengthUnit = null,
                    GenericName = null,
                    PackSize = null,
                    IsMaintDrug = null,
                    Sdgi = null,
                    Gcn = null,
                    GcnSeqNumber = null,
                    DrugManufacturer = null,
                    OrangeBookCode = null,
                    PackPrice = null,
                    PricePerUnit = null,
                    UnitPriceDate = null,
                    PurchIncr = null,
                    PkgSizeIncr = null,
                    Status = null,
                    EffStartDate = null,
                }
            ));
        }
    }
}
