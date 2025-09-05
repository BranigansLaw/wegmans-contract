using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class MedicalBenefitsPrimaryInsurance
    {
        [JsonPropertyName("memberId")]
        public string MemberId { get; set; }

        [JsonPropertyName("planName")]
        public string PlanName { get; set; }

        [JsonPropertyName("group")]
        public string Group { get; set; }

        [JsonPropertyName("isPaRequired")]
        public bool? IsPaRequired { get; set; }

        [JsonPropertyName("lineOfBusiness")]
        public string LineOfBusiness { get; set; }

        [JsonPropertyName("startDate")]
        public DateTimeOffset? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTimeOffset? EndDate { get; set; }

        [JsonPropertyName("benefits")]
        public Benefits Benefits { get; set; }
    }
}
