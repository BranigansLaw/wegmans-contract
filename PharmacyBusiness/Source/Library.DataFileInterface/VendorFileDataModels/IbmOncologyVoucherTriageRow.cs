using Library.DataFileInterface.Exceptions;
using Library.DataFileInterface.DataFileReader;

namespace Library.DataFileInterface.VendorFileDataModels;

public class IbmOncologyVoucherTriageRow : IDataRecord
{
    public string? RecordTimestamp { get; set; }
    public string? CarePathSpecialtyPharmacyName { get; set; }
    public string? CarePathPatientId { get; set; }
    public string? PatientLastName { get; set; }
    public string? PatientFirstName { get; set; }
    public string? PatientDateOfBirth { get; set; }
    public string? PatientGender { get; set; }
    public string? PatientAddress1 { get; set; }
    public string? PatientAddress2 { get; set; }
    public string? PatientCity { get; set; }
    public string? PatientState { get; set; }
    public string? PatientZipCode { get; set; }
    public string? PatientPhoneNumber { get; set; }
    public string? ProductName { get; set; }
    public string? NdcCode { get; set; }
    public string? PrimaryDiagnosisCode { get; set; }
    public string? PrescriberLastName { get; set; }
    public string? PrescriberFirstName { get; set; }
    public string? PrescriberNpi { get; set; }
    public string? PrescriberDea { get; set; }
    public string? PrescriberAddress1 { get; set; }
    public string? PrescriberAddress2 { get; set; }
    public string? PrescriberCity { get; set; }
    public string? PrescriberState { get; set; }
    public string? PrescriberZipCode { get; set; }
    public string? PrescriberPhoneNumber { get; set; }
    public string? PrescriberFaxNumber { get; set; }
    public string? PatientDemographicId { get; set; }
    public string? CarePathTransactionId { get; set; }
    public string? ImageCount { get; set; }
    public string? TreatmentCenterName { get; set; }
    public string? TreatmentCenterContactLastName { get; set; }
    public string? TreatmentCenterContactFirstName { get; set; }
    public string? TreatmentCenterAddress1 { get; set; }
    public string? TreatmentCenterAddress2 { get; set; }
    public string? TreatmentCenterCity { get; set; }
    public string? TreatmentCenterState { get; set; }
    public string? TreatmentCenterZipCode { get; set; }
    public string? TreatmentCenterPhoneNumber { get; set; }
    public string? TreatmentCenterFaxNumber { get; set; }
    public string? TreatmentCenterNpi { get; set; }
    public string? TreatmentCenterDea { get; set; }
    public string? ShipToLocation { get; set; }

    /// <inheritdoc />
    public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns, DateOnly runFor)
    {
        Type classType = typeof(IbmOncologyVoucherTriageRow);
        int numberOfDataFileProperties = classType.GetProperties().Where(r => r.Name.IndexOf("Derived", 0) == -1).ToArray().Length;
        if (dataFileColumns.Count() != numberOfDataFileProperties)
        {
            throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
        }

        RecordTimestamp = dataFileColumns.ElementAt(0).Trim();
        CarePathSpecialtyPharmacyName = dataFileColumns.ElementAt(1).Trim();
        CarePathPatientId = dataFileColumns.ElementAt(2).Trim();
        PatientLastName = dataFileColumns.ElementAt(3).Trim();
        PatientFirstName = dataFileColumns.ElementAt(4).Trim();
        PatientDateOfBirth = dataFileColumns.ElementAt(5).Trim();
        PatientGender = dataFileColumns.ElementAt(6).Trim();
        PatientAddress1 = dataFileColumns.ElementAt(7).Trim();
        PatientAddress2 = dataFileColumns.ElementAt(8).Trim();
        PatientCity = dataFileColumns.ElementAt(9).Trim();
        PatientState = dataFileColumns.ElementAt(10).Trim();
        PatientZipCode = dataFileColumns.ElementAt(11).Trim();
        PatientPhoneNumber = dataFileColumns.ElementAt(12).Trim();
        ProductName = dataFileColumns.ElementAt(13).Trim();
        NdcCode = dataFileColumns.ElementAt(14).Trim();
        PrimaryDiagnosisCode = dataFileColumns.ElementAt(15).Trim();
        PrescriberLastName = dataFileColumns.ElementAt(16).Trim();
        PrescriberFirstName = dataFileColumns.ElementAt(17).Trim();
        PrescriberNpi = dataFileColumns.ElementAt(18).Trim();
        PrescriberDea = dataFileColumns.ElementAt(19).Trim();
        PrescriberAddress1 = dataFileColumns.ElementAt(20).Trim();
        PrescriberAddress2 = dataFileColumns.ElementAt(21).Trim();
        PrescriberCity = dataFileColumns.ElementAt(22).Trim();
        PrescriberState = dataFileColumns.ElementAt(23).Trim();
        PrescriberZipCode = dataFileColumns.ElementAt(24).Trim();
        PrescriberPhoneNumber = dataFileColumns.ElementAt(25).Trim();
        PrescriberFaxNumber = dataFileColumns.ElementAt(26).Trim();
        PatientDemographicId = dataFileColumns.ElementAt(27).Trim();
        CarePathTransactionId = dataFileColumns.ElementAt(28).Trim();
        ImageCount = dataFileColumns.ElementAt(29).Trim();
        TreatmentCenterName = dataFileColumns.ElementAt(30).Trim();
        TreatmentCenterContactLastName = dataFileColumns.ElementAt(31).Trim();
        TreatmentCenterContactFirstName = dataFileColumns.ElementAt(32).Trim();
        TreatmentCenterAddress1 = dataFileColumns.ElementAt(33).Trim();
        TreatmentCenterAddress2 = dataFileColumns.ElementAt(34).Trim();
        TreatmentCenterCity = dataFileColumns.ElementAt(35).Trim();
        TreatmentCenterState = dataFileColumns.ElementAt(36).Trim();
        TreatmentCenterZipCode = dataFileColumns.ElementAt(37).Trim();
        TreatmentCenterPhoneNumber = dataFileColumns.ElementAt(38).Trim();
        TreatmentCenterFaxNumber = dataFileColumns.ElementAt(39).Trim();
        TreatmentCenterNpi = dataFileColumns.ElementAt(40).Trim();
        TreatmentCenterDea = dataFileColumns.ElementAt(41).Trim();
        ShipToLocation = dataFileColumns.ElementAt(42).Trim();
    }
}

