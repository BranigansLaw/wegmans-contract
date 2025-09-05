namespace Library.LibraryUtilities.DataFileWriter
{
    public class DataFileWriterConfig<T>
    {
        /// <summary>
        /// The header of the file, or nothing if null
        /// </summary>
        public string? Header { get; set; }

        /// <summary>
        /// The chosen output file path
        /// </summary>
        public required string OutputFilePath { get; set; }

        /// <summary>
        /// The translation from <typeparamref name="T"/> to a string line in the database
        /// </summary>
        public required Func<T, string> WriteDataLine { get; set; }
    }
}
