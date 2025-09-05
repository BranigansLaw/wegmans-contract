namespace RX.PharmacyBusiness.ETL.CRX576
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX576", "Download Feeds that get SFTP'd to 1010data.", "KBA00024817", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAOKTZGRNAQ6SY5LHK")]
    public class DownloadsForTenTenDataSFTP : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper download = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("Executing data feed 1 of 10, DurConflict_YYYYMMDD ...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX576\bin\DurConflict_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX576\Output\DurConflict_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                false,
                "ENTERPRISE_RX");
            if (fileHelper.FileExists(@"%BATCH_ROOT%\CRX576\Output\DurConflict_" + runDate.ToString("yyyyMMdd") + ".txt"))
            {
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX576\Output\DurConflict_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            Log.LogInfo("Executing data feed 2 of 10, EXTENDED_DRUG_FILE_YYYYMMDD...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX576\bin\EXTENDED_DRUG_FILE_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX576\Output\EXTENDED_DRUG_FILE_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                false,
                "ENTERPRISE_RX");
            if (fileHelper.FileExists(@"%BATCH_ROOT%\CRX576\Output\EXTENDED_DRUG_FILE_" + runDate.ToString("yyyyMMdd") + ".txt"))
            {
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX576\Output\EXTENDED_DRUG_FILE_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            Log.LogInfo("Executing data feed 3 of 10, InvAdj_YYYYMMDD...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX576\bin\InvAdj_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX576\Output\InvAdj_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                false,
                "ENTERPRISE_RX");
            if (fileHelper.FileExists(@"%BATCH_ROOT%\CRX576\Output\InvAdj_" + runDate.ToString("yyyyMMdd") + ".txt"))
            {
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX576\Output\InvAdj_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            Log.LogInfo("Executing data feed 4 of 10, PetPtNums_YYYYMMDD...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX576\bin\PetPtNums_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX576\Output\PetPtNums_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                false,
                "ENTERPRISE_RX");
            if (fileHelper.FileExists(@"%BATCH_ROOT%\CRX576\Output\PetPtNums_" + runDate.ToString("yyyyMMdd") + ".txt"))
            {
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX576\Output\PetPtNums_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            Log.LogInfo("Executing data feed 5 of 10, PRESCRIBER_YYYYMMDD...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX576\bin\PRESCRIBER_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX576\Output\PRESCRIBER_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                false,
                "ENTERPRISE_RX");
            if (fileHelper.FileExists(@"%BATCH_ROOT%\CRX576\Output\PRESCRIBER_" + runDate.ToString("yyyyMMdd") + ".txt"))
            {
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX576\Output\PRESCRIBER_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            Log.LogInfo("Executing data feed 6 of 10, PRESCRIBERADDRESS_YYYYMMDD...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX576\bin\PRESCRIBERADDRESS_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX576\Output\PRESCRIBERADDRESS_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                false,
                "ENTERPRISE_RX");
            if (fileHelper.FileExists(@"%BATCH_ROOT%\CRX576\Output\PRESCRIBERADDRESS_" + runDate.ToString("yyyyMMdd") + ".txt"))
            {
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX576\Output\PRESCRIBERADDRESS_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            Log.LogInfo("Executing data feed 7 of 10, RxTransfer_YYYYMMDD...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX576\bin\RxTransfer_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX576\Output\RxTransfer_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                false,
                "ENTERPRISE_RX");
            if (fileHelper.FileExists(@"%BATCH_ROOT%\CRX576\Output\RxTransfer_" + runDate.ToString("yyyyMMdd") + ".txt"))
            {
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX576\Output\RxTransfer_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            //Uncomment "ServiceLevel" when RXBS wants to reinstate this feed.
            //Log.LogInfo("Executing data feed 8 of 10...");
            //download.DownloadQueryByRunDateToFile(
            //    150,
            //    @"%BATCH_ROOT%\CRX576\bin\ServiceLevel_YYYYMMDD.sql",
            //    @"%BATCH_ROOT%\CRX576\Output\ServiceLevel_" + runDate.ToString("yyyyMMdd") + ".txt",
            //    runDate,
            //    true,
            //    "|",
            //    string.Empty,
            //    false,
            //    "ENTERPRISE_RX");

            Log.LogInfo("Executing data feed 8 of 10, SupplierPriceDrugFile_YYYYMMDD...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX576\bin\SupplierPriceDrugFile_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX576\Output\SupplierPriceDrugFile_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                false,
                "ENTERPRISE_RX");
            if (fileHelper.FileExists(@"%BATCH_ROOT%\CRX576\Output\SupplierPriceDrugFile_" + runDate.ToString("yyyyMMdd") + ".txt"))
            {
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX576\Output\SupplierPriceDrugFile_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            Log.LogInfo("Executing data feed 9 of 10, TagPatientGroups_YYYYMMDD...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX576\bin\TagPatientGroups_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX576\Output\TagPatientGroups_" + runDate.ToString("yyyyMMdd") + ".csv",
                runDate,
                true,
                "|",
                string.Empty,
                false,
                "ENTERPRISE_RX");
            if (fileHelper.FileExists(@"%BATCH_ROOT%\CRX576\Output\TagPatientGroups_" + runDate.ToString("yyyyMMdd") + ".csv"))
            {
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX576\Output\TagPatientGroups_" + runDate.ToString("yyyyMMdd") + ".csv");
            }

            Log.LogInfo("Executing data feed 10 of 10, Rx Ready...");
            download.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX576\bin\RX_READY_YYYYMMDD.sql",
                @"%BATCH_ROOT%\CRX576\Output\RX_READY_" + runDate.ToString("yyyyMMdd") + ".txt",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX");
            if (fileHelper.FileExists(@"%BATCH_ROOT%\CRX576\Output\RX_READY_" + runDate.ToString("yyyyMMdd") + ".txt"))
            {
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\CRX576\Output\RX_READY_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            Log.LogInfo("Finished executing all 1010data feeds for SFTP.");

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
