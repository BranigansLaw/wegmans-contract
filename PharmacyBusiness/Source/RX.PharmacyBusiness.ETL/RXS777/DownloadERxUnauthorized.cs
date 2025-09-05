namespace RX.PharmacyBusiness.ETL.RXS777
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS777", "TEMPLATE - Download ERx Unauthorized Access System Activity Reviewing for RXBST.", "KBA00013238", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0UJZAODGI5U2MV1TQZJWX")]
    public class DownloadERxUnauthorized : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();

            Log.LogInfo("Executing DownloadERxUnauthorized with RunDate [{0}].", runDate);

            oracleHelper.DownloadQueryByRunDateToFile(
              150,
              @"%BATCH_ROOT%\RXS777\bin\SelectERxUnauthorized.sql",
              @"%BATCH_ROOT%\RXS777\Output\ERxUnauthorized_" + runDate.ToString("yyyyMMdd") +  ".txt",
              runDate,
              true,
              "|",
              string.Empty,
              true,
              "ENTERPRISE_RX"
              );

            Log.LogInfo("Finished running DownloadERxUnauthorized.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}