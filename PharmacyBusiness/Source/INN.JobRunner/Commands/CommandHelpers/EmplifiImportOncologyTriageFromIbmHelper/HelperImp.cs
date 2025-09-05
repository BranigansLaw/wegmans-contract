using Library.DataFileInterface;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Helper;
using Library.TenTenInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.EmplifiImportOncologyTriageFromIbmHelper;

public class HelperImp : IHelper
{
    private readonly IOncologyTriageHelper _oncologyTriageHelper;
    private readonly IDataFileInterface _dataFileInterface;

    public HelperImp(
        IOncologyTriageHelper oncologyTriageHelper,
        IDataFileInterface dataFileInterface)
    {
        _oncologyTriageHelper = oncologyTriageHelper ?? throw new ArgumentNullException(nameof(oncologyTriageHelper));
        _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
    }

    public OncologyTriage MapOncologyTriage(
        IbmOncologyTriageRow ibmOncologyTriage,
        IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows)
    {
        var derivedPatientGender = _oncologyTriageHelper.DeriveCrmPatientGender(ibmOncologyTriage.PatientGender);
        var dateOfBirth = _oncologyTriageHelper.DeriveDate(ibmOncologyTriage.PatientDateOfBirth);
        var ndcCode = ibmOncologyTriage.NdcCode;
        var baseProductName = ibmOncologyTriage.ProductName;
        var demographicId = ibmOncologyTriage.PatientDemographicId;
        var carePathPatientId = ibmOncologyTriage.CarePathPatientId;
        var caseId = ibmOncologyTriage.CaseId;

        return new OncologyTriage
        {
            RecordTimestamp = _oncologyTriageHelper.DeriveDate(ibmOncologyTriage.RecordTimestamp),
            CarePathSpecialtyPharmacyName = ibmOncologyTriage.CarePathSpecialtyPharmacyName,
            CarePathPatientId = carePathPatientId,
            PatientLastName = ibmOncologyTriage.PatientLastName,
            PatientFirstName = ibmOncologyTriage.PatientFirstName,
            PatientDateOfBirth = dateOfBirth,
            PatientGender = derivedPatientGender,
            PatientAddress1 = ibmOncologyTriage.PatientAddress1,
            PatientAddress2 = ibmOncologyTriage.PatientAddress2,
            PatientCity = ibmOncologyTriage.PatientCity,
            PatientState = ibmOncologyTriage.PatientState,
            PatientZipCode = ibmOncologyTriage.PatientZipCode,
            PatientPhoneNumber = ibmOncologyTriage.PatientPhoneNumber,
            ProductName = baseProductName,
            PrescriberLastName = ibmOncologyTriage.PrescriberLastName,
            PrescriberFirstName = ibmOncologyTriage.PrescriberFirstName,
            PrescriberNpi = ibmOncologyTriage.PrescriberNpi,
            CarePathPrescriberId = ibmOncologyTriage.CarePathPrescriberId,
            PrescriberAddress1 = ibmOncologyTriage.PrescriberAddress1,
            PrescriberAddress2 = ibmOncologyTriage.PrescriberAddress2,
            PrescriberCity = ibmOncologyTriage.PrescriberCity,
            PrescriberState = ibmOncologyTriage.PrescriberState,
            PrescriberZipCode = ibmOncologyTriage.PrescriberZipCode,
            PrescriberPhoneNumber = ibmOncologyTriage.PrescriberPhoneNumber,
            PriorAuthReceivedFromPayerDate = _oncologyTriageHelper.DeriveDate(ibmOncologyTriage.PriorAuthReceivedFromPayerDate),
            ShipToLocation = ibmOncologyTriage.ShipToLocation,
            TreatmentCenterName = ibmOncologyTriage.TreatmentCenterName,
            TreatmentCenterContactLastName = ibmOncologyTriage.TreatmentCenterContactLastName,
            TreatmentCenterContactFirstName = ibmOncologyTriage.TreatmentCenterContactFirstName,
            TreatmentCenterAddress1 = ibmOncologyTriage.TreatmentCenterAddress1,
            TreatmentCenterAddress2 = ibmOncologyTriage.TreatmentCenterAddress2,
            TreatmentCenterCity = ibmOncologyTriage.TreatmentCenterCity,
            TreatmentCenterState = ibmOncologyTriage.TreatmentCenterState,
            TreatmentCenterZipCode = ibmOncologyTriage.TreatmentCenterZipCode,
            TreatmentCenterPhoneNumber = ibmOncologyTriage.TreatmentCenterPhoneNumber,
            TreatmentCenterFaxNumber = ibmOncologyTriage.TreatmentCenterFaxNumber,
            TreatmentCenterNpi = ibmOncologyTriage.TreatmentCenterNpi,
            TreatmentCenterDea = ibmOncologyTriage.TreatmentCenterDea,
            DoseType = ibmOncologyTriage.DoseType,
            ImageAvailable = ibmOncologyTriage.ImageAvailable,
            CaseId = caseId,
            NdcCode = ndcCode,
            PrimaryDiagnosisCode = ibmOncologyTriage.PrimaryDiagnosisCode,
            PatientDemographicId = demographicId,

            DerivedCaseText = _oncologyTriageHelper.DeriveCaseText(
                carePathPatientId,
                ibmOncologyTriage.PatientFirstName,
                ibmOncologyTriage.PatientLastName,
                dateOfBirth,
                derivedPatientGender,
                ibmOncologyTriage.PatientAddress1,
                ibmOncologyTriage.PatientAddress2,
                ibmOncologyTriage.PatientCity,
                ibmOncologyTriage.PatientState,
                ibmOncologyTriage.PatientZipCode,
                ibmOncologyTriage.PatientPhoneNumber,
                ibmOncologyTriage.PrescriberFirstName,
                ibmOncologyTriage.PrescriberLastName,
                ibmOncologyTriage.PrescriberNpi,
                ibmOncologyTriage.PrescriberAddress1,
                ibmOncologyTriage.PrescriberAddress2,
                ibmOncologyTriage.PrescriberCity,
                ibmOncologyTriage.PrescriberState,
                ibmOncologyTriage.PrescriberZipCode,
                ibmOncologyTriage.PrescriberPhoneNumber,
                demographicId,
                caseId
                ),
            DerivedCrmPatientGender = derivedPatientGender,
            DerivedDrugName = _oncologyTriageHelper.DeriveDrugName(ndcCode, completeSpecialtyItemRows),
            DerivedProgramHeader = _oncologyTriageHelper.DeriveProgramHeader(ndcCode, completeSpecialtyItemRows),
            DerivedImages = GetTriageImages(demographicId, caseId)
        };
    }

    private List<TriageImage> GetTriageImages(
        string? patientDemographicId,
        string? caseId)
    {
        var imageFileNamePattern = _oncologyTriageHelper.DeriveImageFileNamePattern(
            "CarePath_DelayDenial_Triage_",
            patientDemographicId,
            caseId,
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


