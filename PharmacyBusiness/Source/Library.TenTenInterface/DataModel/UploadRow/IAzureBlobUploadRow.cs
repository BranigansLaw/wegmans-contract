namespace Library.TenTenInterface.DataModel.UploadRow
{
    public interface IAzureBlobUploadRow
    {
        /// <summary>
        /// The name of the feed to upload to
        /// </summary>
        string FeedName { get; }
    }
}
