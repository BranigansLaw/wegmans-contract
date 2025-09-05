namespace Library.SnowflakeInterface.Data
{
    public class SelectStoreInventoryHistoryRow
    {
        public required DateTime? DateOfService { get; set; }

        public required string? StoreNum { get; set; }

        public required string? NdcWithoutDashes { get; set; }

        public required string? DrugName { get; set; }

        public required string? Sdgi { get; set; }

        public required string? Gcn { get; set; }

        public required string? GcnSeqNum { get; set; }

        public required string? OrangeBookCode { get; set; }

        public required string? FormCode { get; set; }

        public required decimal? PackSize { get; set; }

        public required decimal? TruePack { get; set; }

        public required string? Pm { get; set; }

        public required string? IsPreferred { get; set; }

        public required decimal? LastAcqCostPack { get; set; }

        public required decimal? LastAcqCostUnit { get; set; }

        public required DateTime? LastAcqCostDate { get; set; }

        public required decimal? OnHandQty { get; set; }

        public required decimal? OnHandValue { get; set; }

        public required decimal? CommitedQty { get; set; }

        public required decimal? CommitedValue { get; set; }

        public required decimal? TotalQty { get; set; }

        public required decimal? TotalValue { get; set; }

        public required decimal? AcqCostPack { get; set; }

        public required decimal? AcqCostUnit { get; set; }

        public required string? PrimarySupplier { get; set; }

        public required DateTime? LastChangeDate { get; set; }
    }
}
