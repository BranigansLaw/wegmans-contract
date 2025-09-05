using Library.TenTenInterface.DataModel.UploadRow;

namespace Library.SnowflakeInterface.Data
{
    public class DurConflictTenTenRow : IAzureBlobUploadRow
    {
        /// <inheritdoc />
        public string FeedName => "wegmans.wegmansdata.dwfeeds.durconflict";

        public required string? StoreNumber { get; set; }

        public required string? RxNumber { get; set; }

        public required long? RefillNumber { get; set; }

        public required long? PartSeqNumber { get; set; }

        public required DateTime? DurDate { get; set; }

        public required long? PatientNumber { get; set; }

        public required string? NdcWo { get; set; }

        public required string? DrugName { get; set; }

        public required string? Sdgi { get; set; }

        public required string? Gcn { get; set; }

        public required string? GcnSequenceNumber { get; set; }

        public required string? DeaClass { get; set; }

        public required string? ConflictCode { get; set; }

        public required string? ConflictDesc { get; set; }

        public required string? ConflictType { get; set; }

        public required string? SeverityDesc { get; set; }

        public required string? ResultOfService { get; set; }

        public required string? ProfService { get; set; }

        public required long? LevelOfEffort { get; set; }

        public required string? ReasonForService { get; set; }

        public required string? IsCritical { get; set; }

        public required string? IsException { get; set; }

        public required long? RxFillSequence { get; set; }

        public required long? RxRecordNumber { get; set; }

        public required long? PrescriberKey { get; set; }

        public required long? UserKey { get; set; }
    }
}
