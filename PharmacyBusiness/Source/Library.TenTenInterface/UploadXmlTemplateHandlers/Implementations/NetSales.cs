namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source (Netezza).
    /// </summary>
    public class NetSales : ITenTenUploadConvertible
    {
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

        /// <inheritdoc />
        public string BaseTemplatePath => "NetSales.xml";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "net_sales";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Net Sales";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Net Sales Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => string.Empty; //On Go Live Date (but NOT before then) change this value to "wegmans.wegmansdata.dwfeeds.netsales", but DO NOT set this value prior to Go Live or you will effectively delete current Production data.

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
               $"<td>{StoreNum}</td>" +
               $"<td>{DateSold}</td>" +
               $"<td>{DisplayTime}</td>" +
               $"<td>{TransactionNumber}</td>" +
               $"<td>{RegisterNumber}</td>" +
               $"<td>{NetSalesAmount}</td>" +
               $"<td>{GPAmount}</td>" +
               $"<td>{Quantity}</td>" +
               $"<td>{NetItemCount}</td>" +
               $"<td>{ItemNumber}</td>" +
               $"<td>{ItemDescription}</td>" +
               $"<td>{DepartmentNumber}</td>" +
               $"<td>{DepartmentName}</td>" +
               $"<td>{PLDepartmentName}</td>" +
               $"<td>{StoreName}</td>" +
               $"<td>{RegisterDescription}</td>" +
               $"<td>{CashierNumber}</td>" +
               $"<td>{PLDepartmentCode}</td>" +
               $"<td>{CouponDescriptionWP}</td>" +
               $"<td>{CouponDescriptionMFG}</td>" +
               $"<td>{RefundAmount}</td>" +
               $"<td>{TenderAmount}</td>" +
               $"<td>{TenderType}</td>" +
               $"<td>{TenderTypeDescription}</td>" +
               $"<td>{TxTypeCode}</td>" +
               $"<td>{TxTypeDescription}</td>" +
               $"<td>{TenderStatusDescription}</td>" +
               $"</tr>";
        }
    }
}
