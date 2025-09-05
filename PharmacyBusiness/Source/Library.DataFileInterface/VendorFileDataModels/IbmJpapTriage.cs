using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.Exceptions;

namespace Library.DataFileInterface.VendorFileDataModels;

public class IbmJpapTriage : IDataRecord
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
    public string? PatientZip { get; set; }
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
    public string? PrescriberZip { get; set; }
    public string? PrescriberPhone { get; set; }
    public string? PrescriberFax { get; set; }
    public string? TreatmentCenterName { get; set; }
    public string? TreatmentCenterContactLastName { get; set; }
    public string? TreatmentCenterContactFirstName { get; set; }
    public string? TreatmentCenterAddress1 { get; set; }
    public string? TreatmentCenterAddress2 { get; set; }
    public string? TreatmentCenterCity { get; set; }
    public string? TreatmentCenterState { get; set; }
    public string? TreatmentCenterZip { get; set; }
    public string? TreatmentCenterPhone { get; set; }
    public string? TreatmentCenterFax { get; set; }
    public string? TreatmentCenterNpi { get; set; }
    public string? TreatmentCenterDea { get; set; }
    public string? ShipToLocation { get; set; }
    public string? PatientDemographicId { get; set; }
    public string? CarePathTransactionId { get; set; }
    public string? ImageCount { get; set; }

    /// <inheritdoc />
    public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns, DateOnly runFor)
    {
        Type classType = typeof(IbmJpapTriage);
        int numberOfDataFileProperties = classType.GetProperties().Where(r => r.Name.IndexOf("Derived", 0) == -1).ToArray().Length;
        if (dataFileColumns.Count() != numberOfDataFileProperties)
        {
            throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
        }

        RecordTimestamp = dataFileColumns.ElementAt(0);
        CarePathSpecialtyPharmacyName = dataFileColumns.ElementAt(1);
        CarePathPatientId = dataFileColumns.ElementAt(2);
        PatientLastName = dataFileColumns.ElementAt(3);
        PatientFirstName = dataFileColumns.ElementAt(4);
        PatientDateOfBirth = dataFileColumns.ElementAt(5);
        PatientGender = dataFileColumns.ElementAt(6);
        PatientAddress1 = dataFileColumns.ElementAt(7);
        PatientAddress2 = dataFileColumns.ElementAt(8);
        PatientCity = dataFileColumns.ElementAt(9);
        PatientState = dataFileColumns.ElementAt(10);
        PatientZip = dataFileColumns.ElementAt(11);
        PatientPhoneNumber = dataFileColumns.ElementAt(12);
        ProductName = dataFileColumns.ElementAt(13);
        NdcCode = dataFileColumns.ElementAt(14);
        PrimaryDiagnosisCode = dataFileColumns.ElementAt(15);
        PrescriberLastName = dataFileColumns.ElementAt(16);
        PrescriberFirstName = dataFileColumns.ElementAt(17);
        PrescriberNpi = dataFileColumns.ElementAt(18);
        PrescriberDea = dataFileColumns.ElementAt(19);
        PrescriberAddress1 = dataFileColumns.ElementAt(20);
        PrescriberAddress2 = dataFileColumns.ElementAt(21);
        PrescriberCity = dataFileColumns.ElementAt(22);
        PrescriberState = dataFileColumns.ElementAt(23);
        PrescriberZip = dataFileColumns.ElementAt(24);
        PrescriberPhone = dataFileColumns.ElementAt(25);
        PrescriberFax = dataFileColumns.ElementAt(26);
        TreatmentCenterName = dataFileColumns.ElementAt(27);
        TreatmentCenterContactLastName = dataFileColumns.ElementAt(28);
        TreatmentCenterContactFirstName = dataFileColumns.ElementAt(29);
        TreatmentCenterAddress1 = dataFileColumns.ElementAt(30);
        TreatmentCenterAddress2 = dataFileColumns.ElementAt(31);
        TreatmentCenterCity = dataFileColumns.ElementAt(32);
        TreatmentCenterState = dataFileColumns.ElementAt(33);
        TreatmentCenterZip = dataFileColumns.ElementAt(34);
        TreatmentCenterPhone = dataFileColumns.ElementAt(35);
        TreatmentCenterFax = dataFileColumns.ElementAt(36);
        TreatmentCenterNpi = dataFileColumns.ElementAt(37);
        TreatmentCenterDea = dataFileColumns.ElementAt(38);
        ShipToLocation = dataFileColumns.ElementAt(39);
        PatientDemographicId = dataFileColumns.ElementAt(40);
        CarePathTransactionId = dataFileColumns.ElementAt(41);
        ImageCount = dataFileColumns.ElementAt(42);
    }
}