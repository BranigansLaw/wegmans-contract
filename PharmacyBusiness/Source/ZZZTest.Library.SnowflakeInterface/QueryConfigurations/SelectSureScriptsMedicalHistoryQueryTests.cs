using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;
using ZZZZTest.TestHelpers;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class SelectSureScriptsMedicalHistoryQueryTests
{
    /// <summary>
    /// Tests that <see cref="SelectSureScriptsMedicalHistoryQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(SelectSureScriptsMedicalHistoryQueryTests.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], SelectSureScriptsMedicalHistoryRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, SelectSureScriptsMedicalHistoryRow expectedResult) = testParameters;
        SelectSureScriptsMedicalHistoryQuery query = new()
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
        Util.SetupNullableReturn(mockedReader, readerIndex, 7);
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
        Util.SetupNullableReturn(mockedReader, readerIndex, 33);
        Util.SetupNullableReturn(mockedReader, readerIndex, 34);
        Util.SetupNullableReturn(mockedReader, readerIndex, 35);
        mockedReader.GetString(36).Returns(readerIndex[36]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 37);
        Util.SetupNullableReturn(mockedReader, readerIndex, 38);
        Util.SetupNullableReturn(mockedReader, readerIndex, 39);
        Util.SetupNullableReturn(mockedReader, readerIndex, 40);
        Util.SetupNullableReturn(mockedReader, readerIndex, 41);
        mockedReader.GetFieldValue<long>(42).Returns(readerIndex[42]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 43);
        mockedReader.GetString(44).Returns(readerIndex[44]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 45);
        Util.SetupNullableReturn(mockedReader, readerIndex, 46);
        mockedReader.GetString(47).Returns(readerIndex[47]);
        mockedReader.GetString(48).Returns(readerIndex[48]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 49);

        // Act
        SelectSureScriptsMedicalHistoryRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
    }

    public class MappingTests : TheoryData<(object[], SelectSureScriptsMedicalHistoryRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                    7999713677650864128,
                    4922945581605198848,
                    "68386",
                    "68375",
                    "37852",
                    "98881",
                    "42774",
                    new DateTime(2029, 4, 8),
                    "21930",
                    "68041",
                    "55501",
                    "84151",
                    "96015",
                    "69983",
                    "86100",
                    "18083",
                    "68673",
                    "18554",
                    "55608",
                    "32995",
                    "12876",
                    "76541",
                    "27263",
                    "57537",
                    "10187",
                    "79558",
                    "78482",
                    "22175",
                    "34768",
                    "33232",
                    1535440336746799104,
                    "87297",
                    "18402",
                    69760M,
                    86533M,
                    198679793381833728,
                    "20147",
                    2795390375308362752,
                    6266464010807134208,
                    593969305756900352,
                    42829M,
                    92740M,
                    7043141740666011648,
                    4622271156660896768,
                    "73089",
                    new DateTime(2012, 9, 9),
                    2365154726322221056,
                    "22776",
                    "46411",
                    new DateTime(2021, 11, 5),
                },
                new SelectSureScriptsMedicalHistoryRow
                {
                    Recordsequencenumber = 7999713677650864128,
                    Participantpatientid = 4922945581605198848,
                    Patientlastname = "68386",
                    Patientfirstname = "68375",
                    Patientmiddlename = "37852",
                    Patientprefix = "98881",
                    Patientsufix = "42774",
                    Patientdateofbirth = new DateTime(2029, 4, 8),
                    Patientgender = "21930",
                    Patientaddress1 = "68041",
                    Patientcity = "55501",
                    Patientstate = "84151",
                    Patientzipcode = "96015",
                    Ncpdpid = "69983",
                    Chainsiteid = "86100",
                    Pharmacyname = "18083",
                    Facilityaddress1 = "68673",
                    Facilitycity = "18554",
                    Facilitystate = "55608",
                    Facilityzipcode = "32995",
                    Facilityphonenumber = "12876",
                    Fprimarycareproviderlastname = "76541",
                    Primarycareproviderfirstname = "27263",
                    Primarycareprovideraddress1 = "57537",
                    Primarycareprovidercity = "10187",
                    Primarycareproviderstate = "79558",
                    Primarycareproviderzipcode = "78482",
                    Primarycareproviderareacode = "22175",
                    Primarycareproviderphonenumber = "34768",
                    Prescriptionnumber = "33232",
                    Fillnumber = 1535440336746799104,
                    Ndcnumberdispensed = "87297",
                    Medicationname = "18402",
                    Quantityprescribed = 69760M,
                    Quantitydispensed = 86533M,
                    Dayssupply = 198679793381833728,
                    Sigtext = "20147",
                    Datewritten = 2795390375308362752,
                    Datefilled = 6266464010807134208,
                    Datepickedupdispensed = 593969305756900352,
                    Refillsoriginallyauthorized = 42829M,
                    Refillsremaining = 92740M,
                    LogicFillfactkey = 7043141740666011648,
                    LogicPdpatientkey = 4622271156660896768,
                    LogicPatientaddressusage = "73089",
                    LogicPatientaddresscreatedate = new DateTime(2012, 9, 9),
                    LogicPresphonekey = 2365154726322221056,
                    LogicPresphonestatus = "22776",
                    LogicPresphonesourcecode = "46411",
                    LogicPresphonehlevel = new DateTime(2021, 11, 5),
                }
            ));

            AddRow((
                new object?[] {
                    7999713677650864128,
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
                    null,
                    null,
                    null,
                    null,
                    7043141740666011648,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                },
                new SelectSureScriptsMedicalHistoryRow
                {
                    Recordsequencenumber = 7999713677650864128,
                    Participantpatientid = null,
                    Patientlastname = null,
                    Patientfirstname = null,
                    Patientmiddlename = null,
                    Patientprefix = null,
                    Patientsufix = null,
                    Patientdateofbirth = null,
                    Patientgender = null,
                    Patientaddress1 = null,
                    Patientcity = null,
                    Patientstate = null,
                    Patientzipcode = null,
                    Ncpdpid = null,
                    Chainsiteid = null,
                    Pharmacyname = null,
                    Facilityaddress1 = null,
                    Facilitycity = null,
                    Facilitystate = null,
                    Facilityzipcode = null,
                    Facilityphonenumber = null,
                    Fprimarycareproviderlastname = null,
                    Primarycareproviderfirstname = null,
                    Primarycareprovideraddress1 = null,
                    Primarycareprovidercity = null,
                    Primarycareproviderstate = null,
                    Primarycareproviderzipcode = null,
                    Primarycareproviderareacode = null,
                    Primarycareproviderphonenumber = null,
                    Prescriptionnumber = null,
                    Fillnumber = null,
                    Ndcnumberdispensed = null,
                    Medicationname = null,
                    Quantityprescribed = null,
                    Quantitydispensed = null,
                    Dayssupply = null,
                    Sigtext = null,
                    Datewritten = null,
                    Datefilled = null,
                    Datepickedupdispensed = null,
                    Refillsoriginallyauthorized = null,
                    Refillsremaining = null,
                    LogicFillfactkey = 7043141740666011648,
                    LogicPdpatientkey = null,
                    LogicPatientaddressusage = null,
                    LogicPatientaddresscreatedate = null,
                    LogicPresphonekey = null,
                    LogicPresphonestatus = null,
                    LogicPresphonesourcecode = null,
                    LogicPresphonehlevel = null,
                }
            ));
        }
    }
}
