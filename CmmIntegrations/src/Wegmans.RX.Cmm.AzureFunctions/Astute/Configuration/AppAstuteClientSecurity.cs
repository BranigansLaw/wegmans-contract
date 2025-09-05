using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AppAstuteClientSecurity
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
