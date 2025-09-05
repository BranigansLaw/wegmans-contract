namespace Library.SnowflakeInterface.Data
{
    public class WorkersCompMonthly
    {
        public required string? FacilityIdNumber { get; set; }

        public required string? RxNumber { get; set; }

        public required string? DateFilled { get; set; }

        public required string? PatientLastName { get; set; }

        public required string? PatientFirstName { get; set; }

        public required string? CardholderId { get; set; }

        public required string? PatientDob { get; set; }

        public required string? DrugNdcNumber { get; set; }

        public required string? DrugName { get; set; }

        public required long? QtyDrugDispensed { get; set; }

        public required decimal TxPrice { get; set; }

        public required decimal PatientPay { get; set; }

        public required decimal AdjAmt { get; set; }

        public required string? ThirdPartyName { get; set; }

        public required string? ThirdPartyCode { get; set; }

        public required string SplitBillIndicator { get; set; }

        public required string? PrescriberName { get; set; }

        public required DateTime? DateSold { get; set; }

        public required long ClaimNumber { get; set; }
    }
}
