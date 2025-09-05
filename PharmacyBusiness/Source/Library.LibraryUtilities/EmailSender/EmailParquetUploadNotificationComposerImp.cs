using System.Net.Mail;

namespace Library.LibraryUtilities.EmailSender
{
    public static class EmailParquetUploadNotificationComposerImp
    {
        private const string _emailSubjectTemplate = @"Parquet Upload: Env={0}, Feed={1}, RunDate={2}, Rows={3}, Completed={4}";

        private const string _emailBodyTemplate = @"<!DOCTYPE html><html><head></head><body><b>Hello Parquet Fans,</b><br />
This email is to inform you of a parquet upload that completed just now.<br />
<br />
<table style='border:1px solid black;' cellpadding='0' cellspacing='0'>
    <tr>
        <th style='border:1px solid black; padding:4px;'>Environment</th>
        <th style='border:1px solid black; padding:4px;'>Data Feed Name</th>
        <th style='border:1px solid black; padding:4px;'>Run Date</th>
        <th style='border:1px solid black; padding:4px;'>Rows of Data</th>
        <th style='border:1px solid black; padding:4px;'>Parquet Upload DateTime</th>
    </tr>
    <tr>
        <td style='border:1px solid black; padding:4px; text-align:left;'>{0}</td>
        <td style='border:1px solid black; padding:4px; text-align:center;'>{1}</td>
        <td style='border:1px solid black; padding:4px; text-align:center;'>{2}</td>
        <td style='border:1px solid black; padding:4px; text-align:center;'>{3}</td>
        <td style='border:1px solid black; padding:4px; text-align:right;'>{4}</td>
    </tr>
</table>
<br />
Thanks,<br />
<i>Wegmans Pharmacy IT Development Team</i><br />
<br />
<div style='background-color:#EFEFEF;'>
    <figure>
        <figcaption>Outlook Tips &amp; Tricks</figcaption>
        <ul>
            <li>If you no longer wish to see any of these emails, just create an Outlook Rule based on the subject beginning with ""Parquet Upload:"".</li>
            <li>To focus only on Production-related emails, create your Outlook Rule based on the subject beginning with ""Parquet Upload: Env=PROD"".</li>
            <li>Future work can include creating a distribultion list to make it easier for folks to opt in or out.</li>
        </ul>
    </figure>
</div>
</body></html>";

        /// <inheritdoc />
        public static MailMessage Compose(
            string environment,
            string dataFeedName,
            DateOnly runFor,
            int rowCount,
            MailPriority emailPriority,
            string? notificationEmailTo)
        {
            DateTimeOffset completedAsOfNow = DateTimeOffset.Now;
            string dataFeedFullName = dataFeedName;
            string dataFeedShortName = dataFeedName.Split('.').Last();

            //For now, we are not setting config values for Cc and Bcc fields, but we may in the future.
            string[] emailCc = new string[] { "" };
            string[] emailBcc = new string[] { "Chris.McCarthy@wegmans.com" }; //KEEP Chris in the loop for all emails.

            MailMessage mailMessage = new MailMessage();
            mailMessage.Priority = emailPriority;
            mailMessage.From = new MailAddress("PharmacyIT@wegmans.com");
            mailMessage.IsBodyHtml = true;

            foreach (var emailAddress in ParseNotificationEmailTo(notificationEmailTo))
            {
                mailMessage.To.Add(new MailAddress(emailAddress.Trim()));
            }
            foreach (string addr in emailCc)
            {
                if (!string.IsNullOrEmpty(addr.Trim()))
                    mailMessage.CC.Add(new MailAddress(addr.Trim()));
            }
            foreach (string addr in emailBcc)
            {
                if (!string.IsNullOrEmpty(addr.Trim()))
                    mailMessage.Bcc.Add(new MailAddress(addr.Trim()));
            }

            mailMessage.Subject = string.Format(_emailSubjectTemplate,
                environment.ToUpper(),
                dataFeedShortName,
                runFor.ToString("yyyyMMdd"),
                rowCount,
                completedAsOfNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'zzz"));

            mailMessage.Body = string.Format(_emailBodyTemplate,
                environment.ToUpper(),
                dataFeedFullName,
                runFor.ToString("yyyyMMdd"),
                rowCount,
                completedAsOfNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'zzz"));

            return mailMessage;
        }

        public static string[] ParseNotificationEmailTo(string? notificationEmailTo)
        {
            char[] emailSeparator = [',', ';'];

            // The "To" email address field is required, so if it's empty, send to the Pharmacy IT team.
            if (string.IsNullOrWhiteSpace(notificationEmailTo))
                return ["PharmacyIT@wegmans.com"];

            return notificationEmailTo.Split(emailSeparator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
