namespace RX.PharmacyBusiness.ETL.CRX540.Core
{
    using System;

    /// <summary>
    /// Class that represents a Mail Sales records converted to UPC items for General Accounting.
    /// </summary>
    public class GeneralAccountingRetailItem
    {
        public const int DefaultLaneNumber = 79;
        public const int DefaultOperatorId = 99810;
        public const int DefaultQuantity = 1;
        public const int DefaultUnits = 1;

        /// <summary>
        /// Initializes a new instance of the GeneralAccountingRetailItem class
        /// </summary>
        public GeneralAccountingRetailItem()
        {
            this.GeneralAccountingRetailType = GeneralAccountingRetailTypes.Undefined;
            this.LaneNumber = DefaultLaneNumber;
            this.OperatorId = DefaultOperatorId;
            this.Quantity = DefaultQuantity;
            this.Units = DefaultUnits;
        }

        public DateTime TransactionDate { get; set; }
        public int StoreNumber { get; set; }
        public int LaneNumber { get; set; }
        public string OrderNumber { get; set; }
        public int OperatorId { get; set; }
        public long ItemUniversalProductNumber { get; set; }
        public int Quantity { get; set; }
        public decimal Retail { get; set; }
        public int Units { get; set; }
        public int Tax { get; set; }
        public int TenderType { get; set; }
        public string TransactionType { get; set; }
        public string TransactionSign { get; set; }
        public string TenderAccountNumber { get; set; }
        public string ShoppersClubCardNumber { get; set; }
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets Type of GeneralAccountingRetailType which is computed based on a set of business rules.
        /// </summary>
        public GeneralAccountingRetailTypes GeneralAccountingRetailType { get; set; }
    }
}
