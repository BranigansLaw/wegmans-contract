namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    public class Copay
    {
        public bool? IsEnrolled { get; set; }

        public string MemberId { get; set; }

        public string AstuteIsEnrolled
        {
            get
            {
                string isEnrolled = string.Empty;
                if (this.IsEnrolled.HasValue)
                {
                    isEnrolled = this.IsEnrolled.Value ? "Yes" : "No";
                }
                return isEnrolled;
            }
        }
    }
}
