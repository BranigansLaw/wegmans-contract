using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound
{
    [ExcludeFromCodeCoverage]
    public class PatientCase
    {
        public int CaseId { get; set; }
        public int PatientId { get; set; }
        public string CmmPatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string PrimaryPhone { get; set; }
        public string PrescriberFirstName { get; set; }
        public string PrescriberLastName { get; set; }
        public string PrescriberNpi { get; set; }
        public string PrescriberPhone { get; set; }
        public string PrescriberFax { get; set; }
        public string PrescriberAddress1 { get; set; }
        public string PrescriberAddress2 { get; set; }
        public string PrescriberCity { get; set; }
        public string PrescriberState { get; set; }
        public string PrescriberZipCode { get; set; }
        public string ShipFirstDoseToPrescriber { get; set; }
        public string PayerType { get; set; }
        public string ProgramType { get; set; }
        public string PriorAuthSource { get; set; }
        public IEnumerable<PatientEvent> PatientEvents { get; set; }
        public DateTime? CaseChangedDate { get; set; }
        public string ExceptionReason { get; set; }
        public string TriageSource { get; set; }
        public string CmmCaseId { get; set; }
        public string CmmReferralId { get; set; }
        public string DiagnosisCode { get; set; }
        public string LinkEnrollment { get; set; }
        public string IsCaseClosed { get; set; }
        public string CaseClosureReason { get; set; }
        public string SentToPlanAt { get; set; }
        public DateTimeOffset? OutcomeReceivedAt { get; set; }
        public DateTimeOffset? AppealSentToPlanAt { get; set; }
        public string AppealCmmPriorAuthStatus { get; set; }
        public string AppealPriorAuthStatus { get; set; }
        public string SpecialtyPharmacyNpi { get; set; }
        public string SpecialtyPharmacyName { get; set; }
        public string SpecialtyPharmacyPhone { get; set; }
        public string GeCheck { get; set; }
        public string PriorAuthStatus { get; set; }
        public string LinkStatus { get; set; }
        public string LinkStatusReason1 { get; set; }
        public string LinkStatusReason2 { get; set; }
        public string JcpId { get; set; }
        public string ExtendedPatientIdList { get; set; }
        public DateTime? ReverificationStartDate { get; set; }
    }
}
