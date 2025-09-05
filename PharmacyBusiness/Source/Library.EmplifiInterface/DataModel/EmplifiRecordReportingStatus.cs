namespace Library.EmplifiInterface.DataModel
{
    public class EmplifiRecordReportingStatus
    {
        public required string CaseId { get; set; }
        public required string IssueSeq { get; set; }
        public required bool IsValidForReporting { get; set; }
        public required bool NotifyEndUsersForCorrection { get; set; }
        public string? ReportingStatusDescription { get; set; }
        public DateTime? ExtractedDateValueSetInUpdateApiCall { get; set; }
        public bool? CaseWasReleasedAfterUpdateApiCall { get; set; }
    }
}
