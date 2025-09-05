namespace RX.PharmacyBusiness.ETL.CRX525.Core
{
    using System;

    public class WcbInventoryRecord
    {
        public const int InputLineArrayLength = 4;
        public const char Delimiter = (char)44; //44=Comma

        public WcbInventoryRecord()
        {
        }

        public WcbInventoryRecord(string input, string fileName)
        {
            string[] values = input.Split(Delimiter);
            if (values.Length == InputLineArrayLength)
            {
                this.RxNumber = values[2].Substring(0, 8).TrimStart(new char[] { '0' });
                this.RefillNumber = Convert.ToInt32(values[2].Substring(8, 2));
                this.PartialFillSequence = Convert.ToInt32(values[2].Substring(10, 2));
                this.Store = values[0].Substring(3);
                this.InventoryDate = DateTime.ParseExact(values[1], "MMddyy", System.Globalization.CultureInfo.InvariantCulture);
                this.InputType = values[3];
                this.FileName = fileName;
            }
            else
            {
                throw new ArgumentNullException("input");
            }
        }

        /// <summary>
        /// Gets or sets the Rx Number
        /// </summary>
        public string RxNumber { get; set; }

        /// <summary>
        /// Gets or sets the Rx refill count
        /// </summary>
        public int RefillNumber { get; set; }

        /// <summary>
        /// Gets or sets the Rx partial fill sequence
        /// </summary>
        public int PartialFillSequence { get; set; }

        /// <summary>
        /// Gets or sets the store the Rx belongs to
        /// </summary>
        public string Store { get; set; }

        /// <summary>
        /// Gets or sets the date the Rx was inventoried
        /// </summary>
        public DateTime InventoryDate { get; set; }

        /// <summary>
        /// Gets or sets the method by which the Rx was inventoried
        /// </summary>
        public string InputType { get; set; }

        /// <summary>
        /// Gets or sets the Erx Rx Record Number
        /// </summary>
        public string RxRecordNumber { get; set; }

        /// <summary>
        /// Gets or sets the Erx Rx Refill Sequence
        /// </summary>
        public string RxFillSequence { get; set; }

        /// <summary>
        /// Gets or sets the Erx Order Number
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the Erx Item Sequence
        /// </summary>
        public string ItemSequence { get; set; }

        /// <summary>
        /// Gets or sets the Erx Image Group Number
        /// </summary>
        public string ImageGroupNumber { get; set; }

        /// <summary>
        /// Gets or sets the Erx Last Sold Date
        /// </summary>
        public DateTime LastSoldDate { get; set; }

        /// <summary>
        /// Gets or sets the Erx Acquisition Cost
        /// </summary>
        public string AcquisitionCost { get; set; }

        /// <summary>
        /// Gets or sets the Erx Returned Date
        /// </summary>
        public DateTime ReturnedDate { get; set; }

        /// <summary>
        /// Gets or sets the Erx Pos Sold Date
        /// </summary>
        public DateTime PosSoldDate { get; set; }

        /// <summary>
        /// Gets or sets the Erx Scanned Signature Date
        /// </summary>
        public DateTime ScannedSignatureDate { get; set; }

        /// <summary>
        /// Gets or sets the Erx Total Price
        /// </summary>
        public string TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the Erx Insurance Payment
        /// </summary>
        public string InsurancePayment { get; set; }

        /// <summary>
        /// Gets or sets the Erx Patient Payment
        /// </summary>
        public string PatientPayment { get; set; }

        /// <summary>
        /// Gets or sets the file name coming from the vendor (WIS or RGS)
        /// </summary>
        public string FileName { get; set; }
        public string DRUG_NDC { get; set; }
        public string QTY_DISPENSED { get; set; }
    }
}
