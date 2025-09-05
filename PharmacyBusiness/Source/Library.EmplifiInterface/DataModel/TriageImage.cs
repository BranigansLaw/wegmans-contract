namespace Library.EmplifiInterface.DataModel;

public class TriageImage
{
    public required string ImageFileName { get; set; }
    public required Stream Image { get; set; }
}
