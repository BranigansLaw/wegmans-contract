namespace Library.EmplifiInterface
{
    public class EmplifiConfig
    {
        public string Url { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string InputFileLocation { get; set; } = null!;

        public string OutputFileLocation { get; set; } = null!;

        public string ArchiveFileLocation { get; set; } = null!;

        public string RejectFileLocation { get; set; } = null!;

        public string? NotificationEmailTo { get; set; }

        public string? EmplifiDispenseNotificationEmailTo { get; set; }

        public string ImageFileLocation { get; set; } = null!;

        public string? EmplifiTriageNotificationEmailTo { get; set; }

        public string? EmplifiEligibilityNotificationEmailTo { get; set; }
    }
}
