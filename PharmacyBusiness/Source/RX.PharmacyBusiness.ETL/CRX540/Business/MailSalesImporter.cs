namespace RX.PharmacyBusiness.ETL.CRX540.Business
{
    using Microsoft.Office.Interop.Excel;
    using RX.PharmacyBusiness.ETL.CRX540.Business.Rules;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Wegmans.PharmacyLibrary.Logging;

    public class MailSalesImporter
    {
        private DateTime runDate { get; set; }
        private int soldDateKey { get; set; }
        private OracleHelper oracleHelper { get; set; }
        public List<PrescriptionSale> prescriptionSales { get; set; }
        public List<GeneralAccountingRetailItem> retailItems { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        public MailSalesImporter(ref OracleHelper oracleHelper, DateTime runDate)
        {
            if (runDate.ToString("MM-dd") == "12-26")
            {
                this.runDate = runDate.AddDays(-1);
                this.soldDateKey = Convert.ToInt32(runDate.AddDays(-2).ToString("yyyyMMdd"));
                Log.LogInfo("RunDate modified to be Xmas day to pick up Mail items sold on 12/24.");
            }
            else
            {
                this.runDate = runDate;
                this.soldDateKey = Convert.ToInt32(runDate.AddDays(-1).ToString("yyyyMMdd"));
            }

            this.oracleHelper = oracleHelper; // new OracleHelper();
            this.prescriptionSales = new List<PrescriptionSale>();
            this.retailItems = new List<GeneralAccountingRetailItem>();
        }

        public void GetPrescriptionSales()
        {
            this.prescriptionSales = new List<PrescriptionSale>();

            Log.LogInfo("Retrieving raw data from ERx that could be sold records, then applying business rules to filter data and derive new data elements.");
            /* use literals instead of bind variables due to Oracle performance issues.
             
            List<PrescriptionSale> mightBeSold = this.oracleHelper.DownloadQueryByRunDateToList<PrescriptionSale>(
                150,
                @"%BATCH_ROOT%\CRX540\bin\MightBeSoldTransactions.sql",
                runDate,
                "ENTERPRISE_RX"
                );
            */
            Dictionary<string, string> queryLiterals = new Dictionary<string, string>
            {
                { ":RunDate", $"To_Date('{runDate.ToString("yyyyMMdd")}','YYYYMMDD')" }
            };
            List<PrescriptionSale> mightBeSold = this.oracleHelper.DownloadQueryWithLiteralsToList<PrescriptionSale>(
                150,
                @"%BATCH_ROOT%\CRX540\bin\MightBeSoldTransactions.sql",
                queryLiterals,
                "ENTERPRISE_RX"
                );
            var soldTransactions = mightBeSold
                .Select(this.AssignSaleType)
                .Where(s => s.PrescriptionSaleType == PrescriptionSaleTypes.Sold)
                .ToArray();
            this.prescriptionSales.AddRange(soldTransactions);

            Log.LogInfo("Retrieving raw data from ERx that could be refund records, then applying business rules to filter data and derive new data elements.");
            List<PrescriptionSale> mightBeRefund = this.oracleHelper.DownloadQueryToList<PrescriptionSale>(
                150,
                @"%BATCH_ROOT%\CRX540\bin\MightBeRefundTransactions.sql",
                new { QueryDateStr = runDate.AddDays(-1).ToString("yyyyMMdd"), QueryDateDt = runDate.AddDays(-1) },
                "ENTERPRISE_RX"
                );
            var refundTransactions = mightBeRefund
                .Select(this.AssignRefundType)
                .Where(s =>
                    s.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardPartialRefund ||
                    s.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
                .ToArray();
            this.prescriptionSales.AddRange(refundTransactions);

            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);
            fileHelper.WriteListToFile<PrescriptionSale>(
                mightBeSold,
                @"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\MightBeSoldTransactions_" + runDate.ToString("yyyyMMdd") + ".txt",
                true,
                "|",
                string.Empty,
                true,
                false,
                false);
            fileHelper.WriteListToFile<PrescriptionSale>(
                mightBeRefund,
                @"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\MightBeRefundTransactions_" + runDate.ToString("yyyyMMdd") + ".txt",
                true,
                "|",
                string.Empty,
                true,
                false,
                false);
            fileHelper.WriteListToFile<PrescriptionSale>(
                this.prescriptionSales,
                @"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\PrescriptionSales_" + runDate.ToString("yyyyMMdd") + ".txt",
                true,
                "|",
                string.Empty,
                true,
                false,
                false);
        }

        public void GetRetailItems()
        {
            Log.LogInfo("Applying business rules to PrescriptionSales data and deriving new data elements like UPC.");
            this.retailItems = new List<GeneralAccountingRetailItem>();
            var factory = new RetailItemFactory();

            foreach (PrescriptionSale ps in this.prescriptionSales)
            {
                this.retailItems.AddRange(factory.Create(ps, this.runDate.AddDays(-1)));
            }

            /// We only want one shipping charge per order.
            /// Group all shipping charges by Order Number and just get the first one.
            var shippingChargeItemsForEDW = this.retailItems
                .Where(item => item.GeneralAccountingRetailType == GeneralAccountingRetailTypes.ShippingCharge)
                .GroupBy(s => new
                {
                    s.GeneralAccountingRetailType,
                    s.OrderNumber,
                    s.StoreNumber,
                    s.TrackingNumber,
                    s.Retail,
                    s.TransactionType,
                    s.TransactionSign
                })
                .Select(g1 => new GeneralAccountingRetailItem
                {
                    GeneralAccountingRetailType = g1.First().GeneralAccountingRetailType,
                    TransactionDate = g1.Max(g => g.TransactionDate),
                    StoreNumber = g1.First().StoreNumber,
                    LaneNumber = g1.First().LaneNumber,
                    OrderNumber = g1.First().OrderNumber,
                    OperatorId = g1.First().OperatorId,
                    ItemUniversalProductNumber = g1.First().ItemUniversalProductNumber,
                    Quantity = g1.First().Quantity,
                    Retail = g1.First().Retail,
                    Units = g1.First().Units,
                    Tax = g1.First().Tax,
                    TenderType = g1.First().TenderType,
                    TransactionType = g1.First().TransactionType,
                    TransactionSign = g1.First().TransactionSign,
                    TenderAccountNumber = g1.First().TenderAccountNumber,
                    ShoppersClubCardNumber = g1.First().ShoppersClubCardNumber,
                    TrackingNumber = g1.First().TrackingNumber
                })
                .Where(g2 => g2.Retail != 0)
                .ToList();

            this.retailItems.RemoveAll(item => item.GeneralAccountingRetailType == GeneralAccountingRetailTypes.ShippingCharge);
            this.retailItems.AddRange(shippingChargeItemsForEDW);
        }

        public PrescriptionSale AssignSaleType(PrescriptionSale sale)
        {
            IRule<PrescriptionSale> soldRule = new IsSoldRule(this.soldDateKey);
            sale.PrescriptionSaleType = soldRule.IsMetBy(sale)
                                            ? PrescriptionSaleTypes.Sold
                                            : PrescriptionSaleTypes.Undefined;
            return sale;
        }

        public PrescriptionSale AssignRefundType(PrescriptionSale sale)
        {
            IRule<PrescriptionSale> fullRefundRule = new IsFullRefundRule(this.soldDateKey, this.runDate.AddDays(-1));
            IRule<PrescriptionSale> partialRefundRule = new IsPartialRefundRule(this.soldDateKey, this.runDate.AddDays(-1));
            sale.PrescriptionSaleType = fullRefundRule.IsMetBy(sale)
                                            ? PrescriptionSaleTypes.CreditCardFullRefund
                                            : partialRefundRule.IsMetBy(sale)
                                                  ? PrescriptionSaleTypes.CreditCardPartialRefund
                                                  : PrescriptionSaleTypes.Undefined;

            return sale;
        }

        public static string DefineTransactionType(PrescriptionSale sale)
        {
            string transactionTypeRefund = "3";
            string transactionTypeSale = "0";

            if (sale.PrescriptionSaleType == PrescriptionSaleTypes.Sold)
                return transactionTypeSale;

            if (sale.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
                return transactionTypeRefund;

            return (DerivePrescriptionSaleData.DeriveTotalPricePaid(sale) >= 0)
                       ? transactionTypeSale
                       : transactionTypeRefund;
        }
    }
}
