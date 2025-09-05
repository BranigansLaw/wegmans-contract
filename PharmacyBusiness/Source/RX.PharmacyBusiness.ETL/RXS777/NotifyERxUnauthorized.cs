namespace RX.PharmacyBusiness.ETL.RXS777
{
    using RX.PharmacyBusiness.ETL.RXS777.Core;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS777", "Download ERx Access Report", "KBA00058735", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGFVBMRACMUJARU8CL5RU8CL5QMR0")]
    public class NotifyERxUnauthorized : ETLBase
    {
        protected override void Execute(out object result)
        {
            Log.LogInfo("Begin ERx Access notification.");

            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            ReturnCode returnCode = new ReturnCode();
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(
                @"%BATCH_ROOT%\RXS777\Input\",
                @"%BATCH_ROOT%\RXS777\Output\",
                @"%BATCH_ROOT%\RXS777\Archive\",
                @"%BATCH_ROOT%\RXS777\Reject\");
            EmailHelper emailHelper = new EmailHelper(
                ConfigurationManager.AppSettings["RXS777.EmailFrom"].ToString(),
                ConfigurationManager.AppSettings["RXS777.EmailToCSV"].ToString(),
                ConfigurationManager.AppSettings["RXS777.EmailCcCSV"].ToString(),
                ConfigurationManager.AppSettings["RXS777.EmailBccCSV"].ToString());
            string emailSubject = "The Wegmans ERx After Hours Access Report for last week is now available.";
            string emailBodyTemplate = @"<!DOCTYPE html><html><head></head><body><b>Hello ERx Access Fans,</b><br />";


            Log.LogInfo("Get ERx access data from McKesson Oracle DW.");
            List<ERxAccessFromMcKessonOracleDW> eRxAccessList = oracleHelper.DownloadQueryByRunDateToList<ERxAccessFromMcKessonOracleDW>(
                150,
                @"%BATCH_ROOT%\RXS777\bin\SelectERxUnauthorized.sql",
                runDate,
                "ENTERPRISE_RX"
                );

            fileHelper.WriteListToFile<ERxAccessFromMcKessonOracleDW>(
                eRxAccessList,
                @"%BATCH_ROOT%\RXS777\Archive\OffHoursERxAccessReportForLastWeekRunOn" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                false,
                false
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS777\Archive\OffHoursERxAccessReportForLastWeekRunOn" + runDate.ToString("yyyyMMdd") + ".csv");

            List<string> attachments = new List<string>();

            if (eRxAccessList.Count == 0)
            {
                emailBodyTemplate += "<br />There are no off hours access records for last week.";
            }
            else
            {
                attachments.Add("%BATCH_ROOT%\\RXS777\\Archive\\OffHoursERxAccessReportForLastWeekRunOn" + runDate.ToString("yyyyMMdd") + ".csv");
                emailBodyTemplate += "<br />There are [" + eRxAccessList.Count.ToString() + "] off hours access records in the CSV attachment for last week.";
            }

            emailBodyTemplate += @"
<br />
<br />Thanks,
<br /><i>Wegmans Pharmacy IT Development Team</i><br />
</body>
</html>";

            emailHelper.SendEmail(
                emailSubject,
                emailBodyTemplate,
                attachments,
                string.Empty,
                eRxAccessList.Count == 0 ? System.Net.Mail.MailPriority.Low : System.Net.Mail.MailPriority.Normal,
                true);

            Log.LogInfo("Completed ERx Access notification.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
