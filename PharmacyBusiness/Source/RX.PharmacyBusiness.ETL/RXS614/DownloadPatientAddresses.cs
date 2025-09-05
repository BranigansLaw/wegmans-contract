namespace RX.PharmacyBusiness.ETL.RXS614
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS614", "Download Patient Addresses Feed.", "KBA00042021", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGGAA5V0FZ07RAQJLPZRQINP3YC8NX")]
    public class DownloadPatientAddresses : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper download = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\RXS614\bin\Patient_Addresses_YYYYMMDD.sql",
                @"%BATCH_ROOT%\RXS614\Output\Patient_Addresses_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS614\Output\Patient_Addresses_" + runDate.ToString("yyyyMMdd") + ".txt");

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
