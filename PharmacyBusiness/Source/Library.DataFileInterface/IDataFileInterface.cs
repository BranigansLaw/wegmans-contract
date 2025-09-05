using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.EmailSender;

namespace Library.DataFileInterface
{
    public interface IDataFileInterface
    {
        /// <summary>
        /// Reads a data file into a list of a typed object, and notifies exceptions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="batchJobName"></param>
        /// <param name="emailPriority"></param>
        /// <param name="dataFileName"></param>
        /// <param name="rowDelimiter"></param>
        /// <param name="columnDelimiter"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="runFor"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> ReadFileToListAndNotifyExceptionsAsync<T>(
            IEmailExceptionComposer emailException,
            string dataFileName,
            string rowDelimiter,
            string columnDelimiter,
            bool hasHeaderRow,
            DateOnly runFor,
            CancellationToken c) where T : class, IDataRecord, new();

        /// <summary>
        /// Reads a data file into a list of a typed object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataFileName"></param>
        /// <param name="rowDelimiter"></param>
        /// <param name="columnDelimiter"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> ReadFileToListAsync<T>(
            string dataFileName,
            string rowDelimiter,
            string columnDelimiter,
            bool hasHeaderRow,
            DateOnly runDate,
            CancellationToken c) where T : class, IDataRecord, new();

        /// <summary>
        /// Writes a list of objects to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadList"></param>
        /// <param name="downloadFileName"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="c"></param>
        Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            CancellationToken c);

        /// <summary>
        /// Writes a list of objects to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadList"></param>
        /// <param name="downloadFileName"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="shouldAppendToExistingFile"></param>
        /// <param name="c"></param>
        Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            bool shouldAppendToExistingFile,
            CancellationToken c);


        /// <summary>
        /// Enumerates the input directory for a file name pattern and returns a list of file names.
        /// </summary>
        /// <param name="fileNamePattern"></param>
        /// <returns></returns>
        IEnumerable<string> GetFileNames(
            string fileNamePattern);

        /// <summary>
        /// Enumerates the image directory for a file name pattern and returns a list of file names.
        /// </summary>
        /// <param name="fileNamePattern"></param>
        /// <returns></returns>
        IEnumerable<string> GetImageFileNames(
            string fileNamePattern);

        /// <summary>
        /// Archive the specified file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        void ArchiveFile(
            string fileName);

        /// <summary>
        /// Reject the specified file.
        /// </summary>
        /// <param name="fileName"></param>
        void RejectFile(
           string fileName);

        /// <summary>
        /// Read the specified image file to a stream.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Stream ReadImageFileToStream(
            string fileName);

        /// <summary>
        /// Reject the specified file.
        /// </summary>
        /// <param name="fileName"></param>
        void RejectImageFile(
            string fileName);

        /// <summary>
        /// Extracts the date from the file name.
        /// </summary>
        /// <param name="actualFileName"></param>
        /// <param name="fileNamePattern"></param>
        /// <param name="dateParsePattern"></param>
        /// <param name="dateWildcard"></param>
        /// <returns></returns>
        DateTime? ExtractDateFromFileName(
            string actualFileName, 
            string fileNamePattern, 
            string dateParsePattern = "yyyyMMdd",
            string dateWildcard = "*");

        /// <summary>
        /// Get file names from the imports and archives directories.
        /// </summary>
        /// <param name="fileNamePattern"></param>
        /// <param name="runForDateTime"></param>
        /// <param name="dateParsePattern"></param>
        /// <param name="dateWildcard"></param>
        /// <returns></returns>
        IEnumerable<string> GetFileNamesFromImportsAndArchives(
            string fileNamePattern,
            DateTime runForDateTime,
            string dateParsePattern = "yyyyMMdd",
            string dateWildcard = "*");
    }
}
