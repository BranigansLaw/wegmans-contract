namespace Library.SnowflakeInterface.Data
{
    public class WegmansHPOnePrescriptionsRow
    {
        public required long Version { get; set; }

        public required long? ClientPatientId { get; set; }

        public required string? LastName { get; set; }

        public required string? FirstName { get; set; }

        public required string Gender { get; set; }

        public required string? AddressLine1 { get; set; }

        public required string? AddressLine2 { get; set; }

        public required string? City { get; set; }

        public required string? State { get; set; }

        public required string? Zipcode { get; set; }

        public required string? Dob { get; set; }

        public required string? Email { get; set; }

        public required string? Phone { get; set; }

        public required string? PhoneType { get; set; }

        public required string? Language { get; set; }

        public required string? Ndc { get; set; }

        public required decimal Qty { get; set; }

        public required decimal DaysSupply { get; set; }

        public required string? ProductName { get; set; }

        public required long? FillDate { get; set; }

        public required long? FillNum { get; set; }

        public required decimal AuthorizedRefills { get; set; }

        public required string? Npi { get; set; }

        public required string? MedicareContractId { get; set; }

        public required string? MedicarePlanId { get; set; }

        public required string? Bin { get; set; }

        public required string? Pcn { get; set; }

        public required string? GroupId { get; set; }

        public required string? Plan { get; set; }

        public required string? PrescriberNpi { get; set; }

        public required string? Medicared { get; set; }

        public required string? MedicareId { get; set; }
    }
}
