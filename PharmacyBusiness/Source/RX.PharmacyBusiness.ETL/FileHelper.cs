namespace RX.PharmacyBusiness.ETL
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    public class FileHelper
    {
        private IFileManager fileManager { get; set; }
        private string inputFileLocation;
        private string outputFileLocation;
        private string archiveFileLocation;
        private string rejectFileLocation;
        public string ArchiveForQAFileLocation;

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        [ExcludeFromCodeCoverage]
        public FileHelper(string inputFileLocation, string outputFileLocation, string archiveFileLocation, string rejectFileLocation)
        {
            this.inputFileLocation = Environment.ExpandEnvironmentVariables(inputFileLocation);
            this.outputFileLocation = Environment.ExpandEnvironmentVariables(outputFileLocation);
            this.archiveFileLocation = Environment.ExpandEnvironmentVariables(archiveFileLocation);
            this.rejectFileLocation = Environment.ExpandEnvironmentVariables(rejectFileLocation);
            this.fileManager = this.fileManager ?? new FileManager();
            CreateDirIfNotExists(@"%BATCH_ROOT%\ArchiveForQA\");
            this.ArchiveForQAFileLocation = CreateDirIfNotExists(@"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\");
        }

        public bool FileExists(string file)
        {
            string filePathAndName = Environment.ExpandEnvironmentVariables(file);
            return this.fileManager.FileExists(filePathAndName);
        }

        public List<T> ReadFilesToList<T>(
            string fileNamePattern, 
            DelimitedStreamReaderOptions sourceFileOptions,
            bool shouldArchiveInputFiles) where T : class, IRecord, new()
        {
            DelimitedLineReadResult<T> lineReadResult = null;
            List<T> records = new List<T>();
            long dataLineCount = 0;
            long notDataLineCount = 0;
            long rejectRecordCount = 0;
            string rejectErrorRemarksPrefix = "*** ";
            List<string> filesToArchive = new List<string>();
            List<string> filesToDelete = new List<string>();
            string rejectFile = string.Empty;

            Log.LogInfo("Delete any reject files from prior run with fileNamePattern [{0}].", fileNamePattern);
            var priorRejectFiles = this.fileManager.EnumerateFiles(this.rejectFileLocation, fileNamePattern).ToList();
            foreach (string priorRejectFile in priorRejectFiles)
            {
                this.fileManager.DeleteFile(priorRejectFile);
            }

            var inputFiles = this.fileManager.EnumerateFiles(this.inputFileLocation, fileNamePattern).ToList();
            foreach (string inputFile in inputFiles)
            {
                dataLineCount = 0;
                notDataLineCount = 0;
                rejectFile = this.rejectFileLocation + Path.GetFileName(inputFile);
                Log.LogInfo("Reading input file [{0}].", inputFile);

                try
                {
                    using (var reader = new DelimitedStreamReader<T>(this.fileManager.OpenRead(inputFile), sourceFileOptions))
                    using (var writerRejectData = this.fileManager.OpenWriter(rejectFile, true))
                    {
                        while (!reader.EndOfStream)
                        {
                            lineReadResult = reader.ReadRecord();

                            if (lineReadResult.IsBlank)
                            {
                                notDataLineCount++;
                                continue;
                            }

                            if (lineReadResult.IsHeader)
                            {
                                //Copy original header to reject file.
                                writerRejectData.WriteLine(lineReadResult.Line);
                                notDataLineCount++;
                                continue;
                            }

                            if (lineReadResult == null || lineReadResult.HasError)
                            {
                                //Continue processing the file with the hopes that only a small percentage of the file has bad data.
                                rejectRecordCount++;
                                writerRejectData.WriteLine(rejectErrorRemarksPrefix + lineReadResult.ErrorMessage);
                                if (lineReadResult.Exception != null && lineReadResult.Exception.Message != null)
                                {
                                    var exceptionErrors = lineReadResult.Exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var exceptionError in exceptionErrors)
                                    {
                                        writerRejectData.WriteLine(rejectErrorRemarksPrefix + exceptionError);
                                    }
                                }
                                writerRejectData.WriteLine(lineReadResult.Line);
                            }
                            else
                            {
                                dataLineCount++;
                                records.Add(lineReadResult.Record);
                            }
                        }
                    }
                }
                finally
                {
                    if (shouldArchiveInputFiles)
                        filesToArchive.Add(inputFile);
                }

                Log.LogWarn("File [{0}] has [{1}] valid data records and [{2}] reject records.", inputFile, dataLineCount, rejectRecordCount);

                if (rejectRecordCount == 0)
                {
                    //Cleanup file that should only contain a header record.
                    filesToDelete.Add(rejectFile);
                }
            }

            foreach (string fileToArchive in filesToArchive)
            {
                Log.LogInfo("Archiving file [{0}].", fileToArchive);
                this.fileManager.MoveFile(fileToArchive, Path.Combine(this.archiveFileLocation, Path.GetFileName(fileToArchive)));
            }

            foreach (string fileToDelete in filesToDelete)
            {
                Log.LogInfo("Deleting file [{0}].", fileToDelete);
                this.fileManager.DeleteFile(fileToDelete);
            }

            if (!inputFiles.Any())
            {
                Log.LogWarn("Could not find any input files matching the file name pattern [{0}].", fileNamePattern);
            }

            return records;
        }

        /// <summary>
        /// Outputs a typed List to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadList"></param>
        /// <param name="downloadFile"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        public void WriteListToFile<T>(
            List<T> downloadList,
            string downloadFile,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData)
        {
            this.WriteListToFile<T>(
               downloadList,
               downloadFile,
               hasHeaderRow,
               delimiter,
               textQualifier,
               makeExtractWhenNoData,
               false,
               false);
        }
        public void WriteListToFile<T>(
            List<T> downloadList,
            string downloadFile,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            bool shouldAppendToExistingFile)
        {
            this.WriteListToFile<T>(
               downloadList,
               downloadFile,
               hasHeaderRow,
               delimiter,
               textQualifier,
               makeExtractWhenNoData,
               shouldAppendToExistingFile,
               false);
        }

        /// <summary>
        /// Outputs a typed List to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadList"></param>
        /// <param name="downloadFile"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="shouldAppendToExistingFile"></param>
        public void WriteListToFile<T>(
            List<T> downloadList,
            string downloadFile,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            bool shouldAppendToExistingFile,
            bool shouldArchiveForQA)
        {
            downloadFile = Environment.ExpandEnvironmentVariables(downloadFile);
            delimiter = delimiter ?? string.Empty;
            textQualifier = textQualifier ?? string.Empty;
            var output = new StringBuilder();
            var fields = new Collection<string>();
            Type elementType = typeof(T);

            Log.LogInfo("Output data in typed List<T> to output file [{0}].", downloadFile);

            //If the file already exists and shouldAppendToExistingFile=true, then do not output the header row with the append.
            hasHeaderRow = (
                hasHeaderRow &&
                shouldAppendToExistingFile &&
                this.fileManager.FileExists(downloadFile)) ?
                    false : hasHeaderRow;

            using (var writerOutputData = this.fileManager.OpenWriter(downloadFile, shouldAppendToExistingFile))
            {
                if (hasHeaderRow)
                {
                    foreach (var propInfo in elementType.GetProperties())
                    {
                        fields.Add(string.Format("{0}{1}{0}", textQualifier, propInfo.Name));
                    }

                    writerOutputData.WriteLine(string.Join(delimiter, fields.ToArray()));
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
                            fields.Add((propInfo.GetValue(record, null) ?? DBNull.Value).ToString());
                        }
                    }

                    writerOutputData.WriteLine(string.Join(delimiter, fields.ToArray()));
                }
            }

            if (shouldArchiveForQA && downloadFile.IndexOf("ArchiveForQA") == -1)
            {
                File.Copy(downloadFile, Path.Combine(this.ArchiveForQAFileLocation, Path.GetFileName(downloadFile)), true);
            }

            if (!makeExtractWhenNoData && downloadList.Count == 0)
            {
                Log.LogInfo("List has zero rows and job is set to not create an empty file, so now deleting this empty output file.");
                this.fileManager.DeleteFile(downloadFile);
            }
        }

        public string WriteListToExcel<T>(
            List<T> downloadList,
            string excelFileName,
            string worksheetName,
            bool hasHeaderRow,
            bool stylizeFirstRow)
        {
            string filePathAndName = Environment.ExpandEnvironmentVariables(this.outputFileLocation + excelFileName);
            this.fileManager.DeleteFile(filePathAndName); //Delete file from a prior run.
            Log.LogInfo("Output data in typed List<T> to Excel file [{0}].", filePathAndName);
            Type elementType = typeof(T);
            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.Application oXL = new Microsoft.Office.Interop.Excel.Application();
            oXL.Visible = false;
            Microsoft.Office.Interop.Excel.Workbook oWB = oXL.Workbooks.Add(missing);
            Microsoft.Office.Interop.Excel.Worksheet oSheet = oWB.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
            var oSheetItems = oSheet;
            if (oSheetItems != null)
            {
                oSheetItems.Name = worksheetName;
                int rowNbr = 1;
                int colNbr = 1;

                if (hasHeaderRow)
                {
                    foreach (var propInfo in elementType.GetProperties())
                    {
                        oSheetItems.Cells[rowNbr, colNbr] = propInfo.Name;
                        colNbr++;
                    }

                    colNbr = 1;
                    rowNbr++;
                }

                if (stylizeFirstRow)
                {
                    foreach (var propInfo in elementType.GetProperties())
                    {
                        oSheetItems.Cells[rowNbr, colNbr].Font.Bold = true;
                        oSheetItems.Cells[rowNbr, colNbr].Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightGrey;

                        colNbr++;
                    }

                    colNbr = 1;
                    rowNbr = 1;
                }

                foreach (var record in downloadList)
                {
                    foreach (var propInfo in elementType.GetProperties())
                    {
                        oSheetItems.Cells[rowNbr, colNbr] = (propInfo.GetValue(record, null) ?? DBNull.Value).ToString();
                        colNbr++;
                    }

                    colNbr = 1;
                    rowNbr++;
                }
            }

            oWB.SaveAs(filePathAndName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook,
                missing, missing, missing, missing,
                Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                missing, missing, missing, missing, missing);
            oWB.Close(missing, missing, missing);
            oXL.UserControl = true;
            oXL.Quit();

            return filePathAndName;
        }

        public string CreateDirIfNotExists(string dir)
        {
            dir = Environment.ExpandEnvironmentVariables(dir);

            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            return dir;
        }

        public void CopyFileToArchiveForQA(string sourceNameAndPath)
        {
            sourceNameAndPath = Environment.ExpandEnvironmentVariables(sourceNameAndPath);
            File.Copy(sourceNameAndPath, Path.Combine(this.ArchiveForQAFileLocation, Path.GetFileName(sourceNameAndPath)), true);
            Log.LogInfo("Archived for QA file [{0}].", sourceNameAndPath);
        }
    }
}
