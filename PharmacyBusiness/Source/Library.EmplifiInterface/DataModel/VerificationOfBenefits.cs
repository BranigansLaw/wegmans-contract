namespace Library.EmplifiInterface.DataModel;

public class VerificationOfBenefits
{
    public DateTime? RecordTimestamp { get; set; }
    public string? CarePathSpecialtyPharmacyName { get; set; }
    public string? CarePathPatientId { get; set; }
    public string? PatientBirthYear { get; set; }
    public string? PayerType { get; set; }
    public string? SpecialtyPharmacyName { get; set; }
    public string? SpecialtyPharmacyPhone { get; set; }
    public string? ImageExists { get; set; }
    public string? CarePathCaseId { get; set; }
    public DateTime? PatientEnrollmentFormReceived { get; set; }
    public string? ExternalPatientId { get; set; }
    public string? ProductName { get; set; }
    public string? DemographicId { get; set; }

    public string? DerivedCaseText { get; set; }
    public List<TriageImage>? DerivedImages { get; set; }
    public List<PatientIdentifier>? PatientIdentifiers { get; set; }
}
