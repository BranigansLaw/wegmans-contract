using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class EagleForce
    {
        [JsonPropertyName("hasGovernmentInsurance")]
        public bool? HasGovernmentInsurance { get; set; }
    }
}
