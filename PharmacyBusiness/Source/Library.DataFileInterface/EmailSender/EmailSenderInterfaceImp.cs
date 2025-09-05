using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Library.DataFileInterface.EmailSender
{
    public class EmailSenderInterfaceImp : IEmailSenderInterface
    {
        /// <summary>
        /// The Wegmans SMTP attachment size limit is 30 MB = 30000000 bytes.
        /// </summary>
        public const long WegmansEmailAttachmentByteSizeLimit = 30000000;

        private readonly SmtpClient client;
        private readonly ILogger<DataFileInterfaceImp> _logger;

        public EmailSenderInterfaceImp(ILogger<DataFileInterfaceImp> logger)
        {
            this.client = new SmtpClient();
            this.client.Host = "smtp.wegmans.com";
            this.client.Port = 25;
            this.client.DeliveryMethod = SmtpDeliveryMethod.Network;
            this.client.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback = (_, __, ___, ____) => true;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public bool ValidateMessage(MailMessage mailMessage)
        {
            bool hasOneSender = !string.IsNullOrEmpty(mailMessage.From?.Address);
            bool hasAtLeastOneRecipient = mailMessage.To.Count > 0 || mailMessage.CC.Count > 0 || mailMessage.Bcc.Count > 0;
            bool hasSubjectLine = !string.IsNullOrEmpty(mailMessage.Subject);
            bool hasValidAttachments = this.ValidateMessageAttachments(mailMessage);
            _logger.LogInformation("Validating email before sending: has one sender [{0}], has at least one recipient [{1}], has a subject line [{2}], and any attachments are within size limits [{3}].",
                hasOneSender,
                hasAtLeastOneRecipient,
                hasSubjectLine,
                hasValidAttachments);
            return hasOneSender && hasAtLeastOneRecipient && hasSubjectLine && hasValidAttachments;
        }

        /// <inheritdoc />
        public bool TryAddAttachmentsToMessage(MailMessage mailMessage, params string[] newAttachments)
        {
            return this.TryAddAttachmentsToMessage(mailMessage, newAttachments.Select(a => new Attachment(Environment.ExpandEnvironmentVariables(a))).ToArray());
        }

        /// <inheritdoc />
        public bool TryAddAttachmentsToMessage(MailMessage mailMessage, params Attachment[] newAttachments)
        {
            bool attachmentsWereAdded = false;

            //Validate existing message attachments.
            if (!this.ValidateMessageAttachments(mailMessage)) return false;

            //Try adding attachments from method parameter.
            foreach (var attachment in newAttachments)
            {
                mailMessage.Attachments.Add(attachment);
                attachmentsWereAdded = this.ValidateMessageAttachments(mailMessage);
                if (!attachmentsWereAdded)
                {
                    mailMessage.Attachments.RemoveAt(mailMessage.Attachments.Count - 1);
                    break;
                }
            }

            return attachmentsWereAdded;
        }

        /// <inheritdoc />
        public bool ValidateMessageAttachments(MailMessage mailMessage)
        {
            long sizeTotal = 0;
            foreach (var attachment in mailMessage.Attachments)
            {
                long size = attachment.ContentDisposition?.Size ?? 0;
                if (size < 0 && attachment.ContentStream != null) size = attachment.ContentStream.Length;
                sizeTotal += size;
            }

            //Log.LogInfo("Message attachments size total is [{0:0.000}] mega bytes (the Wegmans SMTP size limit is {1:0.000} mega bytes).",
            //    sizeTotal / 1000000,
            //    WegmansEmailAttachmentByteSizeLimit / 1000000);
            return sizeTotal < WegmansEmailAttachmentByteSizeLimit;
        }

        /// <inheritdoc />
        public void SendEmail(
            string emailFrom,
            string[] emailTo,
            string[] emailCc,
            string[] emailBcc,
            string emailSubject,
            string emailBody)
        {
            SendEmail(
                emailFrom,
                emailTo,
                emailCc,
                emailBcc,
                emailSubject,
                emailBody,
                new List<string>(),
                MailPriority.Normal,
                true);
        }

        /// <inheritdoc />
        public void SendEmail(
            string emailFrom,
            string[] emailTo,
            string[] emailCc,
            string[] emailBcc,
            string emailSubject,
            string emailBody,
            IEnumerable<string> emailAttachments,
            MailPriority mailPriority,
            bool isBodyHtml)
        {
            try
            {
                if (string.IsNullOrEmpty(emailFrom) || !emailTo.Any() || string.IsNullOrEmpty(emailSubject))
                {
                    _logger.LogWarning("Emails from any Macro Helix jobs have these required parameters: emailFrom [{0}], emailTo [{1}], and emailSubject [{2}].",
                        emailFrom, emailTo, emailSubject);
                }
                else
                {
                    _logger.LogInformation("Drafting the mail message.");
                    using (var emailMessage = new MailMessage())
                    {
                        emailMessage.IsBodyHtml = isBodyHtml;
                        emailMessage.Priority = mailPriority;
                        emailMessage.Subject = emailSubject;
                        emailMessage.Body = emailMessage.IsBodyHtml ? emailBody : Regex.Replace(emailBody, "<.*?>", string.Empty); //Remove all HTML tags for plain text.
                        emailMessage.From = new MailAddress(emailFrom);

                        foreach (string addr in emailTo)
                        {
                            if (!string.IsNullOrEmpty(addr.Trim()))
                                emailMessage.To.Add(new MailAddress(addr.Trim()));
                        }
                        foreach (string addr in emailCc)
                        {
                            if (!string.IsNullOrEmpty(addr.Trim()))
                                emailMessage.CC.Add(new MailAddress(addr.Trim()));
                        }
                        foreach (string addr in emailBcc)
                        {
                            if (!string.IsNullOrEmpty(addr.Trim()))
                                emailMessage.Bcc.Add(new MailAddress(addr.Trim()));
                        }

                        if (emailAttachments.Any())
                        {
                            if (!TryAddAttachmentsToMessage(emailMessage, emailAttachments.ToArray()))
                                _logger.LogWarning("Could not add attachments to the email.");
                        }

                        /*
                        *** Quick reference notes when viewing emails as Plain Text or as HTML in Outlook. ***
                        Formatting, pictures, and links can be turned off in Outlook by going to:
                            1. Click the File tab.
                            2. Click Options.
                            3. Click Trust Center, and then click Trust Center Settings.
                            4. Click Email Security.
                            5. Under Read as Plain Text, select the Read all standard mail in plain text check box.
                        
                        When Outlook is set to view emails as Plain Text there will be line breaks everywhere the cshtml template file has line breaks, so those templates should use line breaks as if read as plain text.
                        */

                        if (emailMessage.IsBodyHtml)
                        {
                            //Create another alternate view of the mail message so that the email client (i.e., Outlook) has the option to view the email as intended in HTML if it is set to do so.
                            AlternateView alternateViewHTML = AlternateView.CreateAlternateViewFromString(emailBody, new System.Net.Mime.ContentType("text/html"));
                            emailMessage.AlternateViews.Add(alternateViewHTML);
                        }

                        _logger.LogInformation("Calling email sender.");
                        this.SmtpClientSendMail(emailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                //Sending emails should not be critical to the process, so log the exception and continue on.
                _logger.LogWarning("Exception occured in drafting the MailMessage:\n{0}.", exception);
            }
        }

        /// <inheritdoc />
        public void SmtpClientSendMail(MailMessage mailMessage)
        {
            if (this.ValidateMessage(mailMessage))
            {
                try
                {
                    _logger.LogInformation("Sending email with subject line [{0}].", mailMessage.Subject);
                    this.client.Send(mailMessage);
                    _logger.LogInformation("Email sent.");
                }
                catch (Exception exception)
                {
                    _logger.LogError("Error sending email with exception.:\n{0}", exception);
                    throw;
                }
            }
            else 
            { 
                throw new ArgumentException("MailMessage did not pass validation!");
            }
        }
    }
}