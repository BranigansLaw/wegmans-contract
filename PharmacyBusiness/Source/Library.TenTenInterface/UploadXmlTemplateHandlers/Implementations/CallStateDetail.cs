namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class CallStateDetail : ITenTenUploadConvertible
    {
        //TODO: UNDER CONSTRUCTION
        public long? StoreNum { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "CallStateDetail.xml";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Call State Detail Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => string.Empty; //On Go Live Date (but NOT before then) change this value to "wegmans.wegmansdata.dwfeeds.call_state_detail", but DO NOT set this value prior to Go Live or you will effectively delete current Production data.

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "call_state_detail";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Call State Detail";

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            //TODO: UNDER CONSTRUCTION
            return $"<tr>" +
               $"<td>{StoreNum}</td>" +
               $"</tr>";
        }
    }
}
