namespace Library.SnowflakeInterface.Data
{
    public class PrescriberAddressRow
    {
        public required long? PrescriberKey { get; set; }

        public required long PadrKey { get; set; }

        public required long PrescribAddrNum { get; set; }

        public required string? AddrType { get; set; }

        public required string? AddressOne { get; set; }

        public required string? AddressTwo { get; set; }

        public required string? AddrCity { get; set; }

        public required string? Email { get; set; }

        public required string? WebAddr { get; set; }

        public required string? State { get; set; }

        public required string? Zipcode { get; set; }

        public required string? ZipExt { get; set; }

        public required string? County { get; set; }

        public required string IsDefault { get; set; }

        public required string AddSource { get; set; }

        public required string? DrugSched { get; set; }

        public required string? PracticeName { get; set; }

        public required long? AddrIdNum { get; set; }

        public required string? PrefContact { get; set; }

        public required string? AddrStatus { get; set; }

        public required string? Npi { get; set; }

        public required string NpiBilling { get; set; }

        public required DateTime? NpiExpireDate { get; set; }

        public required string? StateLicNum { get; set; }

        public required string StateLicBilling { get; set; }

        public required DateTime? StateLicExpireDate { get; set; }

        public required string? LicenseState { get; set; }

        public required string? DeaNum { get; set; }

        public required string DeaBilling { get; set; }

        public required DateTime? DeaExpireDate { get; set; }

        public required string? AreaCodePrim { get; set; }

        public required string? PhoneNumPrim { get; set; }

        public required string? ExtPrim { get; set; }

        public required string? AreaCodeSec { get; set; }

        public required string? PhoneNumSec { get; set; }

        public required string? ExtSec { get; set; }

        public required string? AreaCodeFax { get; set; }

        public required string? PhoneNumFax { get; set; }

        public required string? ExtFax { get; set; }

        public required string? AreaCodeOther { get; set; }

        public required string? PhoneNumOther { get; set; }

        public required string? ExtOther { get; set; }

        public required string? PhoneTypeOther2 { get; set; }

        public required string? AreaCodeOther2 { get; set; }

        public required string? PhoneNumOther2 { get; set; }

        public required string? ExtOther2 { get; set; }

        public required DateTime LastUpdate { get; set; }
    }
}
