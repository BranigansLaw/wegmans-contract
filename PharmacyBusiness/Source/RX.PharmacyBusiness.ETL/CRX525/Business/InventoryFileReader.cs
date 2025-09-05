namespace RX.PharmacyBusiness.ETL.CRX525.Business
{
    using RX.PharmacyBusiness.ETL.CRX525.Core;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    public class InventoryFileReader
    {
        public const string WcbInputFileNamePattern = "*WCB*";
        public const string ShelfInputFileNamePattern = "INV*";
        public const bool WcbInputFileHasHeaderRow = false;
        public const bool ShelfInputFileHasHeaderRow = false;
        public const string RejectFileNamePrefix = "REJECT_RECORDS";

        private string inputFileLocation;
        private string archiveFileLocation;
        private string rejectFileLocation;
        private IFileManager fileManager { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        public InventoryFileReader(string inputFileLocation, string archiveFileLocation, string rejectFileLocation)
        {
            this.inputFileLocation = inputFileLocation;
            this.archiveFileLocation = archiveFileLocation;
            this.rejectFileLocation = rejectFileLocation;
            this.fileManager = this.fileManager ?? new FileManager();
        }

        public List<WcbInventoryRecord> ReadWcbFiles()
        {
            List<WcbInventoryRecord> records = new List<WcbInventoryRecord>();
            int lineNumber = 0;
            int fileCounter = 0;
            string line = string.Empty;

            foreach (string fileName in Directory.EnumerateFiles(this.inputFileLocation, WcbInputFileNamePattern))
            {
                Log.LogInfo("Reading WCB Inventory filename [{0}].", fileName);
                try
                {
                    StreamReader reader = new StreamReader(this.fileManager.OpenRead(fileName));
                    fileCounter++;

                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        lineNumber++;

                        if (!string.IsNullOrEmpty(line) &&
                            !(WcbInputFileHasHeaderRow && lineNumber == 1))
                        {
                            try
                            {
                                records.Add(new WcbInventoryRecord(line, Path.GetFileName(fileName)));
                            }
                            catch (Exception exception)
                            {
                                Log.LogError("Line number [{0}] in file name [{1}] does not have [{2}] array members.",
                                        lineNumber,
                                        fileName,
                                        WcbInventoryRecord.InputLineArrayLength);
                                this.fileManager.AppendAllText(
                                    Path.Combine(this.rejectFileLocation, string.Format("{0}_{1}", RejectFileNamePrefix, Path.GetFileName(fileName))),
                                    line);
                            }
                        }
                    }

                    reader.Close();
                }
                catch (Exception exception)
                {
                    Log.LogError("Line number[{0}] in file name [{1}] threw an exception (see record in Reject file) [{2}].", lineNumber, fileName, exception.Message);
                    this.fileManager.AppendAllText(
                        Path.Combine(this.rejectFileLocation, string.Format("{0}_{1}", RejectFileNamePrefix, Path.GetFileName(fileName))),
                        line);
                }
            }

            Log.LogInfo("Read in [{0}] WCB Inventory file(s).", fileCounter);

            return records;
        }

        public List<ShelfInventoryRecord> ReadShelfFiles()
        {
            List<ShelfInventoryRecord> records = new List<ShelfInventoryRecord>();
            int lineNumber = 0;
            int fileCounter = 0;
            string line = string.Empty;

            foreach (string fileName in Directory.EnumerateFiles(this.inputFileLocation, ShelfInputFileNamePattern))
            {
                Log.LogInfo("Reading Shelf Inventory filename [{0}].", fileName);
                try
                {
                    StreamReader reader = new StreamReader(this.fileManager.OpenRead(fileName));
                    fileCounter++;

                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        lineNumber++;

                        if (!string.IsNullOrEmpty(line) &&
                            !(ShelfInputFileHasHeaderRow && lineNumber == 1))
                        {
                            try
                            {
                                records.Add(new ShelfInventoryRecord(line, Path.GetFileName(fileName)));
                            }
                            catch (Exception exception)
                            {
                                Log.LogError("Line number [{0}] in file name [{1}] does not have [{2}] character length.",
                                        lineNumber,
                                        fileName,
                                        ShelfInventoryRecord.FixedWidthInputLineLength);
                                this.fileManager.AppendAllText(
                                    Path.Combine(this.rejectFileLocation, string.Format("{0}_{1}", RejectFileNamePrefix, Path.GetFileName(fileName))),
                                    line);
                            }
                        }
                    }

                    reader.Close();
                }
                catch (Exception exception)
                {
                    Log.LogError("Line number[{0}] in file name [{1}] threw an exception (see record in Reject file) [{2}].", lineNumber, fileName, exception.Message);
                    this.fileManager.AppendAllText(
                        Path.Combine(this.rejectFileLocation, string.Format("{0}_{1}", RejectFileNamePrefix, Path.GetFileName(fileName))),
                        line);
                }
            }

            Log.LogInfo("Read in [{0}] Shelf Inventory file(s).", fileCounter);

            return records;
        }

        public bool DoRejectFilesExist(string searchPattern)
        {
            return this.fileManager.FileExists(Path.Combine(this.rejectFileLocation, searchPattern));
        }

        public void ArchiveRejectFiles()
        {
            ArchiveFiles(this.rejectFileLocation, WcbInputFileNamePattern);
            ArchiveFiles(this.rejectFileLocation, ShelfInputFileNamePattern);
        }

        public void ArchiveInputFiles()
        {
            ArchiveFiles(this.inputFileLocation, WcbInputFileNamePattern);
            ArchiveFiles(this.inputFileLocation, ShelfInputFileNamePattern);
        }

        public void ArchiveFiles(string fileLocation, string searchPattern)
        {
            foreach (string fileName in Directory.EnumerateFiles(fileLocation, searchPattern))
            {
                Log.LogInfo("Archiving filename [{0}].", fileName);
                this.fileManager.MoveFile(fileName, Path.Combine(this.archiveFileLocation, Path.GetFileName(fileName)));
            }
        }
    }
}
