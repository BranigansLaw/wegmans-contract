using System;
using System.Collections.Generic;
using System.Linq;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    public class AdditionalDetails
    {
        public IEnumerable<PriorAuthorization> PriorAuthorizationAppeals { get; set; }

        public PharmacyBenefits PharmacyBenefits { get; set; }

        public MedicalBenefits MedicalBenefits { get; set; }

        public Copay Copay { get; set; }

        public DateTimeOffset? AppealSentToPlanAt
        {
            get
            {
                return this.PriorAuthorizationAppeals?.FirstOrDefault(x => x.OutcomeReceivedAt == this.PriorAuthorizationAppeals?.Max(y => y.OutcomeReceivedAt))?.SentToPlanAt;
            }
        }

        public string AppealPaStatus
        {
            get
            {
                return this.PriorAuthorizationAppeals?.FirstOrDefault(x => x.OutcomeReceivedAt == this.PriorAuthorizationAppeals?.Max(y => y.OutcomeReceivedAt))?.AppealPaStatus;
            }
        }

        public string AppealCmmPaStatus
        {
            get
            {
                return this.PriorAuthorizationAppeals?.FirstOrDefault(x => x.OutcomeReceivedAt == this.PriorAuthorizationAppeals?.Max(y => y.OutcomeReceivedAt))?.CmmPaStatus;
            }
        }
    }
}
