using CaseServiceWrapper;
using INN.JobRunner.Commands;
using INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper;
using INN.JobRunner.CommonParameters;
using Library.EmplifiInterface;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Helper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ZZZTest.INN.JobRunner.Commands.CommandHelpers;

namespace ZZZTest.INN.JobRunner.Commands;

public class ExportJpapStatusFromEmplifiTests : PharmacyCommandBaseTests
{
    private readonly ExportJpapStatusFromEmplifi _sut;

    private readonly ILogger<ExportJpapStatusFromEmplifi> _loggerMock = Substitute.For<ILogger<ExportJpapStatusFromEmplifi>>();
    private readonly IJpapDispenseAndStatusHelper _jpapDispenseAndStatusHelperMock = Substitute.For<IJpapDispenseAndStatusHelper>();
    private readonly IEmplifiInterface _emplifiInterfaceMock = Substitute.For<IEmplifiInterface>();
    private readonly IHelper _helperMock = Substitute.For<IHelper>();

    public ExportJpapStatusFromEmplifiTests() : base()
    {
        _sut = new(_emplifiInterfaceMock, 
            _jpapDispenseAndStatusHelperMock, 
            _loggerMock, 
            _helperMock,
            _mockGenericHelper,
            _mockCoconaContextWrapper);
    }

    [Fact]
    public async Task RunAsyncTests_CallsAllDependencies_ReturnsSuccess()
    {
        // Arrange
        var date = new DateTime(2025, 1, 8, 14, 30, 0); // 1/8/2025 at 2:30
        DateTime returnedStartDate = new(2025, 1, 8, 8, 30, 0);
        DateTime returnedEndDate = new(2025, 1, 9, 8, 30, 0);
        RunForDateTimeParameter runFor = new RunForDateTimeParameter
        {
            RunFor = date
        };

        string filename = "Wegmans_PDE_Status_Dispense_Data_20250109083000.txt";
        List<JpapStatusRow> jpapStatusRows = [new JpapStatusRow 
            {
                PharmacyCode = "PharmacyCode",
                PharmacyNpi = 1,
                SpTransactionId = "SpTransactionId",
                ProgramId = "ProgramId",
                PatientId = "PatientId",
                PatientFirstName = "Michael",
                PatientLastName = "LastName",
                PatientDob = 20250108,
                Brand = "Wegmans",
                NdcNumber = 2,
                PresLastName = "PresLastName",
                PresFirstName = "PresFirstName",
                PresNpi = 3,
                PresAddr1 = "Home",
                PresCity = "City",
                PresState = "State",
                PresZip = 4,
                DemographicId = "DemographicId",
                CarePathTransactionId = "CarePathTransactionId",
                StatusDate = 20250108161202,
                Status = "Status",
                SubStatus = "SubStatus"
            }
        ];
        IEnumerable<Case> cases = [new Case 
            {
                case_id = "1",
                IssueList = new IssueList()
                {
                    Issue = 
                    [
                        new Issue()
                        {
                            issue_seq = "2"
                        }
                    ]
                }
            }
        ];
        IEnumerable<JpapStatusRow> jpapExportRows = [new JpapStatusRow 
            { 
                PharmacyCode = "PharmacyCode",
                PharmacyNpi = 1,
                SpTransactionId = "SpTransactionId",
                ProgramId = "ProgramId",
                PatientId = "PatientId",
                PatientFirstName = "Michael",
                PatientLastName = "LastName",
                PatientDob = 20250108,
                Brand = "Wegmans",
                NdcNumber = 2,
                PresLastName = "PresLastName",
                PresFirstName = "PresFirstName",
                PresNpi = 3,
                PresAddr1 = "Home",
                PresCity = "City",
                PresState = "State",
                PresZip = 4,
                DemographicId = "DemographicId",
                CarePathTransactionId = "CarePathTransactionId",
                StatusDate = 20250108161202,
                Status = "Status",
                SubStatus = "SubStatus"
            }
        ];
        List<string> exceptions = ["something went wrong!!!"];
        var recordReportingStatus = new List<EmplifiRecordReportingStatus>
        {
            new() {
                CaseId = "1",
                IssueSeq = "2",
                NotifyEndUsersForCorrection = true,
                IsValidForReporting = false,
                ReportingStatusDescription = "Testing"
            }
        };

        _jpapDispenseAndStatusHelperMock.GetDateRangeForOutboundStatus(runFor.RunFor).Returns((returnedStartDate, returnedEndDate));

        _emplifiInterfaceMock.GetJpapOutboundStatusAsync(returnedStartDate, returnedEndDate, TestCancellationToken).Returns(cases);

        _helperMock.SelectDataRowsMatchingBusinessRules(returnedStartDate, returnedEndDate, cases, out Arg.Any<List<EmplifiRecordReportingStatus>>()).Returns(x => 
        {
            x[3] = recordReportingStatus; 
            return jpapExportRows;
        });

        _emplifiInterfaceMock.SetExtractedDateAsync(recordReportingStatus, returnedEndDate, TestCancellationToken).Returns(exceptions);

        // Act
        await _sut.RunAsync(runFor);

        // Assert
        _jpapDispenseAndStatusHelperMock.Received(1).GetDateRangeForOutboundStatus(runFor.RunFor);
        await _emplifiInterfaceMock.Received(1).GetJpapOutboundStatusAsync(returnedStartDate, returnedEndDate, TestCancellationToken);
        _helperMock.Received(1).SelectDataRowsMatchingBusinessRules(returnedStartDate, returnedEndDate, cases, out Arg.Any<List<EmplifiRecordReportingStatus>>());
        await _emplifiInterfaceMock.Received(1).WriteListToFileAsync(Arg.Any<List<JpapStatusRow>>(), filename, true, "|", "", true, TestCancellationToken);
        await _emplifiInterfaceMock.Received(1).SetExtractedDateAsync(recordReportingStatus, returnedEndDate, TestCancellationToken);
        await _emplifiInterfaceMock.Received(1).WriteListToFileAsync(Arg.Any<List<EmplifiRecordReportingStatus>>(), "DEBUG_" + filename, true, "|", "", true, TestCancellationToken);
    }
}
