namespace RX.PharmacyBusiness.ETL.Tests.CRX540
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RX.PharmacyBusiness.ETL.CRX540.Business.Rules;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    /// <summary>
    /// Class for running unit tests on IsItemShippingChargeRule class.
    /// </summary>
    [TestClass]
    public class ItemShippingChargeRuleShould
    {
        private readonly DateTime queryDate;
        private readonly IRule<PrescriptionSale> rule;
        private readonly DateTime runDate;
        private readonly ERxData testCases;

        public ItemShippingChargeRuleShould()
        {
            this.runDate = new DateTime(2015, 12, 30);
            this.queryDate = this.runDate.AddDays(-1);
            this.testCases = new ERxData(this.runDate);
            this.rule = new IsItemShippingChargeRule(this.queryDate);
        }

        [TestMethod]
        public void ReturnFalseForPrescription5000101()
        {
            this.rule.IsMetBy(this.testCases.ItemIsNotShippingChargeBecauseHasInvalidSoldDateKey)
                .Should().Be(false);
        }

        [TestMethod]
        public void ReturnFalseForPrescription5000102()
        {
            this.rule.IsMetBy(this.testCases.ItemIsNotShippingChargeBecauseHasZeroShipHandleFee)
                .Should().Be(false);
        }

        [TestMethod]
        public void ReturnTrueForPrescription5000201()
        {
            this.rule.IsMetBy(this.testCases.ItemIsShippingChargeBecauseHasValidSoldDateHasNonZeroShipHandleFee)
                .Should().Be(true);
        }
    }
}