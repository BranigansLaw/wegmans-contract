using CaseServiceWrapper;
using Library.EmplifiInterface.DataModel;
using Library.McKessonDWInterface.DataModel;

namespace Library.EmplifiInterface.Helper;

public interface IAstuteAdherenceDispenseHelper
{
    
    /// <summary>
    /// Find open cases in CRM application that match the dispense data
    /// </summary>
    /// <param name = "dispense" ></ param >
    /// < returns ></ returns >
    Task<IEnumerable<Case>> FindOpenCasesAsync(
        AstuteAdherenceDispenseReportRow dispense,
        CancellationToken c);

    /// <summary>
    /// Apply the business rules for matching and updating CRM cases for the dispense
    /// </summary>
    /// <param name="dispense">The dispense record being processed</param>
    /// <param name="crmCases">The list of CRM cases found for the dispense</param>
    Task<List<string>> ProcessCasesAsync(
        AstuteAdherenceDispenseReportRow dispense,
        IEnumerable<Case> crmCases,
        CancellationToken c);
    
    Task CreateAdminQueueCaseAsync(
        AstuteAdherenceDispenseReportRow dispense,
        CancellationToken c);

    /// <summary>
    /// Release the case in the CRM application
    /// </summary>
    /// <param name="successfullyExportedRecord"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task ReleaseCaseAsync(
        EmplifiRecordReportingStatus successfullyExportedRecord,
        CancellationToken c);
}
