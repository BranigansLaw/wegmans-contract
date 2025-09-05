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
    public class PartialRefundRuleShould
    {
        private readonly DateTime queryDate;
        private readonly IRule<PrescriptionSale> rule;
        private readonly DateTime runDate;
        private readonly ERxData testCases;

        public PartialRefundRuleShould()
        {
            this.runDate = new DateTime(2015, 12, 30);
            this.queryDate = this.runDate.AddDays(-1);
            this.testCases = new ERxData(this.runDate);
            this.rule = new IsPartialRefundRule(Convert.ToInt32(this.queryDate.ToString("yyyyMMdd")), this.queryDate);
        }

        [TestMethod]
        public void ReturnFalseForPrescription3000101()
        {
            this.rule.IsMetBy(this.testCases.SaleIsNotPartialRefundBecauseHasCancelledDate).Should().Be(false);
        }

        [TestMethod]
        public void ReturnFalseForPrescription3000102()
        {
            this.rule.IsMetBy(this.testCases.SaleIsNotPartialRefundBecauseHasInvalidSoldDateKey)
                .Should().Be(false);
        }

        //[TestMethod]
        //public void ReturnFalseForPrescription3000103()
        //{
        //    this.rule.IsMetBy(this.testCases.SaleIsNotPartialRefundBecauseIsBilledAfterReturn)
        //        .Should().Be(false);
        //}

        [TestMethod]
        public void ReturnTrueForPrescription3000201()
        {
            this.rule.IsMetBy(this.testCases.SaleIsPartialRefundBecauseHasNegativePaymentHasValidCompletionDate)
                .Should().Be(true);
        }

        [TestMethod]
        public void ReturnTrueForPrescription3000202()
        {
            this.rule.IsMetBy(this.testCases.SaleIsPartialRefundBecauseHasReversalDifferentFromPayment)
                .Should().Be(true);
        }

        [TestMethod]
        public void ReturnTrueForPrescription3000203()
        {
            this.rule.IsMetBy(this.testCases.SaleIsPartialRefundBecauseHasWorkflowSold).Should().Be(true);
        }

        [TestMethod]
        public void ReturnTrueForPrescription3000204()
        {
            this.rule.IsMetBy(
                this.testCases.SaleIsPartialRefundBecauseHasZeroPaymentHasCreditCardReversalHasValidSoldDateKey)
                .Should().Be(true);
        }
    }
}