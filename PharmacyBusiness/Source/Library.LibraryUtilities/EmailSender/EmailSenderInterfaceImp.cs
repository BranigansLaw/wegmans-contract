using Microsoft.Extensions.Logging;
using System.Net.Mail;

namespace Library.LibraryUtilities.EmailSender
{
    public class EmailSenderInterfaceImp : IEmailSenderInterface
    {
        private readonly ILogger<EmailSenderInterfaceImp> _logger;

        public EmailSenderInterfaceImp(
            ILogger<EmailSenderInterfaceImp> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public bool IsMailMessageValid(MailMessage mailMessage)
        {
            return (MessageHasOneSender(mailMessage) && 
                    MessageHasAtLeastOneRecipient(mailMessage) &&
                    MessageHasSubjectLine(mailMessage));
        }

        /// <inheritdoc />
        public bool MessageHasOneSender(MailMessage mailMessage)
        {
            bool hasOneSender = 
                mailMessage.From != null && 
                mailMessage.From.Address != null && 
                mailMessage.From.Address.Length > 0;

            if (!hasOneSender)
                _logger.LogWarning("Email has no sender.");

            return hasOneSender;
        }

        /// <inheritdoc />
        public bool MessageHasAtLeastOneRecipient(MailMessage mailMessage)
        {
            bool hasAtLeastOneRecipient = 
                mailMessage.To.Count > 0 || 
                mailMessage.CC.Count > 0 || 
                mailMessage.Bcc.Count > 0;

            if (!hasAtLeastOneRecipient)
                _logger.LogWarning("Email has no recipients.");

            return hasAtLeastOneRecipient;
        }

        /// <inheritdoc />
        public bool MessageHasSubjectLine(MailMessage mailMessage)
        {
            bool hasSubjectLine = !string.IsNullOrEmpty(mailMessage.Subject);

            if (!hasSubjectLine)
                _logger.LogWarning("Email has no subject line.");

            return hasSubjectLine;
        }

        /// <inheritdoc />
        public async Task SmtpClientSendMailAsync(MailMessage mailMessage, CancellationToken c)
        {
            if (!IsMailMessageValid(mailMessage))
                throw new ArgumentException("Email is not valid.");

            using (var smtpClient = new SmtpClient
            {
                Host = "smtp.wegmans.com",
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            })
            {
                await smtpClient.SendMailAsync(mailMessage, c);
            }
        }
    }
}
