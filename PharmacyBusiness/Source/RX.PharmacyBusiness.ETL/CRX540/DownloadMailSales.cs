namespace RX.PharmacyBusiness.ETL.CRX540
{
    using RX.PharmacyBusiness.ETL.CRX540.Business;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.Logging;

    [JobNotes("CRX540", "Download Mail Sales Feeds.", "KBA00013765", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGHAA5V0G0UJZAOIRQVMCNT2O5DE6A")]
    public class DownloadMailSales : ETLBase
    {
        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string targetRecipients = (this.Arguments["-TargetRecipients"] == null) ? "ALL" : this.Arguments["-TargetRecipients"].ToString();
            string outputFileLocation = @"%BATCH_ROOT%\CRX540\Output\";
            OracleHelper oracleHelper = new OracleHelper();
            MailSalesImporter mailSalesImporter = new MailSalesImporter(ref oracleHelper, runDate);
            MailSalesExporter mailSalesExporter = new MailSalesExporter(outputFileLocation);

            Log.LogInfo("Executing Mail Sales with RunDate [{0}], and TargetRecipients [{1}].", runDate, targetRecipients);
            mailSalesImporter.GetPrescriptionSales();
            mailSalesImporter.GetRetailItems();

            if (targetRecipients == "ALL" || targetRecipients == "EDW")
                mailSalesExporter.ExportFilesForEDW(mailSalesImporter.retailItems, runDate);

            //As of 8/11/2021 The following has been replaced by jobs CRX541 and CRX543.
            //if (targetRecipients == "ALL" || targetRecipients == "CASH")
            //    mailSalesExporter.ExportFilesForCashAccounting(mailSalesImporter.retailItems, runDate);

            if (targetRecipients == "ALL" || targetRecipients == "TENTEN")
                mailSalesExporter.ExportFilesFor1010data(mailSalesImporter.prescriptionSales, runDate);

            if (targetRecipients == "TENTEN_ERXCERT")
                mailSalesExporter.ExportFilesFor1010data(mailSalesImporter.prescriptionSales, runDate, "ERXCERT_");

            Log.LogInfo("Finished running Mail Sales.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
