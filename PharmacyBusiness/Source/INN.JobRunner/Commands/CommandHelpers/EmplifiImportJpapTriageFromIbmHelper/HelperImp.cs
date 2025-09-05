using Library.DataFileInterface;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Helper;
using Library.TenTenInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.EmplifiImportJpapTriageFromIbmHelper;

public class HelperImp : IHelper
{
    private readonly IJpapTriageHelper _jpapTriageHelper;
    private readonly IDataFileInterface _dataFileInterface;

    public HelperImp(IJpapTriageHelper jpapTriageHelper, IDataFileInterface dataFileInterface)
    {
        _jpapTriageHelper = jpapTriageHelper ?? throw new ArgumentNullException(nameof(jpapTriageHelper));
        _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
    }

    public JpapTriage MapJpapTriage(IbmJpapTriage ibmJpapTriage, IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows)
    {
        var derivedPatientDateOfBirth = _jpapTriageHelper.DerivePatientDateOfBirth(ibmJpapTriage.PatientDateOfBirth);
        var derivedPatientGender = _jpapTriageHelper.DerivePatientGender(ibmJpapTriage.PatientGender);
        
        return new JpapTriage
        {
            CarePathSpecialtyPharmacyName = ibmJpapTriage.CarePathSpecialtyPharmacyName,
            CarePathPatientId = ibmJpapTriage.CarePathPatientId,
            PatientLastName = ibmJpapTriage.PatientLastName,
            PatientFirstName = ibmJpapTriage.PatientFirstName,
            PatientAddress1 = ibmJpapTriage.PatientAddress1,
            PatientAddress2 = ibmJpapTriage.PatientAddress2,
            PatientCity = ibmJpapTriage.PatientCity,
            PatientState = ibmJpapTriage.PatientState,
            PatientZip = ibmJpapTriage.PatientZip,
            PatientPhoneNumber = ibmJpapTriage.PatientPhoneNumber,
            ProductName = ibmJpapTriage.ProductName,
            NdcCode = ibmJpapTriage.NdcCode,
            PrimaryDiagnosisCode = ibmJpapTriage.PrimaryDiagnosisCode,
            PrescriberLastName = ibmJpapTriage.PrescriberLastName,
            PrescriberFirstName = ibmJpapTriage.PrescriberFirstName,
            PrescriberNpi = ibmJpapTriage.PrescriberNpi,
            PrescriberDea = ibmJpapTriage.PrescriberDea,
            PrescriberAddress1 = ibmJpapTriage.PrescriberAddress1,
            PrescriberAddress2 = ibmJpapTriage.PrescriberAddress2,
            PrescriberCity = ibmJpapTriage.PrescriberCity,
            PrescriberState = ibmJpapTriage.PrescriberState,
            PrescriberZip = ibmJpapTriage.PrescriberZip,
            PrescriberPhone = ibmJpapTriage.PrescriberPhone,
            PrescriberFax = ibmJpapTriage.PrescriberFax,
            TreatmentCenterName = ibmJpapTriage.TreatmentCenterName,
            TreatmentCenterContactLastName = ibmJpapTriage.TreatmentCenterContactLastName,
            TreatmentCenterContactFirstName = ibmJpapTriage.TreatmentCenterContactFirstName,
            TreatmentCenterAddress1 = ibmJpapTriage.TreatmentCenterAddress1,
            TreatmentCenterAddress2 = ibmJpapTriage.TreatmentCenterAddress2,
            TreatmentCenterCity = ibmJpapTriage.TreatmentCenterCity,
            TreatmentCenterState = ibmJpapTriage.TreatmentCenterState,
            TreatmentCenterZip = ibmJpapTriage.TreatmentCenterZip,
            TreatmentCenterPhone = ibmJpapTriage.TreatmentCenterPhone,
            TreatmentCenterFax = ibmJpapTriage.TreatmentCenterFax,
            TreatmentCenterNpi = ibmJpapTriage.TreatmentCenterNpi,
            TreatmentCenterDea = ibmJpapTriage.TreatmentCenterDea,
            ShipToLocation = ibmJpapTriage.ShipToLocation,
            PatientDemographicId = ibmJpapTriage.PatientDemographicId,
            CarePathTransactionId = ibmJpapTriage.CarePathTransactionId,
            ImageCount = ibmJpapTriage.ImageCount,
            DerivedPatientDateOfBirth = derivedPatientDateOfBirth,
            DerivedDrugName = _jpapTriageHelper.DeriveDrugName(ibmJpapTriage.NdcCode, completeSpecialtyItemRows),
            DerivedPatientGender = derivedPatientGender,
            DerivedProgramHeader = _jpapTriageHelper.DeriveProgramHeader(ibmJpapTriage.NdcCode, completeSpecialtyItemRows),
            DerivedCaseText = _jpapTriageHelper.DeriveCaseText(
                ibmJpapTriage.CarePathPatientId,
                ibmJpapTriage.PatientFirstName,
                ibmJpapTriage.PatientLastName,
                derivedPatientDateOfBirth,
                derivedPatientGender,
                ibmJpapTriage.PatientAddress1,
                ibmJpapTriage.PatientAddress2,
                ibmJpapTriage.PatientCity,
                ibmJpapTriage.PatientState,
                ibmJpapTriage.PatientZip,
                ibmJpapTriage.PatientPhoneNumber,
                ibmJpapTriage.PrescriberFirstName,
                ibmJpapTriage.PrescriberLastName,
                ibmJpapTriage.PrescriberNpi,
                ibmJpapTriage.PrescriberDea,
                ibmJpapTriage.PrescriberAddress1,
                ibmJpapTriage.PrescriberAddress2,
                ibmJpapTriage.PrescriberCity,
                ibmJpapTriage.PrescriberState,
                ibmJpapTriage.PrescriberZip,
                ibmJpapTriage.PrescriberPhone,
                ibmJpapTriage.PrescriberFax,
                ibmJpapTriage.PatientDemographicId,
                ibmJpapTriage.CarePathTransactionId),
            DerivedImages = GetTriageImages(ibmJpapTriage.PatientDemographicId, ibmJpapTriage.CarePathTransactionId)

        };
    }

    private List<TriageImage> GetTriageImages(string? patientDemographicId, string? carePathTransactionId)
    {
        var imageFileNamePattern = _jpapTriageHelper.DeriveImageFileNamePattern(
            "Wegmans_CarePath_PDE_Triage_",
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
