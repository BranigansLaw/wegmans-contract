namespace Library.McKessonCPSInterface
{
    public class McKessonCPSConfig
    {
        public string SqlServerDatabaseConnection { get; set; } = null!;

        public string DataFileArchivePath { get; set; } = null!;

        public string DataFileExportPath { get; set; } = null!;

        public string DataFileImportPath { get; set; } = null!;
    }
}
