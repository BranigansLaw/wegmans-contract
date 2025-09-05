using Library.LibraryUtilities.Extensions;
using Library.TenTenInterface.Extensions;

namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class MedicareAutoship : ITenTenUploadConvertible
    {
        public string? PhoneNumber { get; set; }
        public DateOnly DerivedFileNameDate { get; set; }
        public DateOnly DerivedProcessDate { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "MedicareAutoship.xml";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Medicare Autoship Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => string.Empty;

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "medicare_autoship_dnc";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Medicare Autoship DNC";

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
               $"<td>{PhoneNumber}</td>" +
               $"<td>{DerivedFileNameDate.ToDateTime().ConvertToTenTenDataType<int>()}</td>" +
               $"<td>{DerivedProcessDate.ToDateTime().ConvertToTenTenDataType<int>()}</td>" +
               $"</tr>";
        }
    }
}
