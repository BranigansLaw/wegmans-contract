namespace RX.PharmacyBusiness.ETL.RX_Job_Notifier
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using Wegmans.PharmacyLibrary.Email;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;

    public class JobNotifier : ETLBase
    {
        public enum UrgencyLevel
        {
            Low,
            Medium,
            High,
            Critical
        }
        public enum ImpactLevel
        {
            Minor,
            Moderate,
            Significant,
            Extensive
        }
        public enum ColorLevel
        {
            LightGreen,
            Yellow,
            Orange,
            OrangeRed
        }

        protected override void Execute(out object result)
        {

            ReturnCode returnCode = new ReturnCode();
            string jobNbr = (this.Arguments["-JobNbr"] == null) ? string.Empty : this.Arguments["-JobNbr"].ToString();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.MinValue : DateTime.ParseExact(this.Arguments["-RunDate"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
            DateTime executionDateTime = (this.Arguments["-ExecutionDateTime"] == null) ? DateTime.MinValue : DateTime.ParseExact(this.Arguments["-ExecutionDateTime"].ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(jobNbr) || runDate == DateTime.MinValue || executionDateTime == DateTime.MinValue)
            {
                Log.LogError("Error: JobNbr, RunDate, and ExecutionDateTime are required!");
                result = 2;
                return;
            }

            //NOTE: This program will be executed from a command file located at X:\BATCH\TenTendata\
            string inputLocation = @"..\RX_Job_Notifier\Input\";
            string outputLocation = @"..\RX_Job_Notifier\Output\";
            string archiveLocation = @"..\RX_Job_Notifier\Archive\";
            string rejectLocation = @"..\RX_Job_Notifier\Reject\";
            IFileManager fileManager = new FileManager();
            FileHelper fileHelper = new FileHelper(inputLocation, outputLocation, archiveLocation, rejectLocation);
            IEmailSender emailSender = new EmailSender();
            string emailFrom = ConfigurationManager.AppSettings["JobNotifier.EmailFrom"].ToString();
            string emailTo = ConfigurationManager.AppSettings["JobNotifier.EmailToCSV"].ToString();
            string emailCc = ConfigurationManager.AppSettings["JobNotifier.EmailCcCSV"].ToString();
            string emailBcc = ConfigurationManager.AppSettings["JobNotifier.EmailBccCSV"].ToString();
            bool isBodyHtml = true;
            UrgencyLevel urgencyLevel = UrgencyLevel.Medium;
            ImpactLevel impactLevel = ImpactLevel.Moderate;
            List<string> logFilesToAttach = new List<string>();
            List<string> emailAttachments = new List<string>();
            string highPriorityBatchJobsCSV = ConfigurationManager.AppSettings["HighPriorityBatchJobsCSV"].ToString();

            Log.LogInfo("Executing JobNotifier with RunDate [{0}] for JobNbr [{1}].", runDate, jobNbr);

            //Set email attributes for high priority Job Nbrs.
            if (highPriorityBatchJobsCSV.IndexOf(jobNbr) > -1)
            {
                urgencyLevel = UrgencyLevel.High;
                impactLevel = ImpactLevel.Significant;
            }

            MailPriority mailPriority = (urgencyLevel == UrgencyLevel.High || urgencyLevel == UrgencyLevel.Critical) ? 
                MailPriority.High : (urgencyLevel == UrgencyLevel.Low) ? 
                    MailPriority.Low : MailPriority.Normal;
            ColorLevel urgencyColorLevel = (urgencyLevel == UrgencyLevel.Critical) ? ColorLevel.OrangeRed :
                (urgencyLevel == UrgencyLevel.High) ? ColorLevel.Orange :
                (urgencyLevel == UrgencyLevel.Medium) ? ColorLevel.Yellow : ColorLevel.LightGreen;
            ColorLevel impactColorLevel = (impactLevel == ImpactLevel.Extensive) ? ColorLevel.OrangeRed :
                (impactLevel == ImpactLevel.Significant) ? ColorLevel.Orange :
                (impactLevel == ImpactLevel.Moderate) ? ColorLevel.Yellow : ColorLevel.LightGreen;
            string emailSubject = "1010data Batch Processing ERROR: [Urgency: " + Enum.GetName(urgencyLevel.GetType(), urgencyLevel) + "] Control-M job " + jobNbr + " that completed at " + executionDateTime.ToString("MM/dd/yyyy HH:mm:ss") + " had some issues.";
            string emailBody = string.Format(@"<!DOCTYPE html>
<html>
<head></head>
<body>
    <b><u>EVERYONE:</u> An error occured while running a 1010data-related batch process. So, this email is to inform a variety of interested parties in the hopes that information is shared as needed between everyone with whatever is known at the time (i.e., recent maintenace or unplanned outages, etc.). Please reply-all to help us reach a more streamlined and effective outcome.</b><br />
    <br />
    <b>Hello Wegmans On Call Support (ITTCSPharmacySupport),</b><br />
    <i>&nbsp;&nbsp;&nbsp;&nbsp;Please verify the batch process ran as configured, as expected, and no systems were down that may have impacted this process.
    Also, please update SmartIt as needed with any information relevant to this ticket.</i><br />
    <br />
    <b>Hello 1010data Support (Alex Bogdan and Support team),</b><br />
    <i>&nbsp;&nbsp;&nbsp;&nbsp;Please verify the reporting database is running normally, and there were no recent changes that may have impacted this process. 
    Also, please review the logs attached to offer guidance on next steps.</i><br />
    <br />
    <b>Hello Wegmans Business Analysts (1010data-Programming-SMEs),</b><br />
    <i>&nbsp;&nbsp;&nbsp;&nbsp;Please verify there were no recent updates/modifications to tables and/or code libraries that may have impacted this process.
    Also, please review the logs attached for root cause analysis.</i><br />
    <br />
    <b>Hello Wegmans Business Leaders (Pharmacy Accounting and/or RxBST team leaders that own the data),</b><br />
    <i>&nbsp;&nbsp;&nbsp;&nbsp;Please monitor this email thread, escalate as needed, and submit any work requests that might increase quality assurance and thus reduce on call going forward.</i><br />
    <br />
    <u>Job Failure Overview:</u><br />
    <span style=""background-color:#dfdfdf; font-weight:bold;"">Urgency: <span style=""background-color:{0}; font-style:italic; font-weight:normal;"">{1}</span>, Impact: <span style=""background-color:{2}; font-style:italic; font-weight:normal;"">{3}</span></span><br />
    Job [{4}] that ran at [{5}] had issue(s) that need your attention. The ITTCSPharmacySupport team does not have access to 1010data nor the capability of deciphering 1010data-specific log files, so this email is intended to leverage subject matter experts outside the team.<br />
    <br />
    <u>Suggestions for <b>Wegmans Business teams</b> when replying-all to this notification:</u><br />
    <i>Consider replying all to this email highlighting one of the responses listed below:</i>
    <ol type=""A"">
        <li>RxAccounting and/or RxBST teams can live with these issues, and no further action is needed. So, ITTCSPharmacySupport can close out related incidents in SmartIt (email ITTCSPharmacySupport@wegmans.com).</li>
        <li>RxAccounting and/or RxBST teams will review any recent changes in 1010data, then ask ITTCSPharmacySupport (email ITTCSPharmacySupport@wegmans.com) and/or Operations (email CompOp@wegmans.com, or call the Help Desk at 585-429-3699) to rerun job {4} in Control-M (see SPROUT tool <a href=""https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGD3NSSAQRIQARJF2OKRIGCNWZF92"">Knowledge Article # KBA00051738</a> to help writing technical rerun instructions for Operations).</li>
        <li>RxAccounting and/or RxBST teams will request a SWAT meeting on Teams with some or all the folks receiving this email (contact ITSupport@wegmans.com and ask for Major Incidents Manager to organize a swat).</li>
        <li>RxAccounting and/or RxBST teams will escalate to 1010data support (reply all with actions items specifically assigned to support@1010data.com).</li>
    </ol>
    <br />
    <i>Thanks,</i><br />
    <i>IT TCS Pharmacy Support</i><br />
</body>
</html>
",
Enum.GetName(urgencyColorLevel.GetType(), urgencyColorLevel),
Enum.GetName(urgencyLevel.GetType(), urgencyLevel),
Enum.GetName(impactColorLevel.GetType(), impactColorLevel),
Enum.GetName(impactLevel.GetType(), impactLevel),
jobNbr,
executionDateTime.ToString("MM/dd/yyyy HH:mm:ss")
);

            string zipFile = string.Format("ZippedLogs_JobNbr{0}_RunDate{1}_ExecutedOn{2}.zip", jobNbr, runDate.ToString("yyyyMMdd"), executionDateTime.ToString("yyyyMMddHHmmss"));
            string logsFolder = string.Format("{0}\\{1}_ExecutedOn{2}\\", inputLocation, jobNbr, executionDateTime.ToString("yyyyMMddHHmmss"));
            logFilesToAttach = fileManager.EnumerateFiles(logsFolder).ToList();

            if (logFilesToAttach.Count > 0)
            { 
                /* TODO: Uncomment once zip program works, and remove the FOREACH just after it.
                fileManager.MakeZipFile(zipFile, logFilesToZip, false);
                if ((fileManager.GetFileBytes(zipFile) < EmailSender.WegmansEmailAttachmentByteSizeLimit))
                {
                    emailAttachments.Add(zipFile);
                }
                */
                foreach(var logFileToAttach in logFilesToAttach)
                {
                    emailAttachments.Add(logFileToAttach);
                }
            }

            using (var emailMessage = new MailMessage())
            {
                emailMessage.IsBodyHtml = isBodyHtml;
                emailMessage.Priority = mailPriority;
                emailMessage.Subject = emailSubject;
                emailMessage.Body = emailMessage.IsBodyHtml ? emailBody : Regex.Replace(emailBody, "<.*?>", string.Empty); //Remove all HTML tags for plain text.
                emailMessage.From = new MailAddress(emailFrom.Trim());
                foreach(string addr in emailTo.Split(','))
                { 
                    if (!string.IsNullOrEmpty(addr.Trim()))
                        emailMessage.To.Add(new MailAddress(addr.Trim()));
                }
                foreach (string addr in emailCc.Split(','))
                {
                    if (!string.IsNullOrEmpty(addr.Trim()))
                        emailMessage.CC.Add(new MailAddress(addr.Trim()));
                }
                foreach (string addr in emailBcc.Split(','))
                {
                    if (!string.IsNullOrEmpty(addr.Trim()))
                        emailMessage.Bcc.Add(new MailAddress(addr.Trim()));
                }

                if (emailMessage.IsBodyHtml)
                {
                    //Create another alternate view of the mail message so that the email client (i.e., Outlook) has the option to view the email as intended in HTML if it is set to do so.
                    AlternateView alternateViewHTML = AlternateView.CreateAlternateViewFromString(emailBody, new System.Net.Mime.ContentType("text/html"));
                    emailMessage.AlternateViews.Add(alternateViewHTML);
                }

                if (emailAttachments.Count > 0)
                {
                    Log.LogInfo("Adding attachments.");
                    if (!emailSender.TryAddAttachmentsToMessage(emailMessage, emailAttachments.ToArray()))
                    { 
                        Log.LogError("Could not add attachments to the email.");
                    }
                }

                Log.LogInfo("Calling email sender.");
                if (!emailSender.Send(emailMessage))
                {
                    Log.LogError("Could not asend email.");
                    returnCode.IsFailure = true;
                }
                //TODO: Just add one more fileset to NFM Plan "RXS1010_Cleanup" to here "%BATCH_ROOT%\RX_Job_Notifier\Input"
                //else
                //{
                //    Log.LogInfo("Deleting subfolder [{0}].", logsFolder);
                //    System.IO.Directory.Delete(logsFolder, true);
                //}
            }

            Log.LogInfo("Finished running JobNotifier.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
