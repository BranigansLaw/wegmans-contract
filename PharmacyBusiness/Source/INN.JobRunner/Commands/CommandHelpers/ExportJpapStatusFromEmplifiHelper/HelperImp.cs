using CaseServiceWrapper;
using INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.EmplifiMapping;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Exceptions;

namespace INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper;

public class HelperImp : IHelper
{
    private readonly IEmplifiMapper _emplifiMapper;

    public HelperImp(IEmplifiMapper emplifiMapper)
    {
        _emplifiMapper = emplifiMapper ?? throw new ArgumentNullException(nameof(emplifiMapper));
    }

    public const string DuplicateRecordMessage = "Skpping this record because it is duplicate";
    public const string NotLastSequenceMessage = "This Issue Seq is not the last Issue Seq";
    public const string SuccessfulRecordMessage = "Success - MeetsExportCriteria";

    public IEnumerable<JpapStatusRow> SelectDataRowsMatchingBusinessRules(
        DateTime startDateTime,
        DateTime endDateTime,
        IEnumerable<Case> cases,
        out List<EmplifiRecordReportingStatus> recordReportingStatus)
    {
        var rowsToExport = new List<JpapStatusRow>();
        recordReportingStatus = new List<EmplifiRecordReportingStatus>();
        string previousCaseId = string.Empty;

        foreach (var patientCase in cases.OrderBy(c => c.case_id))
        {
            if (string.IsNullOrEmpty(patientCase.case_id))
                continue;

            previousCaseId = patientCase.case_id;
            var patientIssues = patientCase.IssueList.Issue;
            var patientAddress = patientCase.AddressList.Address.First();
            JpapStatusRow jpapStatusRow;

            foreach (var patientIssue in patientIssues.OrderBy(i => i.issue_seq))
            {
                if (string.IsNullOrEmpty(patientIssue.issue_seq))
                    continue;

                if (recordReportingStatus.Where(r => r.CaseId == patientCase.case_id && r.IssueSeq == patientIssue.issue_seq).Any())
                {
                    // Skip if the record has already been processed.
                    recordReportingStatus.Add(new EmplifiRecordReportingStatus
                    {
                        CaseId = patientCase.case_id,
                        IssueSeq = patientIssue.issue_seq,
                        IsValidForReporting = false,
                        NotifyEndUsersForCorrection = false,
                        ReportingStatusDescription = $"{DuplicateRecordMessage} - CaseId [{patientCase.case_id}] Issue Seq [{patientIssue.issue_seq}]."
                    });

                    continue;
                }

                bool isC26 = (DateTime.TryParse(patientIssue.c26_code, out var patientStatusChangedDate) &&
                    (startDateTime <= patientStatusChangedDate && patientStatusChangedDate < endDateTime));

                bool isLastIssueSeq = patientIssue.issue_seq == patientIssues.OrderBy(i => i.issue_seq).Last().issue_seq;

                bool skipThisIssueSequence = false;

                if (!isC26)
                    skipThisIssueSequence = true;

                if (skipThisIssueSequence)
                {
                    recordReportingStatus.Add(new EmplifiRecordReportingStatus
                    {
                        CaseId = patientCase.case_id,
                        IssueSeq = patientIssue.issue_seq,
                        IsValidForReporting = false,
                        NotifyEndUsersForCorrection = false,
                        ReportingStatusDescription = $"{NotLastSequenceMessage}: This Issue Seq is [{patientIssue.issue_seq}], but only [{patientIssues.OrderBy(i => i.issue_seq).Last().issue_seq}], which for this record: Is C26:{isC26}."
                    });

                    continue;
                }

                try
                {
                    jpapStatusRow = _emplifiMapper.MappingForEmplifi(patientCase, patientAddress, patientIssue, startDateTime, endDateTime);
                }
                catch (DataIntegrityException dataIntegrityException)
                {
                    recordReportingStatus.Add(new EmplifiRecordReportingStatus
                    {
                        CaseId = patientCase.case_id,
                        IssueSeq = patientIssue.issue_seq,
                        IsValidForReporting = false,
                        NotifyEndUsersForCorrection = true,
                        ReportingStatusDescription = dataIntegrityException.DataRowConstraintViolations
                    });

                    continue;
                }

                if (MeetsExportCriteria(
                    startDateTime,
                    endDateTime,
                    patientStatusChangedDateString: patientIssue.c26_code,
                    patientStatus: patientIssue.c16_code,
                    shipDateString: patientIssue.c12_code,
                    dispenseLoadedDateString: patientIssue.ca8_code,
                    out string rejectReason))
                {
                    rowsToExport.Add(jpapStatusRow);

                    recordReportingStatus.Add(new EmplifiRecordReportingStatus
                    {
                        CaseId = patientCase.case_id,
                        IssueSeq = patientIssue.issue_seq,
                        IsValidForReporting = true,
                        NotifyEndUsersForCorrection = false,
                        ReportingStatusDescription = SuccessfulRecordMessage
                    });
                }
                else
                {
                    recordReportingStatus.Add(new EmplifiRecordReportingStatus
                    {
                        CaseId = patientCase.case_id,
                        IssueSeq = patientIssue.issue_seq,
                        IsValidForReporting = false,
                        NotifyEndUsersForCorrection = false,
                        ReportingStatusDescription = rejectReason
                    });
                }
            }
        }
        return rowsToExport;
    }

    public bool IsShipmentStatus(string? patientStatus)
    {
        if (string.IsNullOrWhiteSpace(patientStatus))
            return false;

        return string.Equals(patientStatus, "A1=Shipped", StringComparison.OrdinalIgnoreCase);
    }

    public bool MeetsExportCriteria(
        DateTime startDateTime,
        DateTime endDateTime,
        string? patientStatusChangedDateString,
        string? patientStatus,
        string? shipDateString,
        string? dispenseLoadedDateString,
        out string rejectReason)
    {
        rejectReason = string.Empty;
        var patientStatusChangedDate = DateTime.TryParse(patientStatusChangedDateString, out var parsedPatientStatusChangedDate) ? parsedPatientStatusChangedDate : (DateTime?)null;
        var shipDate = DateTime.TryParse(shipDateString, out var parsedShipDate) ? parsedShipDate : (DateTime?)null;
        var dispenseLoadedDate = DateTime.TryParse(dispenseLoadedDateString, out var parsedDispenseLoadedDate) ? parsedDispenseLoadedDate : (DateTime?)null;

        if ((startDateTime <= patientStatusChangedDate && patientStatusChangedDate < endDateTime))
        {
            if (IsShipmentStatus(patientStatus) && (shipDate is null || shipDate == DateTime.MinValue || startDateTime > dispenseLoadedDate || dispenseLoadedDate > endDateTime))
            {
                rejectReason = $"Shipment status is 'Shipped' but shipDate=[{shipDate}] or dispenseLoadedDate=[{dispenseLoadedDate}].";
                return false;
            }
            else
            {
                return true;
            }
        }

        rejectReason = $"No date change within the date range. Start=[{startDateTime}], End=[{endDateTime}], patientStatusChangedDate=[{patientStatusChangedDate}].";
        return false;
    }
}
