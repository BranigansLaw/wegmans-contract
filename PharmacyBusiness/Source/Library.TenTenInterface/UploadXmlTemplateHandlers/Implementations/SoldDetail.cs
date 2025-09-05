using Library.TenTenInterface.Extensions;

namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class SoldDetail : ITenTenUploadConvertible
    {
        public required int? StoreNbr { get; set; }
        public required int? RxNbr { get; set; }
        public required int? RefillNbr { get; set; }
        public required int? PartialFillSequenceNbr { get; set; }
        public required DateTime? SoldDate { get; set; }
        public required string? OrderNbr { get; set; }
        public required decimal? QtyDispensed { get; set; }
        public required string? NdcWithoutDashes { get; set; }
        public required decimal? AcquisitionCost { get; set; }
        public required decimal? ThirdPartyPay { get; set; }
        public required decimal? PatientPay { get; set; }
        public required decimal? TransactionPrice { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "SoldDetail.xml";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "sold_detail";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Sold Detail";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Sold Detail Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => string.Empty; //On Go Live Date (but NOT before then) change this value to "wegmans.wegmansdata.dwfeeds.sold_detail", but DO NOT set this value prior to Go Live or you will effectively delete current Production data.

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
                $"<td>{StoreNbr}</td>" +
                $"<td>{RxNbr}</td>" +
                $"<td>{RefillNbr}</td>" +
                $"<td>{PartialFillSequenceNbr}</td>" +
                $"<td>{SoldDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{OrderNbr.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{QtyDispensed}</td>" +
                $"<td>{NdcWithoutDashes.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{AcquisitionCost}</td>" +
                $"<td>{ThirdPartyPay}</td>" +
                $"<td>{PatientPay}</td>" +
                $"<td>{TransactionPrice}</td>" +
               $"</tr>";
        }
    }
}
