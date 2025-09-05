namespace Library.TenTenInterface
{
    public class TenTenConfig
    {
        public string Url { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string SamOwnerId { get; set; } = null!;

        public string SamGroupId { get; set; } = null!;

        public string SamPassword { get; set; } = null!;

        public string ExecutableBasePath { get; set; } = null!;

        public string TenTenFolderPath { get; set; } = null!;

        public string InputFileLocation { get; set; } = null!;

        public string OutputFileLocation { get; set; } = null!;

        public string ArchiveFileLocation { get; set; } = null!;

        public string RejectFileLocation { get; set; } = null!;

        public bool OverrideTenTenFullTablePath { get; set; }

        public string AzureBlobConnectionString { get; set; } = null!;

        public string AzureBlobEnvironmentFolderName { get; set; } = null!;

        public string KillSettings = "&kill=no";

        public string ParquetUploadNotificationEmailTo { get; set; } = null!;

        public bool? DeveloperMode { get; set; }
    }
}
