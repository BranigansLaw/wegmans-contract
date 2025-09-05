using Parquet.Serialization.Attributes;

namespace Library.TenTenInterface.DataModel.UploadRow.Implementation
{
    public class TestTenTenRow : IAzureBlobUploadRow
    {
        [ParquetIgnore]
        public string FeedName => "wegmans.devpharm.test.test_feed";

        [ParquetRequired]
        public required int Id { get; set; }

        [ParquetIgnore]
        public string? DontWrite { get; set; }

        [ParquetRequired]
        public required float Price { get; set; }

        [ParquetRequired]
        public required string Message { get; set; }

        // ParquetSimpleRepeatable not supported by TenTen
        //[ParquetSimpleRepeatable]
        //public required IEnumerable<int> RelatedIds { get; set; }
    }
}
