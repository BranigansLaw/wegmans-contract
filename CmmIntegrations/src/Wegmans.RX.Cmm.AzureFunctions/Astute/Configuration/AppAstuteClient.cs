using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AppAstuteClient
    {
        public string AddressUrl { get; set; }

        public string CaseUrl { get; set; }

        public string CaseStreamUrl { get; set; }
    }
}
