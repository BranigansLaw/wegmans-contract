using Library.TenTenInterface.Extensions;

namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class StoreInventoryHistory : ITenTenUploadConvertible
    {
        public required DateTime? DateOfService { get; set; }
        public required int? StoreNbr { get; set; }
        public required string? NdcWithoutDashes { get; set; }
        public required string? DrugName { get; set; }
        public required string? Sdgi { get; set; }
        public required string? Gcn { get; set; }
        public required decimal? GcnSequence { get; set; }
        public required string? OrangeBookCode { get; set; }
        public required string? FormCode { get; set; }
        public required decimal? PackSize { get; set; }
        public required decimal? TruePack { get; set; }
        public required string? PM { get; set; }
        public required string? IsPreferred { get; set; }
        public required decimal? LastAcquisitionCostPack { get; set; }
        public required decimal? LastAcquisitionCostUnit { get; set; }
        public required DateTime? LastAcquisitionCostDate { get; set; }
        public required decimal? OnHandQty { get; set; }
        public required decimal? OnHandValue { get; set; }
        public required decimal? ComittedQty { get; set; }
        public required decimal? ComittedValue { get; set; }
        public required decimal? TotalQTY { get; set; }
        public required decimal? TotalValue { get; set; }
        public required decimal? AcquisitionCostPack { get; set; }
        public required decimal? AcquisitionCostUnit { get; set; }
        public required string? PrimarySupplier { get; set; }
        public required DateTime? LastChangeDate { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "StoreInventoryHistory.xml";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "store_inventory_history";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Store Inventory History";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Store Inventory History Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => ["gcn"];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => string.Empty; //On Go Live Date (but NOT before then) change this value to "wegmans.wegmansdata.dwfeeds.store_inv_history", but DO NOT set this value prior to Go Live or you will effectively delete current Production data.

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
                $"<td>{DateOfService.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{StoreNbr}</td>" +
                $"<td>{NdcWithoutDashes.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{DrugName.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{Sdgi.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{Gcn.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{GcnSequence}</td>" +
                $"<td>{OrangeBookCode.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{FormCode.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{PackSize}</td>" +
                $"<td>{TruePack}</td>" +
                $"<td>{PM.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{IsPreferred.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{LastAcquisitionCostPack}</td>" +
                $"<td>{LastAcquisitionCostUnit}</td>" +
                $"<td>{LastAcquisitionCostDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{OnHandQty}</td>" +
                $"<td>{OnHandValue}</td>" +
                $"<td>{ComittedQty}</td>" +
                $"<td>{ComittedValue}</td>" +
                $"<td>{TotalQTY}</td>" +
                $"<td>{TotalValue}</td>" +
                $"<td>{AcquisitionCostPack}</td>" +
                $"<td>{AcquisitionCostUnit}</td>" +
                $"<td>{PrimarySupplier.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{LastChangeDate.ConvertToTenTenDataType<int>()}</td>" +
                $"</tr>";
        }
    }
}
