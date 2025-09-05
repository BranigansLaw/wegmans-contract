namespace RX.PharmacyBusiness.ETL.RXS527
{
    using RX.PharmacyBusiness.ETL.CRX540.Business;
    using RX.PharmacyBusiness.ETL.CRX540.Core;
    using RX.PharmacyBusiness.ETL.CRX542.Business;
    using RX.PharmacyBusiness.ETL.RXS527.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS527", "Download IVR Outbound Calls data feed for ateb/Omnicell.", "KBA00013220", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0UJZAODE3XN1H9Y6OP7A4")]
    public class DownloadIVROutboundCalls : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();
            SqlServerHelper sqlServerHelper = new SqlServerHelper();
            string posInputLocation = @"%BATCH_ROOT%\CRX542\Input\";
            string posOutputLocation = @"%BATCH_ROOT%\CRX542\Output\";
            string posArchiveLocation = @"%BATCH_ROOT%\CRX542\Archive\";
            string posRejectLocation = @"%BATCH_ROOT%\CRX542\Reject\";
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);

            Log.LogInfo("Executing DownloadIVROutboundCalls with RunDate [{0}].", runDate);
            Log.LogInfo("First, collect POS in memory to filter out IVR results of any prescriptions that have been sold after Ready Date by Store, Rx, Refill, and Partial Fill.");
            List<CRX542.Core.PosRecord> posRecords = new List<CRX542.Core.PosRecord>();

            for (int i = -6; i <= 0; i++)
            {
                Log.LogInfo("Pickup reminders go back six days, so subtracting [{0}] days from RunDate to get POS runDate [{1}] to add to a collection of POS in memory.", i, runDate.AddDays(i));
                PosImporter posImporter = new PosImporter(ref oracleHelper, posInputLocation, posOutputLocation, posArchiveLocation, posRejectLocation);
                posImporter.GetBrickAndMortarRecords(runDate.AddDays(i));
                MailSalesImporter mailSalesImporter = new MailSalesImporter(ref oracleHelper, runDate.AddDays(i));
                // mailSalesImporter.GetPrescriptionSales();
                posImporter.MergePosRecords(mailSalesImporter.prescriptionSales);
                posRecords.AddRange(posImporter.mergedPosRecords);
            }

            Log.LogInfo("Voided sales mean the sale record should not exist. So, remove sale records wherever they are voided.");
            //POS_TxType = "0" identifies a SALE.
            //POS_TxType = "1" identifies a VOID SALE.
            //POS_TxType = "3" identifies a REFUND.
            //POS_TxType = "4" identifies a VOID REFUND.
            List<CRX542.Core.PosRecord> posRecordsWithoutVoids = new List<CRX542.Core.PosRecord>();
            List<CRX542.Core.PosRecord> soldTypeRecords = posRecords
                .Where(r => r.POS_TxType == Convert.ToChar("0"))
                .Select(s => s)
                .ToList();
            List<CRX542.Core.PosRecord> voidTypeRecords = posRecords
                .Where(r => r.POS_TxType == Convert.ToChar("1"))
                .Select(s => s)
                .ToList();
            foreach (var soldTypeRecord in soldTypeRecords)
            {
                bool foundMatchingVoid = false;

                foreach (var voidTypeRecord in voidTypeRecords)
                {
                    if (soldTypeRecord.Store_Num == voidTypeRecord.Store_Num &&
                        soldTypeRecord.Rx_Transaction_Num == voidTypeRecord.Rx_Transaction_Num &&
                        soldTypeRecord.Refill_Num == voidTypeRecord.Refill_Num &&
                        soldTypeRecord.Partial_Fill_Sequence_Num == voidTypeRecord.Partial_Fill_Sequence_Num &&
                        soldTypeRecord.Transaction_Date_Time == voidTypeRecord.Transaction_Date_Time)
                    {
                        foundMatchingVoid = true;
                    }
                }

                if (!foundMatchingVoid)
                {
                    posRecordsWithoutVoids.Add(soldTypeRecord);
                }
            }

            Log.LogInfo("Query IVR Call data from McKesson Oracle DW.");
            List<McKessonRxRecord> mcKessonRxList = oracleHelper.DownloadQueryByRunDateToList<McKessonRxRecord>(
                150,
                @"%BATCH_ROOT%\RXS527\bin\Get_IVR_Outbound_Calls.sql",
                runDate,
                "ENTERPRISE_RX"
                );
            fileHelper.WriteListToFile<McKessonRxRecord>(
                mcKessonRxList,
                @"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\Get_IVR_Outbound_Calls_" + runDate.ToString("yyyyMMdd") + ".txt",
                true,
                "|",
                string.Empty,
                true,
                false,
                false);

            Log.LogInfo("Update IVR Calls list with POS Sold Date.");
            foreach (var rxRecord in mcKessonRxList.OrderBy(r => r.ready_dt))
            {
                foreach (var posRecordsWithoutVoid in posRecordsWithoutVoids.OrderBy(r => r.Transaction_Date_Time))
                {
                    if (rxRecord.StoreInteger == posRecordsWithoutVoid.Store_Num &&
                        rxRecord.RxInteger == posRecordsWithoutVoid.Rx_Transaction_Num &&
                        rxRecord.RefillNum == posRecordsWithoutVoid.Refill_Num &&
                        rxRecord.PartialFillSeq == posRecordsWithoutVoid.Partial_Fill_Sequence_Num)
                    {
                        rxRecord.PosSoldDate = posRecordsWithoutVoid.Transaction_Date_Time;
                    }
                }
            }

            Log.LogInfo("Create IVR Call list that excludes items POS Sold on or after READY DATE, but not excluded if POS Sold before READY DATE.");
            List<IVROutboundCallRecord> callRecords = new List<IVROutboundCallRecord>();
            //TODO: Comment out these next lines after certification efforts are complete, but leave them in case get enhancement requests and need to re-certify.
            //List<IVROutboundCallRecord> callsWithoutFluLogic = new List<IVROutboundCallRecord>();
            //List<McKessonRxRecord> callsExcludedDueToSalesSameDayAsReady = new List<McKessonRxRecord>();
            //List<McKessonRxRecord> callsIncludedDueToSalesBeforeReady = new List<McKessonRxRecord>();
            //List<McKessonRxRecord> callsExcludedDueToSalesAfterReady = new List<McKessonRxRecord>();

            foreach (var rxRecord in mcKessonRxList.OrderBy(r => r.ready_dt))
            {
                if (!rxRecord.PosSoldDate.HasValue)
                {
                    if (rxRecord.IsFluShot == "N")
                    {
                        callRecords.Add(new IVROutboundCallRecord(rxRecord));
                    }
                    //TODO: Comment out this next line after certification efforts are complete.
                    //callsWithoutFluLogic.Add(new IVROutboundCallRecord(rxRecord));
                }
                else
                {
                    if (rxRecord.ready_dt == Convert.ToInt32(rxRecord.PosSoldDate.Value.ToString("yyyyMMdd")))
                    {
                        //callsExcludedDueToSalesSameDayAsReady.Add(rxRecord);
                    }
                    else
                    {
                        if (rxRecord.ready_dt > Convert.ToInt32(rxRecord.PosSoldDate.Value.ToString("yyyyMMdd")))
                        {
                            //callsIncludedDueToSalesBeforeReady.Add(rxRecord);

                            if (rxRecord.IsFluShot == "N")
                            {
                                callRecords.Add(new IVROutboundCallRecord(rxRecord));
                            }

                            //callsWithoutFluLogic.Add(new IVROutboundCallRecord(rxRecord));
                        }
                        else
                        {
                            //POS Sales exist after Ready Date.
                            //callsExcludedDueToSalesAfterReady.Add(rxRecord);
                        }
                    }
                }
            }

            string toCallFileName = @"%BATCH_ROOT%\RXS527\Output\Wegmans_tocall_" + runDate.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".txt";
            sqlServerHelper.WriteListToFile<IVROutboundCallRecord>(
                callRecords,
                toCallFileName,
                false,
                "|",
                string.Empty,
                true
                );
            fileHelper.CopyFileToArchiveForQA(toCallFileName);

            //TODO: Comment out these next lines after certification efforts are complete.
            //sqlServerHelper.WriteListToFile<IVROutboundCallRecord>(
            //    callsWithoutFluLogic,
            //    @"%BATCH_ROOT%\RXS527\Output\Wegmans_tocall_WithoutFluLogic_" + runDate.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".txt",
            //    false,
            //    "|",
            //    string.Empty,
            //    true
            //    );
            //sqlServerHelper.WriteListToFile<McKessonRxRecord>(
            //    callsExcludedDueToSalesSameDayAsReady,
            //    @"%BATCH_ROOT%\RXS527\Output\Wegmans_tocall_ExcludedSalesSameDayAsReady_" + runDate.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".txt",
            //    false,
            //    "|",
            //    string.Empty,
            //    true
            //    );
            //sqlServerHelper.WriteListToFile<McKessonRxRecord>(
            //    callsIncludedDueToSalesBeforeReady,
            //    @"%BATCH_ROOT%\RXS527\Output\Wegmans_tocall_IncludedSalesBeforeReady_" + runDate.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".txt",
            //    false,
            //    "|",
            //    string.Empty,
            //    true
            //    );
            //sqlServerHelper.WriteListToFile<McKessonRxRecord>(
            //    callsExcludedDueToSalesAfterReady,
            //    @"%BATCH_ROOT%\RXS527\Output\Wegmans_tocall_ExcludedSalesAfterReady_" + runDate.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".txt",
            //    false,
            //    "|",
            //    string.Empty,
            //    true
            //    );

            Log.LogInfo("Finished running DownloadIVROutboundCalls.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
