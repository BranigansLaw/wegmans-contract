namespace RX.PharmacyBusiness.ETL.CRX540.Business
{
    using RX.PharmacyBusiness.ETL.CRX540.Core;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    public class MailSalesExporter
    {
        private string outputFileLocation;
        private IFileManager fileManager { get; set; }
        private FileHelper fileHelper { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        public MailSalesExporter(string outputFileLocation)
        {
            this.outputFileLocation = Environment.ExpandEnvironmentVariables(outputFileLocation);
            this.fileManager = this.fileManager ?? new FileManager();
            this.fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public void ExportFilesForEDW(IEnumerable<GeneralAccountingRetailItem> records, DateTime runDate)
        {
            //Detail file is for EDW, which in turn feeds Prelim Reports.
            string outputFile = Path.Combine(this.outputFileLocation, string.Format("MailSalesExtract{0:yyyyMMdd}.csv", runDate));
            string delimiter = ",";
            var fields = new Collection<string>();

            Log.LogInfo("Creating detail file that goes to EDW");
            using (var writerOutputFile = this.fileManager.OpenWriter(outputFile, false))
            {
                //Write header row.
                writerOutputFile.WriteLine(string.Format(
                        "TransactionDate{0}StoreNumber{0}LaneNumber{0}OrderNumber{0}OperatorId{0}ItemUniversalProductNumber{0}Quantity{0}Retail{0}Tax{0}TenderType{0}TransactionType{0}TransactionSign{0}TenderAccountNumber{0}ShoppersClubCardNumber",
                        delimiter));

                foreach (GeneralAccountingRetailItem item in
                    records
                        .OrderBy(a => a.TransactionDate)
                        .ThenBy(b => b.OrderNumber)
                        .ThenBy(c => c.ItemUniversalProductNumber)
                        .ThenBy(d => d.TransactionSign)
                        .ThenBy(e => e.TransactionType)
                        .ThenBy(f => f.Retail))
                {
                    fields.Clear();
                    fields.Add(item.TransactionDate.ToString("MM/dd/yyyy HH:mm:ss"));
                    fields.Add(item.StoreNumber.ToString());
                    fields.Add(item.LaneNumber.ToString());
                    fields.Add(item.OrderNumber.Substring(1, item.OrderNumber.Length - 1));
                    fields.Add(item.OperatorId.ToString());
                    fields.Add(item.ItemUniversalProductNumber.ToString());
                    fields.Add(item.Quantity.ToString());
                    fields.Add(Math.Abs(item.Retail).ToString("0.00"));
                    fields.Add(item.Tax.ToString());
                    fields.Add(item.TenderType.ToString());
                    fields.Add(item.TransactionType);
                    fields.Add(item.TransactionSign);
                    fields.Add(item.TenderAccountNumber);
                    fields.Add(item.ShoppersClubCardNumber);

                    writerOutputFile.WriteLine(string.Join(delimiter, fields.ToArray()));
                }
            }
            
            this.fileHelper.CopyFileToArchiveForQA(outputFile);
        }

        public void ExportFilesForCashAccounting(IEnumerable<GeneralAccountingRetailItem> records, DateTime runDate)
        {
            //UPC Summary file is for Sales and Cash Accounting.
            string outputFile = Path.Combine(this.outputFileLocation, string.Format("MailSalesExtractSummary{0:yyyyMMdd}.csv", runDate));
            string delimiter = ",";
            var fields = new Collection<string>();
            
            Log.LogInfo("Creating UPC Summary file that goes to Sales and Cash Accounting.");
            using (var writerOutputFile = this.fileManager.OpenWriter(outputFile, false))
            {
                //Tells Excel the delimiter is a comma.
                writerOutputFile.WriteLine("sep=" + delimiter);

                //Write header row.
                writerOutputFile.WriteLine(string.Format(
                       "TransactionDate{0}StoreNumber{0}ItemUniversalProductNumber{0}TotalQuantity{0}TotalUnits{0}TotalRetail",
                       delimiter));

                //Write detail rows.
                int[] storeNumbers = records
                    .GroupBy(i => new { i.StoreNumber })
                    .Select(g => g.First().StoreNumber)
                    .ToArray();
                Array.Sort(storeNumbers);

                foreach (int storeNumber in storeNumbers)
                {
                    foreach (var validItem in ItemUniversalProductNumbers.ValidItems)
                    {
                        var itemSummary = records
                            .Where(i => i.ItemUniversalProductNumber == validItem && i.StoreNumber == storeNumber)
                            .GroupBy(s => new { s.ItemUniversalProductNumber })
                            .Select(
                                g => new
                                {
                                    g.First().TransactionDate,
                                    storeNumber, //dont use g.First().StoreNumber because there might not be data for this store and upc
                                        g.First().ItemUniversalProductNumber,
                                    TotalQuantity = g.Sum(t => t.Quantity),
                                    TotalUnits = g.Sum(t => t.Units),
                                    TotalRetail = g.Sum(t => t.Retail)
                                })
                            .ToArray();

                        fields.Clear();
                        if (itemSummary.Any() == false)
                        {
                            //Output each UPC with zeroes so the business unit can see there is nothing to report on for that UPC.
                            fields.Add(runDate.AddDays(-1).ToString("yyyyMMdd"));
                            fields.Add(storeNumber.ToString());
                            fields.Add(validItem.ToString());
                            fields.Add("0");
                            fields.Add("0");
                            fields.Add("0.00");
                        }
                        else
                        {
                            fields.Add(itemSummary[0].TransactionDate.ToString("yyyyMMdd"));
                            fields.Add(storeNumber.ToString());
                            fields.Add(itemSummary[0].ItemUniversalProductNumber.ToString());
                            fields.Add(itemSummary[0].TotalQuantity.ToString());
                            fields.Add(itemSummary[0].TotalUnits.ToString());
                            fields.Add(string.Format("{0:0.00}", itemSummary[0].TotalRetail));
                        }

                        writerOutputFile.WriteLine(string.Join(delimiter, fields.ToArray()));
                    }
                }
            }

            this.fileHelper.CopyFileToArchiveForQA(outputFile);
        }

        public void ExportFilesFor1010data(IEnumerable<PrescriptionSale> records, DateTime runDate)
        {
            this.ExportFilesFor1010data(records, runDate, string.Empty);
        }

        public void ExportFilesFor1010data(IEnumerable<PrescriptionSale> records, DateTime runDate, string outputFilePrefix)
        {
            if (!records.Any() && outputFilePrefix != "ERXCERT_")
            {
                throw new Exception("There is no MailFillFact data available today, so check timing of predecessor jobs.");
            }

            string outputFile = Path.Combine(this.outputFileLocation, string.Format("{1}MailFillFact_{0:yyyyMMdd}.txt", runDate, outputFilePrefix));
            string delimiter = "|";
            var fields = new Collection<string>();

            Log.LogInfo("Creating Mail Fill Fact detail file that goes to 1010data.");
            var mailFillFactsFor1010 = records
                .Select(sale => new
                {
                    RunDate = runDate.Date,
                    UploadedDate = DateTime.Now.Date,
                    sale.CancelledDate,
                    sale.CompletionDate,
                    sale.CurrentWorkflowStep,
                    sale.LatestFillStatusDesc,
                    sale.DateOfService,
                    sale.FacilityTableFacilityNumber,
                    sale.FillFactTableFacilityKey,
                    sale.FillFactTableFillSequence,
                    sale.FillFactTableKey,
                    sale.FillFactTablePartitionDate,
                    sale.FillFactTablePatientKey,
                    sale.FillFactTablePrimaryPlanKey,
                    sale.FillFactTableRecordNumber,
                    sale.FillFactTableThirdPartyPlanKey,
                    sale.HealthCardDesignation,
                    sale.IsBilledAfterReturn,
                    sale.ItemTableOrderNumber,
                    sale.ItemTablePaymentGroupNumber,
                    sale.ItemTableProductNumber,
                    sale.LastFour,
                    sale.OrderNumber,
                    PartialFillSequence = sale.PartialFillSequence ?? 0,
                    sale.PatientPricePaid,
                    sale.PatientPricePaidForOrder,
                    sale.PaymentAmount,
                    sale.PaymentNumber,
                    sale.PaymentTableAccountNumber,
                    sale.PaymentTablePaymentTypeNumber,
                    sale.PaymentTableReversalNumber,
                    sale.PaymentTypeName,
                    PrescriptionNumber = Convert.ToInt32(sale.PrescriptionNumber),
                    sale.ProductTableDrugNumber,
                    sale.RefillNumber,
                    sale.ReversalPaymentAmount,
                    sale.ShipHandleFee,
                    sale.SoldDateKey,
                    sale.SoldDateSeconds,
                    sale.StoreNationalDrugProgramsId,
                    StoreNumber = Convert.ToInt32(sale.StoreNumber),
                    sale.StoreNationalProviderId,
                    sale.TargetCentralFillFacility,
                    sale.TotalPricePaid,
                    sale.TotalUsualAndCustomary,
                    sale.TrackingNumber,
                    sale.PrescriptionSaleType,
                    DerivedSoldDate = DerivePrescriptionSaleData.DeriveSoldDate(sale),
                    DerivedPaymentAmount = DerivePrescriptionSaleData.DerivePaymentAmount(sale),
                    DerivedTotalPricePaid = DerivePrescriptionSaleData.DeriveTotalPricePaid(sale),
                    DerivedPatientPricePaid = DerivePrescriptionSaleData.DerivePatientPricePaid(sale),
                    DerivedInsurancePayment = DerivePrescriptionSaleData.DeriveInsurancePayment(sale),
                    DerivedShipHandleFee = DerivePrescriptionSaleData.DeriveShipHandleFee(sale),
                    DerivedPaymentTypeName = DerivePrescriptionSaleData.DerivePaymentTypeName(sale),
                    sale.DeliveryMethod,
                    sale.CourierName,
                    sale.ShippingMethod,
                    CourierShipCharge = DerivePrescriptionSaleData.DeriveCourierShipCharge(sale),
                    sale.PatientNum,
                    sale.PatientAddressKey,
                    sale.CardType
                })
                .ToArray();

            using (var writerOutputFile = this.fileManager.OpenWriter(outputFile, false))
            {
                //Write header row.
                writerOutputFile.WriteLine(string.Format(
                        "RunDate{0}DerivedSoldDate{0}DerivedSoldDateTime{0}StoreNumber{0}PrescriptionNumber{0}RefillNumber{0}PartialFillSequence{0}OrderNumber{0}LatestFillStatusDesc{0}DateOfService{0}DerivedPaymentAmount{0}DerivedTotalPricePaid{0}DerivedPatientPricePaid{0}DerivedInsurancePayment{0}DerivedShipHandleFee{0}LastFour{0}CardType{0}CompletionDateTime{0}FillFactTablePartitionDateTime{0}PrescriptionSaleType{0}DerivedPaymentTypeName{0}StoreNationalProviderId{0}StoreNationalDrugProgramsId{0}PaymentNumber{0}TrackingNumber{0}HealthCardDesignation{0}IsBilledAfterRelease{0}PaymentAmount{0}TotalPricePaid{0}PatientPricePaid{0}PatientPricePaidForOrder{0}TotalUsualAndCustomary{0}ReversalPaymentAmount{0}ShipHandleFee{0}PaymentTableAccountNumber{0}PaymentTablePaymentTypeNumber{0}PaymentTableReversalNumber{0}PaymentTypeName{0}ProductTableDrugNumber{0}TargetCentralFillFacility{0}DeliveryMethod{0}CourierName{0}ShippingMethod{0}CourierShipCharge{0}SoldDateKey{0}SoldDateSeconds{0}ItemTableOrderNumber{0}ItemTablePaymentGroupNumber{0}ItemTableProductNumber{0}FillFactTableFacilityKey{0}FillFactTablePatientKey{0}FillFactTablePrimaryPlanKey{0}FillFactTableRecordNumber{0}FillFactTableThirdPartyPlanKey{0}FillFactTableKey{0}FillFactTableFillSequence{0}FacilityTableFacilityNumber{0}PatientNum{0}PatientAddressKey{0}UploadedDate",
                        delimiter));

                //Write detail rows.
                foreach (var fact in mailFillFactsFor1010
                                            .OrderBy(a => a.DerivedSoldDate)
                                            .ThenByDescending(b => b.PrescriptionSaleType))
                {
                    fields.Clear();
                    fields.Add(TenTenHelper.FormatDateWithoutTimeForTenUp(fact.RunDate));
                    fields.Add(TenTenHelper.FormatDateWithoutTimeForTenUp(fact.DerivedSoldDate));
                    fields.Add(TenTenHelper.FormatDateWithTimeForTenUp(fact.DerivedSoldDate));
                    fields.Add(fact.StoreNumber.ToString());
                    fields.Add(fact.PrescriptionNumber.ToString());
                    fields.Add(fact.RefillNumber.ToString());
                    fields.Add(fact.PartialFillSequence.ToString());
                    fields.Add(fact.OrderNumber);
                    fields.Add(fact.LatestFillStatusDesc);
                    fields.Add(fact.DateOfService.HasValue ? TenTenHelper.FormatDateWithoutTimeForTenUp(fact.DateOfService.Value) : string.Empty);
                    fields.Add(fact.DerivedPaymentAmount.ToString());
                    fields.Add(fact.DerivedTotalPricePaid.ToString());
                    fields.Add(fact.DerivedPatientPricePaid.ToString());
                    fields.Add(fact.DerivedInsurancePayment.ToString());
                    fields.Add(fact.DerivedShipHandleFee.ToString());
                    fields.Add(fact.LastFour);
                    fields.Add(fact.CardType);
                    fields.Add(fact.CompletionDate.HasValue ? TenTenHelper.FormatDateWithTimeForTenUp(fact.CompletionDate.Value) : string.Empty);
                    fields.Add(TenTenHelper.FormatDateWithTimeForTenUp(fact.FillFactTablePartitionDate));
                    fields.Add(fact.PrescriptionSaleType.ToString());
                    fields.Add(fact.DerivedPaymentTypeName);
                    fields.Add(fact.StoreNationalProviderId);
                    fields.Add(fact.StoreNationalDrugProgramsId);
                    fields.Add(fact.PaymentNumber.ToString());
                    fields.Add(fact.TrackingNumber);
                    fields.Add(fact.HealthCardDesignation);
                    fields.Add(fact.IsBilledAfterReturn);
                    fields.Add(fact.PaymentAmount.HasValue ? fact.PaymentAmount.ToString() : string.Empty);
                    fields.Add(fact.TotalPricePaid.HasValue ? fact.TotalPricePaid.ToString() : string.Empty);
                    fields.Add(fact.PatientPricePaid.HasValue ? fact.PatientPricePaid.ToString() : string.Empty);
                    fields.Add(fact.PatientPricePaidForOrder.HasValue ? fact.PatientPricePaidForOrder.ToString() : string.Empty);
                    fields.Add(fact.TotalUsualAndCustomary.HasValue ? fact.TotalUsualAndCustomary.ToString() : string.Empty);
                    fields.Add(fact.ReversalPaymentAmount.HasValue ? fact.ReversalPaymentAmount.ToString() : string.Empty);
                    fields.Add(fact.ShipHandleFee.HasValue ? fact.ShipHandleFee.ToString() : string.Empty);
                    fields.Add(fact.PaymentTableAccountNumber.ToString());
                    fields.Add(fact.PaymentTablePaymentTypeNumber.ToString());
                    fields.Add(fact.PaymentTableReversalNumber.ToString());
                    fields.Add(fact.PaymentTypeName);
                    fields.Add(fact.ProductTableDrugNumber.ToString());
                    fields.Add(fact.TargetCentralFillFacility.HasValue ? fact.TargetCentralFillFacility.ToString() : string.Empty);
                    fields.Add(fact.DeliveryMethod);
                    fields.Add(fact.CourierName);
                    fields.Add(fact.ShippingMethod);
                    fields.Add(fact.CourierShipCharge.ToString());
                    fields.Add(fact.SoldDateKey.ToString());
                    fields.Add(fact.SoldDateSeconds.ToString());
                    fields.Add(fact.ItemTableOrderNumber.ToString());
                    fields.Add(fact.ItemTablePaymentGroupNumber.ToString());
                    fields.Add(fact.ItemTableProductNumber.ToString());
                    fields.Add(fact.FillFactTableFacilityKey.ToString());
                    fields.Add(fact.FillFactTablePatientKey.ToString());
                    fields.Add(fact.FillFactTablePrimaryPlanKey.ToString());
                    fields.Add(fact.FillFactTableRecordNumber.ToString());
                    fields.Add(fact.FillFactTableThirdPartyPlanKey.ToString());
                    fields.Add(fact.FillFactTableKey.ToString());
                    fields.Add(fact.FillFactTableFillSequence.ToString());
                    fields.Add(fact.FacilityTableFacilityNumber.ToString());
                    fields.Add(fact.PatientNum.ToString());
                    fields.Add(fact.PatientAddressKey.ToString());
                    fields.Add(TenTenHelper.FormatDateWithoutTimeForTenUp(fact.UploadedDate));

                    writerOutputFile.WriteLine(string.Join(delimiter, fields.ToArray()));
                }
            }

            this.fileHelper.CopyFileToArchiveForQA(outputFile);
        }
    }
}
