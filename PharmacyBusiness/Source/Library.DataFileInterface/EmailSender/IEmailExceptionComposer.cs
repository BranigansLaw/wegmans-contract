using Library.DataFileInterface.Exceptions;
using System.Net.Mail;

namespace Library.DataFileInterface.EmailSender
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
            string? notificationEmailTo);

        /// <summary>
        /// Create an attachment file of vendor data reject records and set it to the email message.
        /// </summary>
        /// <param name="rejectsFilePath"></param>
        /// <param name="dataIntegrityException"></param>
        void SetEmailRejectsAttachment(string rejectsFilePath, DataIntegrityException dataIntegrityException);
    }
}