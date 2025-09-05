namespace RX.PharmacyBusiness.ETL.CRX540.Business.Rules
{
    using System;
    using System.Linq;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    public class IsAlternativePaymentRefundRule : IRule<PrescriptionSale>
    {
        private readonly Predicate<PrescriptionSale>[] rules;
        private readonly DateTime queryDate;

        public IsAlternativePaymentRefundRule(DateTime queryDate)
        {
            this.queryDate = queryDate;
            this.rules = new Predicate<PrescriptionSale>[]
                             {
                                 this.HasValidCancelledDate, 
                                 this.HasNoCreditCarHasdNoCreditCardReversal,
                                 this.HasNonZeroPayment
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

            if (entity.IsBilledAfterReturn == "Y")
            {
                this.FailReason = "Sale is billed after return.";
                return false;
            }

            return this.rules.All(rule => rule(entity));
        }

        private bool HasValidCancelledDate(PrescriptionSale entity)
        {
            return entity.CancelledDate.HasValue
                   && this.queryDate <= entity.CancelledDate.Value
                   && this.queryDate >= entity.CancelledDate.Value;
        }

        private bool HasNoCreditCarHasdNoCreditCardReversal(PrescriptionSale entity)
        {
            return entity.PaymentTypeName != PaymentTypes.CreditCard
                   && entity.PaymentTypeName != PaymentTypes.CreditCardReversal;
        }

        private bool HasNonZeroPayment(PrescriptionSale entity)
        {
            return entity.PaymentAmount.HasValue 
                   && entity.PaymentAmount.Value != 0;
        }
    }
}