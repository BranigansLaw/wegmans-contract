using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class Benefits
    {
        [JsonPropertyName("coinsurance")]
        public string Coinsurance { get; set; }

        [JsonPropertyName("copay")]
        public string Copay { get; set; }

        [JsonPropertyName("deductible")]
        public string Deductible { get; set; }

        [JsonPropertyName("deductibleMet")]
        public bool? DeductibleMet { get; set; }

        [JsonPropertyName("deductibleRemaining")]
        public string DeductibleRemaining { get; set; }

        [JsonPropertyName("oopMax")]
        public string OopMax { get; set; }

        [JsonPropertyName("oopMaxMet")]
        public bool? OopMaxMet { get; set; }

        [JsonPropertyName("oopMaxRemaining")]
        public string OopMaxRemaining { get; set; }
    }
}
