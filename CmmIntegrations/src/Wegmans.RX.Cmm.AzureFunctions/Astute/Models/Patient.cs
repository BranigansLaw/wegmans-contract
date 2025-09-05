using System;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    public class Patient
    {
        public string Id { get; set; }
        public string JcpId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Address Address { get; set; }
        public Provider Provider { get; set; }
        public string Phone { get; set; }
    }
}