using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class EnrollmentXarelto
    {
        [JsonPropertyName("patient")]
        public EnrollmentXareltoPatient Patient { get; set; }
    }
}
