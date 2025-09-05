namespace RX.PharmacyBusiness.ETL.BOT500_RX
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("BOT500", "Download Bottle Transactions to a file", "KBA00013238", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0UJZAODGI5U2MV1TQZJWX")]
    public class DownloadBottleTransactions : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            SqlServerHelper sqlServerHelper = new SqlServerHelper();

            Log.LogInfo("Executing DownloadBottleTransactions with RunDate [{0}].", runDate);

            Log.LogInfo("Begin output of merged data to a tilda-delimited text file.");
            sqlServerHelper.DownloadQueryByRunDateToFile(
                @"%BATCH_ROOT%\BOT500_RX\bin\SelectBottleTransactions.SQL",
                @"%BATCH_ROOT%\BOT500_RX\Output\bottlev2_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                false,
                "~",
                string.Empty,
                true,
                "BOTTLES");

            Log.LogInfo("Begin maintenance of SQL transaction table");
            sqlServerHelper.ExecuteDeleteFromTable(
                @"%BATCH_ROOT%\BOT500_RX\bin\DBMaintenance.SQL",
                "BOTTLES");

            Log.LogInfo("Finished running DownloadBottleTransactions.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}