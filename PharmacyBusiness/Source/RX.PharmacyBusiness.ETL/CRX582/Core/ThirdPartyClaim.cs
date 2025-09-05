namespace RX.PharmacyBusiness.ETL.CRX582.Core
{
    using System;
    using RX.PharmacyBusiness.ETL.CRX582.Business;

    /// <summary>
    /// Represents the Wegmans definition of a ThirdPartyClaim based on business rules applied to EnterpriseRx FillFact data.
    /// </summary>
    public class ThirdPartyClaim
    {
        public const int DaysSupplyMax = 999;

        private string cardholderGender;
        private int daysSupply;

        /// <summary>
        /// Initializes a new instance of the ThirdPartyClaim class
        /// </summary>
        public ThirdPartyClaim()
        {
            this.PrimaryInsurance = new InsurancePayer();
            this.SecondaryInsurance = new InsurancePayer();
            this.TertiaryInsurance = new InsurancePayer();
            this.PatientMedicarePartDCoverage = string.Empty;
            this.PatientMedigapId = string.Empty;
            this.ProviderIdQualifier = "07";
            this.ProcedureModifierCodeCount = 0;
        }

        public decimal AWPCost { get; set; }
        public decimal AcquisitionCost { get; set; }
        public string BasisOfReimbursement { get; set; }

        public string CardholderGender
        {
            get
            {
                return this.cardholderGender;
            } 
            
            set
            {
                switch (value)
                {
                    case "F":
                        this.cardholderGender = "2";
                        break;
                    case "M":
                        this.cardholderGender = "1";
                        break;
                    case "U":
                        this.cardholderGender = "0";
                        break;
                    default:
                        this.cardholderGender = value;
                        break;
                }
            }
        }

        public string CardholderPostalCode { get; set; }
        public string CardholderState { get; set; }
        public string CentralFillStoreIndicator { get; set; }
        public DateTime ClaimDate { get; set; }
        public decimal CompoundDispensedQuantity1 { get; set; }
        public decimal CompoundDispensedQuantity2 { get; set; }
        public decimal CompoundDispensedQuantity3 { get; set; }
        public decimal CompoundDispensedQuantity4 { get; set; }
        public decimal CompoundDispensedQuantity5 { get; set; }
        public decimal CompoundDispensedQuantity6 { get; set; }
        public string CompoundDrugName { get; set; }
        public decimal CompoundIngredientDrugCost1 { get; set; }
        public decimal CompoundIngredientDrugCost2 { get; set; }
        public decimal CompoundIngredientDrugCost3 { get; set; }
        public decimal CompoundIngredientDrugCost4 { get; set; }
        public decimal CompoundIngredientDrugCost5 { get; set; }
        public decimal CompoundIngredientDrugCost6 { get; set; }
        public string CompoundNDCDispensed1 { get; set; }
        public string CompoundNDCDispensed2 { get; set; }
        public string CompoundNDCDispensed3 { get; set; }
        public string CompoundNDCDispensed4 { get; set; }
        public string CompoundNDCDispensed5 { get; set; }
        public string CompoundNDCDispensed6 { get; set; }
        public string DAW { get; set; }
        public int DaysSupply
        {
            get
            {
                return this.daysSupply;
            } 

            set
            {
                this.daysSupply = value > DaysSupplyMax ? DaysSupplyMax : value;
            }
        }
        public decimal? DecimalPackSize { get; set; }
        public string DiagnosisCode1 { get; set; }
        public int DiagnosisCodeCount 
        {
            get { return string.IsNullOrEmpty(this.DiagnosisCode1) == false ? 1 : 0; }
        }
        public string DiagnosisCodeQualifier1
        {
            get { return this.DiagnosisCodeCount == 1 ? "01" : string.Empty;  }
        }
        public string DispensedDrugName { get; set; }
        public decimal? DispensedQuantity { get; set; }
        public DateTime? FromFillDate { get; set; }
        public string GenericCodeNumber { get; set; }
        public long HostTransactionNumber { get; set; }
        public decimal IncentiveAmountPaid { get; set; }
        public decimal IngredientCostSubmitted { get; set; }
        public DateTime? InitialFillDate { get; set; }
        public string IsClientGeneric { get; set; }
        public string IsCompoundCode { get; set; }
        public string IsEmergencyFill { get; set; }
        public string IsMaintenanceDrug { get; set; }
        public string IsManualBill { get; set; }
        public string IsPartialFill { get; set; }
        public string IsPreferredDrug { get; set; }
        public string IsSenior { get; set; }
        public string IsThirdParty { get; set; }
        public decimal? MetricQuantityDispensed { get; set; }
        public int NumberOfRefillsAuthorized { get; set; }
        public int NumberOfRefillsRemaining { get; set; }
        public string OriginCode { get; set; }
        public string OriginatingStoreId { get; set; }
        public decimal? PackSize { get; set; }
        public long PatientAccountNumber { get; set; }
        public string PatientGender { get; set; }

        public string PatientMedicarePartDCoverage { get; set; }
        public string PatientMedigapId { get; set; }
        public string PatientPostalCode { get; set; }
        public string PatientState { get; set; }
        public string PrescriberNPI { get; set; }
        public string PrescriberPostalCode { get; set; }
        public string PrescriberState { get; set; }
        public string PrescriptionNumber { get; set; }

        public InsurancePayer PrimaryInsurance { get; set; }
        public int ProcedureModifierCodeCount { get; set; }
        public string ProviderId { get; set; }
        public string ProviderIdQualifier { get; set; }
        public decimal? QuantityWritten { get; set; }
        public int RefillNumber { get; set; }

        public string Schedule { get; set; }

        public InsurancePayer SecondaryInsurance { get; set; }
        public string Strength { get; set; }
        public string SubmittedDrugNDC { get; set; }
        public string SubmittedDrugName { get; set; }
        public InsurancePayer TertiaryInsurance { get; set; }
        public long? ThirdPartyPlanNumber { get; set; }
        public string TransactionCode { get; set; }
        public decimal TransactionPrice { get; set; }
        public DateTime? TransmissionDate { get; set; }
        public string Unit { get; set; }
        public decimal UsualAndCustomary { get; set; }
        public DateTime? WrittenDate { get; set; }

        public long? PRS_PRESCRIBER_KEY { get; set; }
        public long? PRS_PRESCRIBER_NUM { get; set; }
        public long? PADR_KEY { get; set; }

        public override string ToString()
        {
            var output =
                string.Format(
                    "Claim for ClaimDate:{0}, Store:{1}, Rx:{2}, Refill:{3}, Splits:{4}, TransactionCode:{5}.", 
                    this.ClaimDate.ToShortDateString(), 
                    this.OriginatingStoreId, 
                    this.PrescriptionNumber, 
                    this.RefillNumber, 
                    string.Format(
                        "{0}|{1}|{2}", 
                        string.IsNullOrEmpty(this.PrimaryInsurance.CardholderId) ? "x" : "1", 
                        string.IsNullOrEmpty(this.SecondaryInsurance.CardholderId) ? "x" : "2", 
                        string.IsNullOrEmpty(this.TertiaryInsurance.CardholderId) ? "x" : "3"), 
                    this.TransactionCode);

            return output;
        }

        public static DateTime? DefineInitalFillDateFrom(FillFact fill)
        {
            var initialFillDate = default(DateTime?);

            if (fill.LookupFirstDispenseDate.HasValue == false && fill.LookupFirstAdjudicatedDate.HasValue)
            {
                initialFillDate = fill.LookupFirstAdjudicatedDate.Value;
            }
            else if (fill.LookupFirstDispenseDate.HasValue && fill.LookupFirstAdjudicatedDate.HasValue == false)
            {
                initialFillDate = fill.LookupFirstDispenseDate.Value;
            }
            else if (fill.LookupFirstDispenseDate.HasValue && fill.LookupFirstAdjudicatedDate.HasValue)
            {
                initialFillDate = (fill.LookupFirstAdjudicatedDate.Value < fill.LookupFirstDispenseDate.Value)
                                      ? fill.LookupFirstAdjudicatedDate.Value
                                      : fill.LookupFirstDispenseDate.Value;
            }

            return initialFillDate;
        }

        public static string DefineScheduleFrom(FillFact fill)
        {
            string schedule;

            switch (fill.PrescriptionNumber.Substring(0, 1))
            {
                case "2":
                    schedule = "2";
                    break;
                case "3":
                    schedule = "2";
                    break;
                case "4":
                    schedule = "4";
                    break;
                case "5":
                    schedule = "4";
                    break;
                case "6":
                    schedule = "6";
                    break;
                case "7":
                    schedule = "6";
                    break;
                case "8":
                    schedule = "8";
                    break;
                case "9":
                    schedule = "8";
                    break;
                default:
                    schedule = "0";
                    break;
            }

            return schedule;
        }

        public static string DefinePatientGenderFrom(FillFact fill)
        {
            string gender;

            switch (fill.PatientGender)
            {
                case "F":
                    gender = "2";
                    break;
                case "M":
                    gender = "1";
                    break;
                case "U":
                    gender = "0";
                    break;
                default:
                    gender = fill.PatientGender;
                    break;
            }

            return gender;
        }

        public static string DefineCardHolderGender(FillFact fill)
        {
            string gender;

            switch (fill.CardholderGender)
            {
                case "F":
                    gender = "2";
                    break;
                case "M":
                    gender = "1";
                    break;
                case "U":
                    gender = "0";
                    break;
                default:
                    gender = fill.CardholderGender;
                    break;
            }

            return gender;
        }

        public static string DefineIsClientGenericFrom(FillFact fill)
        {
            return (fill.IsGeneric == "3").ToShortYesNo();
        }

        public static string DefineIsMaintenanceDrugFrom(FillFact fill)
        {
            return (fill.IsMaintenanceDrug == "1").ToShortYesNo();
        }

        public static string DefineIsPartialFillFrom(FillFact fill)
        {
            return ((fill.PartialFillSequence ?? 0) > 0).ToShortYesNo();
        }
    }
}