using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class Enrollment
    {
        [JsonPropertyName("patient")]
        public EnrollmentPatient Patient { get; set; }
    }
}
