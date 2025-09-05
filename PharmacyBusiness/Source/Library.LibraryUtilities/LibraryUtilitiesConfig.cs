namespace Library.LibraryUtilities
{
    public class LibraryUtilitiesConfig
    {
        public string ExecutableRootDirectory { get; set; } = null!;

        public string InputFileLocation { get; set; } = null!;

        public string OutputFileLocation { get; set; } = null!;

        public string ArchiveFileLocation { get; set; } = null!;

        public string RejectFileLocation { get; set; } = null!;
        
        public string ArchiveForQaFileLocation { get; set; } = null!;
    }
}
