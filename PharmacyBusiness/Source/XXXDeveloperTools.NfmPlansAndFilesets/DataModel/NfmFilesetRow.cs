namespace XXXDeveloperTools.NfmPlansAndFilesets.DataModel
{
    public class NfmFilesetRow
    {
        public required string FilesetName { get; set; }
        public required string SourcePath { get; set; }
        public required string TargetPath { get; set; }
        public required string SourceFileNamePattern { get; set; }
        public string? TargetFileNamePattern { get; set; }
        public DateTime? DeploymentDate { get; set; }
    }
}
