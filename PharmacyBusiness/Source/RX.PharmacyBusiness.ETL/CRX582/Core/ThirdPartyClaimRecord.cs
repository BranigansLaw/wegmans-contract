namespace RX.PharmacyBusiness.ETL.CRX582.Core
{
    using System;

    /// <summary>
    /// Represents the Wegmans definition of a ThirdPartyClaim based on business rules applied to EnterpriseRx FillFact data.
    /// </summary>
    public class ThirdPartyClaimRecord
    {
        public DateTime ClaimDate { get; set; }
        public string CardholderGender { get; set; }
        public string CardholderPostalCode { get; set; }
        public string CardholderState { get; set; }
        public string CompoundDrugName { get; set; }
        public string CompoundNDCDispensed1 { get; set; }
        public string CompoundNDCDispensed2 { get; set; }
        public string CompoundNDCDispensed3 { get; set; }
        public string CompoundNDCDispensed4 { get; set; }
        public string CompoundNDCDispensed5 { get; set; }
        public string CompoundNDCDispensed6 { get; set; }
        public decimal? DecimalPackSize { get; set; }
        public string DiagnosisCode1 { get; set; }
        public int DiagnosisCodeCount { get; set; }
        public string DiagnosisCodeQualifier1 { get; set; }
        public string DispensedDrugName { get; set; }
        public string IsClientGeneric { get; set; }
        public string IsEmergencyFill { get; set; }
        public string IsMaintenanceDrug { get; set; }
        public string IsPartialFill { get; set; }
        public string IsPreferredDrug { get; set; }
        public decimal? MetricQuantityDispensed { get; set; }
        public decimal? PackSize { get; set; }
        public decimal? QuantityWritten { get; set; }
        public string Schedule { get; set; }
        public string Strength { get; set; }
        public string SubmittedDrugName { get; set; }
        public string SubmittedDrugNDC { get; set; }
        public string Unit { get; set; }
        public string IsManualBill { get; set; }
        public decimal AcquisitionCost { get; set; }
        public decimal AWPCost { get; set; }
        public string BasisOfReimbursement { get; set; }
        public decimal CompoundIngredientDrugCost1 { get; set; }
        public decimal CompoundIngredientDrugCost2 { get; set; }
        public decimal CompoundIngredientDrugCost3 { get; set; }
        public decimal CompoundIngredientDrugCost4 { get; set; }
        public decimal CompoundIngredientDrugCost5 { get; set; }
        public decimal CompoundIngredientDrugCost6 { get; set; }
        public decimal IncentiveAmountPaid { get; set; }
        public decimal IngredientCostSubmitted { get; set; }
        public decimal TransactionPrice { get; set; }
        public decimal UsualAndCustomary { get; set; }
        public long PatientAccountNumber { get; set; }
        public string PatientGender { get; set; }
        public string PatientMedicarePartDCoverage { get; set; }
        public string PatientMedigapId { get; set; }
        public string PatientPostalCode { get; set; }
        public string PatientState { get; set; }
        public string OriginatingStoreId { get; set; }
        public string ProviderId { get; set; }
        public string ProviderIdQualifier { get; set; }
        public decimal CompoundDispensedQuantity1 { get; set; }
        public decimal CompoundDispensedQuantity2 { get; set; }
        public decimal CompoundDispensedQuantity3 { get; set; }
        public decimal CompoundDispensedQuantity4 { get; set; }
        public decimal CompoundDispensedQuantity5 { get; set; }
        public decimal CompoundDispensedQuantity6 { get; set; }
        public string DAW { get; set; }
        public int DaysSupply { get; set; }
        public decimal? DispensedQuantity { get; set; }
        public DateTime? FromFillDate { get; set; }
        public long HostTransactionNumber { get; set; }
        public DateTime? InitialFillDate { get; set; }
        public string IsCompoundCode { get; set; }
        public string IsThirdParty { get; set; }
        public int NumberOfRefillsAuthorized { get; set; }
        public int NumberOfRefillsRemaining { get; set; }
        public string PrescriptionNumber { get; set; }
        public int RefillNumber { get; set; }
        public string TransactionCode { get; set; }
        public DateTime? TransmissionDate { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string PrescriberNPI { get; set; }
        public string PrescriberPostalCode { get; set; }
        public string PrescriberState { get; set; }
        public string GenericCodeNumber { get; set; }
        public string OriginCode { get; set; }
        public int ProcedureModifierCodeCount { get; set; }

        //Primary insurance:
        public string SplitFillNumber_1 { get; set; }
        public string ERxSplitFillNumber_1 { get; set; }
        public string CardholderId_1 { get; set; }
        public decimal? AdjudicatedAmount_1 { get; set; }
        public decimal? CopayAmount_1 { get; set; }
        public decimal? DispensingFeePaid_1 { get; set; }
        public decimal? IngredientCostPaid_1 { get; set; }
        public decimal? PatientPayAmount_1 { get; set; }
        public string BIN_1 { get; set; }
        public string InsurancePlan_1 { get; set; }
        public string HowRecordSubmitted_1 { get; set; }
        public string ProcessorControlNumber_1 { get; set; }
        public string GroupInsuranceNumber_1 { get; set; }
        public string AuthorizationNumber_1 { get; set; }
        public long? PatientRelationshipCode_1 { get; set; }
        public string PayerId_1 { get; set; }
        public string NetworkId_1 { get; set; }
        public string ThirdPartySubmitType_1 { get; set; }
        public string OtherCoverageCode_1 { get; set; }

        //Secondary insurance:
        public string SplitFillNumber_2 { get; set; }
        public string ERxSplitFillNumber_2 { get; set; }
        public string CardholderId_2 { get; set; }
        public decimal? AdjudicatedAmount_2 { get; set; }
        public decimal? CopayAmount_2 { get; set; }
        public decimal? DispensingFeePaid_2 { get; set; }
        public decimal? IngredientCostPaid_2 { get; set; }
        public decimal? PatientPayAmount_2 { get; set; }
        public string BIN_2 { get; set; }
        public string InsurancePlan_2 { get; set; }
        public string HowRecordSubmitted_2 { get; set; }
        public string ProcessorControlNumber_2 { get; set; }
        public string GroupInsuranceNumber_2 { get; set; }
        public string AuthorizationNumber_2 { get; set; }
        public long? PatientRelationshipCode_2 { get; set; }
        public string PayerId_2 { get; set; }
        public string NetworkId_2 { get; set; }
        public string ThirdPartySubmitType_2 { get; set; }
        public string OtherCoverageCode_2 { get; set; }

        //Tertiary insurance:
        public string SplitFillNumber_3 { get; set; }
        public string ERxSplitFillNumber_3 { get; set; }
        public string CardholderId_3 { get; set; }
        public decimal? AdjudicatedAmount_3 { get; set; }
        public decimal? CopayAmount_3 { get; set; }
        public decimal? DispensingFeePaid_3 { get; set; }
        public decimal? IngredientCostPaid_3 { get; set; }
        public decimal? PatientPayAmount_3 { get; set; }
        public string BIN_3 { get; set; }
        public string InsurancePlan_3 { get; set; }
        public string HowRecordSubmitted_3 { get; set; }
        public string ProcessorControlNumber_3 { get; set; }
        public string GroupInsuranceNumber_3 { get; set; }
        public string AuthorizationNumber_3 { get; set; }
        public long? PatientRelationshipCode_3 { get; set; }
        public string PayerId_3 { get; set; }
        public string NetworkId_3 { get; set; }
        public string ThirdPartySubmitType_3 { get; set; }
        public string OtherCoverageCode_3 { get; set; }

        public long? PRS_PRESCRIBER_KEY { get; set; }
        public long? PRS_PRESCRIBER_NUM { get; set; }
        public long? PADR_KEY { get; set; }

        public bool IsReversalTransaction()
        {
            return this.TransactionCode == RX.PharmacyBusiness.ETL.CRX582.Core.TransactionCode.NegativeAdjudication;
        }

        public override string ToString()
        {
            var str = string.Format(
                    "Claim for ClaimDate:{0}, Store:{1}, Rx:{2}, Refill:{3}, Splits:{4}, TransactionCode:{5}.",
                    this.ClaimDate.ToShortDateString(),
                    this.OriginatingStoreId,
                    this.PrescriptionNumber,
                    this.RefillNumber.ToString(),
                    string.Format("{0}|{1}|{2}",
                        ((string.IsNullOrEmpty(this.CardholderId_1) == true) ? "x" : "1"),
                        ((string.IsNullOrEmpty(this.CardholderId_2) == true) ? "x" : "2"),
                        ((string.IsNullOrEmpty(this.CardholderId_3) == true) ? "x" : "3")),
                    this.TransactionCode
                );

            return str;
        }
    }
}
