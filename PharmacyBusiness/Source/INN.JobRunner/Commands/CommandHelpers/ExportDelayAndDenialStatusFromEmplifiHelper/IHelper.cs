using CaseServiceWrapper;
using Library.EmplifiInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper;

public interface IHelper
{
    public IEnumerable<DelayAndDenialStatusRow> SelectDataRowsMatchingBusinessRules(
        DateTime startDateTime,
        DateTime endDateTime, 
        IEnumerable<Case> cases,
        out List<EmplifiRecordReportingStatus> recordReportingStatus);

    public bool IsShipmentStatus(string? patientStatus);

    public bool MeetsExportCriteria(
        DateTime startDateTime,
        DateTime endDateTime,
        string? patientStatusChangedDateString,
        string? caseDateChangedString,
        string? addressDateChangedString,
        string? patientStatus,
        string? shipDateString,
        string? dispenseLoadedDateString,
        out string rejectReason);
}
