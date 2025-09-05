using Library.TenTenInterface.Extensions;

namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// The properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class NewTagPatientGroups : ITenTenUploadConvertible
    {
        public required DateTime? PatientAddDate { get; set; }
        public required int? StoreNum { get; set; }
        public required decimal? PatientNum { get; set; }
        public required decimal? GroupNum { get; set; }
        public required string? GroupName { get; set; }
        public required string? GroupDescription { get; set; }
        public required string? EmployeeUserName { get; set; }
        public required string? EmployeeFirstName { get; set; }
        public required string? EmployeeLastName { get; set; }
        public required string? EventDescription { get; set; }
        public required DateTime? GroupStartDate { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "NewTagPatientGroups.xml";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "new_tag_patient_groups";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "New Tag Patient Groups";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "New Tag Patient Groups Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => string.Empty; //On Go Live Date (but NOT before then) change this value to "wegmans.wegmansdata.dwfeeds.newtagpatientgroups", but DO NOT set this value prior to Go Live or you will effectively delete current Production data.

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
                $"<td>{PatientAddDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{StoreNum}</td>" +
                $"<td>{PatientNum}</td>" +
                $"<td>{GroupNum}</td>" +
                $"<td>{GroupName.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{GroupDescription.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{EmployeeUserName.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{EmployeeFirstName.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{EmployeeLastName.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{EventDescription.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{GroupStartDate.ConvertToTenTenDataType<int>()}</td>" +
                $"</tr>";
        }
    }
}
