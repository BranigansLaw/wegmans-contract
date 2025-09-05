using System.Net.Mail;

namespace Library.DataFileInterface.EmailSender
{
    public interface IEmailSenderInterface
    {
        /// <summary>
        /// Validates required properties of a MailMessage object.
        /// </summary>
        /// <param name="mailMessage">A MailMessage object of the email to be sent.</param>
        /// <returns>bool value of overall requirements of a Message object.</returns>
        bool ValidateMessage(MailMessage mailMessage);

        /// <summary>
        /// Adds attachments to MailMessage object if each attachment is valid.
        /// </summary>
        /// <param name="mailMessage">A MailMessage object of the email to be sent.</param>
        /// <param name="newAttachments">List of files to attach.</param>
        /// <returns>bool value if new attachments were added to Message object.</returns>
        bool TryAddAttachmentsToMessage(MailMessage mailMessage, params string[] newAttachments);

        /// <summary>
        /// Adds attachments to MailMessage object if each attachment is valid.
        /// </summary>
        /// <param name="mailMessage">A MailMessage object of the email to be sent.</param>
        /// <param name="newAttachments">List of attachments to attach.</param>
        /// <returns>>bool value if new attachments were added to Message object.</returns>
        bool TryAddAttachmentsToMessage(MailMessage mailMessage, params Attachment[] newAttachments);

        /// <summary>
        /// Validates MailMessage Attachment requirements.
        /// </summary>
        /// <param name="mailMessage">A MailMessage object of the email to be sent.</param>
        /// <returns>>bool value of MailMessage Attachment requirements.</returns>
        bool ValidateMessageAttachments(MailMessage mailMessage);

        /// <summary>
        /// Sends email using SmtpClient.
        /// </summary>
        /// <param name="mailMessage"></param>
        void SmtpClientSendMail(MailMessage mailMessage);


        /// <summary>
        /// Send email without attachments.
        /// </summary>
        /// <param name="emailFrom"></param>
        /// <param name="emailTo"></param>
        /// <param name="emailCc"></param>
        /// <param name="emailBcc"></param>
        /// <param name="emailSubject"></param>
        /// <param name="emailBody"></param>
        void SendEmail(
            string emailFrom,
            string[] emailTo,
            string[] emailCc,
            string[] emailBcc,
            string emailSubject,
            string emailBody);

        /// <summary>
        /// SendEmail with attachements.
        /// </summary>
        /// <param name="emailFrom"></param>
        /// <param name="emailTo"></param>
        /// <param name="emailCc"></param>
        /// <param name="emailBcc"></param>
        /// <param name="emailSubject"></param>
        /// <param name="emailBody"></param>
        /// <param name="emailAttachments"></param>
        /// <param name="mailPriority"></param>
        /// <param name="isBodyHtml"></param>
        void SendEmail(
            string emailFrom,
            string[] emailTo,
            string[] emailCc,
            string[] emailBcc,
            string emailSubject,
            string emailBody,
            IEnumerable<string> emailAttachments,
            MailPriority mailPriority,
            bool isBodyHtml);
    }
}
