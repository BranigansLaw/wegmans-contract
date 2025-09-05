using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class PharmacyBenefits
    {
        [JsonPropertyName("eagleForce")]
        public EagleForce EagleForce { get; set; }

        [JsonPropertyName("primaryInsurance")]
        public PharmacyBenefitsPrimaryInsurance PrimaryInsurance { get; set; }
    }
}
