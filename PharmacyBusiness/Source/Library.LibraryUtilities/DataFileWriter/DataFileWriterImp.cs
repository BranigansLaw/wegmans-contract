namespace Library.LibraryUtilities.DataFileWriter
{
    public class DataFileWriterImp : IDataFileWriter
    {
        /// <inheritdoc />
        public async Task WriteDataToFileAsync<T>(IEnumerable<T> data, DataFileWriterConfig<T> config)
        {
            DirectoryInfo directoryInfo = new(config.OutputFilePath);
            if (directoryInfo.Parent is not null)
            {
                Directory.CreateDirectory(directoryInfo.Parent.FullName);
            }

            using (StreamWriter w = File.CreateText(config.OutputFilePath))
            {
                if (config.Header is not null)
                {
                    await w.WriteLineAsync(config.Header);
                }

                foreach (T d in data)
                {
                    await w.WriteLineAsync(config.WriteDataLine(d));
                }
            }
        }
    }
}
