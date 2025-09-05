namespace RX.PharmacyBusiness.ETL.GMR512_RX
{
    using RX.PharmacyBusiness.ETL.GMR512_RX.Core;
    using System;
    using System.Collections.Generic;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;

    [JobNotes("GMR512", "Updload restaurant sales records to GMS", "KBA00013238", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0UJZAODGI5U2MV1TQZJWX")]
    public class InsertGMSSales : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            SqlServerHelper sqlServerHelper = new SqlServerHelper();
            FileHelper fileHelper = new FileHelper(
                @"%BATCH_ROOT%\GMR512_RX\Input\",
                @"%BATCH_ROOT%\GMR512_RX\Output\",
                @"%BATCH_ROOT%\GMR512_RX\Archive\",
                @"%BATCH_ROOT%\GMR512_RX\Reject\");
            List<GMSRecord> salesRecords = new List<GMSRecord>();


            Log.LogInfo("Executing InsertGMSSales with RunDate [{0}].", runDate);

            Log.LogInfo("Read input files from Controllers (EDW Team) containing brick and mortar sales for the run date.");
            DelimitedStreamReaderOptions GMSFileOptions = new DelimitedStreamReaderOptions(
                Constants.CharComma,
                new Nullable<char>(),
                true,
                false,
                17,
                1
            );

            salesRecords = fileHelper.ReadFilesToList<GMSRecord>(
                "*.csv",
                GMSFileOptions,
                false);

            Log.LogInfo("Begin empty staging table of SQL");
            sqlServerHelper.ExecuteDeleteFromTable(
                @"%BATCH_ROOT%\GMR512_RX\bin\DeleteFromStaging.SQL",
                "GMS");

            Log.LogInfo("Begin execution of SQL");
            bool allRecordsUpdated = sqlServerHelper.ExecuteNonQueryFromList(
                @"%BATCH_ROOT%\GMR512_RX\bin\InsertIntoGMS.SQL",
                salesRecords,
                "GMS");

            Log.LogInfo("Finished running InsertGMSSales.");
            result = allRecordsUpdated ? 0 : 1;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}