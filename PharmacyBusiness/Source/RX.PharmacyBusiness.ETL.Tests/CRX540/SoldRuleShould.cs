namespace RX.PharmacyBusiness.ETL.Tests.CRX540
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RX.PharmacyBusiness.ETL.CRX540.Business.Rules;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    /// <summary>
    /// Class for running unit tests on IsSoldRule class.
    /// </summary>
    [TestClass]
    public class SoldRuleShould
    {
        private readonly DateTime queryDate;
        private readonly IRule<PrescriptionSale> rule;
        private readonly DateTime runDate;
        private readonly ERxData testCases;

        public SoldRuleShould()
        {
            this.runDate = new DateTime(2015, 12, 28);
            this.queryDate = this.runDate.AddDays(-1);
            this.testCases = new ERxData(this.runDate);
            this.rule = new IsSoldRule(Convert.ToInt32(this.queryDate.ToString("yyyyMMdd")));
        }

        [TestMethod]
        public void ReturnFalseForPrescription1000101()
        {
            this.rule.IsMetBy(this.testCases.SaleIsNotSoldBecauseDoesNotMeetAnyOptionalRules)
                .Should().Be(false);
        }

        [TestMethod]
        public void ReturnFalseForPrescription1000102()
        {
            this.rule.IsMetBy(this.testCases.SaleIsNotSoldBecauseHasInvalidSoldDateKey).Should().Be(false);
        }

        //[TestMethod]
        //public void ReturnFalseForPrescription1000103()
        //{
        //    this.rule.IsMetBy(this.testCases.SaleIsNotSoldBecauseIsBilledAfterReturn).Should().Be(false);
        //}

        [TestMethod]
        public void ReturnTrueForPrescription1000201()
        {
            this.rule.IsMetBy(this.testCases.SaleIsSoldBecauseHasPositivePaymentHasCompletionDate)
                .Should().Be(true);
        }

        [TestMethod]
        public void ReturnTrueForPrescription1000202()
        {
            this.rule.IsMetBy(this.testCases.SaleIsSoldBecauseHasPositivePaymentHasNoCreditCardHasNoCreditCardReversal)
                .Should().Be(true);
        }

        [TestMethod]
        public void ReturnTrueForPrescription1000203()
        {
            this.rule.IsMetBy(
                this.testCases.SaleIsSoldBecauseHasValidPaymentHasNoCancelledDateHasNoCompletionDateHasRequestRefill)
                .Should().Be(true);
        }

        [TestMethod]
        public void ReturnTrueForPrescription1000204()
        {
            this.rule.IsMetBy(this.testCases.SaleIsSoldBecauseHasZeroPaymentHasCreditCardHasNoCancelledDate)
                .Should().Be(true);
        }

        [TestMethod]
        public void ReturnTrueForPrescription1000205()
        {
            this.rule.IsMetBy(
                this.testCases.SaleIsSoldBecauseHasZeroPaymentHasNoCreditCardHasNoCreditCardReversalHasNoCancelledDate)
                .Should().Be(true);
        }
    }
}