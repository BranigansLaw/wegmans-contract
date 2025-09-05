namespace Library.SnowflakeInterface.Data
{
    public class SoldDetailRow
    {
        public required string? StoreNumber { get; set; }

        public required string? RxNumber { get; set; }

        public required long? RefillNumber { get; set; }

        public required long PartialFillSequenceNumber { get; set; }

        public required long? SoldDate { get; set; }

        public required string? OrderNumber { get; set; }

        public required decimal? QuantityDispensed { get; set; }

        public required string? Ndc { get; set; }

        public required decimal? AcquisitionCost { get; set; }

        public required decimal? ThirdPartyPricePaid { get; set; }

        public required decimal? PatientPricePaid { get; set; }

        public required decimal? TotalPricePaid { get; set; }
    }
}
