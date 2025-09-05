using System;
using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound
{
    [ExcludeFromCodeCoverage]
    public class PatientEvent
    {
        public int IssueSequence { get; set; }
        public DateTime? CmmDataExtractedDate { get; set; }
        public DateTime? CmmDataProcessedDate { get; set; }
        public DateTime? PatientStatusChangeDate { get; set; }
        public string DrugName { get; set; }
        public string Ndc { get; set; }
        public string ProgramHeader { get; set; }
        public DateTime? TriageReceivedDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string PatientStatus { get; set; }
        public string WorkflowStatus { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DateAdded { get; set; }
        public string IssueStatus { get; set; }
        public string CmmPriorAuthStatus { get; set; }
    }
}
