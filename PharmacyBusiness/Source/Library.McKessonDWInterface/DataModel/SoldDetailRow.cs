namespace Library.McKessonDWInterface.DataModel
{
    /// <summary>
    /// The properties listed here related to data types and data requirements within McKesson DW data source rather than from data destination (1010data).
    /// </summary>
    public class SoldDetailRow
    {
        public required int? StoreNbr { get; set; }
        public required int? RxNbr { get; set; }
        public required int? RefillNbr { get; set; }
        public required int? PartialFillSequenceNbr { get; set; }
        public required DateTime? SoldDate { get; set; }
        public required string? OrderNbr { get; set; }
        public required decimal? QtyDispensed { get; set; }
        public required string? NdcWithoutDashes { get; set; }
        public required decimal? AcquisitionCost { get; set; }
        public required decimal? ThirdPartyPay { get; set; }
        public required decimal? PatientPay { get; set; }
        public required decimal? TransactionPrice { get; set; }
    }
}
