using System;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    public class PharmacyBenefitsPrimaryInsurance
    {
        public string Bin { get; set; }

        public string Pcn { get; set; }

        public string RxGroup { get; set; }

        public string MemberId { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public bool? IsPaRequired { get; set; }

        public string PharmacyNpi { get; set; }

        public string LineOfBusiness { get; set; }

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
