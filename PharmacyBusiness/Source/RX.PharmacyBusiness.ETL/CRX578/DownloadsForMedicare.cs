namespace RX.PharmacyBusiness.ETL.CRX578
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX578", "Download Medicare Feeds.", "KBA00028461", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGGAA5V0FYX55AOUCQU6OTGLZXEXZA")]
    public class DownloadsForMedicare : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("Creating data feed for vendor HPOne of a list of pharmacies.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\Wegmans_HPOne_Pharmacies_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX578\Output\Wegmans_HPOne_Pharmacies_" + runDate.ToString("yyyyMMdd") + ".csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX578\Output\Wegmans_HPOne_Pharmacies_" + runDate.ToString("yyyyMMdd") + ".csv");

            Log.LogInfo("ECreating data feed for vendor HPOne of a list of prescriptions...");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\Wegmans_HPOne_Prescriptions_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX578\Output\Wegmans_HPOne_Prescriptions_" + runDate.ToString("yyyyMMdd") + ".csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX578\Output\Wegmans_HPOne_Prescriptions_" + runDate.ToString("yyyyMMdd") + ".csv");

            Log.LogInfo("Creating data feed for vendor FDS of a list of prescriptions...");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDS_Prescriptions.SQL",
                @"%BATCH_ROOT%\CRX578\Output\Wegmans_FDS_Prescriptions_" + runDate.ToString("yyyyMMdd") + ".csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX578\Output\Wegmans_FDS_Prescriptions_" + runDate.ToString("yyyyMMdd") + ".csv");

            Log.LogInfo("Creating data feed for vendor FDS of a list of pharmacies.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDS_Pharmacies.sql",
                @"%BATCH_ROOT%\CRX578\Output\Wegmans_FDS_Pharmacies_" + runDate.ToString("yyyyMMdd") + ".csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX578\Output\Wegmans_FDS_Pharmacies_" + runDate.ToString("yyyyMMdd") + ".csv");

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
