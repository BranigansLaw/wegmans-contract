namespace RX.PharmacyBusiness.ETL.CRX525
{
    using RX.PharmacyBusiness.ETL.CRX525.Business;
    using RX.PharmacyBusiness.ETL.CRX525.Core;
    using System;
    using System.Collections.Generic;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;

    [JobNotes("CRX525", "Download WCB and Shelf Inventory Feeds.", "KBA00013331", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAODGJS92KALOHTBI7")]
    public class DownloadWcbInventories : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            OracleHelper oracleHelper = new OracleHelper();
            string inputLocation = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX525\Input\");
            string outputLocation = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX525\Output\");
            string archiveLocation = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX525\Archive\");
            string rejectLocation = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX525\Reject\");
            List<WcbInventoryRecord> wcbInventoryRecords = new List<WcbInventoryRecord>();
            List<ShelfInventoryRecord> shelfInventoryRecords = new List<ShelfInventoryRecord>();
            InventoryFileReader inventoryReader = new InventoryFileReader(inputLocation, archiveLocation, rejectLocation);
            InventoryRecordsExporter inventoryExporter = new InventoryRecordsExporter(outputLocation);

            Log.LogInfo("STEP 1 of 7: Archive any reject files from prior run so that what is in that folder is from this run.");
            inventoryReader.ArchiveRejectFiles();

            Log.LogInfo("STEP 2 of 7: Loop through all Will Call Bin Inventory files, potentially from multiple vendors, to merge into one collection.");
            wcbInventoryRecords.AddRange(inventoryReader.ReadWcbFiles());

            Log.LogInfo("STEP 3 of 7: Run query in McKesson ERx to lookup data points not provided by the vendor, one at a time, for [{0}] records.", wcbInventoryRecords.Count);
            try
            {
                List<WcbInventoryRecord> queryResults = new List<WcbInventoryRecord>();
                decimal loggingCounter = 0;
                oracleHelper.OpenConnectionAndSqlForRepeatedUse(
                    @"%BATCH_ROOT%\CRX525\bin\McKesson_Oracle_DW.sql",
                    "ENTERPRISE_RX");
                foreach (WcbInventoryRecord wcbRecord in wcbInventoryRecords)
                {
                    queryResults = oracleHelper.DownloadQueryToListForRepeatedUse<WcbInventoryRecord>(new
                    {
                        RxNumber = wcbRecord.RxNumber,
                        RefillNumber = wcbRecord.RefillNumber,
                        PartialFillSequence = wcbRecord.PartialFillSequence,
                        PartialFillSequence2 = wcbRecord.PartialFillSequence,
                        Store = wcbRecord.Store
                    });

                    if (queryResults.Count > 0)
                    { 
                        wcbRecord.LastSoldDate = queryResults[0].LastSoldDate;
                        wcbRecord.AcquisitionCost = queryResults[0].AcquisitionCost;
                        wcbRecord.ReturnedDate = queryResults[0].ReturnedDate;
                        wcbRecord.PosSoldDate = queryResults[0].PosSoldDate;
                        wcbRecord.ScannedSignatureDate = queryResults[0].ScannedSignatureDate;
                        wcbRecord.TotalPrice = queryResults[0].TotalPrice;
                        wcbRecord.InsurancePayment = queryResults[0].InsurancePayment;
                        wcbRecord.PatientPayment = queryResults[0].PatientPayment;
                        wcbRecord.DRUG_NDC = queryResults[0].DRUG_NDC;
                        wcbRecord.QTY_DISPENSED = queryResults[0].QTY_DISPENSED;
                    }

                    loggingCounter++;
                    if (loggingCounter % 1000 == 0)
                        Log.LogInfo("In case you are watching this run and wondering...just processed record nbr [{0}], so at [{1}]% progress...", 
                            loggingCounter, 
                            Math.Round(((loggingCounter/wcbInventoryRecords.Count) * 100),0)
                            );
                }
            }
            finally
            {
                oracleHelper.CloseConnectionAndResetSqlForRepeatedUse();
            }

            Log.LogInfo("STEP 4 of 7: Loop through all Shelf-Inventory files, potentially from multiple vendors, to merge into one collection.");
            shelfInventoryRecords.AddRange(inventoryReader.ReadShelfFiles());

            Log.LogInfo("STEP 5 of 7: Export all WCB Inventory records to one file per store.");
            inventoryExporter.ExportWcb(wcbInventoryRecords, Constants.CharComma);

            Log.LogInfo("STEP 6 of 7: Export all Shelf Inventory records to one file per store.");
            inventoryExporter.ExportShelf(shelfInventoryRecords, Constants.CharPipe);

            Log.LogInfo("STEP 7 of 7: Archive all input files.");
            inventoryReader.ArchiveInputFiles();

            result = returnCode.IsFailure || inventoryReader.DoRejectFilesExist("*.*") ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}

