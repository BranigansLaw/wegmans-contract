namespace RX.PharmacyBusiness.ETL.RXS573
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    [JobNotes("RXS573", "Download Clinic Health Screening feeds.", "KBA00039207", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGAA5V0FZRJ4APWEX7EPVH3O4G6S6")]
    public class DownloadHealthScreening : ETLBase
    {
        private IFileManager fileManager { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string vendor = (this.Arguments["-Vendor"] == null) ? "ALL" : this.Arguments["-Vendor"].ToString();
            string jobNbr = string.Empty;
            switch (vendor)
            {
                case "EXCELLUS":
                    jobNbr = "RXS564";
                    break;
                case "IHA":
                    jobNbr = "RXS565";
                    break;
                case "HEALTHNOW":
                    jobNbr = "RXS566";
                    break;
                default:
                    jobNbr = "(unspecified)";
                    break;
            };
            this.fileManager = this.fileManager ?? new FileManager();
            OracleHelper oracleHelper = new OracleHelper();
            SqlServerHelper sqlServerHelper = new SqlServerHelper();
            EmailHelper emailHelper = new EmailHelper(
                ConfigurationManager.AppSettings["RXS573." + vendor + ".EmailFrom"].ToString(),
                ConfigurationManager.AppSettings["RXS573." + vendor + ".EmailToCSV"].ToString(),
                ConfigurationManager.AppSettings["RXS573." + vendor + ".EmailCcCSV"].ToString(),
                ConfigurationManager.AppSettings["RXS573." + vendor + ".EmailBccCSV"].ToString());
            int rejectRecordCount = 0;
            string emailSubject = string.Format("Clinic Health Screening records entered on {0:MMM dd, yy} for vendor {1} failed data integrity checks!", runDate.AddDays(-1), vendor);
            string emailBodyTemplate = @"<!DOCTYPE html><html><head></head><body><b>Hello Pharmacy Business Team,</b><br />
<br />
This email is to alert you that job {4} that ran on [{0:MM/dd/yyyy HH:mm:ss}] found [{1}] invalid record(s) from Clinic Health Screening data entered on [{2:MMM dd, yy}] for vendor [{3}].<br />
<br />
Please review and correct the CPS survey record(s) as needed in the reject files located in the path below, then delete those reject files as soon as you are finished with them.<br />
<a href='\\wfm.wegmans.com\departments\Pharmacy\HIPAARxOffice\ClinicHealthScreening_RejectRecords'>\\wfm.wegmans.com\departments\Pharmacy\HIPAARxOffice\ClinicHealthScreening_RejectRecords</a>
<br />
<br />
Thanks,<br />
<i>Dev Pharmacy Team</i><br />
</body></html>
";

            Log.LogInfo("Begin Clinic Health Screening data feeds with RunDate [{0:MM/dd/yyyy}] for vendor [{1}].", runDate, vendor);

            #region Excellus
            if (vendor == "ALL" || vendor == "EXCELLUS")
            {
                Log.LogInfo("Begin Excellus data feed with only GOOD data that passes our data integrity checks found in the WHERE clause of the SQL.");
                sqlServerHelper.DownloadQueryByRunDateToFile(
                    @"%BATCH_ROOT%\RXS573\bin\GetExcellusClinics_GoodDataIntegrity.sql",
                    @"%BATCH_ROOT%\RXS573\Output\Biometric_Wegmans_Excellus_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX_AZURE"
                    );

                Log.LogInfo("Begin Excellus reject file with only REJECT data that fails our data integrity checks found in the WHERE clause of the SQL.");
                string rejectFileExcellus = @"%BATCH_ROOT%\RXS573\Reject\REJECTS_Biometric_Wegmans_Excellus_" + runDate.ToString("yyyyMMdd") + ".csv";
                rejectRecordCount = sqlServerHelper.DownloadQueryByRunDateToFile(
                    @"%BATCH_ROOT%\RXS573\bin\GetExcellusClinics_REJECTS.sql",
                    rejectFileExcellus,
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX_AZURE"
                    );

                returnCode.HasLoadIssues = this.fileManager.FileExists(Environment.ExpandEnvironmentVariables(rejectFileExcellus));
            }
            #endregion

            #region HealthNow
            if (vendor == "ALL" || vendor == "HEALTHNOW")
            {
                Log.LogInfo("Begin HealthNow data feed with only GOOD data that passes our data integrity checks found in the WHERE clause of the SQL.");
                sqlServerHelper.DownloadQueryByRunDateToFile(
                    @"%BATCH_ROOT%\RXS573\bin\GetHealthNowClinics_GoodDataIntegrity.sql",
                    @"%BATCH_ROOT%\RXS573\Output\HNOW_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    false,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX_AZURE"
                    );

                Log.LogInfo("Begin HealthNow reject file with only REJECT data that fails our data integrity checks found in the WHERE clause of the SQL.");
                string rejectFileHealthNow = @"%BATCH_ROOT%\RXS573\Reject\REJECTS_HNOW_" + runDate.ToString("yyyyMMdd") + ".csv";
                rejectRecordCount = sqlServerHelper.DownloadQueryByRunDateToFile(
                    @"%BATCH_ROOT%\RXS573\bin\GetHealthNowClinics_REJECTS.sql",
                    rejectFileHealthNow,
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX_AZURE"
                    );

                returnCode.HasLoadIssues = this.fileManager.FileExists(Environment.ExpandEnvironmentVariables(rejectFileHealthNow));
            }
            #endregion

            #region IHA
            if (vendor == "ALL" || vendor == "IHA")
            {
                Log.LogInfo("Begin IHA data feed with only GOOD data that passes our data integrity checks found in the WHERE clause of the SQL.");
                sqlServerHelper.DownloadQueryByRunDateToFile(
                    @"%BATCH_ROOT%\RXS573\bin\GetIHAClinics_GoodDataIntegrity.sql",
                    @"%BATCH_ROOT%\RXS573\Output\IHA_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX_AZURE"
                    );

                Log.LogInfo("Begin IHA reject file with only REJECT data that fails our data integrity checks found in the WHERE clause of the SQL.");
                string rejectFileIHA = @"%BATCH_ROOT%\RXS573\Reject\REJECTS_IHA_" + runDate.ToString("yyyyMMdd") + ".csv";
                rejectRecordCount = sqlServerHelper.DownloadQueryByRunDateToFile(
                    @"%BATCH_ROOT%\RXS573\bin\GetIHAClinics_REJECTS.sql",
                    rejectFileIHA,
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX_AZURE"
                    );

                returnCode.HasLoadIssues = this.fileManager.FileExists(Environment.ExpandEnvironmentVariables(rejectFileIHA));
            }
            #endregion

            if (returnCode.HasLoadIssues)
            {
                //returnCode.IsFailure = true; //Uncomment this line if you want to fail the job when reject records are found.
                Log.LogWarn("Reject records exist for [{0}], so sending email to RxBST asking to correct bad data (instead of failing the job).", vendor);
                emailHelper.SendEmail(
                    emailSubject,
                    string.Format(emailBodyTemplate, DateTime.Now, rejectRecordCount, runDate.AddDays(-1), vendor, jobNbr),
                    System.Net.Mail.MailPriority.High,
                    true);
                returnCode.HasLoadIssues = true;
            }

            result = returnCode.IsFailure ? 9 : 0;
            Log.LogInfo("Completed Clinic Health Screenings with Return Code [{0}].", Convert.ToInt32(result));
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
