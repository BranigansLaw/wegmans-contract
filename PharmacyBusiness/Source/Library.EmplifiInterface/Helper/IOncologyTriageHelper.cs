using AddressServiceWrapper;
using CaseServiceWrapper;
using Library.EmplifiInterface.DataModel;
using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.DataModel;
using System.ComponentModel.Design;

namespace Library.EmplifiInterface.Helper
{
    public interface IOncologyTriageHelper
    {
        /// <summary>
        /// Derives the drug name field
        /// </summary>
        /// <param name="drugNdc"></param>
        /// <param name="baseProductName"></param>
        /// <param name="completeSpecialtyItemRows"></param>
        /// <returns></returns>
        string DeriveDrugName(
            string? drugNdc,
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows);

        /// <summary>
        /// Derives the patient gender field
        /// </summary>
        /// <param name="patientGender"></param>
        /// <returns></returns>
        string DeriveCrmPatientGender(
            string? patientGender);

        /// <summary>
        /// Derives the program header field
        /// </summary>
        /// <param name="drugNdc"></param>
        /// <param name="baseProductName"></param>
        /// <param name="completeSpecialtyItemRows"></param>
        /// <returns></returns>
        string DeriveProgramHeader(
            string? drugNdc,
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows);

        /// <summary>
        /// Derives the case text field
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        string DeriveCaseText(
           string? carePathPatientId,
           string? PatientFirstName,
           string? PatientLastName,
           DateTime? PatientDateOfBirth,
           string? PatientGender,
           string? PatientAddress1,
           string? PatientAddress2,
           string? PatientCity,
           string? PatientState,
           string? PatientZipCode,
           string? PatientPhoneNumber,
           string? PrescriberFirstName,
           string? PrescriberLastName,
           string? PrescriberNpi,
           string? PrescriberAddress1,
           string? PrescriberAddress2,
           string? PrescriberCity,
           string? PrescriberState,
           string? PrescriberZipCode,
           string? PrescriberPhoneNumber,
           string? PatientDemographicId,
           string? CaseId
           );

        /// <summary>
        /// Derives dates
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        DateTime? DeriveDate(
            string? date);

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
            string? caseId,
            string imageFileNameExtension);

        /// <summary>
        /// Gets rid of any char that is not a digit
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string GetDigits(
            string? input);

        /// <summary>
        /// Works as a hub for the FindAddresses methods
        /// </summary>
        /// <param name="patientDemographicId"></param>
        /// <param name="carePathPatientId"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <returns></returns>
        Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressesForOncologyTriageAsync(
            string? patientDemographicId,
            string? carePathPatientId,
            DateTime? dateOfBirth,
            string? lastName,
            string? firstName);

        /// <summary>
        /// Finds addresses using patientIdType, patientId, and dateOfBirth
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
        /// 
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <param name="dateOfBirth"></param>
        /// <returns></returns>
        Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressUsingLastNameFirstNameAndDateOfBirth(
            string? lastName,
            string? firstName,
            DateTime? dateOfBirth);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="programType"></param>
        /// <param name="caseStatus"></param>
        /// <returns></returns>
        Task<List<Case>> FindCasesByAddressIdProgramAndStatusAsync(
            int addressId,
            string programType,
            string caseStatus);

        /// <summary>
        /// Create the base Address List Search Request
        /// </summary>
        /// <returns></returns>
        AddressListSearch CreateAddressListSearchRequest();

        /// <summary>
        /// Create an Address in Emplifi from a Oncology Triage
        /// </summary>
        /// <param name="oncologyTriage"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<int> CreateAddressFromOncologyTriageAsync(
            OncologyTriage oncologyTriage,
            CancellationToken c);

        /// <summary>
        /// Create a case in Emplifi from a Oncology Triage
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="oncologyTriage"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<int> CreateCaseFromOncologyTriageAsync(
            int addressId,
            OncologyTriage oncologyTriage,
            CancellationToken c);

        /// <summary>
        /// Create a Case Get Request for Emplifi
        /// </summary>
        /// <param name="programType"></param>
        /// <param name="caseStatus"></param>
        /// <returns></returns>
        CaseGetRequest CreateCaseGetRequest(
            string programType,
            string caseStatus);

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
        /// Add an attachment to an Emplifi case
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="attachmentName"></param>
        /// <param name="attachmentDescription"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task AddAttachmentToCaseAsync(
            int caseId,
            string attachmentName, 
            string attachmentDescription,
            CancellationToken c);

        /// <summary>
        /// Post an action to an Emplifi case
        /// </summary>
        /// <param name="actionTypeCode"></param>
        /// <param name="referredToUserCode"></param>
        /// <param name="caseId"></param>
        /// <param name="responseDue"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task PostActionAsync(
            string actionTypeCode,
            string referredToUserCode,
            int caseId, 
            DateTime responseDue,
            CancellationToken c);

        /// <summary>
        /// Update an Address in Emplifi from a Oncology Triage
        /// </summary>
        /// <param name="address"></param>
        /// <param name="oncologyTriage"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task UpdateAddressFromOncologyTriageAsync(
            AddressServiceWrapper.Address address,
            OncologyTriage oncologyTriage,
            CancellationToken c);

        /// <summary>
        /// Update a case in Emplifi from a Oncology Triage
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="oncologyTriage"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task UpdateCaseFromOncologyTriageAsync(
            int caseId,
            OncologyTriage oncologyTriage,
            CancellationToken c);


        /// <summary>
        /// Upload Oncology Triage image files to Emplifi and attach them to a case
        /// </summary>
        /// <param name="caseId"></param>
        /// <param name="oncologyTriageImages"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task UploadOncologyTriageImageFilesAndAttachToCaseAsync(
            int caseId,
            List<TriageImage>? oncologyTriageImages,
            CancellationToken c);

        /// <summary>
        /// Upload a file to Emplifi
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="fileByteStream"></param>
        /// <param name="caseId"></param>
        /// <returns></returns>
        Task<string> UploadFileAsync(
            string sourceFileName,
            Stream fileByteStream,
            int caseId);

    }
}
