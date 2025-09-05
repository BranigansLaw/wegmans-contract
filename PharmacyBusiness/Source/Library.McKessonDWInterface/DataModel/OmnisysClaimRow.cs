namespace Library.McKessonDWInterface.DataModel
{
    public class OmnisysClaimRow
    {
        public required string PharmacyNpi { get; set; }

        public required string PrescriptionNbr { get; set; }

        public required string RefillNumber { get; set; }

        public required DateOnly SoldDate { get; set; }

        public required DateOnly DateOfService { get; set; }

        public required string NdcNumber { get; set; }

        public required string CardholderId { get; set; }

        public required string? AuthorizationNumber { get; set; }

        public required string? ReservedForFutureUse { get; set; }
    }
}
