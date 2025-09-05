namespace RX.PharmacyBusiness.ETL.RXS605
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS605", "Download Alternative Payments Feed.", "KBA00042018", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGGAA5V0FZ07RAQJLP1KQINOPRC0HY")]
    public class DownloadAlternativePayments : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper download = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\RXS605\bin\Alternative_Payments_YYYYMMDD.sql",
                @"%BATCH_ROOT%\RXS605\Output\Alternative_Payments_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS605\Output\Alternative_Payments_" + runDate.ToString("yyyyMMdd") + ".txt");

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
