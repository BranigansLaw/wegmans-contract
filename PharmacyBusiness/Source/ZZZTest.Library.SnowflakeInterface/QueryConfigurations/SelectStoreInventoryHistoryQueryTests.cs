using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class SelectStoreInventoryHistoryQueryTests
{
    /// <summary>
    /// Tests that <see cref="SelectStoreInventoryHistoryQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(SelectStoreInventoryHistoryQueryTests.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], SelectStoreInventoryHistoryRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, SelectStoreInventoryHistoryRow expectedResult) = testParameters;
        SelectStoreInventoryHistoryQuery query = new()
        {
            RunDate = DateOnly.FromDateTime(DateTime.Now)
        };

        DbDataReader mockedReader = Substitute.For<DbDataReader>();
        Util.SetupNullableReturn(mockedReader, readerIndex, 0);
        mockedReader.GetString(1).Returns(readerIndex[1]);
        mockedReader.GetString(2).Returns(readerIndex[2]);
        mockedReader.GetString(3).Returns(readerIndex[3]);
        mockedReader.GetString(4).Returns(readerIndex[4]);
        mockedReader.GetString(5).Returns(readerIndex[5]);
        mockedReader.GetString(6).Returns(readerIndex[6]);
        mockedReader.GetString(7).Returns(readerIndex[7]);
        mockedReader.GetString(8).Returns(readerIndex[8]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 9);
        Util.SetupNullableReturn(mockedReader, readerIndex, 10);
        mockedReader.GetString(11).Returns(readerIndex[11]);
        mockedReader.GetString(12).Returns(readerIndex[12]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 13);
        Util.SetupNullableReturn(mockedReader, readerIndex, 14);
        Util.SetupNullableReturn(mockedReader, readerIndex, 15);
        Util.SetupNullableReturn(mockedReader, readerIndex, 16);
        Util.SetupNullableReturn(mockedReader, readerIndex, 17);
        Util.SetupNullableReturn(mockedReader, readerIndex, 18);
        Util.SetupNullableReturn(mockedReader, readerIndex, 19);
        Util.SetupNullableReturn(mockedReader, readerIndex, 20);
        Util.SetupNullableReturn(mockedReader, readerIndex, 21);
        Util.SetupNullableReturn(mockedReader, readerIndex, 22);
        Util.SetupNullableReturn(mockedReader, readerIndex, 23);
        mockedReader.GetString(24).Returns(readerIndex[24]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 25);

        // Act
        SelectStoreInventoryHistoryRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], SelectStoreInventoryHistoryRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    new DateTime(2031, 2, 18),
                    "59578",
                    "44152",
                    "23953",
                    "44933",
                    "23603",
                    "17399",
                    "34093",
                    "17759",
                    10338M,
                    97939M,
                    "54784",
                    "72537",
                    51823M,
                    83758M,
                    new DateTime(2045, 4, 22),
                    14525M,
                    78487M,
                    93573M,
                    86986M,
                    25345M,
                    37425M,
                    59272M,
                    30981M,
                    "36234",
                    new DateTime(2022, 7, 19),
                },
                new SelectStoreInventoryHistoryRow
                {
                    DateOfService = new DateTime(2031, 2, 18),
                    StoreNum = "59578",
                    NdcWithoutDashes = "44152",
                    DrugName = "23953",
                    Sdgi = "44933",
                    Gcn = "23603",
                    GcnSeqNum = "17399",
                    OrangeBookCode = "34093",
                    FormCode = "17759",
                    PackSize = 10338M,
                    TruePack = 97939M,
                    Pm = "54784",
                    IsPreferred = "72537",
                    LastAcqCostPack = 51823M,
                    LastAcqCostUnit = 83758M,
                    LastAcqCostDate = new DateTime(2045, 4, 22),
                    OnHandQty = 14525M,
                    OnHandValue = 78487M,
                    CommitedQty = 93573M,
                    CommitedValue = 86986M,
                    TotalQty = 25345M,
                    TotalValue = 37425M,
                    AcqCostPack = 59272M,
                    AcqCostUnit = 30981M,
                    PrimarySupplier = "36234",
                    LastChangeDate = new DateTime(2022, 7, 19),
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
                },
                new SelectStoreInventoryHistoryRow
                {
                    DateOfService = null,
                    StoreNum = null,
                    NdcWithoutDashes = null,
                    DrugName = null,
                    Sdgi = null,
                    Gcn = null,
                    GcnSeqNum = null,
                    OrangeBookCode = null,
                    FormCode = null,
                    PackSize = null,
                    TruePack = null,
                    Pm = null,
                    IsPreferred = null,
                    LastAcqCostPack = null,
                    LastAcqCostUnit = null,
                    LastAcqCostDate = null,
                    OnHandQty = null,
                    OnHandValue = null,
                    CommitedQty = null,
                    CommitedValue = null,
                    TotalQty = null,
                    TotalValue = null,
                    AcqCostPack = null,
                    AcqCostUnit = null,
                    PrimarySupplier = null,
                    LastChangeDate = null,
                }
            ));
        }
    }
}
