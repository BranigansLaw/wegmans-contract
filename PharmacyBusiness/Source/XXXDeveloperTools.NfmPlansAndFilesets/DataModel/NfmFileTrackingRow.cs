namespace XXXDeveloperTools.NfmPlansAndFilesets.DataModel
{
    public class NfmFileTrackingRow
    {
        public string? JobNbr { get; set; }
        public required string PlanName { get; set; }
        public required string FilesetName { get; set; }
        public required string SourceNodeName { get; set; }
        public required string TargetNodeName { get; set; }
        public required string SourcePath { get; set; }
        public required string TargetPath { get; set; }
        public required string SourceFileNamePattern { get; set; }
        public string? TargetFileNamePattern { get; set; }
        public DateTime? DeploymentDate { get; set; }
        public required IEnumerable<string> RecentDecryptedFileNames { get; set; }
        public required IEnumerable<string> RecentEncryptedFileNames { get; set; }
    }
}
