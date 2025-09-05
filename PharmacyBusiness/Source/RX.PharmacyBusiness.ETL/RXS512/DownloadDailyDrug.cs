namespace RX.PharmacyBusiness.ETL.RXS512
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS512", "Download Daily Drug Feed.", "KBA00012982", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAODE3161DG95J9419")]
    public class DownloadDailyDrug : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("Executing Rick's local DownloadDailyDrug with RunDate [{0}].", runDate);
            
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\RXS512\bin\DailyDrug_YYYYMMDD.sql",
                @"%BATCH_ROOT%\RXS512\Output\DailyDrug_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "~",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS512\Output\DailyDrug_" + runDate.ToString("yyyyMMdd") + ".txt");

            Log.LogInfo("Finished running DownloadDailyDrug.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
