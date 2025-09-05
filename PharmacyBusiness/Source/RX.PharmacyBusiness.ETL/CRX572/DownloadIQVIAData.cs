namespace RX.PharmacyBusiness.ETL.CRX572
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX572", "Download IQVIA supplementary file for Store 198.", "KBA00024913", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGHAA5V0G0UJZAOKTYP3NCLGUHPXFZ")]
      
    public class DownloadIQVIAData : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper download = new OracleHelper();
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX572\bin\WEG_086_YYYYMMDD_01_For198.sql",
                @"%BATCH_ROOT%\CRX572\Output\WEG_086_" + runDate.ToString("yyyyMMdd") + "_TEST_02.txt",
                runDate,
                false,
                string.Empty,
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX572\Output\WEG_086_" + runDate.ToString("yyyyMMdd") + "_TEST_02.txt");

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
