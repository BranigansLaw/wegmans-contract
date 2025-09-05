using XXXDeveloperTools.CompareDerivedData.DataModels;

namespace XXXDeveloperTools.CompareDerivedData.Helper
{
    public class CompareDerivedDataCollectionsHelperImp<T> where T : class, IDataRow, new()
    {
        public CompareDerivedDataSpecifications Specs { get; set; }
        public T[] NewDataRecords { get; set; }
        public T[] OldDataRecords { get; set; }
        public List<int> CompletelyMatchedNewRecordIndexes { get; set; }
        public List<int> CompletelyMatchedOldRecordIndexes { get; set; }
        public Dictionary<int, Dictionary<int, List<string>>> MatchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperties { get; set; }
        public List<int> NotMatchedNewRecordIndexes { get; set; }
        public List<int> NotMatchedOldRecordIndexes { get; set; }

        public CompareDerivedDataCollectionsHelperImp(CompareDerivedDataSpecifications specs)
        {
            Specs = specs;
            CompletelyMatchedNewRecordIndexes = new List<int>();
            CompletelyMatchedOldRecordIndexes = new List<int>();
            MatchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperties = new Dictionary<int, Dictionary<int, List<string>>>();
            NotMatchedNewRecordIndexes = new List<int>();
            NotMatchedOldRecordIndexes = new List<int>();
            NewDataRecords = [];
            OldDataRecords = [];
        }

        public void ResetComparisonSpecs(string newFileName)
        {
            Specs.FileName = newFileName;
        }

        public void ResetMetrics()
        {
            CompletelyMatchedNewRecordIndexes = new List<int>();
            CompletelyMatchedOldRecordIndexes = new List<int>();
            MatchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperties = new Dictionary<int, Dictionary<int, List<string>>>();
            NotMatchedNewRecordIndexes = new List<int>();
            NotMatchedOldRecordIndexes = new List<int>();
        }

