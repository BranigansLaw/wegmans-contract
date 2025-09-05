namespace RX.PharmacyBusiness.ETL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Mail;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Wegmans.PharmacyLibrary.Email;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;
    using Wegmans.PharmacyLibrary.Settings;

    /// <summary>
    /// This EmailHelper class sends emails, naturally. 
    /// Emails themselves should not be critical to any job functionality due to occasional network outages, and client-side tools like Clutter that will silently block emails.
    /// </summary>
    public class EmailHelper
    {
        private string emailFrom;
        private string emailTo;
        private string emailCc;
        private string emailBcc;
        private IFileManager fileManager;
        private IEmailSender emailSender;

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public EmailHelper(
            string emailFrom,
            string emailTo,
            string emailCc,
            string emailBcc)
        {
            this.fileManager = new FileManager();
            this.emailSender = new EmailSender();
            this.emailFrom = emailFrom;
            this.emailTo = emailTo;
            this.emailCc = emailCc;
            this.emailBcc = emailBcc;
        }

        /// <summary>
        /// Send an email without attachments.
        /// </summary>
        /// <param name="emailSubject">string subject</param>
        /// <param name="emailBody">string body</param>
        /// <param name="mailPriority">enum System.Net.Mail.MailPriority</param>
        /// <param name="isBodyHtml">boolean for HTML formatting</param>
        public void SendEmail(
            string emailSubject,
            string emailBody,
            MailPriority mailPriority,
            bool isBodyHtml)
        {
            SendEmail(
                emailSubject,
                emailBody,
                new List<string>(),
                string.Empty,
                mailPriority,
                isBodyHtml);
        }

        /// <summary>
        /// SendEmail with attachements.
        /// </summary>
        /// <param name="emailSubject">string subject</param>
        /// <param name="emailBody">string body</param>
        /// <param name="emailAttachments">List of type string of file names include path.</param>
        /// <param name="zippedAttachmentsFileName">Leave blank if you do not want to zip the attachments, or specify one zip file name including path to put all attachments into one zip file attachment. For example, could be [@"%BATCH_ROOT%\RXS573\Output\Excellus_" + runDate.ToString("yyyyMMdd") + ".zip"].</param>
        /// <param name="mailPriority">enum System.Net.Mail.MailPriority</param>
        /// <param name="isBodyHtml">boolean for HTML formatting</param>
        public void SendEmail(
            string emailSubject,
            string emailBody,
            IEnumerable<string> emailAttachments,
            string zippedAttachmentsFileName,
            MailPriority mailPriority,
            bool isBodyHtml)
        {
            try
            {
                if (string.IsNullOrEmpty(emailFrom) || string.IsNullOrEmpty(emailTo) || string.IsNullOrEmpty(emailSubject))
                {
                    Log.LogWarn("Emails from any Macro Helix jobs have these required parameters: emailFrom [{0}], emailTo [{1}], and emailSubject [{2}].",
                        emailFrom, emailTo, emailSubject);
                }
                else
                {
                    Log.LogInfo("Drafting the mail message.");
                    using (var emailMessage = new MailMessage())
                    {
                        emailMessage.IsBodyHtml = isBodyHtml;
                        emailMessage.Priority = mailPriority;
                        emailMessage.Subject = emailSubject;
                        emailMessage.Body = emailMessage.IsBodyHtml ? emailBody : Regex.Replace(emailBody, "<.*?>", string.Empty); //Remove all HTML tags for plain text.
                        emailMessage.From = new MailAddress(this.emailFrom);

                        foreach(string addr in this.emailTo.Split(','))
                        { 
                            if (!string.IsNullOrEmpty(addr.Trim()))
                                emailMessage.To.Add(new MailAddress(addr.Trim()));
                        }
                        foreach (string addr in this.emailCc.Split(','))
                        {
                            if (!string.IsNullOrEmpty(addr.Trim()))
                                emailMessage.CC.Add(new MailAddress(addr.Trim()));
                        }
                        foreach (string addr in this.emailBcc.Split(','))
                        {
                            if (!string.IsNullOrEmpty(addr.Trim()))
                                emailMessage.Bcc.Add(new MailAddress(addr.Trim()));
                        }

                        if (emailAttachments.Any())
                        {
                            if (!string.IsNullOrEmpty(zippedAttachmentsFileName))
                            {
                                string zipFile = Environment.ExpandEnvironmentVariables(zippedAttachmentsFileName);
                                Log.LogInfo("Adding [{0}] file(s) to one zip file named [{1}].", emailAttachments.ToList().Count, zipFile);
                                this.fileManager.MakeZipFile(zipFile, emailAttachments, false);
                                if ((this.fileManager.GetFileBytes(zipFile) < EmailSender.WegmansEmailAttachmentByteSizeLimit))
                                {
                                    emailAttachments = new List<string>();
                                    emailAttachments.Append(zipFile);
                                }
                            }

                            if (!this.emailSender.TryAddAttachmentsToMessage(emailMessage, emailAttachments.ToArray()))
                                Log.LogWarn("Could not add attachments to the email.");
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

                        Log.LogInfo("Calling email sender.");
                        this.emailSender.Send(emailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                //Sending emails should not be critical to the process, so log the exception and continue on.
                Log.LogWarn("Exception occured in drafting the MailMessage.", exception);
            }
        }
    }
}
