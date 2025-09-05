using AddressServiceWrapper;
using CaseServiceWrapper;
using Library.EmplifiInterface.DataModel;
using Library.TenTenInterface.DataModel;

namespace Library.EmplifiInterface.Helper;

public interface IJpapTriageHelper
{
    /// <summary>
    /// Derive the Patient Date of Birth for the JPAP Triage
    /// </summary>
    /// <param name="patientDateOfBirth"></param>
    /// <returns></returns>
    DateTime? DerivePatientDateOfBirth(
        string? patientDateOfBirth);

    /// <summary>
    /// Derive the Case Text for the JPAP Triage
    /// </summary>
    /// <returns></returns>
    string? DeriveCaseText(
        string? carePathPatientId,
        string? patientFirstName,
        string? patientLastName,
        DateTime? patientDateOfBirth,
        string? patientGender,
        string? patientAddress1,
        string? patientAddress2,
        string? patientCity,
        string? patientState,
        string? patientZip,
        string? patientPhoneNumber,
        string? prescriberFirstName,
        string? prescriberLastName,
        string? prescriberNpi,
        string? prescriberDea,
        string? prescriberAddress1,
        string? prescriberAddress2,
        string? prescriberCity,
        string? prescriberState,
        string? prescriberZip,
        string? prescriberPhone,
        string? prescriberFax,
        string? patientDemographicId,
        string? carePathTransactionId);

    /// <summary>
    /// Derive the Program Header for the JPAP Triage using the Drug NDC and the Complete Specialty Item List from 1010data
    /// </summary>
    /// <param name="drugNdc"></param>
    /// <param name="completeSpecialtyItemRows"></param>
    /// <returns></returns>
    string DeriveProgramHeader(
        string? drugNdc,
        IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows);

    /// <summary>
    /// Derive the Drug Name for the JPAP Triage using the Drug NDC and the Complete Specialty Item List from 1010data
    /// </summary>
    /// <param name="drugNdc"></param>
    /// <param name="completeSpecialtyItemRows"></param>
    /// <returns></returns>
    string DeriveDrugName(
        string? drugNdc,
        IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows);

    /// <summary>
    /// Derive the Patient Gender for the JPAP Triage
    /// </summary>
    /// <param name="patientGender"></param>
    /// <returns></returns>
    string DerivePatientGender(
        string? patientGender);

    /// <summary>
    /// Derive the image file name pattern for the JPAP Triage
    /// </summary>
    /// <param name="imageFileNamePattern"></param>
    /// <param name="patientDemographicId"></param>
    /// <param name="carePathTransactionId"></param>
    /// <param name="imageFileNameExtension"></param>
    /// <returns></returns>
    string DeriveImageFileNamePattern(
        string imageFileNamePattern,
        string? patientDemographicId,
        string? carePathTransactionId,
        string imageFileNameExtension);

    /// <summary>
    /// Return the digits from the input string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    string GetDigits(string? input);

    /// <summary>
    /// Create the base Address List Search Request
    /// </summary>
    /// <returns></returns>
    AddressListSearch CreateAddressListSearchRequest();

    /// <summary>
    /// Find Addresses in Emplifi using Patient ID and Date of Birth
    /// </summary>
    /// <param name="patientIdType"></param>
    /// <param name="patientId"></param>
    /// <param name="dateOfBirth"></param>
    /// <returns></returns>
    Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressUsingPatientIdAndDateOfBirth(
        string? patientIdType,
        string? patientId,
        DateTime? dateOfBirth);

    /// <summary>
    /// Find Addresses in Emplifi using Last Name, First Name, State, and Date of Birth
    /// </summary>
    /// <param name="lastName"></param>
    /// <param name="firstName"></param>
    /// <param name="state"></param>
    /// <param name="dateOfBirth"></param>
    /// <returns></returns>
    Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressUsingLastNameFirstNameStateAndDateOfBirth(
        string? lastName,
        string? firstName,
        string? state,
        DateTime? dateOfBirth);

    /// <summary>
    /// Find Addresses in Emplifi using fields from the JPAP Triage
    /// </summary>
    /// <param name="patientDemographicId"></param>
    /// <param name="carePathPatientId"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="lastName"></param>
    /// <param name="firstName"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressesForJpapTriageAsync(
        string? patientDemographicId,
        string? carePathPatientId,
        DateTime? dateOfBirth,
        string? lastName,
        string? firstName,
        string? state);

    /// <summary>
    /// Post an action to an Emplifi case
    /// </summary>
    /// <param name="actionTypeCode"></param>
    /// <param name="referredToUserCode"></param>
    /// <param name="caseId"></param>
    /// <param name="responseDue"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task PostActionAsync(String actionTypeCode, string referredToUserCode, int caseId, DateTime responseDue, CancellationToken c);

    /// <summary>
    /// Release an Emplifi case from edit
    /// </summary>
    /// <param name="caseId"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task ReleaseCaseAsync(int caseId, CancellationToken c);

    /// <summary>
    /// Create a Case Get Request for Emplifi
    /// </summary>
    /// <param name="programType"></param>
    /// <param name="caseStatus"></param>
    /// <returns></returns>
    CaseGetRequest CreateCaseGetRequest(string programType, string caseStatus);

    /// <summary>
    /// Create an Address in Emplifi from a JPAP Triage
    /// </summary>
    /// <param name="jpapTriage"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task<int> CreateAddressFromJpapTriageAsync(JpapTriage jpapTriage, CancellationToken c);

    /// <summary>
    /// Upload JPAP Triage image files to Emplifi and attach them to a case
    /// </summary>
    /// <param name="caseId"></param>
    /// <param name="jpapTriageImages"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task UploadJpapTriageImageFilesAndAttachToCaseAsync(int caseId, List<TriageImage>? jpapTriageImages, CancellationToken c);

    /// <summary>
    /// Upload a file to Emplifi
    /// </summary>
    /// <param name="sourceFileName"></param>
    /// <param name="fileByteStream"></param>
    /// <param name="caseId"></param>
    /// <returns></returns>
    Task<string> UploadFileAsync(string sourceFileName, Stream fileByteStream, int caseId);

    /// <summary>
    /// Add an attachment to an Emplifi case
    /// </summary>
    /// <param name="caseId"></param>
    /// <param name="attachmentName"></param>
    /// <param name="attachmentDescription"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task AddAttachmentToCaseAsync(int caseId, string attachmentName, string attachmentDescription, CancellationToken c);

    /// <summary>
    /// Find cases in Emplifi by Address ID, Program Type, and Case Status
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="programType"></param>
    /// <param name="caseStatus"></param>
    /// <returns></returns>
    Task<List<Case>> FindCasesByAddressIdProgramAndStatusAsync(int addressId, string programType, string caseStatus);

    /// <summary>
    /// Update an Address in Emplifi from a JPAP Triage
    /// </summary>
    /// <param name="address"></param>
    /// <param name="jpapTriage"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task UpdateAddressFromJpapTriageAsync(AddressServiceWrapper.Address address, JpapTriage jpapTriage, CancellationToken c);

    /// <summary>
    /// Create a case in Emplifi from a JPAP Triage
    /// </summary>
    /// <param name="addressId"></param>
    /// <param name="jpapTriage"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task<int> CreateCaseFromJpapTriageAsync(int addressId, JpapTriage jpapTriage, CancellationToken c);

    /// <summary>
    /// Update a case in Emplifi from a JPAP Triage
    /// </summary>
    /// <param name="caseId"></param>
    /// <param name="jpapTriage"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    Task UpdateCaseFromJpapTriageAsync(int caseId, JpapTriage jpapTriage, CancellationToken c);
}
