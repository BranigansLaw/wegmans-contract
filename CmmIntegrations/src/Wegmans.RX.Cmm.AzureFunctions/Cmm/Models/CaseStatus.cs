using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class CaseStatus
    {
        [JsonPropertyName("caseId")]
        public string CaseId { get; set; }

        [JsonPropertyName("isCaseClosed")]
        public bool? IsCaseClosed { get; set; }

        [JsonPropertyName("caseClosureReason")]
        public string CaseClosureReason { get; set; }

        [JsonPropertyName("ndc")]
        public string Ndc { get; set; }

        [JsonPropertyName("patient")]
        public CaseStatusPatient Patient { get; set; }

        [JsonPropertyName("priorAuthorization")]
        public PriorAuthorization PriorAuthorization { get; set; }

        [JsonPropertyName("priorAuthorizationAppeals")]
        public IEnumerable<PriorAuthorization> PriorAuthorizationAppeals { get; set; }

        [JsonPropertyName("pharmacyBenefits")]
        public PharmacyBenefits PharmacyBenefits { get; set; }

        [JsonPropertyName("medicalBenefits")]
        public MedicalBenefits MedicalBenefits { get; set; }

        [JsonPropertyName("copay")]
        public Copay Copay { get; set; }

        [JsonPropertyName("linkStatus")]
        public string LinkStatus { get; set; }

        [JsonPropertyName("linkStatusReasons")]
        public IEnumerable<string> LinkStatusReasons { get; set; }

        [JsonPropertyName("linkEligibilityEndDate")]
        public DateTimeOffset? LinkEligibilityEndDate { get; set; }

        [JsonPropertyName("reverificationStartDate")]
        public DateTimeOffset? ReverificationStartDate { get; set; }

        [JsonPropertyName("reverificationReason")]
        public string ReverificationReason { get; set; }
    }
}
