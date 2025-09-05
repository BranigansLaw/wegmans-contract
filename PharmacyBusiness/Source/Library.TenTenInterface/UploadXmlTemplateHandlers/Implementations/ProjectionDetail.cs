using Library.TenTenInterface.Extensions;
using Library.TenTenInterface.XmlTemplateHandlers;

namespace Library.TenTenInterface.UploadXmlTemplateHandlers.Implementations
{
    /// <summary>
    /// The properties listed here are related to data types and data requirements within the 1010data data destination rather than from the data source.
    /// </summary>
    public class ProjectionDetail : ITenTenUploadConvertible
    {
        public int? StoreNumber { get; set; }
        public int? RxNumber { get; set; }
        public int? RefillNumber { get; set; }
        public int? PartialFillNumber { get; set; }
        public int? TransactionNumber { get; set; }
        public decimal? StoreGenericSales { get; set; }
        public decimal? StoreGenericCost { get; set; }
        public int? StoreGenericCount { get; set; }
        public decimal? StoreBrandSales { get; set; }
        public decimal? StoreBrandCost { get; set; }
        public int? StoreBrandCount { get; set; }
        public decimal? CfGenericSales { get; set; }
        public decimal? CfGenericCost { get; set; }
        public int? CfGenericCount { get; set; }
        public decimal? CfBrandSales { get; set; }
        public decimal? CfBrandCost { get; set; }
        public int? CfBrandCount { get; set; }
        public int? Discount { get; set; }
        public string? BillIndicator { get; set; }
        public decimal? RefundPrice { get; set; }
        public decimal? RefundYouPay { get; set; }
        public string? DataFileSource { get; set; }
        public DateTime? SoldDate { get; set; }
        public double? RxId { get; set; }


        /// <inheritdoc />
        public string BaseTemplatePath => "ProjectionDetail.xml";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Projection Detail Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => "wegmans.wegmansdata.dwfeeds.proj_dtl";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "projdtl";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Projection Detail";

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
               $"<td>{StoreNumber}</td>" +
               $"<td>{RxNumber}</td>" +
               $"<td>{RefillNumber}</td>" +
               $"<td>{PartialFillNumber}</td>" +
               $"<td>{TransactionNumber}</td>" +
               $"<td>{StoreGenericSales}</td>" +
               $"<td>{StoreGenericCost}</td>" +
               $"<td>{StoreGenericCount}</td>" +
               $"<td>{StoreBrandSales}</td>" +
               $"<td>{StoreBrandCost}</td>" +
               $"<td>{StoreBrandCount}</td>" +
               $"<td>{CfGenericSales}</td>" +
               $"<td>{CfGenericCost}</td>" +
               $"<td>{CfGenericCount}</td>" +
               $"<td>{CfBrandSales}</td>" +
               $"<td>{CfBrandCost}</td>" +
               $"<td>{CfBrandCount}</td>" +
               $"<td>{Discount}</td>" +
               $"<td>{BillIndicator}</td>" +
               $"<td>{RefundPrice}</td>" +
               $"<td>{RefundYouPay}</td>" +
               $"<td>{DataFileSource}</td>" +
               $"<td>{SoldDate.ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{RxId}</td>" +
               $"</tr>";
        }
    }
}
