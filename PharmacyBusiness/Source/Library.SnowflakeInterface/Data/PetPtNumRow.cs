namespace Library.SnowflakeInterface.Data
{
    public class PetPtNumRow
    {
        public required long? PatientNum { get; set; }

        public required string? Species { get; set; }

        public required DateTime? CreateDate { get; set; }

        public required string Pet { get; set; }
    }
}
