namespace RX.PharmacyBusiness.ETL.CRX582.Core
{
    using System;

    public class FillFact
    {
        public decimal? AcquisitionCost { get; set; }
        public string AuthorizationNumber { get; set; }
        public decimal? AverageWholesalePriceCost { get; set; }
        public string BankingIdentificationNumber { get; set; }
        public long? BasisOfReimbursement { get; set; }
        public string CardholderGender { get; set; }
        public string CardholderId { get; set; }
        public long? CardholderPatientNumber { get; set; }
        public string CardholderPostalCode { get; set; }
        public string CardholderState { get; set; }
        public string CentralFillStoreIndicator { get; set; }
        public decimal? CompoundDispensedQuantity1 { get; set; }
        public decimal? CompoundDispensedQuantity2 { get; set; }
        public decimal? CompoundDispensedQuantity3 { get; set; }
        public decimal? CompoundDispensedQuantity4 { get; set; }
        public decimal? CompoundDispensedQuantity5 { get; set; }
        public decimal? CompoundDispensedQuantity6 { get; set; }
        public decimal? CompoundIngredientDrugCost1 { get; set; }
        public decimal? CompoundIngredientDrugCost2 { get; set; }
        public decimal? CompoundIngredientDrugCost3 { get; set; }
        public decimal? CompoundIngredientDrugCost4 { get; set; }
        public decimal? CompoundIngredientDrugCost5 { get; set; }
        public decimal? CompoundIngredientDrugCost6 { get; set; }
        public string CompoundNDCDispensed1 { get; set; }
        public string CompoundNDCDispensed2 { get; set; }
        public string CompoundNDCDispensed3 { get; set; }
        public string CompoundNDCDispensed4 { get; set; }
        public string CompoundNDCDispensed5 { get; set; }
        public string CompoundNDCDispensed6 { get; set; }
        public int? DaysSupply { get; set; }
        public string DiagnosisCode1 { get; set; }
        public string DispenseAsWritten { get; set; }
        public string DispensedDrugName { get; set; }
        public long? DispensedDrugNumber { get; set; }
        public decimal? DrugPackSize { get; set; }
        public string ExternalBillingIndicator { get; set; }
        public long? FacilityNumber { get; set; }
        public long? FillFactFillSequence { get; set; }
        public string FillFactFillStateCode { get; set; }
        public string FillFactFillStateKey { get; set; }
        public long? FillFactFillStatePriceNumber { get; set; }
        public DateTime FillFactFillStateTimestamp { get; set; }
        public long? FillFactInternalFillId { get; set; }
        public long? FillFactRecordNumber { get; set; }
        public long? FillStateFillSequnce { get; set; }
        public string FillStateFillStateCode { get; set; }
        public DateTime? FillStateFillStateTimestamp { get; set; }
        public long? FillStateRecordNumber { get; set; }
        public decimal? FinalPrice { get; set; }
        public DateTime? FromFillDate { get; set; }
        public string GenericCodeNumber { get; set; }
        public string GroupInsuranceNumber { get; set; }
        public long? HostTransactionNumber { get; set; }
        public decimal? IncentiveAmountPaid { get; set; }
        public decimal? IngredientCostSubmitted { get; set; }
        public string IsCompoundCode { get; set; }
        public string IsEmergencyFill { get; set; }
        public string IsGeneric { get; set; }
        public string IsMaintenanceDrug { get; set; }
        public string IsPreferredDrug { get; set; }
        public string IsSameDayReversal { get; set; }
        public string IsSenior { get; set; }
        public decimal? LookupAcquisitionCost { get; set; }
        public int? LookupDaysSupply { get; set; }
        public DateTime? LookupFirstAdjudicatedDate { get; set; }
        public DateTime? LookupFirstDispenseDate { get; set; }
        public decimal? LookupQtyDispensed { get; set; }
        public DateTime? LookupTransmissionDate { get; set; }
        public long? MedicalConditionNumber { get; set; }
        public decimal? MetricQuantityDispensed { get; set; }
        public string NetworkId { get; set; }
        public int? NumberOfRefillsAuthorized { get; set; }
        public int? NumberOfRefillsRemaining { get; set; }
        public string OriginCode { get; set; }
        public string OriginatingStoreId { get; set; }
        public long? PartialFillSequence { get; set; }
        public long? PatientAccountNumber { get; set; }
        public string PatientGender { get; set; }
        public long? PatientKey { get; set; }
        public decimal? PatientPayAmount { get; set; }
        public long? PatientRelationshipCode { get; set; }
        public string PlanCode { get; set; }
        public long? PrescriberAddressNumber { get; set; }
        public string PrescriberNationalProviderId { get; set; }
        public long? PrescriberNumber { get; set; }
        public string PrescriberState { get; set; }
        public string PrescriberZipcode { get; set; }
        public string PrescriptionNumber { get; set; }
        public string ProcessorControlNumber { get; set; }
        public decimal? ProductPackSize { get; set; }
        public string ProviderId { get; set; }
        public decimal? QuantityWritten { get; set; }
        public int? RefillNumber { get; set; }
        public string RequestMessage { get; set; }
        public DateTime RunDate { get; set; }
        public string ShortFillStatus { get; set; }
        public string SplitFillNumber { get; set; }
        public string Strength { get; set; }
        public string SubmittedNationalDrugCode { get; set; }
        public decimal? ThirdPartyCurrentPatientPay { get; set; }
        public decimal? ThirdPartyCurrentTotalCost { get; set; }
        public decimal? ThirdPartyCurrentTotalFee { get; set; }
        public string ThirdPartyItemClaimKey { get; set; }
        public long? ThirdPartyItemClaimNumber { get; set; }
        public string ThirdPartyMessageKey { get; set; }
        public string ThirdPartyMessageTableHLevel { get; set; }
        public long? ThirdPartyMessageTableMessage { get; set; }
        public long? ThirdPartyPatientNumber { get; set; }
        public long? ThirdPartyPatientPlanNumber { get; set; }
        public decimal? ThirdPartyPlanAmountPaid { get; set; }
        public long? ThirdPartyPlanNumber { get; set; }
        public long? ThirdPartyPriorAuthorization { get; set; }
        public long? ThirdPartyProcessorDestination { get; set; }
        public decimal? TotalFee { get; set; }
        public decimal? TotalUsualAndCustomary { get; set; }
        public string TransactionCode { get; set; }
        public string Unit { get; set; }
        public DateTime? WrittenDate { get; set; }
        
