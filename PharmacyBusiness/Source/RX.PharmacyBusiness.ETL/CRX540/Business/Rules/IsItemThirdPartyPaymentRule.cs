namespace RX.PharmacyBusiness.ETL.CRX540.Business.Rules
{
    using System;
    using System.Linq;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    public class IsItemThirdPartyPaymentRule : IRule<PrescriptionSale>
    {
        private readonly Predicate<PrescriptionSale>[] rules;
        private readonly DateTime queryDate;

        public IsItemThirdPartyPaymentRule(DateTime queryDate)
        {
            this.queryDate = queryDate;
            this.rules = new Predicate<PrescriptionSale>[]
                             {
                                 this.HasValidSoldDate, 
                                 this.HasNoCustomerServiceHasNoGetCreditCard,
                                 this.HasNonZeroInsurancePayment
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

            return this.rules.All(rule => rule(entity));
        }

        private bool HasValidSoldDate(PrescriptionSale entity)
        {
            return DerivePrescriptionSaleData.DeriveSoldDate(entity).Date == this.queryDate.Date;
        }

        private bool HasNoCustomerServiceHasNoGetCreditCard(PrescriptionSale entity)
        {
            string derivedPaymentTypeName = DerivePrescriptionSaleData.DerivePaymentTypeName(entity);

            return derivedPaymentTypeName != PaymentTypes.CustomerService
                   && derivedPaymentTypeName != PaymentTypes.GetCreditCard;
        }

        private bool HasNonZeroInsurancePayment(PrescriptionSale entity)
        {
            return Math.Abs(DerivePrescriptionSaleData.DeriveInsurancePayment(entity)) > 0;
        }
    }
}
