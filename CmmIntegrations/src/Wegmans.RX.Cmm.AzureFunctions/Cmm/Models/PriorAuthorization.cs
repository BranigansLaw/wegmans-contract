using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class PriorAuthorization
    {
        [JsonPropertyName("sentToPlanAt")]
        public DateTimeOffset? SentToPlanAt { get; set; }

        [JsonPropertyName("isApproved")]
        public bool? IsApproved { get; set; }

        [JsonPropertyName("isDenied")]
        public bool? IsDenied { get; set; }

        [JsonPropertyName("outcomeReceivedAt")]
        public DateTimeOffset? OutcomeReceivedAt { get; set; }

        [JsonPropertyName("authorizationPeriod")]
        public AuthorizationPeriod AuthorizationPeriod { get; set; }

        [JsonPropertyName("planOutcome")]
        public string PlanOutcome { get; set; }
    }
}
