using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    [ExcludeFromCodeCoverage]
    public class Provider
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Npi { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public Address Address { get; set; }
    }
}
