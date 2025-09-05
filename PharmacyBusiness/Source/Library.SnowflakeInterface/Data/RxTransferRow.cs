namespace Library.SnowflakeInterface.Data
{
    public class RxTransferRow
    {
        public required string? BaseStoreNum { get; set; }
        public string? BaseStoreName { get; set; }
        public string? ToStoreNum { get; set; }
        public string? ToStore { get; set; }
        public string? FromStoreNum { get; set; }
        public string? FromStore { get; set; }
        public string? TransferDest { get; set; }
        public long? PatientNum { get; set; }
        public string? OrigRxNum { get; set; }
        public long? RefillNum { get; set; }
        public DateTime? TransferDate { get; set; }
        public DateTime? SoldDate { get; set; }
        public DateTime? ReadyDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public long? TransferTimeKey { get; set; }
        public string? WrittenNdcWo { get; set; }
        public string? WrittenDrugName { get; set; }
        public string? DispNdcWo { get; set; }
        public string? DispDrugName { get; set; }
        public decimal? QtyDispensed { get; set; }
        public string? Daw { get; set; }
        public string? TransType { get; set; }
        public string? TransMethod { get; set; }
        public string? SigText { get; set; }
        public long? PrescriberKey { get; set; }
        public long? RxRecordNum { get; set; }
        public long? RxFillSeq { get; set; }
        public decimal? PatientPay { get; set; }
        public decimal? TpPay { get; set; }
        public decimal? TxPrice { get; set; }
        public decimal? AcqCos { get; set; }
        public decimal? UCPrice { get; set; }
        public DateTime? FirstFillDate { get; set; }
        public DateTime? LastFillDate { get; set; }
        public string? SendingRph { get; set; }
        public string? ReceiveRph { get; set; }
        public string? XferAddr { get; set; }
        public string? XferAddre { get; set; }
        public string? XferCity { get; set; }
        public string? XferSt { get; set; }
        public string? XferZip { get; set; }
        public string? XferPhone { get; set; }
        public string? TransferReason { get; set; }
        public long? NewRxRecordNum { get; set; }
        public string? CompetitorGroup { get; set; }
        public string? CompetitorStoreNum { get; set; }
    }
}
