using FluentAssertions;
using System;
using System.Collections.Generic;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models;
using Xunit;

namespace Wegmans.RX.Cmm.AzureFunctions.Tests.Astute.Models
{
    public class AdditionalDetailsTests
    {
        [Theory]
        [MemberData(nameof(AppealSentToPlanAtData))]
        public void AppealSentToPlanAt(IEnumerable<PriorAuthorization> priorAuthorizations, DateTimeOffset? expected)
        {
            var details = new AdditionalDetails { PriorAuthorizationAppeals = priorAuthorizations };

            details.AppealSentToPlanAt.Should().Be(expected);
        }

        public static IEnumerable<object[]> AppealSentToPlanAtData()
        {
            var now = DateTimeOffset.Now;

            var allItems = new List<PriorAuthorization>
            {
                new PriorAuthorization (),
                new PriorAuthorization { OutcomeReceivedAt = now },
                new PriorAuthorization { OutcomeReceivedAt = now, SentToPlanAt = now },
                new PriorAuthorization { OutcomeReceivedAt = now.AddDays(-1), SentToPlanAt = now.AddDays(-1) }
            };

            return new List<object[]>
            {
                new object[] { null, null },
                new object[] { new List<PriorAuthorization> { allItems[0] }, null },
                new object[] { new List<PriorAuthorization> { allItems[1] }, null },
                new object[] { new List<PriorAuthorization> { allItems[2] }, now },
                new object[] { new List<PriorAuthorization> { allItems[2], allItems[3] }, now }
            };
        }
    }
}
