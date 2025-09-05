namespace Library.EmplifiInterface.DataModel;

public class JpapTriage
{
    public string? CarePathSpecialtyPharmacyName { get; set; }
    public string? CarePathPatientId { get; set; }
    public string? PatientLastName { get; set; }
    public string? PatientFirstName { get; set; }
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

    public DateTime? DerivedPatientDateOfBirth { get; set; }
    public string? DerivedDrugName { get; set; }
    public string? DerivedPatientGender { get; set; }
    public string? DerivedProgramHeader { get; set; }
    public string? DerivedCaseText { get; set; }
    public List<TriageImage>? DerivedImages { get; set; }
}
