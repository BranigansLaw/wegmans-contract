using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class MedicalBenefits
    {
        [JsonPropertyName("primaryInsurance")]
        public MedicalBenefitsPrimaryInsurance PrimaryInsurance { get; set; }
    }
}
