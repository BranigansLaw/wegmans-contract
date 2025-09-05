namespace Library.LibraryUtilities.EmailSender
{
    public interface IEmailNotificationHelper
    {
        /// <summary>
        /// Notifies of parquet upload completion.
        /// </summary>
        /// <param name="environmentFolderName"></param>
        /// <param name="feedName"></param>
        /// <param name="runDate"></param>
        /// <param name="uploadRowCount"></param>
        /// <param name="emailTo"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task NotifyOfParquetUploadAsync(
            string environmentFolderName,
            string feedName,
            DateOnly runDate,
            int uploadRowCount,
            string emailTo,
            CancellationToken c);
    }
}
