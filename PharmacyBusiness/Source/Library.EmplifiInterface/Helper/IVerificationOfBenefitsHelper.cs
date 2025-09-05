using CaseServiceWrapper;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;

namespace Library.EmplifiInterface.Helper;

public interface IVerificationOfBenefitsHelper
{
    string DeriveCaseText(
        IbmVerificationOfBenefitsRow ibmVerificationOfBenefitsRow,
        DateTime? recordTimestamp,
        DateTime? patientEnrollmentFormReceived
        );

    DateTime? DeriveLongDate(
       string? date);

    DateTime? DeriveNormalDate(
        string? date);

    string DeriveImageFileNamePattern(
        string imageFileNamePattern,
        string? patientDemographicId,
        string? caseId,
        string imageFileNameExtension);

    string GetDigits(
        string? input);

    Task<int> CreateCaseFromVerificationOfBenefitsAsync(
         VerificationOfBenefits verificationOfBenefits,
         CancellationToken c);

    Task<List<Case>> FindCasesByProgramTypePatientIdAndDob(
         string programType,
         string? birthYear,
         string caseStatus,
         List<PatientIdentifier>? patientIndentifiers);

    Task PostActionAsync(
         string actionTypeCode,
         string referredToUserCode,
         int caseId,
         DateTime responseDue,
         CancellationToken c);

    Task UpdateCaseFromVerificationOfBenefitsAsync(
        Case caseRecord,
        int caseId,
        VerificationOfBenefits verificationOfBenefits,
        CancellationToken c);

    Task UploadVerificationOfBenefitsImageFilesAndAttachToCaseAsync(
        int caseId,
        List<TriageImage>? verificationOfBenefitsImages,
        CancellationToken c);

    Task AppendDataToNewFile(
        string file,
        string newFile);

    CaseGetRequest CreateCaseGetRequest(
        string? programType,
        string caseStatus);
}
