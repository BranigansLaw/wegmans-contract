namespace RX.PharmacyBusiness.ETL.RX_Data_Transfers
{
    using RX.PharmacyBusiness.ETL.RX_Data_Transfers.Core;
    using System;
    using System.Collections.Generic;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("Job-TBD", "Download Rx data for transfer to 1010data.", "KBA-TBD", "")]
    public class DownloadRxData : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string targetContent = (this.Arguments["-TargetContent"] == null) ? "ALL" : this.Arguments["-TargetContent"].ToString();
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(
                    @"%BATCH_ROOT%\RX_Data_Transfers\Input\",
                    @"%BATCH_ROOT%\RX_Data_Transfers\Output\",
                    @"%BATCH_ROOT%\RX_Data_Transfers\Archive\",
                    @"%BATCH_ROOT%\RX_Data_Transfers\Reject\");

            if (targetContent == "ALL" || targetContent == "RESOLVED" || targetContent == "RESOLVED_IVR")
            {
                oracleHelper.DownloadQueryByRunDateToFile(
                     150,
                     @"%BATCH_ROOT%\RX_Data_Transfers\bin\UAT_Resolved_IVR.sql",
                     @"%BATCH_ROOT%\RX_Data_Transfers\Output\UAT_Resolved_IVR_" + runDate.ToString("yyyyMMdd") + ".txt",
                     runDate,
                     true,
                     "|",
                     string.Empty,
                     true,
                     "SCRIPTS_BATCH"
                     );
            }

            if (targetContent == "ALL" || targetContent == "RESOLVED" || targetContent == "RESOLVED_POS")
            {
                oracleHelper.DownloadQueryByRunDateToFile(
                     150,
                     @"%BATCH_ROOT%\RX_Data_Transfers\bin\UAT_Resolved_POS_Exceptions.sql",
                     @"%BATCH_ROOT%\RX_Data_Transfers\Output\UAT_Resolved_POS_Exceptions_" + runDate.ToString("yyyyMMdd") + ".txt",
                     runDate,
                     true,
                     "|",
                     string.Empty,
                     true,
                     "SCRIPTS_BATCH"
                     );
            }

            if (targetContent == "ALL" || targetContent == "MES")
            {
                oracleHelper.DownloadQueryByRunDateToFile(
                     150,
                     @"%BATCH_ROOT%\RX_Data_Transfers\bin\UAT_MES_Request_Queue.sql",
                     @"%BATCH_ROOT%\RX_Data_Transfers\Output\UAT_MES_Request_Queue_" + runDate.ToString("yyyyMMdd") + ".txt",
                     runDate,
                     true,
                     "|",
                     string.Empty,
                     true,
                     "SCRIPTS_BATCH"
                     );
            }

            if (targetContent == "ALL" || targetContent == "UAT_OP" || targetContent == "UAT_OP_BL")
            {
                oracleHelper.DownloadQueryByRunDateToFile(
                     150,
                     @"%BATCH_ROOT%\RX_Data_Transfers\bin\UAT_Blacklist_Order_Points.sql",
                     @"%BATCH_ROOT%\RX_Data_Transfers\Output\Blacklist_Order_Points_" + runDate.ToString("yyyyMMdd") + ".txt",
                     runDate,
                     true,
                     "|",
                     string.Empty,
                     true,
                     "SCRIPTS_BATCH"
                     );
            }

            if (targetContent == "ALL" || targetContent == "UAT_HI" || targetContent == "UAT_HI_VIEW")
            {
                oracleHelper.DownloadQueryByRunDateToFile(
                     150,
                     @"%BATCH_ROOT%\RX_Data_Transfers\bin\UAT_HipaaInc_View.sql",
                     @"%BATCH_ROOT%\RX_Data_Transfers\Output\HipaaInc_View_" + runDate.ToString("yyyyMMdd") + ".txt",
                     runDate,
                     true,
                     "|",
                     string.Empty,
                     true,
                     "SCRIPTS_BATCH"
                     );
            }

            if (targetContent == "ALL" || targetContent == "UAT_HI" || targetContent == "UAT_HI_BREACH")
            {
                oracleHelper.DownloadQueryByRunDateToFile(
                     150,
                     @"%BATCH_ROOT%\RX_Data_Transfers\bin\UAT_HipaaBreachRisk.sql",
                     @"%BATCH_ROOT%\RX_Data_Transfers\Output\HipaaBreachRisk_" + runDate.ToString("yyyyMMdd") + ".txt",
                     runDate,
                     true,
                     "|",
                     string.Empty,
                     true,
                     "SCRIPTS_BATCH"
                     );
            }

            if (targetContent == "ALL" || targetContent == "UAT_HI" || targetContent == "UAT_HI_DISCLOSURE")
            {
                oracleHelper.DownloadQueryByRunDateToFile(
                     150,
                     @"%BATCH_ROOT%\RX_Data_Transfers\bin\UAT_HipaaDisclosure.sql",
                     @"%BATCH_ROOT%\RX_Data_Transfers\Output\HipaaDisclosure_" + runDate.ToString("yyyyMMdd") + ".txt",
                     runDate,
                     true,
                     "|",
                     string.Empty,
                     true,
                     "SCRIPTS_BATCH"
                     );
            }

            if (targetContent == "ALL" || targetContent == "UAT_HI" || targetContent == "UAT_HI_INC_REP")
            {
                oracleHelper.DownloadQueryByRunDateToFile(
                     150,
                     @"%BATCH_ROOT%\RX_Data_Transfers\bin\UAT_Incident_Reporting.sql",
                     @"%BATCH_ROOT%\RX_Data_Transfers\Output\HipaaInc_Reporting_" + runDate.ToString("yyyyMMdd") + ".txt",
                     runDate,
                     true,
                     "|",
                     string.Empty,
                     true,
                     "SCRIPTS_BATCH"
                     );
            }

            if (targetContent == "ALL" || targetContent == "MEDICARE")
            {
                List<MedicareReportRequest_1010> reportRequestsForNew1010 = new List<MedicareReportRequest_1010>();

                Log.LogInfo("The Medicare Report Requests table in Scripts has embedded JSON that we can put into separate columns for 1010.");
                List<MedicareReportRequest_Scripts> reportRequestsInOldScripts = oracleHelper.DownloadQueryByRunDateToList<MedicareReportRequest_Scripts>(
                    150,
                    @"%BATCH_ROOT%\RX_Data_Transfers\bin\UAT_Medicare_Report_Requests.sql",
                    runDate,
                    "SCRIPTS_BATCH"
                    );

                //First split by comma, then split by colon.
                foreach (var reportRequestInOldScripts in reportRequestsInOldScripts)
                {
                    reportRequestsForNew1010.Add(new MedicareReportRequest_1010(reportRequestInOldScripts));
                }

                fileHelper.WriteListToFile<MedicareReportRequest_1010>(
                    reportRequestsForNew1010,
                    @"%BATCH_ROOT%\RX_Data_Transfers\Output\Medicare_Report_Requests_" + runDate.ToString("yyyyMMdd") + ".txt",
                    true,
                    "$",
                    string.Empty,
                    true);
            }

            Log.LogInfo("Finished downloading data feeds.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
