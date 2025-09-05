using System;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    public class MedicalBenefitsPrimaryInsurance
    {
        public string MemberId { get; set; }

        public string PlanName { get; set; }

        public string Group { get; set; }

        public bool? IsPaRequired { get; set; }

        public string LineOfBusiness { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public Benefits Benefits { get; set; }

        public string AstuteIsPaRequired
        {
            get
            {
                string isPaRequired = string.Empty;
                if (this.IsPaRequired.HasValue)
                {
                    isPaRequired = this.IsPaRequired.Value ? "Yes" : "No";
                }
                return isPaRequired;
            }
        }
    }
}
