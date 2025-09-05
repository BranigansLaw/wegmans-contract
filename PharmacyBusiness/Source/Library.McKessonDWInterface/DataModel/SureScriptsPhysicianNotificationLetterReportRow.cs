namespace Library.McKessonDWInterface.DataModel
{
    public class SureScriptsPhysicianNotificationLetterReportRow
    {
        public string? RecordType { get; set; }
        public long? RecordSequenceNumber { get; set; }
        public long? ParticipantPatientId { get; set; }
        public string? PatientIdIssuer { get; set; }
        public string? PatientLastName { get; set; }
        public string? PatientFirstName { get; set; }
        public string? PatientMiddleName { get; set; }
        public string? PatientPhoneNumber { get; set; }
        public DateTime? PatientDob { get; set; }
        public string? PatientGender { get; set; }
        public string? PatientAddress1 { get; set; }
        public string? PatientAddress2 { get; set; }
        public string? PatientCity { get; set; }
        public string? PatientState { get; set; }
        public string? PatientZipCode { get; set; }

        public string? PrimaryCareProviderNpi { get; set; }
        public string? PrimaryCareProviderStateLicenseNumber { get; set; }
        public long? InternalPrimaryCareProviderId { get; set; }
        public string? PrimaryCareProviderLastName { get; set; }
        public string? PrimaryCareProviderFirstName { get; set; }
        public string? PrimaryCareProviderAddress1 { get; set; }
        public string? PrimaryCareProviderAddress2 { get; set; }
        public string? PrimaryCareProviderCity { get; set; }
        public string? PrimaryCareProviderState { get; set; }
        public string? PrimaryCareProviderZipCode { get; set; }
        public string? PrimaryCareProviderPhoneNumber { get; set; }
        public string? PrimaryCareProviderFaxNumber { get; set; }
        
        public string? VaccineCptCode { get; set; }
        public string? VaccineCvxCode { get; set; }
        public string? VaccineName { get; set; }
        public string? VaccineManufacturerName { get; set; }
        public string? VaccineManufacturerCode { get; set; }
        public string? VaccineLotNumber { get; set; }
        public DateTime? VaccineExpirationDate { get; set; }
        public string? VaccineInformationStatementName { get; set; }
        public string? VaccineInformationStatementDate { get; set; }
        public decimal? AdministeredDate { get; set; }
        public string? DataEntryDate { get; set; }
        public decimal? AdministeredAmount { get; set; }
        public string? AdministeredUnits { get; set; }
        public string? AdministeredUnitsCode { get; set; }
        public string? RouteOfAdministrationDescription { get; set; }
        public string? RouteOfAdministrationCode { get; set; }
        public string? SiteOfAdministrationDescription { get; set; }
        public string? SiteOfAdministrationCode { get; set; }
        public string? DoseNumberInSeries { get; set; }
        public string? NumberOfDosesInSeries { get; set; }
        public string? AdministeringProviderInternalId { get; set; }
        public string? AdministeringProviderNpi { get; set; }
        public string? AdministeringProviderLastName { get; set; }
        public string? AdministeringProviderFirstName { get; set; }
        public string? AdministeringProviderStateLicenseNumber { get; set; }
        public string? FacilityNpi { get; set; }
        public string? FacilityIdentification { get; set; }
        public string? FacilityAddress1 { get; set; }
        public string? FacilityAddress2 { get; set; }
        public string? FacilityCity { get; set; }
        public string? FacilityState { get; set; }
        public string? FacilityZipCode { get; set; }
        public string? FacilityPhoneNumber { get; set; }
        public string? NotificationLetterConfigurationCode { get; set; }
        public string? SourceSystemId { get; set; }
        public string? SignedConsentForMinor { get; set; }
        public string? ProfessionalConsultationComplete { get; set; }
        public string? EligibilityConsultationComplete { get; set; }
        public string? CustomerDefinedElement1 { get; set; }
        public string? CustomerDefinedElement2 { get; set; }
        public string? CustomerDefinedElement3 { get; set; }
        public string? CustomerDefinedElement4 { get; set; }
       
        public decimal? FacilityIdRank { get; set; }
        public decimal? PatientPhoneRank { get; set; }
        public decimal? PatientAddressRank { get; set; }
        public decimal? PrescriberPhoneRank { get; set; }
        public decimal? PrescriberPcpFaxRank { get; set; }
        public decimal? PrescriberNpiRank { get; set; }
        public decimal? FillPricingFactRank { get; set; }
        
        public long FillFactKey { get; set; } = 0;
        public decimal? PdPatientKey { get; set; }
        public decimal? FdFaciltyKey { get; set; }
        public decimal? RxNumber { get; set; }
        public decimal? RefillNumber { get; set; }
        public decimal? SoldDateKey { get; set; }
    }
}
