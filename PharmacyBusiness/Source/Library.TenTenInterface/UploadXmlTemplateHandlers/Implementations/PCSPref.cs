namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class PCSPref : ITenTenUploadConvertible
    {
        //TODO: UNDER CONSTRUCTION
        public long? StoreNum { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "PCSPref.xml";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "pcs_pref";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "PCS Pref";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "PCS Pref Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => string.Empty; //On Go Live Date (but NOT before then) change this value to "wegmans.wegmansdata.cps.pcspref", but DO NOT set this value prior to Go Live or you will effectively delete current Production data.

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
