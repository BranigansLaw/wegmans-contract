using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class PharmacyBenefitsPrimaryInsurance
    {
        [JsonPropertyName("bin")]
        public string Bin { get; set; }

        [JsonPropertyName("pcn")]
        public string Pcn { get; set; }

        [JsonPropertyName("rxGroup")]
        public string RxGroup { get; set; }

        [JsonPropertyName("memberId")]
        public string MemberId { get; set; }

        [JsonPropertyName("startDate")]
        public DateTimeOffset? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTimeOffset? EndDate { get; set; }

        [JsonPropertyName("isPaRequired")]
        public bool? IsPaRequired { get; set; }

        [JsonPropertyName("pharmacyNpi")]
        public string PharmacyNpi { get; set; }

        [JsonPropertyName("lineOfBusiness")]
        public string LineOfBusiness { get; set; }

        [JsonPropertyName("benefits")]
        public Benefits Benefits { get; set; }
    }
}
