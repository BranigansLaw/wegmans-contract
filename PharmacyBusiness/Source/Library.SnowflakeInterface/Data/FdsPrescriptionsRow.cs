namespace Library.SnowflakeInterface.Data
{
    public class FdsPrescriptionsRow
    {
        public required long Version { get; set; }

        public required long? Clientpatientid { get; set; }

        public required string? Lastname { get; set; }

        public required string? Firstname { get; set; }

        public required string Gender { get; set; }

        public required string? Addressline1 { get; set; }

        public required string? Addressline2 { get; set; }

        public required string? City { get; set; }

        public required string? State { get; set; }

        public required string? Zipcode { get; set; }

        public required string? Dob { get; set; }

        public required string? Email { get; set; }

        public required string? Phone { get; set; }

        public required string? Phonetype { get; set; }

        public required string? Language { get; set; }

        public required string? Ndc { get; set; }

        public required decimal Qty { get; set; }

        public required decimal Dayssupply { get; set; }

        public required string? Productname { get; set; }

        public required long? Filldate { get; set; }

        public required long? Fillnum { get; set; }

        public required decimal Authorizedrefills { get; set; }

        public required string? Npi { get; set; }

        public required string? Bin { get; set; }

        public required string? Pcn { get; set; }

        public required string? Groupid { get; set; }

        public required string? Plan { get; set; }

        public required string? Prescribernpi { get; set; }

        public required string? Patientmbi { get; set; }

        public required decimal? Copay { get; set; }

        public required decimal? Reimbursement { get; set; }
    }
}
