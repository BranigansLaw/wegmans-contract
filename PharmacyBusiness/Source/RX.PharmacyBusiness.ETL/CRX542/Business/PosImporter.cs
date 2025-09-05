namespace RX.PharmacyBusiness.ETL.CRX542.Business
{
    using RX.PharmacyBusiness.ETL.CRX540.Business;
    using RX.PharmacyBusiness.ETL.CRX540.Core;
    using RX.PharmacyBusiness.ETL.CRX542.Core;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    public class PosImporter
    {
        private FileHelper fileHelper { get; set; }
        private OracleHelper oracleHelper { get; set; }
        public List<BrickAndMortarPosRecord> brickAndMortarPosRecords { get; set; }
        public List<PosRecord> mergedPosRecords { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        public PosImporter(ref OracleHelper oracleHelper, string inputLocation, string outputLocation, string archiveLocation, string rejectLocation)
        {
            this.fileHelper = new FileHelper(inputLocation, outputLocation, archiveLocation, rejectLocation);
            this.oracleHelper = oracleHelper; // new OracleHelper();
            this.brickAndMortarPosRecords = new List<BrickAndMortarPosRecord>();
            this.mergedPosRecords = new List<PosRecord>();
        }

        public void GetBrickAndMortarRecords(DateTime runDate)
        {
            Log.LogInfo("Read input files from Controllers (EDW Team) containing brick and mortar sales for the run date.");
            DelimitedStreamReaderOptions brickAndMortarPosFileOptions = new DelimitedStreamReaderOptions(
                Constants.CharComma,
                new Nullable<char>(),
                true,
                false,
                12,
                1
            );

            //NOTE: An example daily expected file name from EDW is [erxtra_20210307.txt]
            //      On rare occasion there might be a supplemental file of records missed in that first file of the day,
            //      and in that situation the supplemental file might be named [erxtra_20210307_012.txt] with "_012" representing store 12 pos data.
            //      So, by putting in an asterisk in the file name matching pattern, like this [erxtra_20210307*.txt]
            //      then the additional file will automatically be included without any additional code changes.
            this.brickAndMortarPosRecords = this.fileHelper.ReadFilesToList<BrickAndMortarPosRecord>(
                "erxtra_" + runDate.ToString("yyyyMMdd") + "*.txt",
                brickAndMortarPosFileOptions,
                false);
        }

        public void MergePosRecords(IEnumerable<PrescriptionSale> prescriptionSales)
        {
            this.mergedPosRecords = new List<PosRecord>();

            Log.LogInfo("Transform PrescriptionSale records from Mail Sales into PosRecord type.");
            List<PosRecord> homeDeliveryMailPosRecords = prescriptionSales
                .Where(mail =>
                    ((mail.PrescriptionSaleType == PrescriptionSaleTypes.Sold ||
                      mail.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund) &&
                     mail.PaymentTypeName != PaymentTypes.CustomerService &&
                     mail.PaymentTypeName != PaymentTypes.GetCreditCard &&
                     mail.PaymentTypeName != PaymentTypes.CreditCardPartialReversal)
                )
                .Select(mail => new PosRecord
                {
                    Store_Num = Convert.ToInt32(mail.StoreNumber),
                    Terminal_Num = 79,
                    Transaction_Date_Time = DerivePrescriptionSaleData.DeriveSoldDate(mail),
                    Operator = 99810,
                    // All OrderNumber values begin with one letter we can remove to get the number as was set up back in the day for old Scripts.
                    Transaction_Num = Convert.ToInt32(mail.OrderNumber.Substring(1, mail.OrderNumber.Length - 1)),
                    Rx_Transaction_Num = Convert.ToInt32(mail.PrescriptionNumber),
                    Refill_Num = mail.RefillNumber,
                    POS_TxType = Convert.ToChar(MailSalesImporter.DefineTransactionType(mail)),
                    Total_Price = DerivePrescriptionSaleData.DeriveTotalPricePaid(mail),
                    Insurance_Amt = DerivePrescriptionSaleData.DeriveInsurancePayment(mail),
                    Copay_Amt = DerivePrescriptionSaleData.DerivePatientPricePaid(mail),
                    Partial_Fill_Sequence_Num = mail.PartialFillSequence ?? 0,
                    Order_Num = mail.OrderNumber,
                    Mail_Flag = "Y"
                })
                .ToList();            

            Log.LogInfo("Merge [{0}] brickAndMortarPosRecords with [{1}] homeDeliveryMailPosRecords.",
                brickAndMortarPosRecords.Count,
                homeDeliveryMailPosRecords.Count);

            this.mergedPosRecords = this.brickAndMortarPosRecords
                .Select(brick => new PosRecord
                {
                    Store_Num = brick.Store_Num,
                    Terminal_Num = brick.Terminal_Num,
                    Transaction_Date_Time = brick.Transaction_Date_Time,
                    Operator = brick.Operator,
                    Transaction_Num = brick.Transaction_Num,
                    Rx_Transaction_Num = brick.Rx_Transaction_Num,
                    Refill_Num = brick.Refill_Num,
                    POS_TxType = brick.POS_TxType,
                    Total_Price = brick.Total_Price,
                    Insurance_Amt = brick.Insurance_Amt,
                    Copay_Amt = brick.Copay_Amt,
                    Partial_Fill_Sequence_Num = brick.Partial_Fill_Sequence_Num,
                    Order_Num = brick.Order_Num,
                    Mail_Flag = brick.Mail_Flag
                })
                .Concat(homeDeliveryMailPosRecords)
                .ToList();

            if (!this.mergedPosRecords.Any())
            {
                Log.LogWarn("There is no POS data available on this RunDate.");
            }
            else
            {
                Log.LogInfo("Merged brickAndMortarPosRecords with homeDeliveryMailPosRecords for a total of [{0}] POS records.", this.mergedPosRecords.Count);
            }
        }
    }
}
