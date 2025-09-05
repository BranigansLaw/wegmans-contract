using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.Exceptions;

namespace Library.DataFileInterface.VendorFileDataModels;

public class IbmVerificationOfBenefitsRow : IDataRecord
{
    public string? RecordTimestamp { get; set; }
    public string? CarePathSpecialtyPharmacyName { get; set; }
    public string? CarePathPatientId { get; set; }
    public string? PatientBirthYear { get; set; }
    public string? PayerType { get; set; }
    public string? SpecialtyPharmacyName { get; set; }
    public string? SpecialtyPharmacyPhone { get; set; }
    public string? ImageExists { get; set; }
    public string? CarePathCaseId { get; set; }
    public string? PatientEnrollmentFormReceived { get; set; }
    public string? ExternalPatientId { get; set; }
    public string? ProductName { get; set; }
    public string? DemographicId { get; set; }

    /// <inheritdoc />
    public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns, DateOnly runFor)
    {
        Type classType = typeof(IbmVerificationOfBenefitsRow);
        int numberOfDataFileProperties = classType.GetProperties().Where(r => r.Name.IndexOf("Derived", 0) == -1).ToArray().Length;
        if (dataFileColumns.Count() != numberOfDataFileProperties)
        {
            throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
        }

        RecordTimestamp = dataFileColumns.ElementAt(0).Trim();
        CarePathSpecialtyPharmacyName = dataFileColumns.ElementAt(1).Trim();
        CarePathPatientId = dataFileColumns.ElementAt(2).Trim();
        PatientBirthYear = dataFileColumns.ElementAt(3).Trim();
        PayerType = dataFileColumns.ElementAt(4).Trim();
        SpecialtyPharmacyName = dataFileColumns.ElementAt(5).Trim();
        SpecialtyPharmacyPhone = dataFileColumns.ElementAt(6).Trim();
        ImageExists = dataFileColumns.ElementAt(7).Trim();
        CarePathCaseId = dataFileColumns.ElementAt(8).Trim();
        PatientEnrollmentFormReceived = dataFileColumns.ElementAt(9).Trim();
        ExternalPatientId = dataFileColumns.ElementAt(10).Trim();
        ProductName = dataFileColumns.ElementAt(11).Trim();
        DemographicId = dataFileColumns.ElementAt(12).Trim();
    }
}
