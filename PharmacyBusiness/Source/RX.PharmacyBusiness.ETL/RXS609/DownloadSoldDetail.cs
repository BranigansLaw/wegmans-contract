namespace RX.PharmacyBusiness.ETL.RXS609
{
    using Oracle.ManagedDataAccess.Client;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;

    [JobNotes("RXS609", "Download Sold Detail data feeds.", "KBA00049930", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGHEBGOECM51ARBIQSJRAJPPEIUBL")]
    public class DownloadSoldDetail : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("Begin [Sold_Detail] query.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\RXS609\bin\Sold_Detail_YYYYMMDD.sql",
                @"%BATCH_ROOT%\RXS609\Output\Sold_Detail_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS609\Output\Sold_Detail_" + runDate.ToString("yyyyMMdd") + ".txt");

            Log.LogInfo("Finished running DownloadSoldDetail.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
