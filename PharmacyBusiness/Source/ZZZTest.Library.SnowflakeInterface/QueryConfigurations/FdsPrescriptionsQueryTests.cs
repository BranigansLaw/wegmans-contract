using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class FdsPrescriptionsQueryTests
{
    /// <summary>
    /// Tests that <see cref="FdsPrescriptionsQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(FdsPrescriptionsQueryTests.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], FdsPrescriptionsRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, FdsPrescriptionsRow expectedResult) = testParameters;
        FdsPrescriptionsQuery query = new()
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
        Util.SetupNullableReturn(mockedReader, readerIndex, 29);
        Util.SetupNullableReturn(mockedReader, readerIndex, 30);

        // Act
        FdsPrescriptionsRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], FdsPrescriptionsRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    4463974694665980928,
                    6986176624443162624,
                    "67903",
                    "53823",
                    "95256",
                    "19197",
                    "89200",
                    "94147",
                    "26329",
                    "56923",
                    "12724",
                    "46481",
                    "85473",
                    "98973",
                    "78508",
                    "31172",
                    39769M,
                    10959M,
                    "50089",
                    1689524097798563840,
                    830143761870747648,
                    41929M,
                    "30769",
                    "11616",
                    "24729",
                    "31024",
                    "26716",
                    "81986",
                    "67683",
                    22867M,
                    97216M,
                },
                new FdsPrescriptionsRow
                {
                    Version = 4463974694665980928,
                    Clientpatientid = 6986176624443162624,
                    Lastname = "67903",
                    Firstname = "53823",
                    Gender = "95256",
                    Addressline1 = "19197",
                    Addressline2 = "89200",
                    City = "94147",
                    State = "26329",
                    Zipcode = "56923",
                    Dob = "12724",
                    Email = "46481",
                    Phone = "85473",
                    Phonetype = "98973",
                    Language = "78508",
                    Ndc = "31172",
                    Qty = 39769M,
                    Dayssupply = 10959M,
                    Productname = "50089",
                    Filldate = 1689524097798563840,
                    Fillnum = 830143761870747648,
                    Authorizedrefills = 41929M,
                    Npi = "30769",
                    Bin = "11616",
                    Pcn = "24729",
                    Groupid = "31024",
                    Plan = "26716",
                    Prescribernpi = "81986",
                    Patientmbi = "67683",
                    Copay = 22867M,
                    Reimbursement = 97216M,
                }
            ));

            AddRow((
                new object?[] {
                    4463974694665980928,
                    null,
                    null,
                    null,
                    "95256",
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
                    39769M,
                    10959M,
                    null,
                    null,
                    null,
                    41929M,
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
                new FdsPrescriptionsRow
                {
                    Version = 4463974694665980928,
                    Clientpatientid = null,
                    Lastname = null,
                    Firstname = null,
                    Gender = "95256",
                    Addressline1 = null,
                    Addressline2 = null,
                    City = null,
                    State = null,
                    Zipcode = null,
                    Dob = null,
                    Email = null,
                    Phone = null,
                    Phonetype = null,
                    Language = null,
                    Ndc = null,
                    Qty = 39769M,
                    Dayssupply = 10959M,
                    Productname = null,
                    Filldate = null,
                    Fillnum = null,
                    Authorizedrefills = 41929M,
                    Npi = null,
                    Bin = null,
                    Pcn = null,
                    Groupid = null,
                    Plan = null,
                    Prescribernpi = null,
                    Patientmbi = null,
                    Copay = null,
                    Reimbursement = null,
                }
            ));
        }
    }
}
