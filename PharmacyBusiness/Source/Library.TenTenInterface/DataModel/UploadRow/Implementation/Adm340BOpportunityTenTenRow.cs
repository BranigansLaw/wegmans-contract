using Parquet.Serialization.Attributes;

namespace Library.TenTenInterface.DataModel.UploadRow.Implementation
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class Adm340BOpportunityTenTenRow : IAzureBlobUploadRow
    {
        /// <inheritdoc />
        [ParquetIgnore]
        public string FeedName => "wegmans.wegmansdata.dwfeeds.adm340b.opportunity";

        public string? ContractId { get; set; }

        public string? AccountId { get; set; }

        public string? ContractName { get; set; }

        public string? IsPool { get; set; }

        public string? WholesalerNum { get; set; }

        public string? WholesalerName { get; set; }

        public string? NdcWo { get; set; }

        public string? DrugName { get; set; }

        public string? DrugStrength { get; set; }

        public string? DrugPackSize { get; set; }

        public decimal? OrdPkg { get; set; }

        public decimal? ApprPkg { get; set; }

        public DateTime? OrderDate { get; set; }

        public int? DerivedStoreNum { get; set; }

        public string? DerivedDrugNdc { get; set; }

        public DateOnly DerivedRunDate { get; set; }

        public DateOnly DerivedProcessDate { get; set; }
    }
}
