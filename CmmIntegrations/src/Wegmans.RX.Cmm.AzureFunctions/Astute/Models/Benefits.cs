using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    [ExcludeFromCodeCoverage]
    public class Benefits
    {
        public string Coinsurance { get; set; }

        public string Copay { get; set; }

        public string Deductible { get; set; }

        public bool? DeductibleMet { get; set; }

        public string DeductibleRemaining { get; set; }

        public string OopMax { get; set; }

        public bool? OopMaxMet { get; set; }

        public string OopMaxRemaining { get; set; }
    }
}
