namespace Library.McKessonDWInterface.DataModel
{
    public class SureScriptsMedicalHistoryRawDataRow
    {
        public long? RecordSequenceNumber { get; set; }
        public long? ParticipantPatientId { get; set; }
        public string? PatientLastName { get; set; }
        public string? PatientFirstName { get; set; }
        public string? PatientMiddleName { get; set; }
        public string? PatientPrefix { get; set; }
        public string? PatientSuffix { get; set; }
        public DateTime? PatientDateOfBirth { get; set; }
        public string? PatientGender { get; set; }
        public string? PatientAddress1 { get; set; }
        public string? PatientCity { get; set; }
        public string? PatientState { get; set; }
        public string? PatientZipCode { get; set; }
        public string? NcpdpId { get; set; }
        public string? ChainSiteId { get; set; }
        public string? PharmacyName { get; set; }
        public string? FacilityAddress1 { get; set; }
        public string? FacilityCity { get; set; }
        public string? FacilityState { get; set; }
        public string? FacilityZipCode { get; set; }
        public string? FacilityPhoneNumber { get; set; }
        public string? FPrimaryCareProviderLastName { get; set; }
        public string? PrimaryCareProviderFirstName { get; set; }
        public string? PrimaryCareProviderAddress1 { get; set; }
        public string? PrimaryCareProviderCity { get; set; }
        public string? PrimaryCareProviderState { get; set; }
        public string? PrimaryCareProviderZipCode { get; set; }
        public string? PrimaryCareProviderAreaCode { get; set; }
        public string? PrimaryCareProviderPhoneNumber { get; set; }
        public string? PrescriptionNumber { get; set; }
        public long? FillNumber { get; set; }
        public string? NdcNumberDispensed { get; set; }
        public string? MedicationName { get; set; }
        public decimal? QuantityPrescribed { get; set; }
        public decimal? QuantityDispensed { get; set; }
        public long? DaysSupply { get; set; }
        public string? SigText { get; set; }
        public int? DateWritten { get; set; }
        public int? DateFilled { get; set; }
        public decimal? DatePickedUpDispensed { get; set; }
        public decimal? RefillsOriginallyAuthorized { get; set; }
        public decimal? RefillsRemaining { get; set; }
        public long? Logic_FillFactKey { get; set; }
        public long? Logic_PdPatientKey { get; set; }
        public string? Logic_PatientAddressUsage { get; set; }
        public DateTime? Logic_PatientAddressCreateDate { get; set; }
        public long? Logic_PresPhoneKey { get; set; }
        public string? Logic_PresPhoneStatus { get; set; }
        public string? Logic_PresPhoneSourceCode { get; set; }
        public decimal? Logic_PresPhoneHLevel { get; set; }
        public int? Logic_PrescriptionFillRank { get; set; }
    }
}
