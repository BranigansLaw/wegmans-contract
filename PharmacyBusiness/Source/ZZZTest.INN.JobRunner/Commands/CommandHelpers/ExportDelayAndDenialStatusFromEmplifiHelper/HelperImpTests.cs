using CaseServiceWrapper;
using INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper;
using INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifiMapping;
using INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifyFieldConverter;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Helper;
using ZZZTest.INN.JobRunner.TestData;

namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper
{
    public class HelperImpTests
    {
        private readonly HelperImp _sut;
        private readonly IEmplifyFieldConverter _emplifyFieldConverterImp;
        private readonly IEmplifiMapper _emplifiMapperImp;
        private readonly IDelayAndDenialHelper _delayAndDenialHelperImp;
        private EmplifiTestCases _emplifiTestCases;

        public HelperImpTests()
        {
            _emplifiTestCases = new EmplifiTestCases();
            _delayAndDenialHelperImp = new DelayAndDenialHelperImp();
            _emplifyFieldConverterImp = new EmplifyFieldConverterImp();
            _emplifiMapperImp = new EmplifiMapperImp(_emplifyFieldConverterImp);
            _sut = new HelperImp(_emplifiMapperImp);
        }

        [Theory]
        [InlineData("06/13/2024 14:30:00", 1, 1)]
        public void SelectDataRowsMatchingBusinessRules_ShouldSkipDuplicates(string runForDateTime, int expectedSuccessfulRecordCount, int expectedFailureRecordCount)
        {
            // Setup
            string expectedReportingStatusDescription = HelperImp.DuplicateRecordMessage;
            DateTime runFor = DateTime.Parse(runForDateTime);
            var dateRange = _delayAndDenialHelperImp.GetDateRangeForOutboundStatus(runFor);
            IEnumerable<Case> cases = _emplifiTestCases.Cases20240613143000;

            // Act
            var actualResult = _sut.SelectDataRowsMatchingBusinessRules(
                dateRange.startDateTime,
                dateRange.endDateTime,
                cases,
                out List<EmplifiRecordReportingStatus> recordReportingStatus);

            // Assert
            Assert.Equal(actualResult.Count(), recordReportingStatus.Where(r => r.IsValidForReporting == true).Count());
            Assert.Equal(expectedSuccessfulRecordCount, recordReportingStatus.Where(s => s.IsValidForReporting).Count());
            Assert.Equal(expectedFailureRecordCount, recordReportingStatus.Where(s => !s.IsValidForReporting).Count());
            Assert.Contains(expectedReportingStatusDescription, recordReportingStatus.Where(s => !s.IsValidForReporting).Select(s => s.ReportingStatusDescription ?? string.Empty).FirstOrDefault());
        }

        [Theory]
        [InlineData("06/13/2024 14:30:00", 0, 2)]
        public void SelectDataRowsMatchingBusinessRules_ShouldHandleDataIntegrityException(string runForDateTime, int expectedSuccessfulRecordCount, int expectedFailureRecordCount)
        {
            // Setup
            string expectedReportingStatusDescription = EmplifyFieldConverterImp.ErrorAssigningValueMessage;
            DateTime runFor = DateTime.Parse(runForDateTime);
            var dateRange = _delayAndDenialHelperImp.GetDateRangeForOutboundStatus(runFor);
            IEnumerable<Case> cases = _emplifiTestCases.Cases20240613143000;

            foreach (var c in cases)
            {
                foreach (var i in c.IssueList.Issue)
                {
                    //Mock some invalid data
                    i.c39_code = "Invalid Ndc";
                }
            }

            // Act
            var actualResult = _sut.SelectDataRowsMatchingBusinessRules(
                dateRange.startDateTime,
                dateRange.endDateTime, 
                cases,
                out List<EmplifiRecordReportingStatus> recordReportingStatus);

            // Assert
            Assert.Equal(actualResult.Count(), recordReportingStatus.Where(r => r.IsValidForReporting == true).Count());
            Assert.Equal(expectedSuccessfulRecordCount, recordReportingStatus.Where(s => s.IsValidForReporting).Count());
            Assert.Equal(expectedFailureRecordCount, recordReportingStatus.Where(s => !s.IsValidForReporting).Count());
            Assert.Contains(expectedReportingStatusDescription, recordReportingStatus.Where(s => !s.IsValidForReporting).Select(s => s.ReportingStatusDescription ?? string.Empty).FirstOrDefault());
        }
        
        [Theory]
        [InlineData("06/13/2024 14:30:00", 2, 0)]
        public void SelectDataRowsMatchingBusinessRules_ShouldIncludeAllIssueSequences(string runForDateTime, int expectedSuccessfulRecordCount, int expectedFailureRecordCount)
        {
            // Setup
            string expectedReportingStatusDescription = HelperImp.SuccessfulRecordMessage;
            DateTime runFor = DateTime.Parse(runForDateTime);
            var dateRange = _delayAndDenialHelperImp.GetDateRangeForOutboundStatus(runFor);
            IEnumerable<Case> cases = _emplifiTestCases.Cases20240613143000;
            cases.First().IssueList.Issue[0].issue_seq = "1";
            cases.Last().IssueList.Issue[0].issue_seq = "2";
            //These two Case records have one Issue Seq each, and all Issues have isC26 = true, isB43 = true, isA38 = false.

            // Act
            var actualResult = _sut.SelectDataRowsMatchingBusinessRules(
                dateRange.startDateTime,
                dateRange.endDateTime,
                cases,
                out List<EmplifiRecordReportingStatus> recordReportingStatus);

            // Assert
            Assert.Equal(actualResult.Count(), recordReportingStatus.Where(r => r.IsValidForReporting == true).Count());
            Assert.Equal(expectedSuccessfulRecordCount, recordReportingStatus.Where(s => s.IsValidForReporting).Count());
            Assert.Equal(expectedFailureRecordCount, recordReportingStatus.Where(s => !s.IsValidForReporting).Count());
            Assert.Contains(expectedReportingStatusDescription, recordReportingStatus.Where(s => s.IsValidForReporting).Select(s => s.ReportingStatusDescription ?? string.Empty).ToList());
        }

        [Theory]
        [InlineData("06/14/2024 14:30:00", 28, 48, 40, 8, 0)]
        public void SelectDataRowsMatchingBusinessRules_ShouldSkipForKnownBusinessRules(
            string runForDateTime, 
            int expectedSuccessfulExportRecordCount, 
            int expectedFailureRecordCount,
            int expectedDuplicateCount,
            int expectedNotLastSequenceCount,
            int expectedErrorAssigningValueCount)
        {
            // Setup
            string expectedReportingStatusDescriptionNotifyFalse1 = HelperImp.DuplicateRecordMessage;
            string expectedReportingStatusDescriptionNotifyFalse2 = HelperImp.NotLastSequenceMessage;
            string expectedReportingStatusDescriptionNotifyTrue = EmplifyFieldConverterImp.ErrorAssigningValueMessage;
            DateTime runFor = DateTime.Parse(runForDateTime);
            var dateRange = _delayAndDenialHelperImp.GetDateRangeForOutboundStatus(runFor);
            IEnumerable<Case> cases = _emplifiTestCases.Cases20240614143000;

            // Act
            var actualExportRecords = _sut.SelectDataRowsMatchingBusinessRules(
                dateRange.startDateTime,
                dateRange.endDateTime,
                cases,
                out List<EmplifiRecordReportingStatus> recordReportingStatus);

            int actualSuccessfulReportingStatusRecordCount = recordReportingStatus.Where(s => s.IsValidForReporting).Count();

            int actualFailureReportingStatusRecordCount = recordReportingStatus.Where(s => !s.IsValidForReporting).Count();

            int actualDuplicateCount = recordReportingStatus.Where(s =>
                    !s.IsValidForReporting &&
                    !s.NotifyEndUsersForCorrection &&
                    !string.IsNullOrEmpty(s.ReportingStatusDescription) &&
                    s.ReportingStatusDescription.Contains(expectedReportingStatusDescriptionNotifyFalse1)).Count();

            int actualdNotLastSequenceCount = recordReportingStatus.Where(s =>
                    !s.IsValidForReporting &&
                    !s.NotifyEndUsersForCorrection &&
                    !string.IsNullOrEmpty(s.ReportingStatusDescription) &&
                    s.ReportingStatusDescription.Contains(expectedReportingStatusDescriptionNotifyFalse2)).Count();

            int actualErrorAssigningValueCount = recordReportingStatus.Where(s =>
                    !s.IsValidForReporting &&
                    s.NotifyEndUsersForCorrection &&
                    !string.IsNullOrEmpty(s.ReportingStatusDescription) &&
                    s.ReportingStatusDescription.Contains(expectedReportingStatusDescriptionNotifyTrue)).Count();

            // Assert
            Assert.Equal(actualExportRecords.Count(), expectedSuccessfulExportRecordCount);
            Assert.Equal(expectedSuccessfulExportRecordCount, actualSuccessfulReportingStatusRecordCount);
            Assert.Equal(expectedFailureRecordCount, actualFailureReportingStatusRecordCount);

            Assert.Equal(expectedDuplicateCount, actualDuplicateCount);
            Assert.Equal(expectedNotLastSequenceCount, actualdNotLastSequenceCount);
            Assert.Equal(expectedErrorAssigningValueCount, actualErrorAssigningValueCount);
        }

        [Theory]
        [InlineData("789", "123", "4", "7890123004")]
        [InlineData("11111", "222222", "3", "111110222222003")]
        [InlineData("5", "123", "4", "50123004")]
        public void FormatTransactionID_ShouldReturnFormatedValueWithValidInputDataAsExpected(string addressId, string caseId, string issueSeq, string expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.case_id = caseId;
            testCase.AddressList.Address[0].address_id = addressId;
            testCase.IssueList.Issue[0].issue_seq = issueSeq;

            // Act
            var actualResult = _emplifyFieldConverterImp.FormatTransactionID(testCase.AddressList.Address[0].address_id, testCase.case_id, testCase.IssueList.Issue[0].issue_seq, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("", "", "4", "0004")]
        [InlineData("7", "", "4", "70004")]
        [InlineData("", "", "", "0000")]
        public void FormatTransactionID_ShouldReturnDefaultValueWithInvalidInputData(string addressId, string caseId, string issueSeq, string expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.case_id = caseId;
            testCase.AddressList.Address[0].address_id = addressId;
            testCase.IssueList.Issue[0].issue_seq = issueSeq;

            // Act
            var actualResult = _emplifyFieldConverterImp.FormatTransactionID(testCase.AddressList.Address[0].address_id, testCase.case_id, testCase.IssueList.Issue[0].issue_seq, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("10012003")]
        [InlineData("")]
        [InlineData("08//10//1966")]
        public void FormatYYYYMMDDDob_ShouldReturnDefaultOrInvalidInputDataAsExpected(string date)
        {
            // Setup
            int expectedResult = default;
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.AddressList.Address[0].a05_code = date;

            // Act
            var actualResult = _emplifyFieldConverterImp.FormatDateToYYYYMMDDDob(testCase.AddressList.Address[0].a05_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("10/01/2003", 20031001)]
        [InlineData("01/08/2002", 20020108)]
        [InlineData("08/10/1966", 19660810)]
        [InlineData("08/14/1965", 19650814)]
        public void FormatYYYYMMDDobD_ShouldReturnWithValidInputDataAsExpected(string date, int expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.AddressList.Address[0].a05_code = date;

            // Act
            var actualResult = _emplifyFieldConverterImp.FormatDateToYYYYMMDDDob(testCase.AddressList.Address[0].a05_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("06/21/2024 14:31:00", 20240621143100)]
        public void FormatYYYYMMDDHHMMSS_ShouldReturnWithValidDataAsExpected(string date, long expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c26_code = date;

            // Act
            var actualResult = _emplifyFieldConverterImp.FormatDateToYYYYMMDDHHMMSS(testCase.IssueList.Issue[0].c26_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }


        [Theory]
        [InlineData("A1=Shipped", "10/01/2003", 20031001)]
        [InlineData("A1=Shipped", "12/31/2030", 20301231)]
        public void GetShipDateStatus_ShouldReturnWithValidDataAsExpected(string status, string shipDate, int expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;
            testCase.IssueList.Issue[0].c12_code = shipDate;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetShipDateStatus(testCase.IssueList.Issue[0].c16_code, testCase.IssueList.Issue[0].c12_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("P4=Future Shipment", "10/01/2003")]
        [InlineData("D14=Therapy End", "12/34/5678")]
        public void GetShipDateStatus_ShouldReturnDefaultWithInvalidDataAsExpected(string status, string shipDate)
        {
            // Setup
            int? expectedResult = default;
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;
            testCase.IssueList.Issue[0].c12_code = shipDate;

            // Act

            var actualResult = _emplifyFieldConverterImp.GetShipDateStatus(testCase.IssueList.Issue[0].c16_code, testCase.IssueList.Issue[0].c12_code, new List<string>());

            // Assert

            Assert.Equal(expectedResult, actualResult);

        }


        [Theory]
        [InlineData("A1=Shipped", "LI")]
        public void GetTransactionType_ShouldReturnWithValidDataAsExpected(string status, string expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetTransactionType(testCase.IssueList.Issue[0].c16_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("anything", null)]
        [InlineData("P4=Future Shipment", null)]
        [InlineData(null, "")]
        [InlineData("", null)]
        public void GetTransactionType_ShouldReturnWithInvalidDataAsExpected(string status, string expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetTransactionType(testCase.IssueList.Issue[0].c16_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("A1=Shipped", "Carrier", "Carrier")]
        [InlineData("AShipped", "Carrier", null)]
        [InlineData("Anything", "Carrier", null)]
        [InlineData("", "Carrier", null)]
        public void GetCarrier_ShouldReturnWithValidDataAsExpected(string status, string carrier, string expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;
            testCase.IssueList.Issue[0].c84_code = carrier;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetCarrier(testCase.IssueList.Issue[0].c16_code, testCase.IssueList.Issue[0].c84_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        //here

        [Theory]
        [InlineData("A1=Shipped", "7", 7)]
        [InlineData("A1=Shipped", "0", 0)]
        [InlineData("A1=Shipped", "2", 2)]
        [InlineData("Anything but shipped", "999", null)]
        public void GetQuantity_ShouldReturnWithValidDataAsExpected(string status, string quantity, int? expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;
            testCase.IssueList.Issue[0].c07_code = quantity;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetQuantity(testCase.IssueList.Issue[0].c16_code, testCase.IssueList.Issue[0].c07_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("7", "A1=Shipped", null)]
        [InlineData(null, "12", null)]
        public void GetQuantity_ShouldReturnWithInalidDataAsExpected(string status, string quantity, int? expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;
            testCase.IssueList.Issue[0].c07_code = quantity;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetQuantity(testCase.IssueList.Issue[0].c16_code, testCase.IssueList.Issue[0].c07_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("Some Non-shipment", "1", null)]
        [InlineData("A1=Shipped", "1", "N")]
        [InlineData("A1=Shipped", "0", "R")]
        public void GetFillType_ShouldReturnWithValidDataAsExpected(string status, string fillNumString, string? expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;
            testCase.IssueList.Issue[0].c09_code = fillNumString;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetFillType(testCase.IssueList.Issue[0].c16_code, testCase.IssueList.Issue[0].c09_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("Some Non-shipment", "1", null)]
        [InlineData("3", "r", null)]
        [InlineData("1", "", null)]
        public void GetFillType_ShouldReturnWithInvalidDataAsExpected(string status, string fillNumString, string? expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;
            testCase.IssueList.Issue[0].c09_code = fillNumString;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetFillType(testCase.IssueList.Issue[0].c16_code, testCase.IssueList.Issue[0].c09_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("200 Market St", "123 Address Ln", "200 Market St")]
        [InlineData(null, "123 Address Ln", "123 Address Ln")]
        [InlineData("14", "Wegmans", "14")]
        public void GetXPres_ShouldReturnWithValidDataAsExpected(string? pres, string healthCare, string expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.AddressList.Address[0].a44_code = pres;
            testCase.AddressList.Address[0].a24_code = healthCare;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetPresAddr1(testCase.AddressList.Address[0].a44_code, testCase.AddressList.Address[0].a24_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(null, null, "")]
        public void GetXPres_ShouldReturnWithInvalidDataAsExpected(string? pres, string healthCare, string expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.AddressList.Address[0].a44_code = pres;
            testCase.AddressList.Address[0].a24_code = healthCare;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetPresAddr1(testCase.AddressList.Address[0].a44_code, testCase.AddressList.Address[0].a24_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("A1=Shipped", "ACTIVE")]
        [InlineData("P1=New Referral", "PENDING")]
        public void GetStatus_ShouldReturnWithValidDataAsExpected(string status, string expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetStatus(testCase.IssueList.Issue[0].c16_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        public void GetStatus_ShouldReturnWithInvalidDataAsExpected(string status, string expectedResult)
        {
            // Setup
            Case testCase = _emplifiTestCases.Cases20240613143000.First();
            testCase.IssueList.Issue[0].c16_code = status;

            // Act
            var actualResult = _emplifyFieldConverterImp.GetStatus(testCase.IssueList.Issue[0].c16_code, new List<string>());

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
