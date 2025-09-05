using Library.DataFileInterface.Exceptions;
using System.Net.Mail;

namespace Library.DataFileInterface.EmailSender
{
    public class EmailExceptionComposerImp : IEmailExceptionComposer
    {
        public const string RejectFilePrefix = "DATA_INTEGRITY_DETAILS_";
        private const string _emailSubjectTemplate = @"INNOVATION ALERT: Batch job [{0}] for Run Date [{1}] has failed and requires your attention.";

        private const string _emailBodyTemplate = @"<!DOCTYPE html><html><head></head><body><b>Hello Business Team,</b><br /><br />
This email is to inform you that Control-M batch job <b>[{0}]</b> has failed, and that your feedback is required for next steps.
 {1} 
See attachment containing line numbers and column names of all the data integrity issues within this vendor file, minus the data itself.
<br /><br />
<u>Please reply all to let the support team know what next steps to take, and here are some suggested options:</u>
<ol>
  <li type=""A"">The business team will request a corrected file from the vendor:
    <ol>
      <li type=""1"">The business team will open a ticket with the vendor. Maybe forward this email with its attachment to the vendor, so they know the full scope of issues. The attachment does NOT contain any data but simply line numbers and column references, which should suffice since the vendor should have the original data file.</li>
      <li type=""1"">The support team should then wait until notified that the vendor has provided a revised data file for reprocessing.</li>
      <li type=""1"">The support team should then restart the job.</li>
      <li type=""1"">The support team should then monitor the job rerun and reply all with a status update.</li>
    </ol>
  </li>
  <li type=""A"">The business team has determined these data integrity issues are acceptable, and that a corrected file will not be requested from the vendor:
    <ol>
      <li type=""1"">The support team should force complete the job so that downstream processing can continue with the good data portion of the file already uploaded to 1010data.</li>
    </ol>
  </li>
  <li type=""A"">The business requests the development team to investigate potential changes to the upload process:
    <ol>
      <li type=""1"">The business team believes the data file is accurate, but the upload process has opportunities for improvement which the development team should look into.</li>
      <li type=""1"">The development team might then deploy a bug fix.</li>
      <li type=""1"">The development team might then restart the job.</li>
      <li type=""1"">The development team should then monitor the job rerun and reply all with a status update.</li>
    </ol>
  </li>
</ol>
Thanks,<br />
<i>Wegmans Pharmacy IT Development Team</i><br />
<br />
If you no longer wish to receive these emails, please create an email rule based on the subject beginning with ""INNOVATION ALERT"".
</body></html>";

        private MailMessage _mailMessage;
        private string _batchJobName;
        private DateOnly _runFor;
        private static readonly char[] emailSeparator = [',', ';'];

        public EmailExceptionComposerImp(
            string batchJobName,
            DateOnly runFor,
            MailPriority emailFailuresPriority)
        {
            _batchJobName = batchJobName;
            _runFor = runFor;

            _mailMessage = new MailMessage();
            _mailMessage.Priority = emailFailuresPriority;
            _mailMessage.From = new MailAddress("PharmacyIT@wegmans.com");

            string[] emailCc = new string[] { "" };
            string[] emailBcc = new string[] { "Chris.McCarthy@wegmans.com" }; //KEEP Chris in the loop for all emails.

            foreach (string addr in emailCc)
            {
                if (!string.IsNullOrEmpty(addr.Trim()))
                    _mailMessage.CC.Add(new MailAddress(addr.Trim()));
            }
            foreach (string addr in emailBcc)
            {
                if (!string.IsNullOrEmpty(addr.Trim()))
                    _mailMessage.Bcc.Add(new MailAddress(addr.Trim()));
            }
            _mailMessage.IsBodyHtml = true;
        }

        /// <inheritdoc />
        public MailMessage Compose(
            string rejectsFilePath,
            DataIntegrityException dataIntegrityException,
            string? notificationEmailTo)
        {
            if (string.IsNullOrEmpty(rejectsFilePath))
            {
                throw new ArgumentNullException(nameof(rejectsFilePath));
            }

            _mailMessage.Subject = string.Format(_emailSubjectTemplate,
                _batchJobName,
                _runFor.ToString("MM/dd/yyyy"));

            _mailMessage.Body = string.Format(_emailBodyTemplate,
                _batchJobName,
                dataIntegrityException.DataSourceSummaryOfConstraintViolations);

            foreach (var emailAddress in ParseNotificationEmailTo(notificationEmailTo))
            {
                _mailMessage.To.Add(new MailAddress(emailAddress.Trim()));
            }

            SetEmailRejectsAttachment(rejectsFilePath, dataIntegrityException);
            
            return _mailMessage;
        }

        /// <inheritdoc />
        public void SetEmailRejectsAttachment(string rejectsFilePath, DataIntegrityException dataIntegrityException)
        {
            if (string.IsNullOrEmpty(rejectsFilePath))
            {
                throw new ArgumentNullException(nameof(rejectsFilePath));
            }

            if (dataIntegrityException == null ||
                dataIntegrityException?.DataSourceName == null ||
                dataIntegrityException?.DataSourceConstraintViolations == null ||
                dataIntegrityException?.DataSourceConstraintViolations.Count() == 0)
            {
                return;
            }

            string rejectDetails = string.Join("\n", dataIntegrityException?.DataSourceConstraintViolations?.ToArray() ?? new[] {""} );
            string rejectsOnlyFile = Path.Combine(rejectsFilePath, $"{RejectFilePrefix}{dataIntegrityException?.DataSourceName ?? ".txt"}");
            using (StreamWriter sw = File.CreateText(rejectsOnlyFile))
            {
                sw.Write(rejectDetails);
            }

            _mailMessage.Attachments.Add(new Attachment(rejectsOnlyFile));
        }

        public static string[] ParseNotificationEmailTo(string? notificationEmailTo)
        {
            // The "TO" field is required, so if it's empty, send to the Pharmacy IT team.
            if (string.IsNullOrWhiteSpace(notificationEmailTo))
            {
                return ["PharmacyIT@wegmans.com"];
            }

            return notificationEmailTo.Split(emailSeparator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
