using Library.LibraryUtilities.Extensions;
using Library.TenTenInterface.Extensions;

namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class Adm340BEligibleClaims : ITenTenUploadConvertible
    {
        public string? ClaimId { get; set; }
        public string? Type { get; set; }
        public DateTime? ClaimDate { get; set; }
        public DateTime? DateOfService { get; set; }
        public int? RefillNum { get; set; }
        public int? RxNum { get; set; }
        public string? DrugName { get; set; }
        public decimal? DrugPackSize { get; set; }
        public int? PackageQty { get; set; }
        public decimal? QtyPerUoi { get; set; }
        public string? DrugNdcWo { get; set; }
        public decimal? QtyDispensed { get; set; }
        public decimal? DaysSupply { get; set; }
        public string? BrandGeneric { get; set; }
        public string? CashThirdParty { get; set; }
        public string? ClaimType { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? CoPay { get; set; }
        public decimal? TpPay { get; set; }
        public decimal? HcFacilityFee { get; set; }
        public decimal? AmtDueHcFacility { get; set; }
        public string? PharmName { get; set; }
        public string? HcFacility { get; set; }
        public string? UniqueClaimId { get; set; }
        public string? ContractId { get; set; }
        public int? DerivedStoreNum { get; set; }
        public string? DerivedDrugNdc { get; set; }
        public DateOnly DerivedRunDate { get; set; }
        public DateOnly DerivedProcessDate { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "Adm340BEligibleClaims.xml";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Adm 340B EligibleClaims Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => "wegmans.wegmansdata.dwfeeds.adm340b.eligible";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "adm_340b_eligible_claims";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Adm 340B EligibleClaims";

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [
            @"<in>
  <name>default.lonely</name>
	<ops>
	  <library>
        <block name=""do_seg_and_sort"" saved_path="""">
          <do action_=""savetable"" value_=""@saved_path"" path_=""" + TenTenGoLiveRollupTableFullPathOverride + @""" replace_=""1"" use_materialize_=""1"" viewmode_=""data"" segby_=""contract_id"" owner_=""wegmans_rx_batch_inn"">
              <base table=""" + TenTenGoLiveRollupTableFullPathOverride + @"""/>
          </do>
        </block>
      </library>
      <insert block=""do_seg_and_sort"" saved_path=""" + TenTenGoLiveRollupTableFullPathOverride + @"""/>
	</ops>
</in>",
        ];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
               $"<td>{ClaimId}</td>" +
               $"<td>{Type}</td>" +
               $"<td>{ClaimDate.ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{DateOfService.ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{RefillNum}</td>" +
               $"<td>{RxNum}</td>" +
               $"<td>{DrugName}</td>" +
               $"<td>{DrugPackSize}</td>" +
               $"<td>{PackageQty}</td>" +
               $"<td>{QtyPerUoi}</td>" +
               $"<td>{DrugNdcWo}</td>" +
               $"<td>{QtyDispensed}</td>" +
               $"<td>{DaysSupply}</td>" +
               $"<td>{BrandGeneric}</td>" +
               $"<td>{CashThirdParty}</td>" +
               $"<td>{ClaimType}</td>" +
               $"<td>{SalesTax}</td>" +
               $"<td>{CoPay}</td>" +
               $"<td>{TpPay}</td>" +
               $"<td>{HcFacilityFee}</td>" +
               $"<td>{AmtDueHcFacility}</td>" +
               $"<td>{PharmName}</td>" +
               $"<td>{HcFacility}</td>" +
               $"<td>{UniqueClaimId}</td>" +
               $"<td>{ContractId}</td>" +
               $"<td>{DerivedStoreNum}</td>" +
               $"<td>{DerivedDrugNdc}</td>" +
               $"<td>{DerivedRunDate.ToDateTime().ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{DerivedProcessDate.ToDateTime().ConvertToTenTenDataType<int>()}</td>" +
               $"</tr>";
        }
    }
}
