namespace RX.PharmacyBusiness.ETL.Tests.CRX540
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RX.PharmacyBusiness.ETL.CRX540.Business.Rules;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    /// <summary>
    /// Class for running unit tests on IsItemThirdPartyPaymentRule class.
    /// </summary>
    [TestClass]
    public class ItemThirdPartyPaymentRuleShould
    {
        private readonly DateTime queryDate;
        private readonly IRule<PrescriptionSale> rule;
        private readonly DateTime runDate;
        private readonly ERxData testCases;

        public ItemThirdPartyPaymentRuleShould()
        {
            this.runDate = new DateTime(2015, 12, 30);
            this.queryDate = this.runDate.AddDays(-1);
            this.testCases = new ERxData(this.runDate);
            this.rule = new IsItemThirdPartyPaymentRule(this.queryDate);
        }

        [TestMethod]
        public void ReturnFalseForPrescription6000101()
        {
            this.rule.IsMetBy(this.testCases.ItemIsNotThirdPartyPaymentBecauseHasInvalidSoldDateKey)
                .Should().Be(false);
        }

        [TestMethod]
        public void ReturnFalseForPrescription6000102()
        {
            this.rule.IsMetBy(this.testCases.ItemIsNotThirdPartyPaymentBecauseHasCustomerService)
                .Should().Be(false);
        }

        [TestMethod]
        public void ReturnFalseForPrescription6000103()
        {
            this.rule.IsMetBy(this.testCases.ItemIsNotThirdPartyPaymentBecauseHasGetCreditCard)
                .Should().Be(false);
        }

        [TestMethod]
        public void ReturnTrueForPrescription6000201()
        {
            this.rule.IsMetBy(
                this.testCases
                    .ItemIsThirdPartyPaymentBecauseHasValidSoldDateHasValidPaymentTypeHasNonZeroInsurancePayment)
                .Should().Be(true);
        }
    }
}