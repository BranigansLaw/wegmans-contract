using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class CaseStatusPatient
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("jcpId")]
        public string JcpId { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public DateTimeOffset? DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("primaryPhone")]
        public string PrimaryPhone { get; set; }

        [JsonPropertyName("address")]
        public Address Address { get; set; }
    }
}
