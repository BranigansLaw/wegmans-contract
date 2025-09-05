using AddressServiceWrapper;
using CaseServiceWrapper;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;
using Library.TenTenInterface.DataModel;

namespace Library.EmplifiInterface.Helper;

public interface IOncologyVoucherTriageHelper
{
    string DeriveDrugName(
            string? drugNdc,
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows);

    string DeriveCrmPatientGender(
            string? patientGender);

    string DeriveProgramHeader(
           string? drugNdc,
           IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows);

    string DeriveCaseText(
            IbmOncologyVoucherTriageRow? ibmOncologyVoucherTriageRow,
            DateTime? PatientDateOfBirth,
            string? PatientGender
            );

    DateTime? DeriveDate(
           string? date);

    string DeriveImageFileNamePattern(
            string imageFileNamePattern,
            string? patientDemographicId,
            string? carePathTransactionId,
            string imageFileNameExtension);

    string GetDigits(string? input);

    Task AddAttachmentToCaseAsync(
            int caseId,
            string attachmentName,
            string attachmentDescription,
            CancellationToken c);

    Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressesForOncologyVoucherTriageAsync(
            string? carePathPatientId,
            string? demographicId,
            string? state,
            DateTime? dateOfBirth,
            string? lastName,
            string? firstName);

    Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressUsingLastNameFirstNameStateAndDateOfBirth(
            string? lastName,
            string? firstName,
            string? state,
            DateTime? dateOfBirth);

    Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressUsingPatientIdAndDateOfBirth(
            string? patientIdType,
            string? patientId,
            DateTime? dateOfBirth);

    Task<List<Case>> FindCasesByAddressIdProgramAndStatusAsync(
        int addressId,
        string programType,
        string caseStatus);

    AddressListSearch CreateAddressListSearchRequest();

    Task<int> CreateAddressFromOncologyVoucherTriageAsync(
            OncologyVoucherTriage oncologyVoucherTriage,
            CancellationToken c);

    Task<int> CreateCaseFromOncologyVoucherTriageAsync(
            int addressId,
            OncologyVoucherTriage oncologyVoucherTriage,
            CancellationToken c);

    CaseGetRequest CreateCaseGetRequest(
            string programType,
            string caseStatus);

    Task UploadOncologyVoucherTriageImageFilesAndAttachToCaseAsync(
            int caseId,
            List<TriageImage>? oncologyVoucherTriageImages,
            CancellationToken c);

    Task<string> UploadFileAsync(
            string sourceFileName,
            Stream fileByteStream,
            int caseId);

    Task UpdateAddressFromOncologyVoucherTriageAsync(
            AddressServiceWrapper.Address address,
            OncologyVoucherTriage oncologyVoucherTriage,
            CancellationToken c);

    Task UpdateCaseFromOncologyVoucherTriageAsync(
           int caseId,
           OncologyVoucherTriage oncologyVoucherTriage,
           CancellationToken c);

    Task PostActionAsync(
            string actionTypeCode,
            string referredToUserCode,
            int caseId,
            DateTime responseDue,
            CancellationToken c);

    Task ReleaseCaseAsync(
            int caseId,
            CancellationToken c);
}

