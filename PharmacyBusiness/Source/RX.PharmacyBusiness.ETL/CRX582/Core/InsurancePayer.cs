namespace RX.PharmacyBusiness.ETL.CRX582.Core
{
    public class InsurancePayer
    {
        public InsurancePayer()
        {
            this.HowRecordSubmitted = "T";
            this.InsurancePlan = "CASH";
            this.AdjudicatedAmount = 0;
        }

        public decimal? AdjudicatedAmount { get; set; }
        public string AuthorizationNumber { get; set; }
        public string BIN { get; set; }
        public string CardholderId { get; set; }
        public decimal? CopayAmount { get; set; }
        public decimal? DispensingFeePaid { get; set; }
        public string ERxSplitFillNumber { get; set; }
        public string GroupInsuranceNumber { get; set; }
        public string HowRecordSubmitted { get; set; }
        public decimal? IngredientCostPaid { get; set; }
        public string InsurancePlan { get; set; }
        public string NetworkId { get; set; }
        public string OtherCoverageCode { get; set; }
        public decimal? PatientPayAmount { get; set; }
        public long? PatientRelationshipCode { get; set; }
        public string PayerId { get; set; }
        public string ProcessorControlNumber { get; set; }
        public string ThirdPartySubmitType { get; set; }

        public static string DefineBinFrom(FillFact fact)
        {
            var bin = fact.IsCashTransaction() ? "CASH" : fact.BankingIdentificationNumber;

            //if (fact.SplitFillNumber == "2")
            //{
            //    bin = fact.BankingIdentificationNumber;
            //}

            //if (fact.SplitFillNumber == "3")
            //{
            //    bin = string.Empty;
            //}

            return bin;
        }

        public static string DefineCardHolderIdFrom(FillFact fact)
        {
            return string.IsNullOrEmpty(fact.CardholderId)
                       ? string.Empty
                       : ((fact.CardholderId.Length > 18) ? fact.CardholderId.Substring(0, 18) : fact.CardholderId);
        }

        public static string DefinePayerIdFrom(FillFact fact)
        {
            return string.IsNullOrEmpty(fact.PlanCode)
                       ? string.Empty
                       : ((fact.PlanCode.Length > 3) ? fact.PlanCode.Substring(0, 3) : fact.PlanCode);
        }

        public static string DefineThirdPartySubmitTypeFrom(FillFact fact)
        {
            return (fact.FillFactFillStateCode == FillFactStatus.NegativeAdjudication)
                       ? "C"
                       : (fact.ExternalBillingIndicator == "H") ? "D" : "S";
        }
    }
}