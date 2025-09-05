using System.Net.Mail;

namespace Library.LibraryUtilities.EmailSender
{
    public interface IEmailSenderInterface
    {
        /// <summary>
        /// Validates required properties of a MailMessage object.
        /// </summary>
        /// <param name="mailMessage">A MailMessage object of the email to be sent.</param>
        bool IsMailMessageValid(MailMessage mailMessage);

        /// <summary>
        /// Validates Mail Message has just one sender.
        /// </summary>
        /// <param name="mailMessage">A MailMessage object of the email to be sent.</param>
        bool MessageHasOneSender(MailMessage mailMessage);

        /// <summary>
        /// Validates Mail Message has at least one recipient.
        /// </summary>
        /// <param name="mailMessage">A MailMessage object of the email to be sent.</param>
        bool MessageHasAtLeastOneRecipient(MailMessage mailMessage);

        /// <summary>
        /// Validates Mail Message has a subject line.
        /// </summary>
        /// <param name="mailMessage">A MailMessage object of the email to be sent.</param>
        bool MessageHasSubjectLine(MailMessage mailMessage);

        /// <summary>
        /// Sends email using SmtpClient.
        /// </summary>
        /// <param name="mailMessage"></param>
        /// <param name="c"></param>
        Task SmtpClientSendMailAsync(MailMessage mailMessage, CancellationToken c);
    }
}
