namespace Library.McKessonDWInterface
{
    public class McKessonDWConfig
    {
        /// <summary>
        /// If FetchSizeMultiplier is not set then Oracle.ManagedDataAccess.Client will default to 1 which means fetching one row at a time from the database transferring to the batch server running this program causing the downloading of query results to potentially take hours. 
        /// More than 150 might cause other performance issues.
        /// </summary>
        public int FetchSizeMultiplier => 150;

        public bool SelfTuning => false;

        public bool BindByName => true;

        public int CommandTimeout => 0; //Set to 0 seconds for no timeout (take as long as you want).

        public int FetchSize => 1024 * 1024; //About 1 MB           

        public bool DisableOOB => true;

        public string TnsName => "ENTERPRISE_RX";

        public string OracleDatabaseConnection => $"Data Source={TnsDescriptor}; User id={Username}; Password={Password};";

        public string TnsDescriptor { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string DataFileArchivePath { get; set; } = null!;

        public string DataFileExportPath { get; set; } = null!;

        public string DataFileImportPath { get; set; } = null!;
    }
}
