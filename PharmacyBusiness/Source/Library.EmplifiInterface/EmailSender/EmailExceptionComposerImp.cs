using Library.EmplifiInterface.Exceptions;
using System.Net.Mail;
using System.Text;

namespace Library.EmplifiInterface.EmailSender
{
    public class EmailExceptionComposerImp : IEmailExceptionComposer
    {
        public const string RejectFilePrefix = "DATA_INTEGRITY_DETAILS_";
        private const string _emailSubjectTemplate = @"INNOVATION ALERT: Batch job [{0}] for Run Date and Time [{1}] has some data corrections that require your attention.";

        private const string _emailBodyTemplate = @"<!DOCTYPE html><html><head></head><body><b>Hello Business Team,</b><br /><br />
This email is to inform you that Control-M batch job <b>[{0}]</b> Delay and Denial Status has some data corrections that require your attention.
 {1} 
See attachment containing Case and Issue numbers that have data integrity issues, minus the data itself.
<br /><br />
Any corrections made will be picked up in the next job run following your updates.
Thanks,<br />
<i>Wegmans Pharmacy IT Development Team</i><br />
<br />
If you no longer wish to receive these emails, please create an email rule based on the subject beginning with ""INNOVATION ALERT"".
</body></html>";

        private const string _emailBodyDataTemplate = @"<!DOCTYPE html><html><head></head><body><b>Hello Business Team,</b><br /><br />
This email is to inform you that Control-M batch job <b>[{0}]</b> Astute Adherence Calls has some data corrections that require your attention.
See attachment containing data corrections to be made, minus the data itself.
<br /><br />
Any corrections made will be picked up in the next job run following your updates.
Thanks,<br />
<i>Wegmans Pharmacy IT Development Team</i><br />
<br />
If you no longer wish to receive these emails, please create an email rule based on the subject beginning with ""INNOVATION ALERT"".
</body></html>";

        private const string _emailBodyTriageTemplate = @"<!DOCTYPE html><html><head></head><body>{0}</body></html>";

        private MailMessage _mailMessage;
        private string _batchJobName;
        private DateTime _runFor;
        private static readonly char[] emailSeparator = [',', ';'];

        public EmailExceptionComposerImp(
            string batchJobName,
            DateTime runFor,
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
            string? notificationEmailTo,
            List<string> exceptions)
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

            SetEmailRejectsAttachment(rejectsFilePath, dataIntegrityException, exceptions);

            return _mailMessage;
        }

        public MailMessage ComposeDataEmail(
            string rejectsFilePath,
            Dictionary<decimal, List<string>> dataConstraints,
            string? notificationEmailTo,
            string outputFileName,
            List<string> exceptions)
        {
            if (string.IsNullOrEmpty(rejectsFilePath))
            {
                throw new ArgumentNullException(nameof(rejectsFilePath));
            }

            _mailMessage.Subject = string.Format(_emailSubjectTemplate,
                _batchJobName,
                _runFor.ToString("MM/dd/yyyy"));

            _mailMessage.Body = string.Format(_emailBodyDataTemplate,
                _batchJobName);

            foreach (var emailAddress in ParseNotificationEmailTo(notificationEmailTo))
            {
                _mailMessage.To.Add(new MailAddress(emailAddress.Trim()));
            }

            SetEmailRejectsAttachmentDataErrors(rejectsFilePath, dataConstraints, outputFileName, exceptions);

            return _mailMessage;
        }

        public MailMessage ComposeNotificationEmail(
            string? notificationEmailTo,
            string? notificationEmailSubject,
            string? notificationEmailBody)
        {
            _mailMessage.Subject = notificationEmailSubject;
            _mailMessage.Body = string.Format(_emailBodyTriageTemplate, notificationEmailBody);

            foreach (var emailAddress in ParseNotificationEmailTo(notificationEmailTo))
            {
                _mailMessage.To.Add(new MailAddress(emailAddress.Trim()));
            }

            return _mailMessage;
        }

        public void SetEmailRejectsAttachmentDataErrors(string rejectsFilePath,
            Dictionary<decimal, List<string>> dataConstraints,
            string outputFileName,
            List<string> exceptions)
        {
            if (string.IsNullOrEmpty(rejectsFilePath))
            {
                throw new ArgumentNullException(nameof(rejectsFilePath));
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("RxFillSequence, RequiredField");

            if (dataConstraints is not null && dataConstraints.Count > 0)
            {
                foreach (var dataConstraint in dataConstraints)
                {
                    sb.AppendLine($"{dataConstraint.Key}, {string.Join(", ", dataConstraint.Value)}");
                }
            }

            if (exceptions is not null && exceptions.Count > 0)
            {
                exceptions = exceptions.Distinct().OrderBy(x => x).ToList();
                sb.AppendLine("\nErrors Encountered");
                foreach (var exception in exceptions)
                {
                    sb.AppendLine(exception);
                }
            }

            string rejectsOnlyFile = Path.Combine(rejectsFilePath, $"{RejectFilePrefix}{outputFileName}");
            using (StreamWriter sw = File.CreateText(rejectsOnlyFile))
            {
                sw.Write(sb.ToString());
            }

            _mailMessage.Attachments.Add(new Attachment(rejectsOnlyFile));
        }

        /// <inheritdoc />
        public void SetEmailRejectsAttachment(string rejectsFilePath, DataIntegrityException dataIntegrityException, List<string> exceptions)
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

            string rejectDetails = string.Join("\n", dataIntegrityException?.DataSourceConstraintViolations?.ToArray() ?? new[] { "" });
            
            rejectDetails += "\n\nErrors Encountered\n";
            foreach (var exception in exceptions)
            {
                rejectDetails += exception + "\n";
            }

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
