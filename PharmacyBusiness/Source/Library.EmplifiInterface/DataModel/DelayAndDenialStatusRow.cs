namespace Library.EmplifiInterface.DataModel
{
    public class DelayAndDenialStatusRow
    {
        [ExportHeaderColumnLabel("PHARM_CODE")]
        public required string PharmCode { get; set; }

        [ExportHeaderColumnLabel("PHARM_NPI")]
        public required int PharmNpi { get; set; }

        [ExportHeaderColumnLabel("SP_TRANSACTION_ID")]
        public required string SpTransactionId { get; set; }

        [ExportHeaderColumnLabel("PROGRAM_ID")]
        public required string ProgramId { get; set; }

        [ExportHeaderColumnLabel("PATIENT_ID")]
        public required string PatientId { get; set; }

        [ExportHeaderColumnLabel("PAT_LAST_NAME")]
        public required string PatientLastName { get; set; }

        [ExportHeaderColumnLabel("PAT_FIRST_NAME")]
        public required string PatientFirstName { get; set; }

        [ExportHeaderColumnLabel("PAT_DOB")]
        public required int PatientDob { get; set; } //YYYYMMDD

        [ExportHeaderColumnLabel("BRAND")]
        public required string Brand { get; set; }

        [ExportHeaderColumnLabel("NDC_NUMBER")]
        public long? NdcNumber { get; set; }

        [ExportHeaderColumnLabel("SHIP_DATE")]
        public int? ShipDate { get; set; } //YYYYMMDD

        [ExportHeaderColumnLabel("TRANSACTION_TYPE")]
        public string? TransactionType { get; set; }

        [ExportHeaderColumnLabel("CARRIER")]
        public string? Carrier { get; set; }

        [ExportHeaderColumnLabel("TRACKING_NUM")]
        public string? TrackingNumber { get; set; }

        [ExportHeaderColumnLabel("QUANTITY")]
        public int? Quantity { get; set; }

        [ExportHeaderColumnLabel("DAY_SUPPLY")]
        public int? DaySupply { get; set; }

        [ExportHeaderColumnLabel("FILL_TYPE")]
        public string? FillType { get; set; } //Although it is required in the specs from Melissa, the same specs also say it is nullable, so no "required" attribute here.

        [ExportHeaderColumnLabel("PRES_LAST_NAME")]
        public required string PresLastName { get; set; }

        [ExportHeaderColumnLabel("PRES_FIRST_NAME")]
        public required string PresFirstName { get; set; }

        [ExportHeaderColumnLabel("PRES_NPI")]
        public required long PresNpi { get; set; }

        [ExportHeaderColumnLabel("PRES_DEA")]
        public string? PresDea { get; set; }

        [ExportHeaderColumnLabel("PRES_ADDR_1")]
        public required string PresAddr1 { get; set; }

        [ExportHeaderColumnLabel("PRES_ADDR_2")]
        public string? PresAddr2 { get; set; }

        [ExportHeaderColumnLabel("PRES_CITY")]
        public required string PresCity { get; set; }

        [ExportHeaderColumnLabel("PRES_STATE")]
        public required string PresState { get; set; }

        [ExportHeaderColumnLabel("PRES_ZIP")]
        public required int PresZip { get; set; }

        [ExportHeaderColumnLabel("PRES_PHONE")]
        public long? PresPhone { get; set; } //0123456789, no dashes

        [ExportHeaderColumnLabel("PRES_FAX")]
        public long? PresFax { get; set; }  // ^

        [ExportHeaderColumnLabel("DEMOGRAPHICID")]
        public required string DemographicId { get; set; }

        [ExportHeaderColumnLabel("CASEID")]
        public required string CaseId { get; set; }

        [ExportHeaderColumnLabel("STATUS_DATE")]
        public required long StatusDate { get; set; } //YYYYMMddHHmmss

        [ExportHeaderColumnLabel("STATUS")]
        public required string Status { get; set; }

        [ExportHeaderColumnLabel("SUB_STATUS")]
        public required string SubStatus { get; set; }

        [ExportHeaderColumnLabel("ENROLLMENT_STATUS")]
        public string? EnrollmentStatus { get; set; }

        [ExportHeaderColumnLabel("TRANSFER_PHARMACY_NAME")]
        public string? TransferPharmacyName { get; set; }

        [ExportHeaderColumnLabel("INSURANCE_NAME")]
        public string? InsuranceName { get; set; }

        [ExportHeaderColumnLabel("INSURANCE_BIN")]
        public string? InsuranceBin { get; set; }

        [ExportHeaderColumnLabel("INSURANCE_PCN")]
        public string? InsurancePcn { get; set; }

        [ExportHeaderColumnLabel("INSURANCE_GROUP")]
        public string? InsuranceGroup { get; set; }

        [ExportHeaderColumnLabel("INSURANCE_ID")]
        public string? InsuranceId { get; set; }

        [ExportHeaderColumnLabel("UPDATE_TYPE")]
        public string? UpdateType { get; set; }
    }
}