        public int CompareCollections()
        {
            if (string.IsNullOrEmpty(Specs.NewFilePath) || string.IsNullOrEmpty(Specs.OldFilePath) || string.IsNullOrEmpty(Specs.FileName))
            {
                Console.WriteLine($"File info is required.");
                return (int)CompareDerivedDataReturnCodes.Failure_UnableToCompareFiles;
            }

            NewDataRecords = ReadFileToList(Path.Combine(Specs.NewFilePath, Specs.FileName)).ToArray();
            OldDataRecords = ReadFileToList(Path.Combine(Specs.OldFilePath, Specs.FileName)).ToArray();

            if (NewDataRecords.Length == 0 || OldDataRecords.Length == 0)
            {
                Console.WriteLine($"Cannot compare empty record collections! New record count=[{NewDataRecords.Length}] and old record count=[{OldDataRecords.Length}]");
                return (int)CompareDerivedDataReturnCodes.Failure_UnableToCompareFiles;
            }

            try
            {
                ResetMetrics();
                if (DataValuesMatchWithSameSortOrder())
                {
                    return (int)CompareDerivedDataReturnCodes.ConditionalSuccess_DataValuesMatchWithSameSortOrder;
                }

                ResetMetrics();
                if (DataValuesMatchWithDifferentSortOrder(false))
                {
                    return (int)CompareDerivedDataReturnCodes.ConditionalSuccess_DataValuesMatchWithDifferentSortOrder;
                }

                ResetMetrics();
                if (DataValuesMatchWithDifferentSortOrder(true))
                {
                    return (int)CompareDerivedDataReturnCodes.ConditionalSuccess_DataValuesMatchWithCloseDecimalRounding;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to compare files: [{Specs.NewFilePath}] and [{Specs.OldFilePath}] with file name [{Specs.FileName}].");
                return (int)CompareDerivedDataReturnCodes.Failure_UnableToCompareFiles;
            }

            return (int)CompareDerivedDataReturnCodes.Failure_IrreconcilableDifferencesWereFound;
        }

        /// <summary>
        /// Compares the data values to see if they are an exact match with the same sort order.
        /// </summary>
        /// <returns>bool</returns>
        public bool DataValuesMatchWithSameSortOrder()
        {
            for (int newIndex = 0; newIndex < NewDataRecords.Length; newIndex++)
            {
                int oldIndex = newIndex;
                bool foundMatch = false;

                do
                {
                    if (NewDataRecords[newIndex].IsMatchingOnAllProperties(OldDataRecords[oldIndex]))
                    {
                        CompletelyMatchedNewRecordIndexes.Add(newIndex);
                        CompletelyMatchedOldRecordIndexes.Add(oldIndex);
                        foundMatch = true;
                    }
                    oldIndex++;
                } while (!foundMatch && oldIndex < OldDataRecords.Length);

                if (!foundMatch)
                {
                    NotMatchedNewRecordIndexes.Add(newIndex);
                }
            }

            for (int oldIndex = 0; oldIndex < OldDataRecords.Length; oldIndex++)
            {
                if (CompletelyMatchedOldRecordIndexes.Contains(oldIndex))
                    continue;

                int newIndex = oldIndex;
                bool foundMatch = false;

                do
                {
                    if (NewDataRecords[newIndex].IsMatchingOnAllProperties(OldDataRecords[oldIndex]))
                    {
                        CompletelyMatchedNewRecordIndexes.Add(newIndex);
                        CompletelyMatchedOldRecordIndexes.Add(oldIndex);
                        foundMatch = true;
                    }
                    newIndex++;
                } while (!foundMatch && newIndex < NewDataRecords.Length);

                if (!foundMatch)
                {
                    NotMatchedOldRecordIndexes.Add(oldIndex);
                }
            }

            return (NewDataRecords.Length == OldDataRecords.Length &&
                    NewDataRecords.Length == CompletelyMatchedNewRecordIndexes.Count() &&
                    OldDataRecords.Length == CompletelyMatchedOldRecordIndexes.Count() &&
                    NotMatchedNewRecordIndexes.Count() == 0 &&
                    NotMatchedOldRecordIndexes.Count() == 0);
        }

        /// <summary>
        /// Compares the data values to see if they are an exact match with the different sort order.
        /// </summary>
        /// <returns>bool</returns>
        public bool DataValuesMatchWithDifferentSortOrder(bool endUsersAgreeToDecimalDifferenceThreshold = false)
        {
            for (int newIndex = 0; newIndex < NewDataRecords.Length; newIndex++)
            {
                //First pass at old records looks for complete record value matches which may be out of sort order.
                for (int oldIndex = 0; oldIndex < OldDataRecords.Length; oldIndex++)
                {
                    if (CompletelyMatchedOldRecordIndexes.Contains(oldIndex))
                        continue;

                    if (NewDataRecords[newIndex].IsMatchingOnAllProperties(OldDataRecords[oldIndex]))
                    {
                        CompletelyMatchedNewRecordIndexes.Add(newIndex);
                        CompletelyMatchedOldRecordIndexes.Add(oldIndex);
                        break;
                    }
                }

                if (CompletelyMatchedNewRecordIndexes.Contains(newIndex))
                    continue;

                //Second pass at old records looks for partial matches.
                //This is where we might find differences in derived data values that are outside tolerance threshholds.
                //This section of code is extremely helpful to developers in the middle of a project when they are trying to figure out why the derived data is not matching.
                //This section of code will probably not be useful once the project is in production and the data is stable.
                for (int oldIndex = 0; oldIndex < OldDataRecords.Length; oldIndex++)
                {
                    if (CompletelyMatchedOldRecordIndexes.Contains(oldIndex))
                        continue;

                    if (NewDataRecords[newIndex].IsMatchingOnAllNonderivedProperties(OldDataRecords[oldIndex]))
                    {
                        List<string> mismatchRemarks = NewDataRecords[newIndex].GetListOfDerivedDataPropertyMismatches(OldDataRecords[oldIndex], endUsersAgreeToDecimalDifferenceThreshold).ToList();

                        if (mismatchRemarks.Count > 0)
                        {
                            Dictionary<int, List<string>> oldRecordIndexAndRemarks = new Dictionary<int, List<string>>();
                            oldRecordIndexAndRemarks.Add(oldIndex, mismatchRemarks);
                            MatchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperties.Add(newIndex, oldRecordIndexAndRemarks);
                            break;
                        }
                        else
                        {
                            //These records are now matching within tolerance threshholds.
                            CompletelyMatchedNewRecordIndexes.Add(newIndex);
                            CompletelyMatchedOldRecordIndexes.Add(oldIndex);
                            break;
                        }
                    }
                }

                if (!CompletelyMatchedNewRecordIndexes.Contains(newIndex) &&
                    !MatchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperties.ContainsKey(newIndex))
                    NotMatchedNewRecordIndexes.Add(newIndex);
            }

            //Next reverse the process by finding records in the old collection that are not in the new collection.
            for (int oldIndex = 0; oldIndex < OldDataRecords.Length; oldIndex++)
            {
                if (CompletelyMatchedOldRecordIndexes.Contains(oldIndex))
                    continue;

                bool foundThisIndex = false;
                foreach (var matchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperty in MatchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperties)
                {
                    var oldRecordIndexMatchedOnlyOnNonderivedProperties = matchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperty.Value.Keys.First();
                    if (oldRecordIndexMatchedOnlyOnNonderivedProperties == oldIndex)
                    {
                        foundThisIndex = true;
                        break;
                    }
                }

                if (!foundThisIndex)
                    NotMatchedOldRecordIndexes.Add(oldIndex);
            }

            return (NewDataRecords.Length == OldDataRecords.Length &&
                    NewDataRecords.Length == CompletelyMatchedNewRecordIndexes.Count() &&
                    OldDataRecords.Length == CompletelyMatchedOldRecordIndexes.Count());
        }

        public IEnumerable<T> ReadFileToList(string dataFilePathAndName)
        {
            List<T> returnDataRows = new List<T>();
            string dataFromFile = ReadDataFile(dataFilePathAndName);
            string[] allRowsInFile = dataFromFile.Split(Specs.RowDelimiter);
            long dataFileLineCount = 0;
            long dataFileDataRowsCount = 0;
            foreach (string row in allRowsInFile)
            {
                dataFileLineCount++;

                if (Specs.HasHeaderRow && dataFileLineCount == 1)
                    continue;

                if (string.IsNullOrWhiteSpace(row))
                    continue;

                string[] columns = row.Split(Specs.ColumnDelimiter);
                var recordObject = new T();
                dataFileDataRowsCount++;
                recordObject.SetRecordPropertiesFromFileRow(columns); //Note: Do not wrap this in a Try Catch block. Let the exception bubble up and stop the program.
                returnDataRows.Add(recordObject);
            }

            return returnDataRows;
        }

        public bool WereFilesLastModifiedOnSameDate()
        {
            return File.GetLastWriteTime(Path.Combine(Specs.NewFilePath, Specs.FileName)).Date == File.GetLastWriteTime(Path.Combine(Specs.OldFilePath, Specs.FileName)).Date;
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

            mismatchRemarks.Add($"New Records Count=[{NewDataRecords.Length}]");
            mismatchRemarks.Add($"Old Records Count=[{OldDataRecords.Length}]");

            if (NewDataRecords.Length == 0 || OldDataRecords.Length == 0)
            {
                mismatchRemarks.Add("Cannot compare zero records!!!");
                return string.Join(Environment.NewLine, mismatchRemarks);
            }

            mismatchRemarks.Add($"Completely Matched Record Count=[{CompletelyMatchedNewRecordIndexes.Count()}]");
            if (CompletelyMatchedNewRecordIndexes.Count() == NewDataRecords.Count() && CompletelyMatchedNewRecordIndexes.Count() == OldDataRecords.Count())
                mismatchRemarks.Add("SUCCESS - All records are completely matched.");
            else if (!WereFilesLastModifiedOnSameDate())
                mismatchRemarks.Add("SUGGESTION: Consider recreating both new and old files at the same time in case the source data changes over time.");

            mismatchRemarks.Add(Environment.NewLine);
            mismatchRemarks.Add($"Not Matched New Record Count=[{NotMatchedNewRecordIndexes.Count()}]");
            if (NotMatchedNewRecordIndexes.Count() > 0)
                mismatchRemarks.Add("SUGGESTION: Consider reviewing the data filters used in the new batch job to see if they are insufficient.");
            mismatchRemarks.Add(underlineCategories);
            foreach (var notMatchedNewRecordIndex in NotMatchedNewRecordIndexes)
            {
                if (mismatchCounter >= limitReportingOfMismatches)
                {
                    mismatchRemarks.Add(Environment.NewLine);
                    mismatchRemarks.Add($"NOTE: There are more records to display but output is limited to first [{limitReportingOfMismatches}] occurences.");
                    break;
                }

                mismatchRemarks.Add($"New File Line Nbr [{(notMatchedNewRecordIndex + (Specs.HasHeaderRow ? 2 : 1))}].");
                mismatchCounter++;
            }

            mismatchCounter = 0;
            mismatchRemarks.Add(Environment.NewLine);
            mismatchRemarks.Add($"Not Matched Old Record Count=[{NotMatchedOldRecordIndexes.Count()}]");
            if (NotMatchedOldRecordIndexes.Count() > 0)
                mismatchRemarks.Add("SUGGESTION: Consider reviewing the data filters used in the new batch job to see if they are too restrictive.");
            mismatchRemarks.Add(underlineCategories);
            foreach (var notMatchedOldRecordIndex in NotMatchedOldRecordIndexes)
            {
                if (mismatchCounter >= limitReportingOfMismatches)
                {
                    mismatchRemarks.Add(Environment.NewLine);
                    mismatchRemarks.Add($"NOTE: There are more records to display but output is limited to first [{limitReportingOfMismatches}] occurences.");
                    break;
                }

                mismatchRemarks.Add($"Old File Line Nbr [{(notMatchedOldRecordIndex + (Specs.HasHeaderRow ? 2 : 1))}].");
                mismatchCounter++;
            }

            mismatchCounter = 0;
            mismatchRemarks.Add(Environment.NewLine);
            mismatchRemarks.Add($"Partially Matched Record Count=[{MatchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperties.Count()}]");
            mismatchRemarks.Add($"These are records that are Matched On Only Nonderived Properties But Not Matched On Derived Properties:");
            if (MatchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperties.Count() > 0)
                mismatchRemarks.Add("SUGGESTION: Consider reviewing the business logic used to populate derived data values, and/or review if the data differences are within end user tolerance threshholds.");
            mismatchRemarks.Add(underlineCategories);
            foreach (var partialMatches in MatchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperties)
            {
                if (mismatchCounter >= limitReportingOfMismatches)
                {
                    mismatchRemarks.Add(Environment.NewLine);
                    mismatchRemarks.Add($"NOTE: There are more records to display but output is limited to first [{limitReportingOfMismatches}] occurences.");
                    break;
                }

                mismatchRemarks.Add($"New File Line Nbr [{(partialMatches.Key + (Specs.HasHeaderRow ? 2 : 1))}] Old File Line Nbr [{(partialMatches.Value.Keys.First() + (Specs.HasHeaderRow ? 2 : 1))}]:");
                mismatchRemarks.AddRange(partialMatches.Value.Values.First());
                mismatchCounter++;
            }

            return string.Join(Environment.NewLine, mismatchRemarks);
        }
    }
}
