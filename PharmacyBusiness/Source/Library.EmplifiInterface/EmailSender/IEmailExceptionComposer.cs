using Library.EmplifiInterface.Exceptions;
using System.Net.Mail;

namespace Library.EmplifiInterface.EmailSender
{
    public interface IEmailExceptionComposer
    {
        /// <summary>
        /// Compose an email message for a data integrity exception.
        /// </summary>
        /// <param name="rejectsFilePath"></param>
        /// <param name="dataIntegrityException"></param>
        /// <param name="notificationEmailTo"></param>
        /// <returns></returns>
        MailMessage Compose(
            string rejectsFilePath,
            DataIntegrityException dataIntegrityException,
            string? notificationEmailTo,
            List<string> exceptions);

        /// <summary>
        /// Create an attachment file of vendor data reject records and set it to the email message.
        /// </summary>
        /// <param name="rejectsFilePath"></param>
        /// <param name="dataIntegrityException"></param>
        void SetEmailRejectsAttachment(
            string rejectsFilePath, 
            DataIntegrityException dataIntegrityException,
            List<string> exceptions);

        MailMessage ComposeDataEmail(
            string rejectsFilePath,
            Dictionary<decimal, List<string>> dataConstraints,
            string? notificationEmailTo,
            string outputFileName,
            List<string> exceptions);

        void SetEmailRejectsAttachmentDataErrors(string rejectsFilePath,
            Dictionary<decimal, List<string>> dataConstraints,
            string outputFileName,
            List<string> exceptions);

        /// <summary>
        /// Compose a notification email message.
        /// </summary>
        /// <param name="notificationEmailTo"></param>
        /// <param name="notificationEmailSubject"></param>
        /// <param name="notificationEmailBody"></param>
        /// <returns></returns>
        MailMessage ComposeNotificationEmail(
            string? notificationEmailTo,
            string? notificationEmailSubject,
            string? notificationEmailBody);
    }
}