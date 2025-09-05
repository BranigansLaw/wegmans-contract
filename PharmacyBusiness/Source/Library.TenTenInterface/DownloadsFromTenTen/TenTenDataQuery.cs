namespace Library.TenTenInterface.DownloadsFromTenTen
{
    public class TenTenDataQuery
    {
        public TenTenDataQuery(string tenTenDownloadQueryXml, string[] columnNames, DateOnly runFor)
        {
            TenTenDownloadQueryXml = tenTenDownloadQueryXml;
            ColumnNames = columnNames;
            RunFor = runFor;
        }

        public string TenTenDownloadQueryXml { get; init; }

        public string[] ColumnNames { get; init; }

        public DateOnly RunFor { get; init; }
    }
}
