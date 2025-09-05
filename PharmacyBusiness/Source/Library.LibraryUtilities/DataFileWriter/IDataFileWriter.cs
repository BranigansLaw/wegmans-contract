namespace Library.LibraryUtilities.DataFileWriter
{
    public interface IDataFileWriter
    {
        /// <summary>
        /// Write the given data to a file using the passed <paramref name="config"/>
        /// </summary>
        Task WriteDataToFileAsync<T>(IEnumerable<T> data, DataFileWriterConfig<T> config);
    }
}
