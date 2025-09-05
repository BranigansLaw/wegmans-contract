namespace RX.PharmacyBusiness.ETL.CRX525.Business
{
    using RX.PharmacyBusiness.ETL.CRX525.Core;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    public class InventoryRecordsExporter
    {
        public const string ShelfOutputFileNameTemplate = @"{0}INV{1}{2}.txt";
        public const string WcbOutputFileNameTemplate = @"{0}WCB{1:yyyyMMdd}{2}.csv";

        private string outputFileLocation;
        private IFileManager fileManager { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        public InventoryRecordsExporter(string outputFileLocation)
        {
            this.outputFileLocation = outputFileLocation;
            this.fileManager = this.fileManager ?? new FileManager();
        }

        public void ExportShelf(IEnumerable<ShelfInventoryRecord> records, char delimiter)
        {
            StringBuilder storeStringBuilder = new StringBuilder();
            var groupedFileName = records.GroupBy(info => info.FileName);
            string firstRowStore = string.Empty;
            string firstRowDate = string.Empty;
            string header = string.Format("Store{0}Date{0}AreaBay{0}Shelf{0}Code{0}NDC{0}Description{0}Cost{0}Quantity{0}ExtendedCost", delimiter);

            foreach (var fileName in groupedFileName)
            {
                storeStringBuilder.Clear();
                storeStringBuilder.AppendLine(header);

                foreach (ShelfInventoryRecord info in fileName)
                {
                    if (string.IsNullOrEmpty(firstRowStore))
                    {
                        firstRowStore = info.Store.Substring(3, 3);
                        firstRowDate = DateReformat(info.Date);
                    }

                    storeStringBuilder.AppendFormat(
                        "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{11}",
                        delimiter,
                        info.Store,
                        DateReformat(info.Date),
                        info.AreaBay,
                        info.Shelf,
                        info.Code,
                        info.NDC,
                        info.Description,
                        info.Cost,
                        info.Quantity,
                        info.ExtendedCost,
                        Constants.NewLine);
                }

                string storeFilename = string.Format(ShelfOutputFileNameTemplate, this.outputFileLocation, firstRowDate, firstRowStore);
                this.fileManager.WriteAllText(storeFilename, storeStringBuilder.ToString());
                firstRowStore = string.Empty;
                firstRowDate = string.Empty;
            }
        }

        public void ExportWcb(IEnumerable<WcbInventoryRecord> records, char delimiter)
        {
            StringBuilder storeStringBuilder = new StringBuilder();
            var groupedFileName = records.GroupBy(info => info.FileName);
            string firstRowStore = string.Empty;
            string firstRowDate = string.Empty;
            string header = string.Format("STORENBR{0}TXNBR{0}RXNBR{0}REFILLNBR{0}PARTFILLSEQ{0}ACQUISITIONCOST{0}DATECREDIT{0}DATEPDXSOLD{0}DATEPOSSOLD{0}DATESIGSCAN{0}EXTYPE{0}EXDATE{0}EXASSIGNEDTO{0}TOTALPRICE{0}INSPAYS{0}YOUPAY{0}DRUG_NDC{0}QTY_DISPENSED", delimiter);

            foreach (var fileName in groupedFileName)
            {
                storeStringBuilder.Clear();
                storeStringBuilder.AppendLine(header);

                foreach (WcbInventoryRecord info in fileName)
                {
                    if (string.IsNullOrEmpty(firstRowStore))
                    {
                        firstRowStore = info.Store;
                        firstRowDate = info.InventoryDate.ToString("yyyyMMdd");
                    }

                    storeStringBuilder.AppendFormat(
                        "{1}{0}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{0}{7}{0}{8}{0}{0}{0}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{14}",
                        delimiter, 
                        info.Store, 
                        info.RxNumber, 
                        info.RefillNumber, 
                        info.PartialFillSequence,
                        info.AcquisitionCost, 
                        DateToString(info.ReturnedDate),
                        DateToString(info.PosSoldDate),
                        DateToString(info.ScannedSignatureDate),
                        info.TotalPrice,
                        info.InsurancePayment,
                        info.PatientPayment,
                        info.DRUG_NDC,
                        info.QTY_DISPENSED,
                        Constants.NewLine);
                }

                string storeFilename = string.Format(WcbOutputFileNameTemplate, this.outputFileLocation, firstRowDate, firstRowStore);
                this.fileManager.WriteAllText(storeFilename, storeStringBuilder.ToString());
                firstRowStore = string.Empty;
                firstRowDate = string.Empty;
            }
        }

        private static string DateToString(DateTime dateTime)
        {
            if (dateTime.Equals(DateTime.MinValue))
            {
                return string.Empty;
            }
            else
            {
                return dateTime.ToString();
            }
        }

        /// <summary>
        /// Inventory file has date in MMddyy format, and needs to be changed to yyyyMMdd.
        /// </summary>
        /// <param name="origDateString"></param>
        /// <returns></returns>
        private static string DateReformat(string origDateString)
        {
            string formattedDate = string.Empty;

            if (!string.IsNullOrEmpty(origDateString) && origDateString.Length == 6)
            {
                formattedDate = string.Format("20{0}{1}",
                    origDateString.Substring(4, 2),
                    origDateString.Substring(0, 4));
            }

            return formattedDate;
        }
    }
}
