using System;
using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    [ExcludeFromCodeCoverage]
    public class CaseStatusPatient
    {
        public string Id { get; set; }

        public string JcpId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string PrimaryPhone { get; set; }

        public Address Address { get; set; }
    }
}
