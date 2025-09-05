namespace Library.SnowflakeInterface.Data
{
    public class FdsPharmaciesRow
    {
        public required string? NcpdpId { get; set; }

        public required string? Npi { get; set; }

        public required string? PharmacyName { get; set; }

        public required string? AddressLine1 { get; set; }

        public required string? AddressLine2 { get; set; }

        public required string? City { get; set; }

        public required string? State { get; set; }

        public required string? ZipCode { get; set; }

        public required string? StorePhoneNumber { get; set; }

        public required string? FaxPhoneNumber { get; set; }

        public required string? Banner { get; set; }
    }
}
