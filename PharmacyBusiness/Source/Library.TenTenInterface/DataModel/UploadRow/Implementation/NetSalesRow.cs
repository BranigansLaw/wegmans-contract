using Parquet.Serialization.Attributes;

namespace Library.TenTenInterface.DataModel.UploadRow.Implementation
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source (Netezza).
    /// </summary>
    public class NetSalesRow : IAzureBlobUploadRow
    {
        /// <inheritdoc />
        [ParquetIgnore]
        public string FeedName => "net_sales";

        public long? StoreNum { get; set; }

        public int? DateSold { get; set; }

        public string? DisplayTime { get; set; }

        public string? TransactionNumber { get; set; }

        public string? RegisterNumber { get; set; }

        public decimal? NetSalesAmount { get; set; }

        public decimal? GPAmount { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? NetItemCount { get; set; }

        public string? ItemNumber { get; set; }

        public string? ItemDescription { get; set; }

        public string? DepartmentNumber { get; set; }

        public string? DepartmentName { get; set; }

        public string? PLDepartmentName { get; set; }

        public string? StoreName { get; set; }

        public string? RegisterDescription { get; set; }

        public string? CashierNumber { get; set; }

        public string? PLDepartmentCode { get; set; }

        public string? CouponDescriptionWP { get; set; }

        public string? CouponDescriptionMFG { get; set; }

        public decimal? RefundAmount { get; set; }

        public decimal? TenderAmount { get; set; }

        public string? TenderType { get; set; }

        public string? TenderTypeDescription { get; set; }

        public string? TxTypeCode { get; set; }

        public string? TxTypeDescription { get; set; }

        public string? TenderStatusDescription { get; set; }
    }
}
