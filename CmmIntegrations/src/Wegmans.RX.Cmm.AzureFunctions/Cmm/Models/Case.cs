using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class Case
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("referralId")]
        public string ReferralId { get; set; }

        [JsonPropertyName("ndc")]
        public string Ndc { get; set; }

        [JsonPropertyName("diagnosisCode")]
        public string DiagnosisCode { get; set; }

        [JsonPropertyName("programName")]
        public string ProgramName { get; set; }

        [JsonPropertyName("shipStarterDoseToPatient")]
        public bool? ShipStarterDoseToPatient { get; set; }

        [JsonPropertyName("shipStarterDoseToProvider")]
        public bool? ShipStarterDoseToProvider { get; set; }

        [JsonPropertyName("shipStarterDoseToInfusionClinic")]
        public bool? ShipStarterDoseToInfusionClinic { get; set; }

        [JsonPropertyName("infusionInductionDoseDate")]
        public string InfusionInductionDoseDate { get; set; }

        [JsonPropertyName("initialInfusionMaintenanceDoseDate")]
        public string InitialInfusionMaintenanceDoseDate { get; set; }

        [JsonPropertyName("provider")]
        public Provider Provider { get; set; }
    }
}
