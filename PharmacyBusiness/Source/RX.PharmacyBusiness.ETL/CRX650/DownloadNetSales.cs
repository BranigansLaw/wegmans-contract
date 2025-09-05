namespace RX.PharmacyBusiness.ETL.CRX650
{
    using System;
    using System.Data.OleDb;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX650", "Download NetSales from EDW/Netezza for future upload to 1010data for Pharmacy Accounting.", "KBA-TBD", "link - tbd")]
    public class DownloadNetSales : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            NetezzaHelper netezzaHelper = new NetezzaHelper();
            OleDbParameter[] queryParams = new OleDbParameter[1];

            queryParams[0] = new OleDbParameter("@QueryDateNbr", OleDbType.Integer);

            if (runDate.ToString("MM-dd") == "12-26")
            {
                queryParams[0].Value = Convert.ToUInt32(runDate.AddDays(-2).ToString("yyyyMMdd"));
                Log.LogInfo("RunDate modified to be Xmas day to pick up Mail items sold on 12/24.");
            }
            else
            {
                queryParams[0].Value = Convert.ToUInt32(runDate.AddDays(-1).ToString("yyyyMMdd"));
            }


            Log.LogInfo("Executing DownloadNetSales with RunDate [{0}].", runDate);

            netezzaHelper.DownloadQueryWithParamsToFile(
                @"%BATCH_ROOT%\CRX650\bin\GetNetSales.sql",
                @"%BATCH_ROOT%\CRX650\Output\NetSales_" + runDate.ToString("yyyyMMdd") + ".txt",
                queryParams,
                true,
                "|",
                string.Empty,
                false,
                "EDW_RX_NETEZZA",
                false
                );

            Log.LogInfo("Finished running DownloadNetSales.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
