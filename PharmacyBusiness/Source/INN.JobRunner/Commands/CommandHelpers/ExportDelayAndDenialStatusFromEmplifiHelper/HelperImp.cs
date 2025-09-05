using CaseServiceWrapper;
using INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifiMapping;
using Library.EmplifiInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper;

public class HelperImp : IHelper
{
    private readonly IEmplifiMapper _emplifiMapper;

    public const string DuplicateRecordMessage = "Skpping this record because it is duplicate";
    public const string NotLastSequenceMessage = "This Issue Seq is not the last Issue Seq";
    public const string SuccessfulRecordMessage = "Success - MeetsExportCriteria";

    public HelperImp(IEmplifiMapper emplifiMapper)
    {
        _emplifiMapper = emplifiMapper ?? throw new ArgumentNullException(nameof(emplifiMapper));
    }

    public IEnumerable<DelayAndDenialStatusRow> SelectDataRowsMatchingBusinessRules(
        DateTime startDateTime,
        DateTime endDateTime, 
        IEnumerable<Case> cases, 
        out List<EmplifiRecordReportingStatus> recordReportingStatus)
    {
        var rowsToExport = new List<DelayAndDenialStatusRow>();
        recordReportingStatus = new List<EmplifiRecordReportingStatus>();
        bool thisCaseContainsAtLeastOne_IsC26 = false;
        string previousCaseId = string.Empty;

        foreach (var patientCase in cases.OrderBy(c => c.case_id))
        {
            if (string.IsNullOrEmpty(patientCase.case_id))
                continue;

            if (patientCase.case_id != previousCaseId)
                thisCaseContainsAtLeastOne_IsC26 = false;

            previousCaseId = patientCase.case_id;
            var patientIssues = patientCase.IssueList.Issue;
            var patientAddress = patientCase.AddressList.Address.First();
            DelayAndDenialStatusRow delayAndDenialStatusRow;

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

                // Check if the issue is the last issue for B43 or A38, as only those Issues get reported.
                // All Issues for C26 get reported.,
                bool isC26 = (DateTime.TryParse(patientIssue.c26_code, out var patientStatusChangedDate) &&
                    (startDateTime <= patientStatusChangedDate && patientStatusChangedDate < endDateTime));

                bool isB43 = (DateTime.TryParse(patientCase.b43_code, out var parsedCaseDateChanged) &&
                    (startDateTime <= parsedCaseDateChanged && parsedCaseDateChanged < endDateTime));

                bool isA38 = (DateTime.TryParse(patientAddress.a38_code, out var parsedAddressDateChanged) &&
                    (startDateTime <= parsedAddressDateChanged && parsedAddressDateChanged < endDateTime));

                bool isLastIssueSeq = patientIssue.issue_seq == patientIssues.OrderBy(i => i.issue_seq).Last().issue_seq;

                //Once you find a C26 Issue Seq, then any subsequent Issue Seqs should be reported only if also a C26.
                //If you do not encounter a C26 Issue Seq, then only the last Issue Seq for B43 and A38 should be reported.
                bool skipThisIssueSequence = false;

                if (isC26)
                { 
                    thisCaseContainsAtLeastOne_IsC26 = true;
                    skipThisIssueSequence = false;
                }
                else
                    if (isB43 && isLastIssueSeq && !thisCaseContainsAtLeastOne_IsC26)
                        skipThisIssueSequence = false;
                    else
                        if (isA38 && isLastIssueSeq && !thisCaseContainsAtLeastOne_IsC26)
                            skipThisIssueSequence = false;
                        else
                            skipThisIssueSequence = true;

                if (skipThisIssueSequence)
                { 
                    recordReportingStatus.Add(new EmplifiRecordReportingStatus
                    {
                        CaseId = patientCase.case_id,
                        IssueSeq = patientIssue.issue_seq,
                        IsValidForReporting = false,
                        NotifyEndUsersForCorrection = false,
                        ReportingStatusDescription = $"{NotLastSequenceMessage}: This Issue Seq is [{patientIssue.issue_seq}], but only [{patientIssues.OrderBy(i => i.issue_seq).Last().issue_seq}] gets reported on for B43 and A38, which for this record: Is C26:{isC26}, Is B43:{isB43}, Is A38:{isA38}."
                    });

                    continue;
                }

                try
                {
                    delayAndDenialStatusRow = _emplifiMapper.MappingForEmplifi(patientCase, patientAddress, patientIssue, startDateTime, endDateTime);
                }
                catch (Library.EmplifiInterface.Exceptions.DataIntegrityException dataIntegrityException)
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
                    caseDateChangedString: patientCase.b43_code,
                    addressDateChangedString: patientAddress.a38_code,
                    patientStatus: patientIssue.c16_code,
                    shipDateString: patientIssue.c12_code,
                    dispenseLoadedDateString: patientIssue.ca8_code,
                    out string rejectReason))
                {
                    rowsToExport.Add(delayAndDenialStatusRow);

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
        string? caseDateChangedString,
        string? addressDateChangedString,
        string? patientStatus,
        string? shipDateString,
        string? dispenseLoadedDateString,
        out string rejectReason)
    {
        rejectReason = string.Empty;
        var patientStatusChangedDate = DateTime.TryParse(patientStatusChangedDateString, out var parsedPatientStatusChangedDate) ? parsedPatientStatusChangedDate : (DateTime?)null;
        var caseDateChanged = DateTime.TryParse(caseDateChangedString, out var parsedCaseDateChanged) ? parsedCaseDateChanged : (DateTime?)null;
        var addressDateChanged = DateTime.TryParse(addressDateChangedString, out var parsedAddressDateChanged) ? parsedAddressDateChanged : (DateTime?)null;
        var shipDate = DateTime.TryParse(shipDateString, out var parsedShipDate) ? parsedShipDate : (DateTime?)null;
        var dispenseLoadedDate = DateTime.TryParse(dispenseLoadedDateString, out var parsedDispenseLoadedDate) ? parsedDispenseLoadedDate : (DateTime?)null;

        if ((startDateTime <= patientStatusChangedDate && patientStatusChangedDate < endDateTime) ||
            (startDateTime <= caseDateChanged && caseDateChanged < endDateTime) ||
            (startDateTime <= addressDateChanged && addressDateChanged < endDateTime))
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

        rejectReason = $"No date change within the date range. Start=[{startDateTime}], End=[{endDateTime}], patientStatusChangedDate=[{patientStatusChangedDate}], caseDateChanged=[{caseDateChanged}], addressDateChanged=[{addressDateChanged}].";
        return false;
    }
}
