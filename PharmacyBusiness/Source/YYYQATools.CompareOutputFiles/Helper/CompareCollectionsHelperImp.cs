namespace YYYQATools.CompareOutputFiles.Helper
{
    public class CompareCollectionsHelperImp : ICompareCollectionsHelper
    {
        public string NewFileNameAndPath { get; }
        public string OldFileNameAndPath { get; }
        public string FileRowDelimiter { get; set; }
        public string[]? NewFileLines { get; set; }
        public string[]? OldFileLines { get; set; }
        public List<int> CompletelyMatchedNewRecordIndexes { get; set; }
        public List<int> CompletelyMatchedOldRecordIndexes { get; set; }
        public List<int> NotMatchedNewRecordIndexes { get; set; }
        public List<int> NotMatchedOldRecordIndexes { get; set; }

        public CompareCollectionsHelperImp(string newFileNameAndPath, string oldFileNameAndPath)
        {
            NewFileNameAndPath = newFileNameAndPath;
            OldFileNameAndPath = oldFileNameAndPath;
            FileRowDelimiter = Environment.NewLine;
            NewFileLines = null;
            OldFileLines = null;
            CompletelyMatchedNewRecordIndexes = new List<int>();
            CompletelyMatchedOldRecordIndexes = new List<int>();
            NotMatchedNewRecordIndexes = new List<int>();
            NotMatchedOldRecordIndexes = new List<int>();
        }

        public void ResetMetrics()
        {
            CompletelyMatchedNewRecordIndexes = new List<int>();
            CompletelyMatchedOldRecordIndexes = new List<int>();
            NotMatchedNewRecordIndexes = new List<int>();
            NotMatchedOldRecordIndexes = new List<int>();
        }

        public int CompareCollections()
        {
            if (string.IsNullOrEmpty(NewFileNameAndPath) || string.IsNullOrEmpty(OldFileNameAndPath))
            {
                Console.WriteLine($"Cannot compare empty files! New file name and path=[{NewFileNameAndPath}] and Old file name and path=[{OldFileNameAndPath}]");
                return (int)CompareReturnCodes.Failure_UnableToCompareFiles;
            }

            string newFileContents = ReadDataFile(NewFileNameAndPath);
            string oldFileContents = ReadDataFile(OldFileNameAndPath);

            if (string.IsNullOrEmpty(newFileContents) || string.IsNullOrEmpty(oldFileContents))
            {
                Console.WriteLine($"Cannot compare empty files! New file string length=[{newFileContents.Length}] and old file string length=[{oldFileContents.Length}]");
                return (int)CompareReturnCodes.Failure_UnableToCompareFiles;
            }

            try
            {
                ResetMetrics();
                if (FilesArePerfectlyIdentical(newFileContents, oldFileContents))
                {
                    return (int)CompareReturnCodes.Success_FilesArePerfectlyIdentical;
                }
                
                ResetMetrics();
                if (LinesFromFilesMatchButHaveDifferentSortOrder(newFileContents, oldFileContents))
                {
                    return (int)CompareReturnCodes.ConditionalSuccess_LinesFromFilesMatchButHaveDifferentSortOrder;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to compare new file [{NewFileNameAndPath}] to old file [{OldFileNameAndPath}].");
                return (int)CompareReturnCodes.Failure_UnableToCompareFiles;
            }

            return (int)CompareReturnCodes.Failure_IrreconcilableDifferencesWereFound;
        }

        /// <summary>
        /// This method compares the contents of two files to see if they are an exact match...including sort order and data formatting.
        /// </summary>
        /// <returns>bool</returns>
        public bool FilesArePerfectlyIdentical(string newFileContents, string oldFileContents)
        {
            NewFileLines = newFileContents.Split(FileRowDelimiter);
            OldFileLines = oldFileContents.Split(FileRowDelimiter);

            for (int newIndex = 0; newIndex < NewFileLines.Length; newIndex++)
            {
                int oldIndex = newIndex;
                bool foundMatch = false;

                do
                { 
                    if (NewFileLines[newIndex] == OldFileLines[oldIndex])
                    { 
                        CompletelyMatchedNewRecordIndexes.Add(newIndex);
                        CompletelyMatchedOldRecordIndexes.Add(oldIndex);
                        foundMatch = true;
                    }
                    oldIndex++;
                } while (!foundMatch && oldIndex < OldFileLines.Length);

                if (!foundMatch)
                {
                    NotMatchedNewRecordIndexes.Add(newIndex);
                }
            }

            for (int oldIndex = 0; oldIndex < OldFileLines.Length; oldIndex++)
            {
                if (CompletelyMatchedOldRecordIndexes.Contains(oldIndex))
                    continue;

                int newIndex = oldIndex;
                bool foundMatch = false;

                do
                {
                    if (NewFileLines[newIndex] == OldFileLines[oldIndex])
                    {
                        CompletelyMatchedNewRecordIndexes.Add(newIndex);
                        CompletelyMatchedOldRecordIndexes.Add(oldIndex);
                        foundMatch = true;
                    }
                    newIndex++;
                } while (!foundMatch && newIndex < NewFileLines.Length);

                if (!foundMatch)
                {
                    NotMatchedOldRecordIndexes.Add(oldIndex);
                }
            }

            return (NewFileLines.Length == OldFileLines.Length &&
                    NewFileLines.Length == CompletelyMatchedNewRecordIndexes.Count() &&
                    OldFileLines.Length == CompletelyMatchedOldRecordIndexes.Count() &&
                    NotMatchedNewRecordIndexes.Count() == 0 &&
                    NotMatchedOldRecordIndexes.Count() == 0);
        }

        /// <summary>
        /// This method compares the contents of two files to see if they are an exact match...including sort order and data formatting.
        /// </summary>
        /// <returns>bool</returns>
        public bool LinesFromFilesMatchButHaveDifferentSortOrder(string newFileContents, string oldFileContents)
        {
            NewFileLines = newFileContents.Split(FileRowDelimiter);
            OldFileLines = oldFileContents.Split(FileRowDelimiter);

            for (int newIndex = 0; newIndex < NewFileLines.Length; newIndex++)
            {
                for (int oldIndex = 0; oldIndex < OldFileLines.Length; oldIndex++)
                {
                    if (CompletelyMatchedOldRecordIndexes.Contains(oldIndex))
                        continue;

                    if (NewFileLines[newIndex] == OldFileLines[oldIndex])
                    {
                        CompletelyMatchedNewRecordIndexes.Add(newIndex);
                        CompletelyMatchedOldRecordIndexes.Add(oldIndex);
                        break;
                    }
                }

                if (!CompletelyMatchedNewRecordIndexes.Contains(newIndex))
                    NotMatchedNewRecordIndexes.Add(newIndex);
            }

            for (int oldIndex = 0; oldIndex < OldFileLines.Length; oldIndex++)
            {
                if (CompletelyMatchedOldRecordIndexes.Contains(oldIndex))
                    continue;

                for (int newIndex = 0; newIndex < NewFileLines.Length; newIndex++)
                {
                    if (CompletelyMatchedNewRecordIndexes.Contains(newIndex))
                        continue;

                    if (NotMatchedNewRecordIndexes.Contains(newIndex))
                        continue;

                    if (NewFileLines[newIndex] == OldFileLines[oldIndex])
                    {
                        CompletelyMatchedNewRecordIndexes.Add(newIndex);
                        CompletelyMatchedOldRecordIndexes.Add(oldIndex);
                        break;
                    }
                }

                if (!CompletelyMatchedOldRecordIndexes.Contains(oldIndex))
                    NotMatchedOldRecordIndexes.Add(oldIndex);
            }

            return (NewFileLines.Length == OldFileLines.Length &&
                    NewFileLines.Length == CompletelyMatchedNewRecordIndexes.Count() &&
                    OldFileLines.Length == CompletelyMatchedOldRecordIndexes.Count());
        }

        public bool WereFilesLastModifiedOnSameDate()
        {
            return File.GetLastWriteTime(NewFileNameAndPath).Date == File.GetLastWriteTime(OldFileNameAndPath).Date;
        }

        public string ReadDataFile(string dataFilePathAndName)
        {
            if (File.Exists($"{dataFilePathAndName}") == true)
                return File.ReadAllText($"{dataFilePathAndName}");
            else
                throw new Exception($"File not found: [{dataFilePathAndName}].");
        }

        public string GetCompareSummaryReport(int limitReportingOfMismatches = 100)
        {
            List<string> mismatchRemarks = new List<string>();
            int mismatchCounter = 0;
            string underlineCategories = "---------------------------"; 

            mismatchRemarks.Add($"New File Name And Path=[{NewFileNameAndPath}]");
            mismatchRemarks.Add($"Old File Name And Path=[{OldFileNameAndPath}]");

            mismatchRemarks.Add($"New Records Count=[{NewFileLines?.Length}]");
            mismatchRemarks.Add($"Old Records Count=[{OldFileLines?.Length}]");

            if (NewFileLines == null || OldFileLines == null)
            {
                mismatchRemarks.Add("Unable to compare files!!!");
                return string.Join(Environment.NewLine, mismatchRemarks);
            }

            if (NewFileLines?.Length == 0 || OldFileLines?.Length == 0)
            {
                mismatchRemarks.Add("Cannot compare zero records!!!");
                return string.Join(Environment.NewLine, mismatchRemarks);
            }

            mismatchRemarks.Add($"Completely Matched Record Count=[{CompletelyMatchedNewRecordIndexes.Count()}]");
            if (CompletelyMatchedNewRecordIndexes.Count() == NewFileLines?.Count() && CompletelyMatchedNewRecordIndexes.Count() == OldFileLines?.Count())
                mismatchRemarks.Add("SUCCESS - All records are completely matched.");
            else if (!WereFilesLastModifiedOnSameDate())
                mismatchRemarks.Add("NOTE: The two files were made on different dates. Consider recreating both new and old files at the same time, or at least on the same date, in case the source data changes over time.");

            mismatchRemarks.Add(Environment.NewLine);
            mismatchRemarks.Add($"Not Matched New Record Count=[{NotMatchedNewRecordIndexes.Count()}]");
            mismatchRemarks.Add(underlineCategories);
            foreach (var notMatchedNewRecordIndex in NotMatchedNewRecordIndexes)
            {
                if (mismatchCounter >= limitReportingOfMismatches)
                {
                    mismatchRemarks.Add(Environment.NewLine);
                    mismatchRemarks.Add($"NOTE: There are more records to display but output is limited to first [{limitReportingOfMismatches}] occurences.");
                    break;
                }

                mismatchRemarks.Add($"New File Line Nbr [{(notMatchedNewRecordIndex + 1)}].");
                mismatchCounter++;
            }

            mismatchCounter = 0;
            mismatchRemarks.Add(Environment.NewLine);
            mismatchRemarks.Add($"Not Matched Old Record Count=[{NotMatchedOldRecordIndexes.Count()}]");
            mismatchRemarks.Add(underlineCategories);
            foreach (var notMatchedOldRecordIndex in NotMatchedOldRecordIndexes)
            {
                if (mismatchCounter >= limitReportingOfMismatches)
                {
                    mismatchRemarks.Add(Environment.NewLine);
                    mismatchRemarks.Add($"NOTE: There are more records to display but output is limited to first [{limitReportingOfMismatches}] occurences.");
                    break;
                }

                mismatchRemarks.Add($"Old File Line Nbr [{(notMatchedOldRecordIndex + 1)}].");
                mismatchCounter++;
            }

            return string.Join(Environment.NewLine, mismatchRemarks);
        }
    }
}
 