namespace RX.PharmacyBusiness.ETL.CRX000
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX000", "TEMPLATE - Download Something Feed(s) for Somebody.", "KBA00013238", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0UJZAODGI5U2MV1TQZJWX")]
    public class DownloadSomething : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();

            Log.LogInfo("Executing DownloadSomething with RunDate [{0}].", runDate);

            //Do stuff here...

            Log.LogInfo("Finished running DownloadSomething.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}