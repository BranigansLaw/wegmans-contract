namespace RX.PharmacyBusiness.ETL.RXS501
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS501", "TEMPLATE - Download ERx Access System Activity Reviewing for RXBST.", "KBA00013238", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0UJZAODGI5U2MV1TQZJWX")]
    public class DownloadAllActive : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();

           

            Log.LogInfo("Downloading SystemUser with RunDate [{0}].", runDate);
            oracleHelper.DownloadQueryByRunDateToFile(
              150,
              @"%BATCH_ROOT%\RXS501\bin\SelectAllActiveSystemUser.sql",
              @"%BATCH_ROOT%\RXS501\Output\SystemAllUser_" + runDate.ToString("yyyyMMdd") + ".txt",
              runDate,
              true,
              "|",
              string.Empty,
              true,
              "ENTERPRISE_RX"
              );

            Log.LogInfo("Finished running DownloadAllActiveUser.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}