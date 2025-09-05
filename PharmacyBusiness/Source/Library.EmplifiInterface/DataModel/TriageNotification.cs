namespace Library.EmplifiInterface.DataModel;

public class TriageNotification
{
    public required int RecordsRead { get; set; }
    public required int RecordsLoaded { get; set; }
    public required int RecordsFailed { get; set; }
    public required List<string> FailedImageFileNames { get; set; }
    public required List<string> ErrorMessages { get; set; }
}
