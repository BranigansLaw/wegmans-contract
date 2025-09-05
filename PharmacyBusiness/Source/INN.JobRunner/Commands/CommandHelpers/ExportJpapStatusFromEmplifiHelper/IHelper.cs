using CaseServiceWrapper;
using Library.EmplifiInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper;

public interface IHelper
{
    IEnumerable<JpapStatusRow> SelectDataRowsMatchingBusinessRules(
        DateTime startDateTime,
        DateTime endDateTime,
        IEnumerable<Case> cases,
        out List<EmplifiRecordReportingStatus> recordReportingStatus);

    bool IsShipmentStatus(string? patientStatus);

    bool MeetsExportCriteria(
       DateTime startDateTime,
       DateTime endDateTime,
       string? patientStatusChangedDateString,
       string? patientStatus,
       string? shipDateString,
       string? dispenseLoadedDateString,
       out string rejectReason);
}
