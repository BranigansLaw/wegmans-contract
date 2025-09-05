namespace Library.DataFileInterface
{
    public class DataFileConfig
    {
        public string ExecutableRootDirectory { get; set; } = null!;

        public string InputFileLocation { get; set; } = null!;

        public string OutputFileLocation { get; set; } = null!;

        public string ArchiveFileLocation { get; set; } = null!;

        public string RejectFileLocation { get; set; } = null!;

        public string? NotificationEmailTo { get; set; }

        public string ImageFileLocation { get; set; } = null!;
    }
}
