using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class ServiceRequestEvent
    {
        [JsonPropertyName("type_name")]
        public string TypeName { get; set; }

        [JsonPropertyName("source_identifier")]
        public string SourceIdentifier { get; set; }

        [JsonPropertyName("source_type")]
        public string SourceType { get; set; }

        [JsonPropertyName("data")]
        public PatientStatusData Data { get; set; }
    }
}
