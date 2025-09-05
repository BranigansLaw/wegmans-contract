namespace RX.PharmacyBusiness.ETL.Tests.RXS812
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RX.PharmacyBusiness.ETL.RXS812;

    /// <summary>
    /// Class for running unit tests on GetLastQuarterDateRange method.
    /// </summary>
    [TestClass]
    public class GetLastQuarterDateRangeShould
    {
        public GetLastQuarterDateRangeShould()
        {
        }

        [TestMethod]
        public void ReturnPriorQuarterForGivenQ1Rundate()
        {
            DateTime runDate = new DateTime(2000, 1, 1);
            DateTime expectedStartDate = new DateTime(1999, 1, 1);
            DateTime expectedSEndDate = new DateTime(2000, 1, 1);
            DownloadNaloxone.GetLastQuarterDateRange(runDate, out DateTime startDate, out DateTime endDate);
            startDate.Should().Equals(expectedStartDate);
        }

        [TestMethod]
        public void ReturnPriorQuarterForGivenQ2Rundate()
        {
            DateTime runDate = new DateTime(2000, 5, 5);
            DateTime expectedStartDate = new DateTime(2000, 1, 1);
            DateTime expectedSEndDate = new DateTime(2000, 4, 1);
            DownloadNaloxone.GetLastQuarterDateRange(runDate, out DateTime startDate, out DateTime endDate);
            startDate.Should().Equals(expectedStartDate);
        }

        [TestMethod]
        public void ReturnPriorQuarterForGivenQ3Rundate()
        {
            DateTime runDate = new DateTime(2000, 9, 9);
            DateTime expectedStartDate = new DateTime(2000, 4, 1);
            DateTime expectedSEndDate = new DateTime(2000, 6, 1);
            DownloadNaloxone.GetLastQuarterDateRange(runDate, out DateTime startDate, out DateTime endDate);
            startDate.Should().Equals(expectedStartDate);
        }

        [TestMethod]
        public void ReturnPriorQuarterForGivenQ4Rundate()
        {
            DateTime runDate = new DateTime(2000, 10, 2);
            DateTime expectedStartDate = new DateTime(2000, 7, 1);
            DateTime expectedSEndDate = new DateTime(2000, 9, 1);
            DownloadNaloxone.GetLastQuarterDateRange(runDate, out DateTime startDate, out DateTime endDate);
            startDate.Should().Equals(expectedStartDate);
        }
    }
}
