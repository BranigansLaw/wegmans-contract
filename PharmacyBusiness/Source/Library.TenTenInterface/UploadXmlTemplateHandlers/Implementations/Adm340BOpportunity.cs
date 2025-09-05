using Library.LibraryUtilities.Extensions;
using Library.TenTenInterface.Extensions;

namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class Adm340BOpportunity : ITenTenUploadConvertible
    {
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

        /// <inheritdoc />
        public string BaseTemplatePath => "Adm340BOpportunity.xml";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Adm 340B Opportunity Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => "wegmans.wegmansdata.dwfeeds.adm340b.opportunity";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "adm_340b_opportunity";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Adm 340B Opportunity";

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
               $"<td>{ContractId}</td>" +
               $"<td>{AccountId}</td>" +
               $"<td>{ContractName}</td>" +
               $"<td>{IsPool}</td>" +
               $"<td>{WholesalerNum}</td>" +
               $"<td>{WholesalerName}</td>" +
               $"<td>{NdcWo}</td>" +
               $"<td>{DrugName}</td>" +
               $"<td>{DrugStrength}</td>" +
               $"<td>{DrugPackSize}</td>" +
               $"<td>{OrdPkg}</td>" +
               $"<td>{ApprPkg}</td>" +
               $"<td>{OrderDate.ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{DerivedStoreNum}</td>" +
               $"<td>{DerivedDrugNdc}</td>" +
               $"<td>{DerivedRunDate.ToDateTime().ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{DerivedProcessDate.ToDateTime().ConvertToTenTenDataType<int>()}</td>" +
               $"</tr>";
        }
    }
}
