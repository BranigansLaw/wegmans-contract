namespace Library.TenTenInterface.ParquetFileGeneration
{
    public interface IParquetHelper
    {
        /// <summary>
        /// Returns the TenTen Azure blob file stream if <see cref="TenTenConfig.DeveloperMode"/> is not true, a file upload stream otherwise
        /// </summary>
        /// <returns></returns>
        Task<Stream> GetParquetUploadStreamAsync(string feedName, string fileName, CancellationToken c);

        /// <summary>
        /// Serialized <paramref name="toSerialize"/> to the stream <paramref name="writeSteam"/>
        /// </summary>
        Task WriteParquetToStreamAsync<T>(IEnumerable<T> toSerialize, Stream writeSteam, CancellationToken c);
    }
}
