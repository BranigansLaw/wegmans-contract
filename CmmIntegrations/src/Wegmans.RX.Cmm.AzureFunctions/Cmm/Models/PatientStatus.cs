using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class PatientStatus
    {
        [JsonPropertyName("service_request_event")]
        public ServiceRequestEvent ServiceRequestEvent { get; set; }
    }
}
