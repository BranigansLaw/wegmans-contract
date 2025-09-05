using System;
using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    [ExcludeFromCodeCoverage]
    public class PatientStatus
    {
        public string TypeName { get; set; } = "patient_status_update";
        public string SourceIdentifier { get; set; }
        public string SourceType { get; set; } = "Free Goods";
        public string PatientStatusCode { get; set; }
        public DateTime? LastShipmentDate { get; set; }
        public string TransferPharmacyNpi { get; set; }
        public string TransferPharmacyName { get; set; }
        public string TransferPharmacyPhoneNumber { get; set; }
        public string LinkEnrollmentFlag { get; set; }
        public DateTime? LinkEnrollmentDate { get; set; }
        public DateTime? ReverificationStartDate { get; set; }
        public string PrimaryPbmName { get; set; }
        public string PrimaryPbmBin { get; set; }
        public string PrimaryPbmPcn { get; set; }
        public string PrimaryPbmGroupId { get; set; }
        public string PrimaryPbmPlanId { get; set; }
        public string AstuteCaseId { get; set; }
        public string AstuteIssueSequence { get; set; }
    }
}
