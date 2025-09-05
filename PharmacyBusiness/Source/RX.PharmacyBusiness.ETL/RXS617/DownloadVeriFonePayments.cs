namespace RX.PharmacyBusiness.ETL.RXS617
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS617", "Download VeriFone Payments Feed.", "KBA00042310", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGGAA5V0FZQ1XAQ0PCULQJRAU1J9K5")]
    public class DownloadVeriFonePayments : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper download = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\RXS617\bin\VeriFone_Payments_YYYYMMDD.sql",
                @"%BATCH_ROOT%\RXS617\Output\VeriFone_Payments_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS617\Output\VeriFone_Payments_" + runDate.ToString("yyyyMMdd") + ".txt");

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
