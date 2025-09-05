using Library.DataFileInterface;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Helper;
using System.Text;
using System.Text.RegularExpressions;

namespace INN.JobRunner.Commands.CommandHelpers.EmplifiImportVerificationOfBenefitsFromIbmHelper;

public class HelperImp : IHelper
{
    private readonly IVerificationOfBenefitsHelper _verificationOfBenefitsHelper;
    private readonly IDataFileInterface _dataFileInterface;
    private readonly IEmplifiInterface _emplifiInterface;

    private const string PatientIdType = "ONC Delay Denial CarePath Patient ID";
    private const string DemographicIdType = "JCP ID";

    public HelperImp(
        IVerificationOfBenefitsHelper verificationOfBenefitsHelper,
        IDataFileInterface dataFileInterface,
        IEmplifiInterface emplifiInterface)
    {
        _verificationOfBenefitsHelper = verificationOfBenefitsHelper ?? throw new ArgumentNullException(nameof(verificationOfBenefitsHelper));
        _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
        _emplifiInterface = emplifiInterface ?? throw new ArgumentNullException(nameof(emplifiInterface));
    }

    public VerificationOfBenefits MapVerificationOfBenefits(
        IbmVerificationOfBenefitsRow ibmVerificationOfBenefitsRow)
    {
        var recordTimestamp = _verificationOfBenefitsHelper.DeriveLongDate(ibmVerificationOfBenefitsRow.RecordTimestamp);
        var patientEnrollmentFormReceived = _verificationOfBenefitsHelper.DeriveNormalDate(ibmVerificationOfBenefitsRow.PatientEnrollmentFormReceived);

        return new VerificationOfBenefits
        {
            RecordTimestamp = recordTimestamp,
            CarePathSpecialtyPharmacyName = ibmVerificationOfBenefitsRow.CarePathSpecialtyPharmacyName,
            CarePathPatientId = ibmVerificationOfBenefitsRow.CarePathPatientId,
            PatientBirthYear = ibmVerificationOfBenefitsRow.PatientBirthYear,
            PayerType = ibmVerificationOfBenefitsRow.PayerType,
            SpecialtyPharmacyName = ibmVerificationOfBenefitsRow.SpecialtyPharmacyName,
            SpecialtyPharmacyPhone = ibmVerificationOfBenefitsRow.SpecialtyPharmacyPhone,
            ImageExists = ibmVerificationOfBenefitsRow.ImageExists,
            CarePathCaseId = ibmVerificationOfBenefitsRow.CarePathCaseId,
            PatientEnrollmentFormReceived = patientEnrollmentFormReceived,
            ExternalPatientId= ibmVerificationOfBenefitsRow.ExternalPatientId,
            ProductName = ibmVerificationOfBenefitsRow.ProductName,
            DemographicId = ibmVerificationOfBenefitsRow.DemographicId,

            DerivedCaseText = _verificationOfBenefitsHelper.DeriveCaseText(
                        ibmVerificationOfBenefitsRow,
                        recordTimestamp,
                        patientEnrollmentFormReceived
                        ),
            DerivedImages = GetTriageImages(ibmVerificationOfBenefitsRow.DemographicId, ibmVerificationOfBenefitsRow.CarePathCaseId),
            PatientIdentifiers = GetPatientIdentifiers(ibmVerificationOfBenefitsRow.CarePathPatientId, ibmVerificationOfBenefitsRow.DemographicId)
        };
    }

    private List<TriageImage> GetTriageImages(
        string? patientDemographicId,
        string? caseId)
    {
        var imageFileNamePattern = _verificationOfBenefitsHelper.DeriveImageFileNamePattern(
            "CarePath_VOB_",
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

    public async Task<(StringBuilder, int)> ProcessFilesAndBuildReportEmailAsync(IEnumerable<string> fileNames,
        CancellationToken c)
    {
        int dataFileCount = 0;
        DateOnly runFor = DateOnly.FromDateTime(DateTime.Now);
        StringBuilder emailBody = new();
        emailBody.AppendLine($"IBM reconciliation reporting for {runFor:MM/dd/yyyy}");

        foreach (var file in fileNames)
        {
            // Archive control files, which are not used
            if (file.Contains("_Control", StringComparison.OrdinalIgnoreCase))
            {
                _dataFileInterface.ArchiveFile(file);
                continue;
            }

            if (file.Contains("_RERUN", StringComparison.OrdinalIgnoreCase))
            {
                var fileDate = DateTime.ParseExact(Regex.Match(file, @"\d{14}").Value, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                TimeSpan time = DateTime.Now - fileDate;
                if (time.Days > 7)
                {
                    _dataFileInterface.ArchiveFile(file);
                    continue;
                }
            }
            dataFileCount++;

            IEnumerable<IbmVerificationOfBenefitsRow> ibmVerificationOfBenefits = await _dataFileInterface.ReadFileToListAsync<IbmVerificationOfBenefitsRow>(
                file,
                "\n",
                "|",
                true,
                runFor,
                c).ConfigureAwait(false);

            var verificationOfBenefit = ibmVerificationOfBenefits.Select(MapVerificationOfBenefits);

            // Make API calls to import triage data to Emplifi
            var verificationOfBenefitsNotification = await _emplifiInterface.ProcessVerificationOfBenefitsAsync(verificationOfBenefit, file, c).ConfigureAwait(false);


            emailBody.AppendLine($"<br /><br />File: {Path.GetFileName(file)}");
            emailBody.AppendLine($"<br />Records Received: {verificationOfBenefitsNotification.RecordsRead:#,##0}");
            emailBody.AppendLine($"<br />Records Loaded: {verificationOfBenefitsNotification.RecordsLoaded:#,##0}");
            emailBody.AppendLine($"<br />Records Failed: {verificationOfBenefitsNotification.RecordsFailed:#,##0}");

            if (verificationOfBenefitsNotification.FailedImageFileNames.Count > 0)
            {
                foreach (var failedImageFileName in verificationOfBenefitsNotification.FailedImageFileNames)
                {
                    _dataFileInterface.RejectImageFile(failedImageFileName);
                }

                emailBody.AppendLine($"<br />Images Failed: {verificationOfBenefitsNotification.FailedImageFileNames.Count:#,##0}" +
                    $" - see \\\\wfm.wegmans.com\\departments\\Pharmacy\\HIPAARxOffice\\failed_images\\");
            }

            if (verificationOfBenefitsNotification.ErrorMessages.Any())
            {
                emailBody.AppendLine($"<br />");
                verificationOfBenefitsNotification.ErrorMessages.ForEach(error => emailBody.AppendLine($"<br />{error}"));
            }
        }

        return (emailBody, dataFileCount);
    }


    private List<PatientIdentifier> GetPatientIdentifiers(
        string? carePathPatientId,
        string? demographicId)
    {
        List<PatientIdentifier> patientIndentifiers = [];
        if (!string.IsNullOrWhiteSpace(demographicId))
        {
            patientIndentifiers.Add(new PatientIdentifier
            {
                Type = DemographicIdType,
                Value = demographicId
            });
        }

        if (!string.IsNullOrWhiteSpace(carePathPatientId))
        {
            patientIndentifiers.Add(new PatientIdentifier
            {
                Type = PatientIdType,
                Value = carePathPatientId
            });
        }

        return patientIndentifiers;
    }
}
