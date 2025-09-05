namespace Library.TenTenInterface.XmlTemplateHandlers
{
    public interface ITenTenUploadConvertible
    {
        /// <summary>
        /// The table definition required for 1010data upload.
        /// </summary>
        string BaseTemplatePath { get; }

        /// <summary>
        /// The folder name in 1010data and must be lower case and no spaces.
        /// This folder will contain potentially thousands of dated tables that get rolled up into another table in a differnt folder.
        /// </summary>
        string FolderNameOfDatedTables { get; }

        /// <summary>
        /// The folder title in 1010data and can be lower or upper case and can have spaces.
        /// This folder will contain potentially thousands of dated tables that get rolled up into another table in a differnt folder.
        /// </summary>
        string FolderTitleOfDatedTables { get; }

        /// <summary>
        /// Rollup table title in 1010data.
        /// </summary>
        string TenTenRollupTableTitle { get; }

        /// <summary>
        /// The columns that will be used to segment the rollup table in 1010data.
        /// </summary>
        string[] TenTenRollupTableSegmentColumns { get; }

        /// <summary>
        /// The columns that will be used to sort the rollup table in 1010data.
        /// </summary>
        string[] TenTenRollupTableSortColumns { get; }

        /// <summary>
        /// The table name in 1010data that will be the final rollup table. DO NOT set the value before Go Live Date. or else you will effectively delete the current Production table.
        /// </summary>
        string TenTenGoLiveRollupTableFullPathOverride { get; }

        /// <summary>
        /// When true will replace existing data within the individual dated table on upload.
        /// When false will append to existing data within the individual dated table on upload.
        /// </summary>
        bool ReplaceExistingDataOnUpload { get; }

        string ToTenTenUploadXml();

        /// <summary>
        /// Any additional postprocessing queries that should be run after the rollup table is created.
        /// </summary>
        IEnumerable<string> AdditionalPostprocessingSteps { get; }
    }
}
