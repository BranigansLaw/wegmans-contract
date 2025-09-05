using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class Copay
    {
        [JsonPropertyName("isEnrolled")]
        public bool? IsEnrolled { get; set; }

        [JsonPropertyName("memberId")]
        public string MemberId { get; set; }
    }
}
