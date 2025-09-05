namespace Library.McKessonDWInterface.DataModel
{
    public class SureScriptsMedicalHistoryReportRow
    {
        public string? RecordType { get; set; }
        public long? RecordSequenceNumber { get; set; }
        public long? ParticipantPatientId { get; set; }
        public string? AlternatePatientIdQualifier { get; set; }
        public string? AlternatePatientId { get; set; }
        public string? CustomerDlNumber { get; set; }
        
        public string? PatientLocationCode { get; set; }
        public string? PatientLastName { get; set; }
        public string? PatientFirstName { get; set; }
        public string? PatientMiddleName { get; set; }
        public string? PatientPrefix { get; set; }
        public string? PatientSuffix { get; set; }
        public string? PatientDob { get; set; }
        public string? PatientGender { get; set; }
        public string? PatientAddress1 { get; set; }
        public string? PatientAddress2 { get; set; }
        public string? PatientCity { get; set; }
        public string? PatientState { get; set; }
        public string? PatientZipCode { get; set; }
        public string? PatientCountry { get; set; }
        public string? PatientPhoneNumber { get; set; }
        public decimal? PatientActiveIndicator { get; set; }
        
        public decimal? PdmpIndicator { get; set; }
        public decimal? DrugRecordIndicator { get; set; }
        public string? Allergy { get; set; }
        public string? DiagnosisCodeQualifier { get; set; }
        public string? DiagnosisCode { get; set; }
        public string? SpeciesCode { get; set; }
        public string? NameOfAnimal { get; set; }
        
        public string? NcpdpId { get; set; }
        public string? ChainSiteId { get; set; }
        public string? PharmacyType { get; set; }
        public string? PharmacyName { get; set; }
        public string? RxOriginCode { get; set; }
        public string? PharmacyDea { get; set; }
        public string? PharmacyDeaSuffix { get; set; }
        public string? PharmacyNpi { get; set; }
        public string? FacilityAddress1 { get; set; }
        public string? FacilityAddress2 { get; set; }
        public string? FacilityCity { get; set; }
        public string? FacilityState { get; set; }
        public string? FacilityZipCode { get; set; }
        public string? FacilityPhoneNumber { get; set; }
        public string? ContactName { get; set; }
        public string? PharmacistNpi { get; set; }
        public string? PharmacistLastName { get; set; }
        public string? PharmacistFirstName { get; set; }
        public string? PharmacistMiddleName { get; set; }
        public string? PharmacistStateLicense { get; set; }
        public string? StateCodeIssuingRxSerialNumber { get; set; }
        public string? StateIssuedRxSerialNumber { get; set; }
        public string? PharmacyBoardLicenseNumber { get; set; }
        public string? PharmacyAlternateLicenseNumber { get; set; }
        public string? CountyCode { get; set; }

        public string? PrescriberOrderNumber { get; set; }
        public string? PrescriberSpi { get; set; }
        public string? PrescriberNpi { get; set; }
        public string? AlternatePrescriberId { get; set; }
        public string? PrescriberDea { get; set; }
        public string? DeaSuffix { get; set; }
        public string? StateLicenseNumber { get; set; }

        // Should I change PrimaryCareProvider to Pcp
        public string? PrimaryCareProviderLastName { get; set; }
        public string? PrimaryCareProviderFirstName { get; set; }
        public string? PrimaryCareProviderMiddleName { get; set; }
        public string? PrimaryCareProviderPrefix { get; set; }
        public string? PrimaryCareProviderSuffix { get; set; }
        public string? PrimaryCareProviderAddress1 { get; set; }
        public string? PrimaryCareProviderAddress2 { get; set; }
        public string? PrimaryCareProviderCity { get; set; }
        public string? PrimaryCareProviderState { get; set; }
        public string? PrimaryCareProviderZipCode { get; set; }
        public string? PrimaryCareProviderPhoneNumber { get; set; }
        public string? PrimaryCareProviderFaxNumber { get; set; }

        public string? PrescriptionNumber { get; set; }
        public string? FillNumber { get; set; }
        public string? PartialFillIndicator { get; set; }
        public string? NdcNumberDispensed { get; set; }
        public string? DrugClass { get; set; }
        public string? MedicationName { get; set; }
        public string? QuantityPrescribed { get; set; }
        public string? QuantityDispensed { get; set; }
        public string? QualifierUnitsOfMeasure { get; set; }
        public long? DaysSupply { get; set; }
        public string? BasisOfDaysSupply { get; set; }
        public string? SigText { get; set; }
        public int? DateWritten { get; set; }
        public int? DateAdjudicated { get; set; }
        public int? DateFilled { get; set; }
        public decimal? DateDispensed { get; set; }
        public decimal? RefillsOriginallyAuthorized { get; set; }
        public decimal? RefillsRemaining { get; set; }
        public string? RxNormCode { get; set; }
        public string? ElectronicRxReferenceNumber { get; set; }
        
        public int CompoundIngredientSequenceNumber { get; set; }
        public string? CompoundIngredientId { get; set; }
        public string? CompoundIngredientQuantity { get; set; }
        public string? CompoundIngredientUnitsOfMeasure { get; set; }
        public string? CompoundIngredientMedicationName { get; set; }
        public string? CompoundIngredientDrugClass { get; set; }

        public string? PlanCode { get; set; }
        public string? PaymentCode { get; set; }
        public string? Bin { get; set; }
        public string? Pcn { get; set; }
        public string? GroupId { get; set; }
        public string? CardholderNumber { get; set; }

        public string? IssuingJurisdiction { get; set; }
        public string? IdQualifier { get; set; }
        public string? DropOffPickUpPersonId { get; set; }
        public string? DropOffPickUpPersonRelationship { get; set; }
        public string? RxDropOffPickUpLastName { get; set; }
        public string? RxDropOffPickUpFirstName { get; set; }
        public string? Message { get; set; }
        public string? RxNormProductQualifier { get; set; }
        public string? ElectronicPrescriptionOrderNumber { get; set; }
        public string? DropOffPickUpQualifier { get; set; }
        public string? PartialFillSequenceNumber { get; set; }
    }
}
