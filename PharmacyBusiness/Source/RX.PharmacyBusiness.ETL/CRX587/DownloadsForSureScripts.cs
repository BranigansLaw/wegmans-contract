namespace RX.PharmacyBusiness.ETL.CRX587
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX587", "Download SureScripts Feeds.", "KBA00029390", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGGAA5V0FYX55AOWWKOROWA3U28TQB")]
    public class DownloadsForSureScripts : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper download = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("Executing data feed 1 of 3...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX587\bin\IMMWegmansPAYYYYMMDD01.sql",
                @"%BATCH_ROOT%\CRX587\Output\IMMWegmansPA" + runDate.ToString("yyyyMMdd") + "01.txt",
                runDate,
                false,
                string.Empty,
                string.Empty,
                true,
                "ENTERPRISE_RX");
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX587\Output\IMMWegmansPA" + runDate.ToString("yyyyMMdd") + "01.txt");

            Log.LogInfo("Executing data feed 2 of 3...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX587\bin\IMMWegmansYYYYMMDD01.sql",
                @"%BATCH_ROOT%\CRX587\Output\IMMWegmans" + runDate.ToString("yyyyMMdd") + "01.txt",
                runDate,
                false,
                string.Empty,
                string.Empty,
                true,
                "ENTERPRISE_RX");
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX587\Output\IMMWegmans" + runDate.ToString("yyyyMMdd") + "01.txt");

            Log.LogInfo("Executing data feed 3 of 3...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX587\bin\PEFWegmansYYYYMMDD01.sql",
                @"%BATCH_ROOT%\CRX587\Output\PEFWegmans" + runDate.ToString("yyyyMMdd") + "01.txt",
                runDate,
                false,
                string.Empty,
                string.Empty,
                true,
                "ENTERPRISE_RX");
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX587\Output\PEFWegmans" + runDate.ToString("yyyyMMdd") + "01.txt");

            Log.LogInfo("Finished executing all SureScripts feeds.");

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
