namespace Library.TenTenInterface.DownloadsFromTenTen
{
    public class TenTenDataExtracts : TenTenDataQuery
    {
        public TenTenDataExtracts(string tenTenDownloadQueryXml, DataExtractFileSpecifications outputFileSpecifications, string[] columnNames, DateOnly runFor) :
            base(tenTenDownloadQueryXml, columnNames, runFor)
        {
            OutputFileSpecifications = outputFileSpecifications;
        }

        public DataExtractFileSpecifications OutputFileSpecifications { get; init; }
    }
}
