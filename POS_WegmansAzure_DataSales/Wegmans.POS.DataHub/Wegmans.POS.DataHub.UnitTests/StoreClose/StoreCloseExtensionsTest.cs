using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.Util;
using FluentAssertions;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.StoreClose.v1;


namespace Wegmans.POS.DataHub.UnitTests.StoreClose
{
    public class StoreCloseExtensionsTest
    {
        [Theory]
        [InlineData("0")]
        [InlineData("00")]
        [InlineData("01")]
        [InlineData("02")]
        [InlineData("03")]
        [InlineData("99")]
        public void ConvertToCloseTypeIndication_ShouldReturnCorrectCloseType_WhenIndicatorIsValid(string idToTest)
        {
            var response = idToTest.ConvertToCloseTypeIndication();

            List<CloseTypeIndication> myListCT = new List<CloseTypeIndication>() { CloseTypeIndication.CloseFiles, CloseTypeIndication.CloseReportingPeriodLong, CloseTypeIndication.CloseReportingPeriodShort, CloseTypeIndication.CloseDepartmentTotals, CloseTypeIndication.InvalidClose };

            response.Should().BeOneOf(myListCT);
        }

        [Theory]
        [InlineData("05")]
        [InlineData("")]
        [InlineData(" ")]
        public void ConvertToCloseTypeIndication_ShouldReturnNull_WhenIdicatorIsNotValid(string idToTest)
        {
            var response = idToTest.ConvertToCloseTypeIndication();
            response.Should().BeNull();
        }
    }
}
