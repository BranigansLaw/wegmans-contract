namespace RX.PharmacyBusiness.ETL.Tests.CRX540
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RX.PharmacyBusiness.ETL.CRX540.Business.Rules;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    /// <summary>
    /// Class for running unit tests on IsItemPrescriptionSaleRule class.
    /// </summary>
    [TestClass]
    public class ItemPrescriptionSaleRuleShould
    {
        private IRule<PrescriptionSale> rule;
        private DateTime runDate;
        private DateTime queryDate;
        private ERxData testCases;

        public ItemPrescriptionSaleRuleShould()
        {
            this.runDate = new DateTime(2015, 12, 30);
            this.queryDate = this.runDate.AddDays(-1);
            this.testCases = new ERxData(this.runDate);
            this.rule = new IsItemPrescriptionSaleRule(this.queryDate);
        }

        [TestMethod]
        public void ReturnFalseForPrescription4000101()
        {
            this.rule.IsMetBy(this.testCases.ItemIsNotPrescriptionSaleBecauseHasInvalidSoldDateKey).Should().Be(false);
        }

        [TestMethod]
        public void ReturnFalseForPrescription4000102()
        {
            this.rule.IsMetBy(this.testCases.ItemIsNotPrescriptionSaleBecauseHasCustomerService).Should().Be(false);
        }

        [TestMethod]
        public void ReturnFalseForPrescription4000103()
        {
            this.rule.IsMetBy(this.testCases.ItemIsNotPrescriptionSaleBecauseHasGetCreditCard).Should().Be(false);
        }

        [TestMethod]
        public void ReturnTrueForPrescription4000201()
        {
            this.rule.IsMetBy(this.testCases.ItemIsPrescriptionSaleBecauseHasValidSoldDateHasNoCustomerServiceHasNoGetCreditCard).Should().Be(true);
        }
    }
}

