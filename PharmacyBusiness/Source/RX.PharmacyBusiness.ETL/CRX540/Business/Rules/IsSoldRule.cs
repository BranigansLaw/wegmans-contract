namespace RX.PharmacyBusiness.ETL.CRX540.Business.Rules
{
    using System;
    using System.Linq;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    public class IsSoldRule : IRule<PrescriptionSale>
    {
        public const string RequestRefill = "RR";
        private readonly Predicate<PrescriptionSale>[] rules;
        private readonly int soldDateKey;

        public IsSoldRule(int soldDateKey)
        {
            this.soldDateKey = soldDateKey;
            this.rules = new Predicate<PrescriptionSale>[]
                             {
                                 this.HasPositivePaymentHasCompletionDate,
                                 this.HasZeroPaymentHasCreditCardHasNoCancelledDate,
                                 this.HasPositivePaymentHasNoCreditCardHasNoCreditCardReversal,
                                 this.HasZeroPaymentHasNoCreditCardHasNoCreditCardReversalHasNoCancelledDate,
                                 this.HasValidPaymentHasNoCancelledDateHasNoCompletionDateHasRequestRefill
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
                entity.SoldDateKey != this.soldDateKey)
            {
                //this.FailReason = "Sale is billed after return or sold date doesn't match current.";
                this.FailReason = "Sale has sold date that doesn't match targeted date.";
                return false;
            }

            return this.rules.Any(rule => rule(entity));
        }

        private bool HasPositivePayment(PrescriptionSale entity)
        {
            var paymentAmount = entity.PaymentAmount ?? 0;
            if (paymentAmount == -0.01M && entity.PaymentTableReversalNumber == 0) paymentAmount = 0.01M;
            return paymentAmount > 0;
        }

        private bool HasPositivePaymentHasCompletionDate(PrescriptionSale entity)
        {
            return this.HasPositivePayment(entity) && entity.CompletionDate.HasValue;
        }

        private bool HasPositivePaymentHasNoCreditCardHasNoCreditCardReversal(PrescriptionSale entity)
        {
            return this.HasPositivePayment(entity) && entity.PaymentTypeName != PaymentTypes.CreditCard
                   && entity.PaymentTypeName != PaymentTypes.CreditCardReversal;
        }

        private bool HasValidPaymentHasNoCancelledDateHasNoCompletionDateHasRequestRefill(PrescriptionSale entity)
        {
            return (this.HasPositivePayment(entity) || this.HasZeroPayment(entity))
                   && entity.CancelledDate.HasValue == false && entity.CompletionDate.HasValue == false
                   && entity.PrescriptionNumber.Length >= 2
                   && entity.PrescriptionNumber.Substring(0, 2).ToUpper() == RequestRefill;
        }

        private bool HasZeroPayment(PrescriptionSale entity)
        {
            var paymentAmount = entity.PaymentAmount ?? 0;
            return paymentAmount == 0;
        }

        private bool HasZeroPaymentHasCreditCardHasNoCancelledDate(PrescriptionSale entity)
        {
            return this.HasZeroPayment(entity) && entity.PaymentTypeName == PaymentTypes.CreditCard
                   && entity.CancelledDate.HasValue == false;
        }

        private bool HasZeroPaymentHasNoCreditCardHasNoCreditCardReversalHasNoCancelledDate(PrescriptionSale entity)
        {
            return this.HasZeroPayment(entity) && entity.PaymentTypeName != PaymentTypes.CreditCard
                   && entity.PaymentTypeName != PaymentTypes.CreditCardReversal
                   && entity.CancelledDate.HasValue == false;
        }
    }
}