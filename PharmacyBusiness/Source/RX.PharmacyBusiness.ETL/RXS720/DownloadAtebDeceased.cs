namespace RX.PharmacyBusiness.ETL.RXS720
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS720", "TEMPLATE - Download Deceased for Ateb/Omnicell.", "KBA00013318", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAODE6IU1F5UX8KD9K")]
    public class DownloadAtebDeceased : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("Executing DownloadAtebDeceased with RunDate [{0}].", runDate);
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\RXS720\bin\Deceased_YYYYMMDD.sql",
                @"%BATCH_ROOT%\RXS720\Output\Wegmans_DeceasedPatients_" + runDate.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmm") + ".txt",
                runDate,
                false,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );
            fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS720\Output\Wegmans_DeceasedPatients_" + runDate.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmm") + ".txt");

            Log.LogInfo("Finished running DownloadAtebDeceased.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
