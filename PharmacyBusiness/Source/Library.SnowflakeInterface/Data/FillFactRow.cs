namespace Library.SnowflakeInterface.Data
{
    public class FillFactRow
    {
        public required long FillFactKey { get; set; }

        public required string Source { get; set; }

        public required long OrderDateKey { get; set; }

        public required decimal RefillQuantity { get; set; }

        public required decimal? FullPackageUnc { get; set; }
    }
}
