namespace RX.PharmacyBusiness.ETL.CRX575
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX575", "Download Pharmacy Business Feeds.", "KBA00024816", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAOKTZCRNAN3QC5KQ1")]
    public class DownloadsForPharmacyBusiness : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper download = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("The Turnaround Time (TAT) Reports only run on Mondays, and RunDate [{0}] is a [{1}].", runDate.ToShortDateString(), runDate.DayOfWeek);
            if (runDate.DayOfWeek == DayOfWeek.Monday)
            { 
                Log.LogInfo("Executing TAT Report 1 of 9...");
                download.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\CRX575\bin\TAT_Excellus_MaxRx_Prior_Week_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\CRX575\Output\TAT_Excellus_MaxRx_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    ",",
                    "\"",
                    true,
                    "ENTERPRISE_RX");
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX575\Output\TAT_Excellus_MaxRx_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

                Log.LogInfo("Executing TAT Report 2 of 9...");
                download.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\CRX575\bin\TAT_Excellus_Prior_Week_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\CRX575\Output\TAT_Excellus_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    ",",
                    "\"",
                    true,
                    "ENTERPRISE_RX");
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX575\Output\TAT_Excellus_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

                Log.LogInfo("Executing TAT Report 3 of 9...");
                download.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\CRX575\bin\TAT_Excellus_Raw_Data_Prior_Week_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\CRX575\Output\TAT_Excellus_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    ",",
                    "\"",
                    true,
                    "ENTERPRISE_RX");
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX575\Output\TAT_Excellus_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

                Log.LogInfo("Executing TAT Report 4 of 9...");
                download.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\CRX575\bin\TAT_IHA_MaxRx_Prior_Week_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\CRX575\Output\TAT_IHA_MaxRx_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    ",",
                    "\"",
                    true,
                    "ENTERPRISE_RX");
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX575\Output\TAT_IHA_MaxRx_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

                Log.LogInfo("Executing TAT Report 5 of 9...");
                download.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\CRX575\bin\TAT_IHA_Prior_Week_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\CRX575\Output\TAT_IHA_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    ",",
                    "\"",
                    true,
                    "ENTERPRISE_RX");
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX575\Output\TAT_IHA_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

                Log.LogInfo("Executing TAT Report 6 of 9...");
                download.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\CRX575\bin\TAT_IHA_Raw_Data_Prior_Week_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\CRX575\Output\TAT_IHA_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    ",",
                    "\"",
                    true,
                    "ENTERPRISE_RX");
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX575\Output\TAT_IHA_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

                Log.LogInfo("Executing TAT Report 7 of 9...");
                download.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\CRX575\bin\TAT_Specialty_Prior_Week_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\CRX575\Output\TAT_Specialty_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    ",",
                    "\"",
                    true,
                    "ENTERPRISE_RX");
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX575\Output\TAT_Specialty_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

                Log.LogInfo("Executing TAT Report 8 of 9...");
                download.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\CRX575\bin\TAT_Specialty_Raw_Data_Prior_Week_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\CRX575\Output\TAT_Specialty_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    ",",
                    "\"",
                    true,
                    "ENTERPRISE_RX");
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX575\Output\TAT_Specialty_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

                Log.LogInfo("Executing TAT Report 9 of 9...");
                download.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\CRX575\bin\TAT_Specialty_YTD_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\CRX575\Output\TAT_Specialty_YTD_" + runDate.ToString("yyyyMMdd") + ".csv",
                    runDate,
                    true,
                    ",",
                    "\"",
                    true,
                    "ENTERPRISE_RX");
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX575\Output\TAT_Specialty_YTD_" + runDate.ToString("yyyyMMdd") + ".csv");

                Log.LogInfo("Finished executing all TAT Reports.");
            }

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
