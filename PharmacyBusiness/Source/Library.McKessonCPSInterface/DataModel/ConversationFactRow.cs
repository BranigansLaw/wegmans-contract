namespace Library.McKessonCPSInterface.DataModel
{
    /// <summary>
    /// The properties listed here related to data types and data requirements within McKesson CPS data source rather than from data destination (1010data).
    /// </summary>
    public class ConversationFactRow
    {
        public required double MessageKey { get; set; }

        public required string ConversationText { get; set; }

        public required DateTime SentDate { get; set; }

        public required string Sender { get; set; }

        public required double CpsConversationNumber { get; set; }

        public required DateTime DueDate { get; set; }

        public required string FollowUpResponse { get; set; }
    }
}
