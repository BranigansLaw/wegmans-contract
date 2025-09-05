namespace RX.PharmacyBusiness.ETL.CRX542
{
    using RX.PharmacyBusiness.ETL.CRX540.Business;
    using RX.PharmacyBusiness.ETL.CRX542.Business;
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX542", "Disseminate POS from its source to any consumers.", "KBA00014605", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAO6S98SATS823ERVD")]
    public class DownloadPos : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string targetRecipients = (this.Arguments["-TargetRecipients"] == null) ? "ALL" : this.Arguments["-TargetRecipients"].ToString();
            string inputLocation = @"%BATCH_ROOT%\CRX542\Input\";
            string outputLocation = @"%BATCH_ROOT%\CRX542\Output\";
            string archiveLocation = @"%BATCH_ROOT%\CRX542\Archive\";
            string rejectLocation = @"%BATCH_ROOT%\CRX542\Reject\";
            OracleHelper oracleHelper = new OracleHelper();
            PosImporter posImporter = new PosImporter(ref oracleHelper, inputLocation, outputLocation, archiveLocation, rejectLocation);
            PosExporter posExporter = new PosExporter(ref oracleHelper, inputLocation, outputLocation, archiveLocation, rejectLocation);
            PosAlerts posAlerts = new PosAlerts();

            Log.LogInfo("Executing DownloadPos with RunDate [{0:MM/dd/yyyy}] and TargetRecipients [{1}].", runDate, targetRecipients);

            Log.LogInfo($"Import brick and mortar stores.");
            posImporter.GetBrickAndMortarRecords(runDate);

            Log.LogInfo("Look for gaps in time between POS records across all stores.");
            posAlerts.GetPosGapsInReportingTimes(runDate, posImporter.brickAndMortarPosRecords);

            Log.LogInfo("Call CRX540-Home Delivery (aka Mail Sales) Get Prescription Sales collection, then transform those to POS type records.");
            MailSalesImporter mailSalesImporter = new MailSalesImporter(ref oracleHelper, runDate);
            mailSalesImporter.GetPrescriptionSales();

            Log.LogInfo("Merge brick and mortar stores with online Home Delivery Mail Sales.");
            posImporter.MergePosRecords(mailSalesImporter.prescriptionSales);

            //if (targetRecipients == "ALL" || targetRecipients == "SCRIPTS")
            //    posExporter.ExportPosToScripts(runDate, posImporter.mergedPosRecords);

            if (targetRecipients == "ALL" || targetRecipients == "MCKESSON")
                posExporter.ExportPosToMcKesson(runDate, posImporter.mergedPosRecords);

            if (targetRecipients == "ALL" || targetRecipients == "TENTEN_SFTP")
                posExporter.ExportPosToFileForSFTPto1010data(runDate, posImporter.mergedPosRecords);

            if (targetRecipients == "TENTEN_TENUP")
                posExporter.ExportPosToFileForTENUPto1010data(runDate, posImporter.mergedPosRecords);

            if (targetRecipients == "TENTEN_TENUP_ERXCERT")
                posExporter.ExportPosToFileForTENUPto1010data(runDate, posImporter.mergedPosRecords, "ERXCERT_");

            Log.LogInfo("Finished running DownloadPos.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
