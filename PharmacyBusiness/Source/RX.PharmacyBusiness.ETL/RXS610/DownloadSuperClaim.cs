namespace RX.PharmacyBusiness.ETL.RXS610
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS610", "Download Super Claim Feed.", "KBA00039041", "https://smartit.wegmans.com/ux/smart-it/#/knowledge/AGGAA5V0FYX55APUDA9ZPTFKY82TBK")]
    public class DownloadSuperClaim : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string targetContent = (this.Arguments["-TargetContent"] == null) ? "ALL" : this.Arguments["-TargetContent"].ToString();
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            if (targetContent == "ALL" || targetContent == "SC")
            {
                Log.LogInfo("Begin download of Super Claim for TenUp...");
                oracleHelper.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\RXS610\bin\Super_Duper_Claim_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\RXS610\Output\Super_Claim_" + runDate.ToString("yyyyMMdd") + ".txt",
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX"
                    );
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS610\Output\Super_Claim_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            if (targetContent == "debug")
            { 
                oracleHelper.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\RXS610\bin\DEBUG_Super_Duper_Claim_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\RXS610\Output\DEBUG_Super_Claim_" + runDate.ToString("yyyyMMdd") + ".txt",
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX"
                    );
            }

            if (targetContent == "ERXCERT")
            {
                Log.LogInfo("Begin download of ERXCERT Super Claim for TenUp...");
                oracleHelper.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\RXS610\bin\Super_Duper_Claim_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\RXS610\Output\ERXCERT_Super_Claim_" + runDate.ToString("yyyyMMdd") + ".txt",
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX"
                    ); 
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS610\Output\ERXCERT_Super_Claim_" + runDate.ToString("yyyyMMdd") + ".txt");

                Log.LogInfo("Begin download of ERXCERT Rx Ready for TenUp...");
                oracleHelper.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\RXS610\bin\RxReady_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\RXS610\Output\ERXCERT_RxReady_" + runDate.ToString("yyyyMMdd") + ".txt",
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX");
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS610\Output\ERXCERT_RxReady_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            Log.LogInfo("Finished downloading data feeds.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
