namespace Library.SnowflakeInterface.Data
{
    public class PrescriberRow
    {
        public required long PrescriberKey { get; set; }

        public required long? PrescriberNum { get; set; }

        public required long? ActivePrescriberNum { get; set; }

        public required string? TitleAbbr { get; set; }

        public required string? FirstName { get; set; }

        public required string? MiddleName { get; set; }

        public required string? LastName { get; set; }

        public required string? SuffixAbbr { get; set; }

        public required string? GenderCode { get; set; }

        public required string? Status { get; set; }

        public required string? AmaActivity { get; set; }

        public required string? InactiveCode { get; set; }

        public required string? PrefGeneric { get; set; }

        public required string? PrefTherSub { get; set; }

        public required long? CreateUserKey { get; set; }

        public required string AddSource { get; set; }

        public required long? SupvPrsNum { get; set; }

        public required long? UniqPrsNum { get; set; }

        public required string? PrescriberId { get; set; }

        public required string? EsvMatch { get; set; }

        public required string? IsEsvValid { get; set; }

        public required DateTime? CreateDate { get; set; }

        public required DateTime? BirthDate { get; set; }

        public required DateTime? DeceasedDate { get; set; }

        public required DateTime? InactiveDate { get; set; }

        public required string? PpiEnabled { get; set; }

        public required string? Npi { get; set; }

        public required string NpiBilling { get; set; }

        public required DateTime? NpiExpireDate { get; set; }

        public required string? StateLicNum { get; set; }

        public required string StateLicBilling { get; set; }

        public required string? LicenseState { get; set; }

        public required DateTime? StateLicExpireDate { get; set; }

        public required string? DeaNum { get; set; }

        public required string DeaBilling { get; set; }

        public required DateTime? DeaExpireDate { get; set; }

        public required string? MedicaidNum { get; set; }

        public required string? FedtaxidNum { get; set; }

        public required string? StateissueNum { get; set; }

        public required string? NarcdeaNum { get; set; }

        public required string? NcpdpNum { get; set; }

        public required DateTime? LastUpdate { get; set; }
    }
}
