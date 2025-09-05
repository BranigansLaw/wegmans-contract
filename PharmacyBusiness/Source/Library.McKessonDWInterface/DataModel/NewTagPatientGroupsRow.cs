namespace Library.McKessonDWInterface.DataModel
{
    /// <summary>
    /// The properties listed here related to data types and data requirements within McKesson DW data source rather than from data destination (1010data).
    /// </summary>
    public class NewTagPatientGroupsRow
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
    }
}
