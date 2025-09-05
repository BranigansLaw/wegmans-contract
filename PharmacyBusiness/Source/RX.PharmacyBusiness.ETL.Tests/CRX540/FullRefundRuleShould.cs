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
    public class FullRefundRuleShould
    {
        private readonly DateTime queryDate;
        private readonly IRule<PrescriptionSale> rule;
        private readonly DateTime runDate;
        private readonly ERxData testCases;

        public FullRefundRuleShould()
        {
            this.runDate = new DateTime(2015, 12, 30);
            this.queryDate = this.runDate.AddDays(-1);
            this.testCases = new ERxData(this.runDate);
            this.rule = new IsFullRefundRule(Convert.ToInt32(this.queryDate.ToString("yyyyMMdd")), this.queryDate);
        }

        [TestMethod]
        public void ReturnFalseForPrescription2000101()
        {
            this.rule.IsMetBy(this.testCases.SaleIsNotFullRefundBecauseHasCancelledDate)
                .Should()
                .BeFalse(this.rule.FailReason);
        }

        [TestMethod]
        public void ReturnFalseForPrescription2000102()
        {
            this.rule.IsMetBy(this.testCases.SaleIsNotFullRefundBecauseHasInvalidSoldDateKey)
                .Should().Be(false);
        }

        [TestMethod]
        public void ReturnFalseForPrescription2000103()
        {
            this.rule.IsMetBy(this.testCases.SaleIsNotFullRefundBecauseHasReversalDifferentThanPayment)
                .Should().Be(false);
        }

        //[TestMethod]
        //public void ReturnFalseForPrescription2000104()
        //{
        //    this.rule.IsMetBy(this.testCases.SaleIsNotFullRefundBecauseIsBilledAfterReturn).Should().Be(false);
        //}

        [TestMethod]
        public void ReturnTrueForPrescription2000201()
        {
            this.rule.IsMetBy(
                this.testCases.SaleIsFullRefundBecauseHasValidCompletionDateHasNegativePaymentHasNoWorkflowSold)
                .Should().Be(true);
        }

        [TestMethod]
        public void ReturnTrueForPrescription2000202()
        {
            this.rule.IsMetBy(
                this.testCases.SaleIsFullRefundBecauseHasValidSoldDateKeyHasZeroPaymentHasCreditCardReversal)
                .Should().Be(true);
        }

        [TestMethod]
        public void ReturnFalseForSoldFillState()
        {
            this.rule.IsMetBy(
                this.testCases.SaleIsNotFullRefundBecauseHasFillStateCodeSold)
                .Should().Be(false);
        }
    }
}