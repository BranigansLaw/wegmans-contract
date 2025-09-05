namespace Library.TenTenInterface.DownloadsFromTenTen
{
    public class DataExtractFileSpecifications
    {
        public string FileName { get; set; }
        public string FieldDelimiter { get; set; }
        public bool HasFileHeader { get; set; }
        public string LineTerminator { get; set; }
        public string? ReplacementHeader { get; set; }

        public DataExtractFileSpecifications(
            string fileNamePattern, 
            string fieldDelimiter, 
            bool hasHeaderRow,
            string lineTerminator = "\n",
            string? replacementHeader = null)
        {
            this.FileName = fileNamePattern;
            this.FieldDelimiter = fieldDelimiter;
            this.LineTerminator = lineTerminator;
            this.HasFileHeader = hasHeaderRow;
            this.ReplacementHeader = replacementHeader;
        }
    }
}
