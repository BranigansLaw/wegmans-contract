using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class InvAdjustmentQueryTests
{
    /// <summary>
    /// Tests that <see cref="InvAdjustmentQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(InvAdjustmentQueryTests.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], InvAdjustmentRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, InvAdjustmentRow expectedResult) = testParameters;
        InvAdjustmentQuery query = new()
        {
            RunDate = DateOnly.FromDateTime(DateTime.Now)
        };

        DbDataReader mockedReader = Substitute.For<DbDataReader>();
        mockedReader.GetFieldValue<long>(0).Returns(readerIndex[0]);
        mockedReader.GetString(1).Returns(readerIndex[1]);
        mockedReader.GetString(2).Returns(readerIndex[2]);
        mockedReader.GetString(3).Returns(readerIndex[3]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 4);
        Util.SetupNullableReturn(mockedReader, readerIndex, 5);
        Util.SetupNullableReturn(mockedReader, readerIndex, 6);
        mockedReader.GetString(7).Returns(readerIndex[7]);
        mockedReader.GetFieldValue<long>(8).Returns(readerIndex[8]);
        mockedReader.GetString(9).Returns(readerIndex[9]);
        mockedReader.GetString(10).Returns(readerIndex[10]);
        mockedReader.GetString(11).Returns(readerIndex[11]);
        mockedReader.GetString(12).Returns(readerIndex[12]);
        mockedReader.GetString(13).Returns(readerIndex[13]);
        mockedReader.GetString(14).Returns(readerIndex[14]);
        mockedReader.GetString(15).Returns(readerIndex[15]);

        // Act
        InvAdjustmentRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], InvAdjustmentRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    (long) 20240702,
                    "275",
                    "Lisinprol",
                    "60012822",
                    0M,
                    90M,
                    9.3M,
                    "A020931",
                    (long) 0,
                    "68180097901",
                    "Lisinprol",
                    "John",
                    "Doe",
                    "0",
                    "68180097901",
                    "A020931"
                },
                new InvAdjustmentRow
                {
                    DateKey = 20240702,
                    StoreNumber = "275",
                    DrugLabelName = "Lisinprol",
                    DrugNDC = "60012822",
                    AdjustmentQuantity = 0,
                    AdjustmentCost = 90,
                    AdjustmentExtendedCost = 9.3M,
                    AdjustmentTypeCode = "A020931",
                    InventoryAdjustmentNumber = 0,
                    AdjustmentReasonCode = "68180097901",
                    Description = "Lisinprol",
                    SystemUserFirstName = "John",
                    SystemUserLastName = "Doe",
                    SystemUserKey = "0",
                    NDC = "68180097901",
                    ReferenceNumber = "A020931"
                }
            ));

            AddRow((
                new object[] {
                    (long) 20240228,
                    "299",
                    "LisinprolX",
                    "60012823",
                    5M,
                    0M,
                    19.3M,
                    "B020931",
                    (long) 10,
                    "78180097901",
                    "LisinprolX",
                    "Jane",
                    "Doe",
                    "1",
                    "68180097901",
                    ""
                },
                new InvAdjustmentRow
                {
                    DateKey = 20240228,
                    StoreNumber = "299",
                    DrugLabelName = "LisinprolX",
                    DrugNDC = "60012823",
                    AdjustmentQuantity = 5,
                    AdjustmentCost = 0,
                    AdjustmentExtendedCost = 19.3M,
                    AdjustmentTypeCode = "B020931",
                    InventoryAdjustmentNumber = 10,
                    AdjustmentReasonCode = "78180097901",
                    Description = "LisinprolX",
                    SystemUserFirstName = "Jane",
                    SystemUserLastName = "Doe",
                    SystemUserKey = "1",
                    NDC = "68180097901",
                    ReferenceNumber = ""
                }
            ));

            AddRow((
                new object[] {
                    (long) 20210702,
                    "1",
                    "Gummy Bears",
                    "60012822",
                    1M,
                    19.9M,
                    0M,
                    "C020931",
                    (long) 0,
                    "68180397901",
                    "Gummy Bears",
                    "John",
                    "Smith",
                    "4",
                    "68180397901",
                    "C020931"
                },
                new InvAdjustmentRow
                {
                    DateKey = 20210702,
                    StoreNumber = "1",
                    DrugLabelName = "Gummy Bears",
                    DrugNDC = "60012822",
                    AdjustmentQuantity = 1,
                    AdjustmentCost = 19.9M,
                    AdjustmentExtendedCost = 0,
                    AdjustmentTypeCode = "C020931",
                    InventoryAdjustmentNumber = 0,
                    AdjustmentReasonCode = "68180397901",
                    Description = "Gummy Bears",
                    SystemUserFirstName = "John",
                    SystemUserLastName = "Smith",
                    SystemUserKey = "4",
                    NDC = "68180397901",
                    ReferenceNumber = "C020931"
                }
            ));
        }
    }
}
