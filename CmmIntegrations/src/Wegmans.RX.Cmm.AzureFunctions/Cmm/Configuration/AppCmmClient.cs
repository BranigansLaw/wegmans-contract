using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AppCmmClient
    {
        public string BaseUrl { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
