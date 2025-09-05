namespace RX.PharmacyBusiness.ETL.CRX582.Business
{
    using RX.PharmacyBusiness.ETL.CRX582.Contracts;
    using RX.PharmacyBusiness.ETL.CRX582.Core;

    public class InsurancePayerFactory : IInsurancePayerFactory
    {
        public InsurancePayer Create(FillFact fact)
        {
            if (fact == null)
            {
                return new InsurancePayer();
            }
            else if (fact.IsCashTransaction())
            {
                return new InsurancePayer { ERxSplitFillNumber = fact.SplitFillNumber };
            }

            var insurance = new InsurancePayer
                                {
                                    CopayAmount = fact.ToMoney(fact.PatientPayAmount),
                                    DispensingFeePaid = fact.ToMoney(fact.TotalFee),
                                    IngredientCostPaid = fact.ToMoney(fact.FinalPrice),
                                    ERxSplitFillNumber = fact.SplitFillNumber,
                                    CardholderId = InsurancePayer.DefineCardHolderIdFrom(fact),
                                    BIN = InsurancePayer.DefineBinFrom(fact),
                                    ProcessorControlNumber = fact.ProcessorControlNumber,
                                    GroupInsuranceNumber = fact.GroupInsuranceNumber,
                                    AuthorizationNumber = fact.AuthorizationNumber,
                                    PatientRelationshipCode = fact.PatientRelationshipCode,
                                    PayerId = InsurancePayer.DefinePayerIdFrom(fact),
                                    NetworkId = fact.NetworkId,
                                    ThirdPartySubmitType = InsurancePayer.DefineThirdPartySubmitTypeFrom(fact),
                                    OtherCoverageCode =
                                        fact.SplitFillNumber == "1"
                                            ? string.Empty
                                            : RequestMessageParser.GetValue(fact.RequestMessage, "C8")
                                };

            insurance.PatientPayAmount = insurance.CopayAmount;

            SetValuesForNoCashTransaction(insurance, fact);

            return insurance;
        }

        private static void SetValuesForNoCashTransaction(InsurancePayer insurance, FillFact fact)
        {
            if (fact.IsCashTransaction())
            {
                return;
            }

            insurance.CopayAmount = fact.ToMoney(fact.ThirdPartyCurrentPatientPay);
            insurance.DispensingFeePaid = fact.ToMoney(fact.ThirdPartyCurrentTotalFee);

            switch (fact.ExternalBillingIndicator)
            {
                case "M":
                    insurance.HowRecordSubmitted = "P";
                    break;
                case "N":
                    insurance.HowRecordSubmitted = "P";
                    break;
                case "B":
                    insurance.HowRecordSubmitted = "P";
                    break;
                case "H":
                    insurance.HowRecordSubmitted = "H";
                    break;
                default:
                    insurance.HowRecordSubmitted = "T";
                    break;
            }

            insurance.InsurancePlan = fact.PlanCode;
            insurance.AdjudicatedAmount = fact.ToMoney(fact.ThirdPartyPlanAmountPaid);
            insurance.IngredientCostPaid = fact.ToMoney(fact.ThirdPartyCurrentTotalCost);
        }
    }
}