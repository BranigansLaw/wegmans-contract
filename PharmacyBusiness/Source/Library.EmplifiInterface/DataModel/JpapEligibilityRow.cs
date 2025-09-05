namespace Library.EmplifiInterface.DataModel;

public class JpapEligibilityRow
{
    public string? RecordTimestamp { get; set; }
    public string? PatientProgramEnrollmentName { get; set; }
    public string? Status { get; set; }
    public string? EnrollmentStatus { get; set; }
    public string? Outcome { get; set; }
    public string? Product { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? PatientId { get; set; }

    public string? DerivedCaseText { get; set; }
}
