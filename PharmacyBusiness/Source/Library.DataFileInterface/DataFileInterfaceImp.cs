using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.EmailSender;
using Library.DataFileInterface.Exceptions;
using Library.DataFileInterface.VendorFileDataModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace Library.DataFileInterface
{
    public class DataFileInterfaceImp : IDataFileInterface
    {
        private readonly IDataFileReader _dataFileReader;
        private readonly IOptions<DataFileConfig> _config;
        private readonly ILogger<DataFileInterfaceImp> _logger;

        public DataFileInterfaceImp(
            IDataFileReader dataFileReader,
            IOptions<DataFileConfig> config,
            ILogger<DataFileInterfaceImp> logger)
        {
            _dataFileReader = dataFileReader ?? throw new ArgumentNullException(nameof(dataFileReader));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> ReadFileToListAndNotifyExceptionsAsync<T>(
            IEmailExceptionComposer emailException,
            string dataFileName,
            string rowDelimiter,
            string columnDelimiter,
            bool hasHeaderRow,
            DateOnly runFor,
            CancellationToken c) where T : class, IDataRecord, new()
        {
            if (string.IsNullOrEmpty(dataFileName))
            {
                throw new ArgumentNullException(nameof(dataFileName));
            }
            if (string.IsNullOrEmpty(rowDelimiter))
            {
                throw new ArgumentNullException(nameof(rowDelimiter));
            }
            if (string.IsNullOrEmpty(columnDelimiter))
            {
                throw new ArgumentNullException(nameof(columnDelimiter));
            }

            IEnumerable<T> returnDataRecords = new List<T>();

            try
            {
                returnDataRecords = await ReadFileToListAsync<T>(
                    dataFileName,
                    rowDelimiter,
                    columnDelimiter,
                    hasHeaderRow,
                    runFor,
                    c).ConfigureAwait(false);
            }
            catch (DataIntegrityException dataIntegrityException)
            {
                _logger.LogWarning(dataIntegrityException.DataSourceSummaryOfConstraintViolations);
                IEmailSenderInterface sender = new EmailSenderInterfaceImp(_logger);
                sender.SmtpClientSendMail(emailException.Compose(_config.Value.RejectFileLocation, dataIntegrityException, _config.Value.NotificationEmailTo));
                throw;
            }

            return returnDataRecords;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> ReadFileToListAsync<T>(
            string dataFileName,
            string rowDelimiter,
            string columnDelimiter,
            bool hasHeaderRow,
            DateOnly runFor,
            CancellationToken c) where T : class, IDataRecord, new()
        {
            if (string.IsNullOrEmpty(dataFileName))
            {
                throw new ArgumentNullException(nameof(dataFileName));
            }
            if (string.IsNullOrEmpty(rowDelimiter))
            {
                throw new ArgumentNullException(nameof(rowDelimiter));
            }
            if (string.IsNullOrEmpty(columnDelimiter))
            {
                throw new ArgumentNullException(nameof(columnDelimiter));
            }

            List<T> validDataRows = new List<T>();
            List<string> rejectDataRows = new List<string>();
            string dataFromFile = await _dataFileReader.ReadDataFileAsync(dataFileName, c).ConfigureAwait(false);
            string[] allRowsInFile = dataFromFile.Split(rowDelimiter);
            long dataFileLineCount = 0;
            long dataFileDataRowsCount = 0;
            foreach (string row in allRowsInFile)
            {
                dataFileLineCount++;

                if (hasHeaderRow && dataFileLineCount == 1)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(row))
                {
                    continue;
                }

                string[] columns = row.Split(columnDelimiter);

                var recordObject = new T();
                dataFileDataRowsCount++;

                try
                {
                    recordObject.SetRecordPropertiesFromFileRow(columns, runFor);
                    validDataRows.Add(recordObject);
                }
                catch (DataIntegrityException dataIntegrityException)
                {
                    rejectDataRows.Add($"Data file line nbr {dataFileLineCount}: {dataIntegrityException.DataRowConstraintViolations}");
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Error reading row {0} from file {1}.", dataFileLineCount, dataFileName);
                    throw;
                }
            }

            if (rejectDataRows.Any())
            {
                _logger.LogWarning($"Moving data file [{dataFileName}] to rejects folder.");
                File.Move(Path.Combine(_config.Value.InputFileLocation, dataFileName), Path.Combine(_config.Value.RejectFileLocation, dataFileName), true);
                throw new DataIntegrityException(dataFileName, dataFileDataRowsCount, rejectDataRows);
            }
            else
            {
                _logger.LogInformation($"Successfully read [{validDataRows.Count}] data rows from file [{dataFileName}] so now moving file to Archive folder.");
                File.Move(Path.Combine(_config.Value.InputFileLocation, dataFileName), Path.Combine(_config.Value.ArchiveFileLocation, dataFileName), true);
                if (File.Exists($"{_config.Value.RejectFileLocation}/{dataFileName}") == true)
                {
                    File.Delete(Path.Combine(_config.Value.RejectFileLocation, dataFileName));
                    _logger.LogDebug($"File {dataFileName} was deleted from the rejects folder because this rerun was successful.");
                }
            }

            return validDataRows;
        }

        /// <inheritdoc />
        public async Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            CancellationToken c)
        {
            await WriteListToFileAsync<T>(
               downloadList,
               downloadFileName,
               hasHeaderRow,
               delimiter,
               textQualifier,
               makeExtractWhenNoData,
               false,
               c).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            bool shouldAppendToExistingFile,
            CancellationToken c)
        {
            downloadFileName = Path.Combine(_config.Value.OutputFileLocation, downloadFileName);
            delimiter = delimiter ?? string.Empty;
            textQualifier = textQualifier ?? string.Empty;
            var output = new StringBuilder();
            var fields = new Collection<string>();
            Type elementType = typeof(T);

            _logger.LogInformation("Output data in typed List<T> to output file [{0}].", downloadFileName);

            if (File.Exists(downloadFileName) && !shouldAppendToExistingFile)
            {
                _logger.LogWarning("Output file [{0}] already exists and shouldAppendToExistingFile=false, so now deleting this existing output file.", downloadFileName);
                File.Delete(downloadFileName);
            }

            //If the file already exists and shouldAppendToExistingFile=true, then do not output the header row with the append.
            hasHeaderRow = (
                hasHeaderRow &&
                shouldAppendToExistingFile &&
                File.Exists(downloadFileName) ?
                    false : hasHeaderRow);

            using (var writerOutputData = new StreamWriter(downloadFileName, shouldAppendToExistingFile))
            {
                if (hasHeaderRow)
                {
                    foreach (var propInfo in elementType.GetProperties())
                    {
                        //Get the column label from the ExportHeaderColumnLabelAttribute if it exists, otherwise use the property name.
                        string columnLabel = propInfo.Name;
                        var attributes = propInfo.GetCustomAttributes(false);
                        var columnMapping = attributes.FirstOrDefault(a => a.GetType() == typeof(ExportHeaderColumnLabelAttribute));
                        if (columnMapping != null)
                        {
                            var mapsto = columnMapping as ExportHeaderColumnLabelAttribute;

                            if (mapsto != null)
                                columnLabel = mapsto.Name;
                        }

                        fields.Add(string.Format("{0}{1}{0}", textQualifier, columnLabel));
                    }

                    await writerOutputData.WriteLineAsync(string.Join(delimiter, fields.ToArray())).ConfigureAwait(false);
                    fields.Clear();
                    hasHeaderRow = false;
                }

                foreach (T record in downloadList)
                {
                    fields.Clear();

                    foreach (var propInfo in elementType.GetProperties())
                    {
                        if ((propInfo.GetValue(record, null) ?? DBNull.Value).GetType() == typeof(System.String))
                        {
                            fields.Add(string.Format("{0}{1}{0}", textQualifier, (propInfo.GetValue(record, null) ?? DBNull.Value).ToString()));
                        }
                        else
                        {
                            fields.Add((propInfo.GetValue(record, null) ?? DBNull.Value).ToString() ?? string.Empty);
                        }
                    }

                    await writerOutputData.WriteLineAsync(string.Join(delimiter, fields.ToArray())).ConfigureAwait(false);
                }
            }

            if (!makeExtractWhenNoData && downloadList.Count == 0)
            {
                _logger.LogInformation("List has zero rows and job is set to not create an empty file, so now deleting this empty output file.");
                if (File.Exists(downloadFileName))
                    File.Delete(downloadFileName);
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFileNames(
            string fileNamePattern)
        {
            if (string.IsNullOrWhiteSpace(fileNamePattern))
            {
                throw new ArgumentNullException(nameof(fileNamePattern));
            }

            var files = Directory.EnumerateFiles(_config.Value.InputFileLocation, fileNamePattern);

            return files.Select(r => Path.GetFileName(r));
        }

        /// <inheritdoc />
        public IEnumerable<string> GetImageFileNames(
            string fileNamePattern)
        {
            if (string.IsNullOrWhiteSpace(fileNamePattern))
            {
                throw new ArgumentNullException(nameof(fileNamePattern));
            }

            var files = Directory.EnumerateFiles(_config.Value.ImageFileLocation, fileNamePattern);

            return files.Select(r => Path.GetFileName(r));
        }

        /// <inheritdoc />
        public void ArchiveFile(
            string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            string sourceFile = Path.Combine(_config.Value.InputFileLocation, fileName);
            string destinationFile = Path.Combine(_config.Value.ArchiveFileLocation, fileName);

            if (File.Exists(sourceFile))
            {
                File.Move(sourceFile, destinationFile, true);
            }
            else
            {
                _logger.LogWarning("File [{0}] does not exist in the input folder so it cannot be archived.", fileName);
            }
        }

        /// <inheritdoc />
        public void RejectFile(
            string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            string sourceFile = Path.Combine(_config.Value.InputFileLocation, fileName);
            string destinationFile = Path.Combine(_config.Value.RejectFileLocation, fileName);

            if (File.Exists(sourceFile))
            {
                File.Move(sourceFile, destinationFile, true);
            }
            else
            {
                _logger.LogWarning("File [{0}] does not exist in the input folder so it cannot be put into Rejects folder.", fileName);
            }
        }

        /// <inheritdoc />
        public Stream ReadImageFileToStream(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var file = Path.Combine(_config.Value.ImageFileLocation, fileName);

            return File.OpenRead(file);
        }

        /// <inheritdoc />
        public void RejectImageFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            string sourceFile = Path.Combine(_config.Value.ImageFileLocation, fileName);
            string destinationFile = Path.Combine(_config.Value.RejectFileLocation, fileName);

            if (File.Exists(sourceFile))
            {
                File.Copy(sourceFile, destinationFile, true);
            }
            else
            {
                throw new FileNotFoundException("File [{0}] does not exist in the image folder so it cannot be moved to the reject folder.", fileName);
            }
        }

        public DateTime? ExtractDateFromFileName(
            string actualFileName, 
            string fileNamePattern, 
            string dateParsePattern = "yyyyMMdd",
            string dateWildcard = "*")
        {
            if (string.IsNullOrWhiteSpace(actualFileName))
            {
                throw new ArgumentNullException(nameof(actualFileName));
            }
            if (string.IsNullOrWhiteSpace(fileNamePattern))
            {
                throw new ArgumentNullException(nameof(fileNamePattern));
            }

            int dateStringIndexStart = fileNamePattern.IndexOf(dateWildcard);
            int substringStart = (dateStringIndexStart + dateWildcard.Length);
            int substringLength = fileNamePattern.Length - substringStart;
            string remainingFileNameAfterDateString = fileNamePattern.Substring(substringStart, substringLength);
            int dateStringIndexEnd = actualFileName.IndexOf(remainingFileNameAfterDateString);
            if (dateStringIndexStart > -1)
            {
                string fileDateString = Path.GetFileName(actualFileName).Substring(dateStringIndexStart, (dateStringIndexEnd - dateStringIndexStart));

                //if (DateTime.TryParseExact(fileDateString, dateParsePattern, null, DateTimeStyles.None, out DateTime returnDateTime))
                //CultureInfo provider = CultureInfo.InvariantCulture;

                if (DateTime.TryParseExact(fileDateString, dateParsePattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime returnDateTime))
                    return returnDateTime;
            }

            return default;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFileNamesFromImportsAndArchives(
            string fileNamePattern,
            DateTime runForDateTime,
            string dateParsePattern = "yyyyMMdd",
            string dateWildcard = "*")
        {
            if (string.IsNullOrWhiteSpace(fileNamePattern))
            {
                throw new ArgumentNullException(nameof(fileNamePattern));
            }

            //If the imports folder does not contain a file with the RunFor date parameter, then look in the Archives folder in case the intent is to reprocess that file.
            //It is OK if no file exists for the RunFor date parameter in either the Imports or Archives folders.
            string runForDateString = runForDateTime.ToString(dateParsePattern);
            string archiveFileName = fileNamePattern.Replace("*", runForDateString);

            if (!File.Exists(Path.Combine(_config.Value.InputFileLocation, archiveFileName)) &&
                File.Exists(Path.Combine(_config.Value.ArchiveFileLocation, archiveFileName)))
                File.Move(Path.Combine(_config.Value.ArchiveFileLocation, archiveFileName), Path.Combine(_config.Value.InputFileLocation, archiveFileName), false);

            return GetFileNames(fileNamePattern);
        }
    }
}
