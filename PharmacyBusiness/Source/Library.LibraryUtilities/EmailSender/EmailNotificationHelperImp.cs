using Microsoft.Extensions.Logging;
using System.Net.Mail;

namespace Library.LibraryUtilities.EmailSender
{
    public class EmailNotificationHelperImp : IEmailNotificationHelper
    {
        public MailPriority ParquetUploadEmailPriority { get; set; } = MailPriority.Low;

        private readonly IEmailSenderInterface _emailSender;
        private readonly ILogger<EmailNotificationHelperImp> _logger;

        public EmailNotificationHelperImp(
            IEmailSenderInterface emailSender,
            ILogger<EmailNotificationHelperImp> logger)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task NotifyOfParquetUploadAsync(
            string environmentFolderName, 
            string feedName, 
            DateOnly runDate,
            int uploadRowCount,
            string emailTo,
            CancellationToken c)
        {
            _logger.LogInformation($"Sending email notification that parquet upload just completed to Env=[{environmentFolderName}] Feed=[{feedName}].");

            // Send email notifcation that parquet upload just completed just as a courtesy to the 1010data team.
            MailMessage emailNotification = EmailParquetUploadNotificationComposerImp.Compose(
                environmentFolderName,
                feedName,
                runDate,
                uploadRowCount,
                ParquetUploadEmailPriority,
                emailTo);

            try
            {
                await _emailSender.SmtpClientSendMailAsync(emailNotification, c).ConfigureAwait(false); ;
            }
            catch (Exception ex)
            {
                // No need to throw exceptions here, as this email is just a courtesy notification, so just log any errors.
                _logger.LogError(ex, "Failed to send email notification that parquet upload just completed, but a failure while sending an email is not a good reason to also fail the job.");
            }
        }
    }
}
