using Library.LibraryUtilities.Extensions;
using Library.TenTenInterface.Extensions;

namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class Adm340BPurchases : ITenTenUploadConvertible
    {
        public string? ContractId { get; set; }
        public string? ContractName { get; set; }
        public string? Ncpdp { get; set; }
        public DateTime? DatePurchased { get; set; }
        public string? InvNum { get; set; }
        public string? NdcWo { get; set; }
        public string? DrugPackSize { get; set; }
        public string? DrugName { get; set; }
        public decimal? QtyPurchased { get; set; }
        public decimal? CostPkg { get; set; }
        public decimal? ExtCost { get; set; }
        public int? DerivedStoreNum { get; set; }
        public string? DerivedDrugNdc { get; set; }
        public DateOnly DerivedRunDate { get; set; }
        public DateOnly DerivedProcessDate { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "Adm340BPurchases.xml";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Adm 340B Purchases Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => "wegmans.wegmansdata.dwfeeds.adm340b.purchase";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "adm_340b_purchases";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Adm 340B Purchases";

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
               $"<td>{ContractId}</td>" +
               $"<td>{ContractName}</td>" +
               $"<td>{Ncpdp}</td>" +
               $"<td>{DatePurchased.ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{InvNum}</td>" +
               $"<td>{NdcWo}</td>" +
               $"<td>{DrugPackSize}</td>" +
               $"<td>{DrugName}</td>" +
               $"<td>{QtyPurchased}</td>" +
               $"<td>{CostPkg}</td>" +
               $"<td>{ExtCost}</td>" +
               $"<td>{DerivedStoreNum}</td>" +
               $"<td>{DerivedDrugNdc}</td>" +
               $"<td>{DerivedRunDate.ToDateTime().ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{DerivedProcessDate.ToDateTime().ConvertToTenTenDataType<int>()}</td>" +
               $"</tr>";
        }
    }
}
