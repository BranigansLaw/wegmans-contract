namespace Library.SnowflakeInterface.Data
{
    public class SupplierPriceDrugFileRow
    {
        public required DateTime DateOfService { get; set; }

        public required string? SupplierName { get; set; }

        public required string? VendorItemNumber { get; set; }

        public required string? Ndc { get; set; }

        public required string? NdcWo { get; set; }

        public required string? DrugName { get; set; }

        public required string? DrugForm { get; set; }

        public required string? DeaClass { get; set; }

        public required string? Strength { get; set; }

        public required string? StrengthUnit { get; set; }

        public required string? GenericName { get; set; }

        public required decimal? PackSize { get; set; }

        public required string? IsMaintDrug { get; set; }

        public required string? Sdgi { get; set; }

        public required string? Gcn { get; set; }

        public required string? GcnSeqNumber { get; set; }

        public required string? DrugManufacturer { get; set; }

        public required string? OrangeBookCode { get; set; }

        public required decimal? PackPrice { get; set; }

        public required decimal? PricePerUnit { get; set; }

        public required DateTime? UnitPriceDate { get; set; }

        public required long? PurchIncr { get; set; }

        public required decimal? PkgSizeIncr { get; set; }

        public required string? Status { get; set; }

        public required DateTime? EffStartDate { get; set; }
    }
}
