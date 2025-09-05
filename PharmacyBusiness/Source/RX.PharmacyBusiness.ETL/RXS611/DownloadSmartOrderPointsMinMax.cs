namespace RX.PharmacyBusiness.ETL.RXS611
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS611", "Download Smart Order Points Min Max.", "KBA00049639", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGJ66T6GQQ8HARAHCHBQZICSCCGM4")]
    public class DownloadSmartOrderPointsMinMax : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("Begin download of Smart Order Points Min Max (which is not date driven but only runs for right now)...");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\RXS611\bin\GetSmartOrderPointsMinMax.sql",
                @"%BATCH_ROOT%\RXS611\Output\SmartOrderPoints_MinMax_" + DateTime.Now.ToString("yyyyMMdd") + ".txt",
                DateTime.Now.Date,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS611\Output\SmartOrderPoints_MinMax_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");

            Log.LogInfo("Finished downloading data feed.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
