using System;
using System.Collections.Generic;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    public class PriorAuthorization
    {
        private static readonly Dictionary<string, string> PaStatusMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Favorable", "PA-Approved" },
            { "Unfavorable", "PA-Denied" },
            { "Cancelled", "PA-Cancelled" },
            { "Pending", "PA-Pending" },
            { "Unknown", "PA-Determination Not Made" },
            { "Unsent", "PA-Not Submitted" },
            { "N/A", "PA-Not Required" }
        };

        private static readonly Dictionary<string, string> AppealPaStatusMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Favorable", "Approved" },
            { "Unfavorable", "Denied" },
            { "Cancelled", "Cancelled" },
            { "Pending", "Pending" },
            { "Unknown", "No PA Determination Made" },
            { "Unsent", "Cancelled" },
            { "N/A", "Not Required" }
        };

        public DateTimeOffset? SentToPlanAt { get; set; }

        public bool? IsApproved { get; set; }

        public bool? IsDenied { get; set; }

        public DateTimeOffset? OutcomeReceivedAt { get; set; }

        public AuthorizationPeriod AuthorizationPeriod { get; set; }

        public string PlanOutcome { get; set; }

        public string CmmPaStatus
        {
            get
            {
                string isApproved = string.Empty;
                string isDenied = string.Empty;
                if (this.IsApproved.HasValue)
                {
                    isApproved = this.IsApproved.Value ? "Yes" : "No";
                }
                if (this.IsDenied.HasValue)
                {
                    isDenied = this.IsDenied.Value ? "Yes" : "No";
                }
                return $"Is Approved: {isApproved}, Is Denied: {isDenied}";
            } 
        }

        public string PaStatus
        {
            get
            {
                if (this.PlanOutcome is null)
                {
                    return "PA-Not Submitted";
                }
                else if (PaStatusMap.ContainsKey(this.PlanOutcome))
                {
                    return PaStatusMap[this.PlanOutcome];
                }
                else
                {
                    return this.PlanOutcome;
                }
            }
        }

        public string AppealPaStatus
        {
            get
            {
                if (this.PlanOutcome is null)
                {
                    return "Not Submitted";
                }
                else if (AppealPaStatusMap.ContainsKey(this.PlanOutcome))
                {
                    return AppealPaStatusMap[this.PlanOutcome];
                }
                else
                {
                    return this.PlanOutcome;
                }
            }
        }
    }
}
