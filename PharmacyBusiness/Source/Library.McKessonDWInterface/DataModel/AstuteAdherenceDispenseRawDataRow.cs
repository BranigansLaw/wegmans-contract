namespace Library.McKessonDWInterface.DataModel
{
    public class AstuteAdherenceDispenseRawDataRow
    {
        public required string? RxNumber { get; set; }
        public required DateTime? SoldDate { get; set; }
        public required string? FacilityId { get; set; }
        public required string? PatientLastName { get; set; }
        public required string? PatientFirstName { get; set; }
        public required int? RefillNumber { get; set; }
        public required string? ProductName { get; set; }
        public required decimal? Quantity { get; set; }
        public required double? DaysSupply { get; set; }
        public required decimal? TotalRefillsRemaining { get; set; }
        public required string? DrugNDC { get; set; }
        public required string? CardholderOrPatientId { get; set; }
        public required string? PlanCode { get; set; }
        public required string? ShipAddress1 { get; set; }
        public required string? ShipAddress2 { get; set; }
        public required string? ShipCity { get; set; }
        public required string? ShipState { get; set; }
        public required string? ShipZipCode { get; set; }
        public required DateTime? PatientDateOfBirth { get; set; }
        public required string? SoldTime { get; set; }
        public required DateTime? DateOfService { get; set; }
        public required string? PersonCode { get; set; }
        public required decimal? RxFillSequence { get; set; }
        public required DateTime? WrittenDate { get; set; }
        public required string? PrescriberFirstName { get; set; }
        public required string? PrescriberLastName { get; set; }
        public required string? PrescriberNpi { get; set; }
        public required string? PrescriberDea { get; set; }
        public required string? PrescriberAddress1 { get; set; }
        public required string? PrescriberAddress2 { get; set; }
        public required string? PrescriberCity { get; set; }
        public required string? PrescriberState { get; set; }
        public required string? PrescriberZip { get; set; }
        public required string? PrescriberPhone { get; set; }
        public required string? PatientNum { get; set; }
        public required decimal? PatientPricePaid { get; set; }
        public required string? CourierName { get; set; }
        public required string? TrackingNumber { get; set; }
        public required decimal? TotalRefillsAllowed { get; set; }
        public required string? OrderNumber { get; set; }
        public required string? PatientGroupName { get; set; }
    }
}
