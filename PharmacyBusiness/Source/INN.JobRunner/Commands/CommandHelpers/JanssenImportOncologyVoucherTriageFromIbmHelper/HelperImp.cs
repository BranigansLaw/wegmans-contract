using Library.DataFileInterface;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Helper;
using Library.TenTenInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.JanssenImportOncologyVoucherTriageFromIbmHelper;

public class HelperImp : IHelper
{
    private readonly IOncologyVoucherTriageHelper _oncologyVoucherTriageHelper;
    private readonly IDataFileInterface _dataFileInterface;

    public HelperImp(IOncologyVoucherTriageHelper oncologyVoucherTriageHelper, IDataFileInterface dataFileInterface)
    {
        _oncologyVoucherTriageHelper = oncologyVoucherTriageHelper ?? throw new ArgumentNullException(nameof(oncologyVoucherTriageHelper));
        _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
    }

    public OncologyVoucherTriage MapOncologyVoucherTriage(IbmOncologyVoucherTriageRow ibmOncologyVoucherTriage, IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows)
    {
        var derivedPatientGender = _oncologyVoucherTriageHelper.DeriveCrmPatientGender(ibmOncologyVoucherTriage.PatientGender);
        var dateOfBirth = _oncologyVoucherTriageHelper.DeriveDate(ibmOncologyVoucherTriage.PatientDateOfBirth);
        var ndcCode = ibmOncologyVoucherTriage.NdcCode;
        var baseProductName = ibmOncologyVoucherTriage.ProductName;
        var demographicId = ibmOncologyVoucherTriage.PatientDemographicId;
        var carePathPatientId = ibmOncologyVoucherTriage.CarePathPatientId;

        return new OncologyVoucherTriage
        {
            RecordTimestamp = _oncologyVoucherTriageHelper.DeriveDate(ibmOncologyVoucherTriage.RecordTimestamp),
            CarePathSpecialtyPharmacyName = ibmOncologyVoucherTriage.CarePathSpecialtyPharmacyName,
            CarePathPatientId = ibmOncologyVoucherTriage.CarePathPatientId,
            PatientLastName = ibmOncologyVoucherTriage.PatientLastName,
            PatientFirstName = ibmOncologyVoucherTriage.PatientFirstName,
            PatientDateOfBirth = dateOfBirth,
            PatientGender = derivedPatientGender,
            PatientAddress1 = ibmOncologyVoucherTriage.PatientAddress1,
            PatientAddress2 = ibmOncologyVoucherTriage.PatientAddress2,
            PatientCity = ibmOncologyVoucherTriage.PatientCity,
            PatientState = ibmOncologyVoucherTriage.PatientState,
            PatientZipCode = ibmOncologyVoucherTriage.PatientZipCode,
            PatientPhoneNumber = ibmOncologyVoucherTriage.PatientPhoneNumber,
            ProductName = ibmOncologyVoucherTriage.ProductName,
            NdcCode = ibmOncologyVoucherTriage.NdcCode,
            PrimaryDiagnosisCode = ibmOncologyVoucherTriage.PrimaryDiagnosisCode,
            PrescriberLastName = ibmOncologyVoucherTriage.PrescriberLastName,
            PrescriberFirstName = ibmOncologyVoucherTriage.PrescriberFirstName,
            PrescriberNpi = ibmOncologyVoucherTriage.PrescriberNpi,
            PrescriberDea = ibmOncologyVoucherTriage.PrescriberDea,
            PrescriberAddress1 = ibmOncologyVoucherTriage.PrescriberAddress1,
            PrescriberAddress2 = ibmOncologyVoucherTriage.PrescriberAddress2,
            PrescriberCity = ibmOncologyVoucherTriage.PrescriberCity,
            PrescriberState = ibmOncologyVoucherTriage.PrescriberState,
            PrescriberZipCode = ibmOncologyVoucherTriage.PrescriberZipCode,
            PrescriberPhoneNumber = ibmOncologyVoucherTriage.PrescriberPhoneNumber,
            PrescriberFaxNumber = ibmOncologyVoucherTriage.PrescriberFaxNumber,
            TreatmentCenterName = ibmOncologyVoucherTriage.TreatmentCenterName,
            TreatmentCenterContactLastName = ibmOncologyVoucherTriage.TreatmentCenterContactLastName,
            TreatmentCenterContactFirstName = ibmOncologyVoucherTriage.TreatmentCenterContactFirstName,
            TreatmentCenterAddress1 = ibmOncologyVoucherTriage.TreatmentCenterAddress1,
            TreatmentCenterAddress2 = ibmOncologyVoucherTriage.TreatmentCenterAddress2,
            TreatmentCenterCity = ibmOncologyVoucherTriage.TreatmentCenterCity,
            TreatmentCenterState = ibmOncologyVoucherTriage.TreatmentCenterState,
            TreatmentCenterZipCode = ibmOncologyVoucherTriage.TreatmentCenterZipCode,
            TreatmentCenterPhoneNumber = ibmOncologyVoucherTriage.TreatmentCenterPhoneNumber,
            TreatmentCenterFaxNumber = ibmOncologyVoucherTriage.TreatmentCenterFaxNumber,
            TreatmentCenterNpi = ibmOncologyVoucherTriage.TreatmentCenterNpi,
            TreatmentCenterDea = ibmOncologyVoucherTriage.TreatmentCenterDea,
            ShipToLocation = ibmOncologyVoucherTriage.ShipToLocation,
            PatientDemographicId = ibmOncologyVoucherTriage.PatientDemographicId,
            CarePathTransactionId = ibmOncologyVoucherTriage.CarePathTransactionId,
            ImageCount = ibmOncologyVoucherTriage.ImageCount,
            
            DerivedDrugName = _oncologyVoucherTriageHelper.DeriveDrugName(ibmOncologyVoucherTriage.NdcCode, completeSpecialtyItemRows),
            DerivedCrmPatientGender = derivedPatientGender,
            DerivedProgramHeader = _oncologyVoucherTriageHelper.DeriveProgramHeader(ibmOncologyVoucherTriage.NdcCode, completeSpecialtyItemRows),
            DerivedCaseText = _oncologyVoucherTriageHelper.DeriveCaseText(
                ibmOncologyVoucherTriage,
                dateOfBirth,
                derivedPatientGender),
            DerivedImages = GetTriageImages(ibmOncologyVoucherTriage.PatientDemographicId, ibmOncologyVoucherTriage.CarePathTransactionId)

        };
    }

    private List<TriageImage> GetTriageImages(string? patientDemographicId, string? carePathTransactionId)
    {
        var imageFileNamePattern = _oncologyVoucherTriageHelper.DeriveImageFileNamePattern(
            "Wegmans_CarePath_Voucher_Triage_",
            patientDemographicId,
            carePathTransactionId,
            ".tiff");

        var triageImages = new List<TriageImage>();

        if (!string.IsNullOrWhiteSpace(imageFileNamePattern))
        {
            var imageFileNames = _dataFileInterface.GetImageFileNames(imageFileNamePattern);

            foreach (var imageFileName in imageFileNames)
            {
                var imageStream = _dataFileInterface.ReadImageFileToStream(imageFileName);

                if (imageStream is not null)
                {
                    triageImages.Add(new TriageImage
                    {
                        ImageFileName = imageFileName,
                        Image = imageStream
                    });
                }
            }
        }

        return triageImages;
    }
}
