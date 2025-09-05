namespace RX.PharmacyBusiness.ETL.RXS580
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS580", "Download IVR MD Feed(s) for Omnicel.", "KBA00013308", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAODE5LH1EXLZY9X7I")]
    public class IVRMDFeed : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("Executing IVRMDFeed with RunDate [{0}].", runDate);

            string outputFilename = @"%BATCH_ROOT%\RXS580\Output\Wegmans_Master_" + runDate.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmm") + ".txt";

            Log.LogInfo("Begin download IVRMDFeed...");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\RXS580\bin\AtebMaster_YYYYMMDD.sql",
                outputFilename,
                runDate,
                false,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(outputFilename);

            Log.LogInfo("Finished running IVRMDFeed.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}