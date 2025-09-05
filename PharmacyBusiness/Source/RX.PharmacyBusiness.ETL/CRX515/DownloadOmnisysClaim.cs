namespace RX.PharmacyBusiness.ETL.CRX515
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX515", "Download Download Omnisys Claim.", "KBA00013330", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAODGJOF29XHYHTA8K")]
    public class DownloadOmnisysClaim : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();

            Log.LogInfo("Executing DownloadOmnisysClaim with RunDate [{0}].", runDate);

            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX515\bin\Omnisys_Claim_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX515\Output\Wegmans_SOLDDATE_REQ_" + runDate.AddDays(-1).ToString("yyyyMMdd") + ".TXT",
                runDate,
                true,
                "\r",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX515\Output\Wegmans_SOLDDATE_REQ_" + runDate.AddDays(-1).ToString("yyyyMMdd") + ".TXT");

            Log.LogInfo("Finished running DownloadOmnisysClaim.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}