using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Models
{
    [ExcludeFromCodeCoverage]
    public class PatientStatusData
    {
        [JsonPropertyName("patient_status")]
        public string PatientStatus { get; set; }

        [JsonPropertyName("last_shipment_date")]
        public DateTimeOffset? LastShipmentDate { get; set; }

        [JsonPropertyName("transfer_pharmacy_npi")]
        public string TransferPharmacyNpi { get; set; }

        [JsonPropertyName("transfer_pharmacy_name")]
        public string TransferPharmacyName { get; set; }

        [JsonPropertyName("transfer_pharmacy_phone_number")]
        public string TransferPharmacyPhoneNumber { get; set; }

        [JsonPropertyName("link_enrollment_flag")]
        public string LinkEnrollmentFlag { get; set; }

        [JsonPropertyName("link_enrollment_date")]
        public DateTimeOffset? LinkEnrollmentDate { get; set; }

        [JsonPropertyName("reverification_start_date")]
        public DateTimeOffset? ReverificationStartDate { get; set; }

        [JsonPropertyName("primary_pbm_name")]
        public string PrimaryPbmName { get; set; }
        
        [JsonPropertyName("primary_pbm_bin")]
        public string PrimaryPbmBin { get; set; }

        [JsonPropertyName("primary_pbm_pcn")]
        public string PrimaryPbmPcn { get; set; }

        [JsonPropertyName("primary_pbm_group_id")]
        public string PrimaryPbmGroupId { get; set; }

        [JsonPropertyName("primary_pbm_plan_id")]
        public string PrimaryPbmPlanId { get; set; }
    }
}
