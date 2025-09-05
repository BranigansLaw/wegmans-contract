namespace RX.PharmacyBusiness.ETL.CRX540.Core
{
    using System;

    /// <summary>
    /// Class that represents a summary report of GeneralAccountingRetailItem data.
    /// </summary>
    public class GeneralAccountingRetailItemSummary
    {
        public DateTime TransactionDate { get; set; }
        public int StoreNumber { get; set; }
        public long ItemUniversalProductNumber { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalUnits { get; set; }
        public decimal TotalRetail { get; set; }
    }
}
