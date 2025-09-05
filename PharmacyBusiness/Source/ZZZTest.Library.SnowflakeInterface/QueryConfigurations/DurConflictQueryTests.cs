using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class DurConflictQueryTests
{
    /// <summary>
    /// Tests that <see cref="DurConflictQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(DurConflictQueryTests.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], DurConflictRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, DurConflictRow expectedResult) = testParameters;
        DurConflictQuery query = new()
        {
            RunDate = DateOnly.FromDateTime(DateTime.Now)
        };

        DbDataReader mockedReader = Substitute.For<DbDataReader>();
        mockedReader.GetString(0).Returns(readerIndex[0]);
        mockedReader.GetString(1).Returns(readerIndex[1]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 2);
        Util.SetupNullableReturn(mockedReader, readerIndex, 3);
        Util.SetupNullableReturn(mockedReader, readerIndex, 4);
        Util.SetupNullableReturn(mockedReader, readerIndex, 5);
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
        mockedReader.GetString(17).Returns(readerIndex[17]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 18);
        mockedReader.GetString(19).Returns(readerIndex[19]);
        mockedReader.GetString(20).Returns(readerIndex[20]);
        mockedReader.GetString(21).Returns(readerIndex[21]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 22);
        Util.SetupNullableReturn(mockedReader, readerIndex, 23);
        Util.SetupNullableReturn(mockedReader, readerIndex, 24);
        Util.SetupNullableReturn(mockedReader, readerIndex, 25);

        // Act
        DurConflictRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], DurConflictRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    "36270",
                    "73142",
                    8550444167746989056,
                    6797347674885293056,
                    new DateTime(2046, 9, 24),
                    3673866345644522496,
                    "73115",
                    "65825",
                    "20301",
                    "34176",
                    "26841",
                    "60029",
                    "83180",
                    "63625",
                    "23216",
                    "52173",
                    "94176",
                    "45267",
                    2503951164593591296,
                    "34382",
                    "35862",
                    "50636",
                    7604909139423627264,
                    2475043406360137728,
                    6106460841148602368,
                    4962855725795401728,
                },
                new DurConflictRow
                {
                    StoreNumber = "36270",
                    RxNumber = "73142",
                    RefillNumber = 8550444167746989056,
                    PartSeqNumber = 6797347674885293056,
                    DurDate = new DateTime(2046, 9, 24),
                    PatientNumber = 3673866345644522496,
                    NdcWo = "73115",
                    DrugName = "65825",
                    Sdgi = "20301",
                    Gcn = "34176",
                    GcnSequenceNumber = "26841",
                    DeaClass = "60029",
                    ConflictCode = "83180",
                    ConflictDesc = "63625",
                    ConflictType = "23216",
                    SeverityDesc = "52173",
                    ResultOfService = "94176",
                    ProfService = "45267",
                    LevelOfEffort = 2503951164593591296,
                    ReasonForService = "34382",
                    IsCritical = "35862",
                    IsException = "50636",
                    RxFillSequence = 7604909139423627264,
                    RxRecordNumber = 2475043406360137728,
                    PrescriberKey = 6106460841148602368,
                    UserKey = 4962855725795401728,
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
                new DurConflictRow
                {
                    StoreNumber = null,
                    RxNumber = null,
                    RefillNumber = null,
                    PartSeqNumber = null,
                    DurDate = null,
                    PatientNumber = null,
                    NdcWo = null,
                    DrugName = null,
                    Sdgi = null,
                    Gcn = null,
                    GcnSequenceNumber = null,
                    DeaClass = null,
                    ConflictCode = null,
                    ConflictDesc = null,
                    ConflictType = null,
                    SeverityDesc = null,
                    ResultOfService = null,
                    ProfService = null,
                    LevelOfEffort = null,
                    ReasonForService = null,
                    IsCritical = null,
                    IsException = null,
                    RxFillSequence = null,
                    RxRecordNumber = null,
                    PrescriberKey = null,
                    UserKey = null,
                }
            ));
        }
    }
}
