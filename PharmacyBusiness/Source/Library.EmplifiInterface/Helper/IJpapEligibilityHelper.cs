using CaseServiceWrapper;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;

namespace Library.EmplifiInterface.Helper;

public interface IJpapEligibilityHelper
{
    string DeriveCaseText(
        IbmJpapEligibilityRow ibmJpapEligibilityRow,
        DateTime? DateOfBirth
        );

    DateTime? DeriveDate(
        string? date);

    /// <summary>
    /// Find any cases in the CRM application matching the search criteria
    /// </summary>
    /// <param name="programType">Program type to search on</param>
    /// <param name="patientId">Patient ID to search on</param>
    /// <param name="dateOfBirth">Date of birth to search on</param>
    /// <param name="caseStatus">Status of case to search on</param>
    /// <returns>List of cases that match to the search criteria</returns>
    Task<List<Case>> FindCasesByProgramTypePatientIdAndDob(
        string programType,
        string? patientId,
        DateTime? dateOfBirth,
        string caseStatus);

    CaseGetRequest CreateCaseGetRequest(
        string programType,
        string caseStatus);

    /// <summary>
    /// Create a new case with no address in the admin queue
    /// </summary>
    /// <param name="textTypeCode">Text type code to use</param>
    /// <param name="description">Description to use</param>
    /// <param name="caseText">Case text to use</param>
    /// <returns></returns>
    Task CreateBlankCase(
        JpapEligibilityRow jpapEligibilityRow,
        CancellationToken c);

    /// <summary>
    /// Release an Emplifi case from edit
    /// </summary>
    /// <param name="caseId"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task ReleaseCaseAsync(
        int caseId,
        CancellationToken c);

    /// <summary>
    /// Update a case in Emplifi from a Jpap Eligiblity
    /// </summary>
    /// <param name="caseId"></param>
    /// <param name="oncologyTriage"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task UpdateCaseFromJpapEligibilityAsync(
            int caseId,
            JpapEligibilityRow jpapEligibilityRow,
            CancellationToken c);
}
