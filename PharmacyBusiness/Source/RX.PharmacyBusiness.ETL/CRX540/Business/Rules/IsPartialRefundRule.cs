namespace RX.PharmacyBusiness.ETL.CRX540.Business.Rules
{
    using System;
    using System.Linq;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    public class IsPartialRefundRule : IRule<PrescriptionSale>
    {
        private readonly Predicate<PrescriptionSale>[] rules;
        private readonly int soldDateKey;
        private readonly DateTime queryDate;

        public IsPartialRefundRule(int soldDateKey, DateTime queryDate)
        {
            this.soldDateKey = soldDateKey;
            this.queryDate = queryDate;
            this.rules = new Predicate<PrescriptionSale>[]
                             {
                                 this.HasReversalDifferentFromPaymentOrHasWorkflowSold,
                                 this.HasNegativePaymentOrHasZeroCreditCardReversal
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
                   entity.CancelledDate.HasValue
                || entity.SoldDateKey == 0)
            {
                //this.FailReason = "Sale is billed after return, or cancelled date has a value, or sold date is zero.";
                this.FailReason = "Sale has a value for cancelled date, or sold date key is zero.";
                return false;
            }

            if (this.HasFillStateCodeNegativeAdjudicationAndNotReversalForCob(entity))
            {
                this.FailReason = "Sale HasFillStateCodeNegativeAdjudicationAndNotReversalForCob.";
                return false;
            }

            return this.rules.All(rule => rule(entity));
        }

        private bool HasFillStateCodeNegativeAdjudicationAndNotReversalForCob(PrescriptionSale entity)
        {
            return entity.FillStateCode == FillStateCodes.NegativeAdjudication &&
                   entity.IsReversalForCob == "N";
        }

        private bool HasNegativePaymentOrHasZeroCreditCardReversal(PrescriptionSale entity)
        {
           
            return this.HasValidCompletionDateHasNegativePayment(entity) == true
                   || this.HasValidSoldDateKeyHasZeroPaymentHasCreditCardReversal(entity) == true;
        }

        private bool HasReversalDifferentFromPaymentOrHasWorkflowSold(PrescriptionSale entity)
        {
            
            return this.HasReversalSameAsPayment(entity) == false
                   || this.HasFillStatusSold(entity) == true;
            //|| this.HasCurrentWorkflowStepSold(entity) == true;
        }

        private bool HasValidCompletionDateHasNegativePayment(PrescriptionSale entity)
        {
            
            return this.HasValidCompletionDate(entity) == true
                   && this.HasNegativePayment(entity) == true;
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
            return
                Math.Abs(reversalPaymentAmount) == Math.Abs(paymentAmount) ||
                Math.Abs(reversalPaymentAmount) == Math.Abs(patientPricePaidForOrder);
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
    }
}