        public long? PRS_PRESCRIBER_KEY { get; set; }
        public long? PRS_PRESCRIBER_NUM { get; set; }
        public long? PADR_KEY { get; set; }

        public bool IsReversalTransaction()
        {
            return this.FillFactFillStateCode == FillFactStatus.NegativeAdjudication;
        }

        public bool IsDrugCompound()
        {
            return this.IsCompoundCode == "Y";
        }

        public bool IsCashTransaction()
        {
            return (this.ThirdPartyPlanNumber ?? 0) == 0 || string.IsNullOrEmpty(this.SplitFillNumber) == true;
        }

        public decimal ToMoney(decimal? value)
        {
            var output = value ?? 0;
            return this.IsReversalTransaction() ? output * -1 : output;
        }

        public override string ToString()
        {
            var output = string.Format(
                "Claim for Store:{0}, Rx:{1}, Refill:{2}, Partial:{3}, FillStateCode:{4}, Time:{5}.", 
                this.OriginatingStoreId, 
                this.PrescriptionNumber, 
                (this.RefillNumber ?? 0), 
                (this.PartialFillSequence ?? 0), 
                this.FillFactFillStateCode, 
                this.FillFactFillStateTimestamp.ToString("yyyy-MM-dd HH:mm:ss"));

            return output;
        }
    }
}