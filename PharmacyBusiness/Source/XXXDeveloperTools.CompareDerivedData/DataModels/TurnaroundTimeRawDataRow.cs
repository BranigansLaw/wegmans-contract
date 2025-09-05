using System.Collections.ObjectModel;

namespace XXXDeveloperTools.CompareDerivedData.DataModels
{
    public class TurnaroundTimeRawDataRow : IDataRow
    {
        private const decimal _maxDecimalRoundingDifference = 0.000001M;

        public string? OrderNbr { get; set; }
        public string? Facility { get; set; }
        public long? McKessonPatientKey { get; set; }
        public string? RxNbr { get; set; }
        public long? RefillNbr { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public long? WfsdKey { get; set; }
        public string? WfsdDescription { get; set; }
        public string? IsIntervention { get; set; }
        public string? IsException { get; set; }
        public decimal? ElapsedDaysInStep { get; set; }
        public decimal? ElapsedDaysInStepOffHrs { get; set; }
        public int? DateInRank { get; set; }
        public int? DateOutRank { get; set; }

        /// <inheritdoc />
        public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns)
        {
            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();
            if (!long.TryParse(dataFileColumns.ElementAt(2), out long mcKessonPatientKey))
                constraintViolations.Add("McKessonPatientKey is not a long.");
            if (!long.TryParse(dataFileColumns.ElementAt(4), out long refillNbr))
                constraintViolations.Add("RefillNbr is not a long.");
            if (!DateTime.TryParse(dataFileColumns.ElementAt(5), out DateTime dateIn))
                constraintViolations.Add("DateIn is not a date.");
            if (!DateTime.TryParse(dataFileColumns.ElementAt(6), out DateTime dateOut))
                constraintViolations.Add("DateOut is not a date.");
            if (!long.TryParse(dataFileColumns.ElementAt(7), out long wfsdKey))
                constraintViolations.Add("WfsdKey is not a long.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(11), out decimal elapsedDaysInStep))
                constraintViolations.Add("ElapsedDaysInStep is not a decimal.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(12), out decimal elapsedDaysInStepOffHrs))
                constraintViolations.Add("ElapsedDaysInStepOffHrs is not a decimal.");
            if (!int.TryParse(dataFileColumns.ElementAt(13), out int dateInRank))
                constraintViolations.Add("DateInRank is not an int.");
            if (!int.TryParse(dataFileColumns.ElementAt(14), out int dateOutRank))
                constraintViolations.Add("DateOutRank is not an int.");

            if (constraintViolations.ToArray().Length > 0)
            {
                //Note: This would only be a failure point during initial setup of a new job.
                throw new Exception(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                OrderNbr = dataFileColumns.ElementAt(0).Trim('"');
                Facility = dataFileColumns.ElementAt(1).Trim('"');
                McKessonPatientKey = mcKessonPatientKey;
                RxNbr = dataFileColumns.ElementAt(3).Trim('"');
                RefillNbr = refillNbr;
                DateIn = dateIn;
                DateOut = dateOut;
                WfsdKey = wfsdKey;
                WfsdDescription = dataFileColumns.ElementAt(8).Trim('"');
                IsIntervention = dataFileColumns.ElementAt(9).Trim('"');
                IsException = dataFileColumns.ElementAt(10).Trim('"');
                ElapsedDaysInStep = elapsedDaysInStep;
                ElapsedDaysInStepOffHrs = elapsedDaysInStepOffHrs;
                DateInRank = dateInRank;
                DateOutRank = dateOutRank;
            }
        }

        /// <inheritdoc />
        public bool IsMatchingOnAllProperties(IDataRow recordToCompare)
        {
            if (recordToCompare is TurnaroundTimeRawDataRow turnaroundTimeRawDataRow)
            {
                return OrderNbr == turnaroundTimeRawDataRow.OrderNbr &&
                       Facility == turnaroundTimeRawDataRow.Facility &&
                       McKessonPatientKey == turnaroundTimeRawDataRow.McKessonPatientKey &&
                       RxNbr == turnaroundTimeRawDataRow.RxNbr &&
                       RefillNbr == turnaroundTimeRawDataRow.RefillNbr &&
                       DateIn == turnaroundTimeRawDataRow.DateIn &&
                       DateOut == turnaroundTimeRawDataRow.DateOut &&
                       WfsdKey == turnaroundTimeRawDataRow.WfsdKey &&
                       WfsdDescription == turnaroundTimeRawDataRow.WfsdDescription &&
                       IsIntervention == turnaroundTimeRawDataRow.IsIntervention &&
                       IsException == turnaroundTimeRawDataRow.IsException &&
                       ElapsedDaysInStep == turnaroundTimeRawDataRow.ElapsedDaysInStep &&
                       ElapsedDaysInStepOffHrs == turnaroundTimeRawDataRow.ElapsedDaysInStepOffHrs &&
                       DateInRank == turnaroundTimeRawDataRow.DateInRank &&
                       DateOutRank == turnaroundTimeRawDataRow.DateOutRank;
            }
            return false;
        }

        /// <inheritdoc />
        public bool IsMatchingOnAllNonderivedProperties(IDataRow recordToCompare)
        {
            if (recordToCompare is TurnaroundTimeRawDataRow turnaroundTimeRawDataRow)
            {
                //These fields can be considered as the primary key fields for this data row, and are essentially populated from the data source rather than derived data elements.
                return OrderNbr == turnaroundTimeRawDataRow.OrderNbr &&
                       Facility == turnaroundTimeRawDataRow.Facility &&
                       McKessonPatientKey == turnaroundTimeRawDataRow.McKessonPatientKey &&
                       RxNbr == turnaroundTimeRawDataRow.RxNbr &&
                       RefillNbr == turnaroundTimeRawDataRow.RefillNbr &&
                       DateIn == turnaroundTimeRawDataRow.DateIn &&
                       DateOut == turnaroundTimeRawDataRow.DateOut &&
                       WfsdKey == turnaroundTimeRawDataRow.WfsdKey &&
                       DateInRank == turnaroundTimeRawDataRow.DateInRank &&
                       DateOutRank == turnaroundTimeRawDataRow.DateOutRank;
            }
            return false;
        }

        /// <inheritdoc />
        public List<string> GetListOfDerivedDataPropertyMismatches(IDataRow recordToCompare, bool endUsersApproveDecimalDifferencesWithinTolerances = false)
        {
            List<string> returnDerivedDataProperties = new List<string>();
            decimal maxDecimalDifference = endUsersApproveDecimalDifferencesWithinTolerances? _maxDecimalRoundingDifference : 0M;

            if (recordToCompare is TurnaroundTimeRawDataRow turnaroundTimeRawDataRow)
            { 
                if (IsMatchingOnAllNonderivedProperties(recordToCompare))
                { 
                    //All of these fields below are derived data fields that were populated by complex business logic programming that may need to be under development during the QA process.
                    if (IsIntervention != turnaroundTimeRawDataRow.IsIntervention)
                    {
                        returnDerivedDataProperties.Add($"IsIntervention=[{IsIntervention}] CompareToValue=[{turnaroundTimeRawDataRow.IsIntervention}]");
                    }
                    if (IsException != turnaroundTimeRawDataRow.IsException)
                    {
                        returnDerivedDataProperties.Add($"IsException=[{IsException}] CompareToValue=[{turnaroundTimeRawDataRow.IsException}]");
                    }
                    if (Math.Abs((ElapsedDaysInStep ?? 0M) - (turnaroundTimeRawDataRow.ElapsedDaysInStep ?? 0M)) > maxDecimalDifference)
                    {
                        returnDerivedDataProperties.Add($"ElapsedDaysInStep=[{ElapsedDaysInStep}] CompareToValue=[{turnaroundTimeRawDataRow.ElapsedDaysInStep}] difference is [{((ElapsedDaysInStep ?? 0M) - (turnaroundTimeRawDataRow.ElapsedDaysInStep ?? 0M))}]");
                    }
                    if (Math.Abs((ElapsedDaysInStepOffHrs ?? 0M) - (turnaroundTimeRawDataRow.ElapsedDaysInStepOffHrs ?? 0M)) > maxDecimalDifference)
                    {
                        returnDerivedDataProperties.Add($"ElapsedDaysInStepOffHrs=[{ElapsedDaysInStepOffHrs}] CompareToValue=[{turnaroundTimeRawDataRow.ElapsedDaysInStepOffHrs}] difference is [{((ElapsedDaysInStepOffHrs ?? 0M) - (turnaroundTimeRawDataRow.ElapsedDaysInStepOffHrs ?? 0M))}]");
                    }
                }
            }

            return returnDerivedDataProperties;
        }
    }
}
