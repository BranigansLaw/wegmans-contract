namespace RX.PharmacyBusiness.ETL.CRX540.Business.Rules
{
    using System;
    using System.Linq;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    public class IsFullRefundRule : IRule<PrescriptionSale>
    {
        private readonly Predicate<PrescriptionSale>[] rules;
        private readonly int soldDateKey;
        private readonly DateTime queryDate;

        public IsFullRefundRule(int soldDateKey, DateTime queryDate)
        {
            this.soldDateKey = soldDateKey;
            this.queryDate = queryDate;
            this.rules = new Predicate<PrescriptionSale>[]
                             {
                                 this.HasFillStateCodeDeclineSoldOrReversalForCob,
                                 this.HasValidCompletionDateHasNegativePaymentHasNoWorkflowSold,
                                 this.HasValidSoldDateKeyHasZeroPaymentHasCreditCardReversal
                             };
        }

        /// <summary>
        /// Gets the failure reason when the rule is not passed
        /// </summary>
        public string FailReason { get; private set; }

        /// <summary>
        /// Executes the rule against the entity
        /// </summary>
        /// <param name="entity">Entity that will be checked</param>
        /// <returns>True if rule passes</returns>
        public bool IsMetBy(PrescriptionSale entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.FailReason = string.Empty;

            if (//entity.IsBilledAfterReturn == "Y" ||
                entity.CancelledDate.HasValue ||
                entity.SoldDateKey == 0 ||
                (this.HasReversalSameAsPayment(entity) == false &&
                 this.HasFillStateCodeDeclineSoldOrReversalForCob(entity) == false)
                )
            {
                //this.FailReason = "Sale is billed after return, or canceled date has a value, sold date is zero, or reversal amount differs from payment amount.";
                this.FailReason = "Sale has a value for canceled date has a value, sold date key is zero, reversal amount differs from payment amount, or fill state code is 14.";
                return false;
            }

            return this.rules.Any(rule => rule(entity));
        }

        private bool HasFillStateCodeDeclineSoldOrReversalForCob(PrescriptionSale entity)
        {
            return entity.FillStateCode == FillStateCodes.DeclineSold ||
                   entity.IsReversalForCob == "Y";
        }

        private bool HasValidCompletionDateHasNegativePaymentHasNoWorkflowSold(PrescriptionSale entity)
        {
            return this.HasValidCompletionDate(entity) == true
                   && this.HasNegativePayment(entity) == true
                   && this.HasFillStatusSold(entity) == false;
                   //&& this.HasCurrentWorkflowStepSold(entity) == false;
        }

        private bool HasValidCompletionDate(PrescriptionSale entity)
        {
            return entity.CompletionDate.HasValue
                   && entity.CompletionDate.Value.Date == this.queryDate.Date;
        }

        private bool HasNegativePayment(PrescriptionSale entity)
        {
            decimal paymentAmount = entity.PaymentAmount ?? 0;
            return paymentAmount < 0;
        }

        //NOTE: As of June 2020, CurrentWorkflowStep functionality replaced by HasFillStatusSold.
        //private bool HasCurrentWorkflowStepSold(PrescriptionSale entity)
        //{
        //    return entity.CurrentWorkflowStep == WorkflowSteps.Sold;
        //}

        private bool HasFillStatusSold(PrescriptionSale entity)
        {
            return entity.LatestFillStatusDesc == FillStatusTypes.Sold;
        }

        private bool HasValidSoldDateKeyHasZeroPaymentHasCreditCardReversal(PrescriptionSale entity)
        {
            return this.HasValidSoldDateKey(entity) == true
                   && this.HasZeroPayment(entity) == true
                   && this.HasCreditCardReversal(entity) == true;
        }

        private bool HasValidSoldDateKey(PrescriptionSale entity)
        {
            return entity.SoldDateKey == this.soldDateKey;
        }

        private bool HasZeroPayment(PrescriptionSale entity)
        {
            decimal paymentAmount = entity.PaymentAmount ?? 0;
            return paymentAmount == 0;
        }

        private bool HasCreditCardReversal(PrescriptionSale entity)
        {
            return entity.PaymentTypeName == PaymentTypes.CreditCardReversal;
        }

        private bool HasReversalSameAsPayment(PrescriptionSale entity)
        {
            decimal paymentAmount = entity.PaymentAmount ?? 0;
            decimal patientPricePaidForOrder = entity.PatientPricePaidForOrder ?? 0;
            decimal reversalPaymentAmount = entity.ReversalPaymentAmount ?? 0;
            bool hasFillStatusSold = entity.FillStateCode == FillStateCodes.Sold;
            return 
                ((Math.Abs(reversalPaymentAmount) == Math.Abs(paymentAmount) || 
                Math.Abs(reversalPaymentAmount) == Math.Abs(patientPricePaidForOrder)) &&
                hasFillStatusSold == false);
        }
    }
}