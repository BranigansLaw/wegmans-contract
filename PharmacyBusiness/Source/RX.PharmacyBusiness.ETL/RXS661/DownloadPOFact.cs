namespace RX.PharmacyBusiness.ETL.RXS661
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS661", "DownloadRX PO Fact.", "KBA00060243", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGE6LWPFJRENASF3JMUSF3JMUQ8RG")]
    public class DownloadPOFact : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("Executing DownloadPOFact with RunDate [{0}].", runDate);

            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\RXS661\bin\GetPOFact_YYYYMMDD.sql",
                @"%BATCH_ROOT%\RXS661\Output\PO_Fact_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS661\Output\PO_Fact_" + runDate.ToString("yyyyMMdd") + ".txt");

            Log.LogInfo("Finished running DownloadPOFact.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}