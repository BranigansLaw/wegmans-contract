namespace Library.DataFileInterface.DataFileReader
{
    public interface IDataFileReader
    {
        /// <summary>
        /// Reads a data file and returns its contents.
        /// To make jobs re-runnable, the file should be moved from the Archives folder back to the Imports folder, and if not found in the Archives folder then check the Rejects folder.
        /// </summary>
        /// <param name="dataFileName"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<string> ReadDataFileAsync(
            string dataFileName,
            CancellationToken c);
    }
}
