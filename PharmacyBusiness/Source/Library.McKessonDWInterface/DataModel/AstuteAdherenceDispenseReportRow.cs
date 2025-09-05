namespace Library.McKessonDWInterface.DataModel
{
    public class AstuteAdherenceDispenseReportRow
    {
        /// <summary>
        /// Date when the patient should be called
        /// </summary>
        public DateTime? CallDate { get; set; }

        /// <summary>
        /// Next Fill Date
        /// </summary>
        public DateTime? FillDate { get; set; }

        /// <summary>
        /// Fill number for the dispense, derived field which is equal to the refill number incremented by 1
        /// </summary>
        public int? FillNumber { get; set; }

        /// <summary>
        /// Sold date/timestamp
        /// </summary>
        public DateTime? SoldDateTime { get; set; }

        /// <summary>
        /// Normalized product name to use with the CRM
        /// </summary>
        public string? CrmProductName { get; set; }

        /// <summary>
        /// Number of cases found in the CRM application that match to the dispense information
        /// </summary>
        public int? CasesMatchedInCrm { get; set; } = 0;

        /// <summary>
        /// Program header to be used in the CRM application
        /// </summary>
        public string? ProgramHeader { get; set; }

        /// <summary>
        /// The titration dose flag = Y if certain criteria met
        /// </summary>
        public bool? TitrationDoseFlag { get; set; }

        /// <summary>
        /// The next workflow status to apply for the follow-up issue
        /// </summary>
        public string? NextWorkflowStatus { get; set; }

        /// <summary>
        /// Result of the patient lookup in the CRM application
        /// </summary>
        public string? PatientLookupStatus { get; set; }

        /// <summary>
        /// Program type of the dispense drug/product
        /// </summary>
        public string? ProgramType { get; set; }

        /// <summary>
        /// Patient ID type of the drug/product
        /// </summary>
        public string? PatientIdType { get; set; }

        /// <summary>
        /// Base product name without dosage, etc.
        /// </summary>
        public string? BaseProductName { get; set; }

        /// <summary>
        /// Store number - the EnterpriseRx facility ID converted to a numeric value to remove zero padding
        /// </summary>
        public int? StoreNumber { get; set; }

        public AstuteAdherenceDispenseRawDataRow rawDataRow { get; set; }

        public AstuteAdherenceDispenseReportRow(AstuteAdherenceDispenseRawDataRow rawDataRow)
        {
            this.rawDataRow = rawDataRow;
        }
    }
}
