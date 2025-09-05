namespace XXXDeveloperTools.CompareDerivedData.Helper
{
    public class CompareDerivedDataSpecifications
    {
        public required string NewFilePath { get; set; }
        public required string OldFilePath { get; set; }
        public required string FileName { get; set; }
        public required string RowDelimiter { get; set; }
        public required string ColumnDelimiter { get; set; }
        public required bool HasHeaderRow { get; set; }
    }
}
