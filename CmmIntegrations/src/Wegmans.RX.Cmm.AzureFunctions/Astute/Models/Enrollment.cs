using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    [ExcludeFromCodeCoverage]
    public class Enrollment
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("sampleDate")]
        public DateTimeOffset SampleDate { get; set; }

        public Patient Patient { get; set; }

        public Case Case { get; set; }
    }
}
