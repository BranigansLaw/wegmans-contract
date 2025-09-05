namespace Library.NetezzaInterface.DataModel
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within Netezza data source rather than from data destination (1010data).
    /// </summary>
    public class NetSaleRow
    {
        public int? StoreNum { get; set; }
        public int? DateSold { get; set; }
        public string? DisplayTime { get; set; }
        public string? TxNum { get; set; }
        public string? RegNum { get; set; }
        public decimal? NetSalesAmt { get; set; }
        public decimal? GpAmt { get; set; }
        public decimal? Qty { get; set; }
        public decimal? NetCnt { get; set; }
        public string? ItemNum { get; set; }
        public string? ItemDesc { get; set; }
        public string? DeptNum { get; set; }
        public string? DeptName { get; set; }
        public string? PlName { get; set; }
        public string? StoreName { get; set; }
        public string? RegDesc { get; set; }
        public string? CashNum { get; set; }
        public string? PlCode { get; set; }
        public string? CouponDescWp { get; set; }
        public string? CouponDescMfg { get; set; }
        public decimal? RefundAmt { get; set; }
        public decimal? TenderAmt { get; set; }
        public string? TenderType { get; set; }
        public string? TenderTypeDesc { get; set; }
        public string? TxType { get; set; }
        public string? TxTypeDesc { get; set; }
        public string? TenderDesc { get; set; }
    }
}
