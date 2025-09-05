using FluentAssertions;
using System.Collections.Generic;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Extensions;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound;
using Xunit;

namespace Wegmans.RX.Cmm.AzureFunctions.Tests.Astute.Extensions
{
    public class AstutePatientServiceExtensionsTests
    {
        [Theory]
        [MemberData(nameof(CreatePatientCaseData))]
        public void CreatePatientCase(AstutePatientService.Address astuteAddress, PatientCase expected)
        {
            var result = astuteAddress.CreatePatientCase();

            result.Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> CreatePatientCaseData()
        {
            var astuteAdresses = new Dictionary<string, AstutePatientService.Address>
            {
                { "Minimum Address", new AstutePatientService.Address { a05_code = "" } }
            };

            return new List<object[]>
            {
                new object[] { astuteAdresses["Minimum Address"], new PatientCase() }
            };
        }
    }
}
