namespace Library.SnowflakeInterface.Data
{
    public class SelectSoldDetailRow
    {
        public string? StoreNumber { get; set; }
        
        public string? RxNumber { get; set; }

        public int? RefillNumber { get; set; }

        public int? PartSequenceNumber { get; set; }

        public DateTime? SoldDate { get; set; }

        public string? OrderNumber { get; set; }

        public decimal? QtyDispensed { get; set; }

        public string? NdcWo { get; set; }

        public decimal? AdqCost { get; set; }

        public decimal? TpPay { get; set; }

        public decimal? PatientPay { get; set; }

        public decimal? TxPrice { get; set; }

    }
}
