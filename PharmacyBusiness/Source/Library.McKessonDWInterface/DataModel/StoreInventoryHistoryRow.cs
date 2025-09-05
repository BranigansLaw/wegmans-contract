namespace Library.McKessonDWInterface.DataModel
{
    /// <summary>
    /// The properties listed here related to data types and data requirements within McKesson DW data source rather than from data destination (1010data).
    /// </summary>
    public class StoreInventoryHistoryRow
    {
        public required DateTime? DateOfService { get; set; }
        public required int? StoreNbr { get; set; }
        public required string? NdcWithoutDashes { get; set; }
        public required string? DrugName { get; set; }
        public required string? Sdgi { get; set; }
        public required string? Gcn { get; set; }
        public required decimal? GcnSequence { get; set; }
        public required string? OrangeBookCode { get; set; }
        public required string? FormCode { get; set; }
        public required decimal? PackSize { get; set; }
        public required decimal? TruePack { get; set; }
        public required string? PM { get; set; }
        public required string? IsPreferred { get; set; }
        public required decimal? LastAcquisitionCostPack { get; set; }
        public required decimal? LastAcquisitionCostUnit { get; set; }
        public required DateTime? LastAcquisitionCostDate { get; set; }
        public required decimal? OnHandQty { get; set; }
        public required decimal? OnHandValue { get; set; }
        public required decimal? ComittedQty { get; set; }
        public required decimal? ComittedValue { get; set; }
        public required decimal? TotalQTY { get; set; }
        public required decimal? TotalValue { get; set; }
        public required decimal? AcquisitionCostPack { get; set; }
        public required decimal? AcquisitionCostUnit { get; set; }
        public required string? PrimarySupplier { get; set; }
        public required DateTime? LastChangeDate { get; set; }
    }
}
