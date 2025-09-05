namespace RX.PharmacyBusiness.ETL.CRX801
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX801", "Download Risk Management Feeds.", "KBA00026856", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAOSHY7VJGC4AVYKY0")]
    public class DownloadsForRiskManagement : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper download = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX801\bin\WorkersCompMonthly_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX801\Output\WorkersCompMonthly_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX801\Output\WorkersCompMonthly_" + runDate.ToString("yyyyMMdd") + ".txt");

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
