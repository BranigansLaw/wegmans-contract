namespace Library.EmplifiInterface.DataModel;

public class EligibilityNotification
{
    public required int RecordsRead { get; set; }
    public required int RecordsLoaded { get; set; }
    public required int RecordsFailed { get; set; }
    public required int RecordsSkipped { get; set; }
    public required List<string> FailedFileNames { get; set; }
    public required List<string> ErrorMessages { get; set; }
}
