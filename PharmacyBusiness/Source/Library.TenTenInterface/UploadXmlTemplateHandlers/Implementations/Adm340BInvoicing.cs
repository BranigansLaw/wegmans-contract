using Library.LibraryUtilities.Extensions;
using Library.TenTenInterface.Extensions;

namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class Adm340BInvoicing : ITenTenUploadConvertible
    {
        public string? ClaimId { get; set; }
        public DateTime? ClaimDate { get; set; }
        public DateTime? DateOfService { get; set; }
        public int? RefillNum { get; set; }
        public int? RxNum { get; set; }
        public string? DrugName { get; set; }
        public decimal? DrugPackSize { get; set; }
        public string? DrugNdc { get; set; }
        public decimal? QtyDispensed { get; set; }
        public string? BrandGeneric { get; set; }
        public decimal? DrugCost { get; set; }
        public decimal? CoPay { get; set; }
        public decimal? TpPay { get; set; }
        public decimal? HcFacilityFee { get; set; }
        public decimal? PercentReplenished { get; set; }
        public decimal? AmtDueHcFacility { get; set; }
        public string? Ncpdp { get; set; }
        public string? PharmName { get; set; }
        public string? HcFacility { get; set; }
        public string? ContractId { get; set; }
        public int? DerivedStoreNum { get; set; }
        public string? DerivedDrugNdcWo { get; set; }
        public DateOnly DerivedRunDate { get; set; }
        public DateOnly DerivedProcessDate { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "Adm340BInvoicing.xml";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Adm 340B Invoicing Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => "wegmans.wegmansdata.dwfeeds.adm340b.invoice";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "adm_340b_invoicing";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Adm 340B Invoicing";

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
               $"<td>{ClaimId}</td>" +
               $"<td>{ClaimDate.ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{DateOfService.ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{RefillNum}</td>" +
               $"<td>{RxNum}</td>" +
               $"<td>{DrugName}</td>" +
               $"<td>{DrugPackSize}</td>" +
               $"<td>{DrugNdc}</td>" +
               $"<td>{QtyDispensed}</td>" +
               $"<td>{BrandGeneric}</td>" +
               $"<td>{DrugCost}</td>" +
               $"<td>{CoPay}</td>" +
               $"<td>{TpPay}</td>" +
               $"<td>{HcFacilityFee}</td>" +
               $"<td>{PercentReplenished}</td>" +
               $"<td>{AmtDueHcFacility}</td>" +
               $"<td>{Ncpdp}</td>" +
               $"<td>{PharmName}</td>" +
               $"<td>{HcFacility}</td>" +
               $"<td>{ContractId}</td>" +
               $"<td>{DerivedStoreNum}</td>" +
               $"<td>{DerivedDrugNdcWo}</td>" +
               $"<td>{DerivedRunDate.ToDateTime().ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{DerivedProcessDate.ToDateTime().ConvertToTenTenDataType<int>()}</td>" +
               $"</tr>";
        }
    }
}
