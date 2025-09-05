namespace Library.SnowflakeInterface.Data
{
    public class PatientAddressesRow
    {
        public required long PadrKey { get; set; }

        public required long? PatientNum { get; set; }

        public required string? AddressOne { get; set; }

        public required string? AddressTwo { get; set; }

        public required string? AddrCity { get; set; }

        public required string? AddrState { get; set; }

        public required string? AddrZipcode { get; set; }

        public required string? AddressZipext { get; set; }

        public required string? County { get; set; }

        public required string? AddressType { get; set; }

        public required string? AddressUsage { get; set; }

        public required string? AddrStatus { get; set; }

        public required string? AreaCode { get; set; }

        public required string? PhoneNum { get; set; }

        public required string? PhoneExt { get; set; }

        public required long AddressUpdated { get; set; }

        public required long PhoneUpdated { get; set; }

        public required long? Dwaddressnum { get; set; }

        public required long? Dwphonenum { get; set; }
    }
}
