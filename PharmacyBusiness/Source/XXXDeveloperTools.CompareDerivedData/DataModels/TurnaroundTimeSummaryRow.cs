using System.Collections.ObjectModel;

namespace XXXDeveloperTools.CompareDerivedData.DataModels
{
    public class TurnaroundTimeSummaryRow : IDataRow
    {
        private const decimal _maxDecimalRoundingDifference = 0.000001M;

        public string? OrderNbr { get; set; }
        public string? Facility { get; set; }
        public string? RxNbr { get; set; }
        public long? RefillNbr { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public decimal? DaysOverall { get; set; }
        public decimal? DeductDaysIntervention { get; set; }
        public decimal? DeductDaysOffHours { get; set; }
        public decimal? DaysNetTat { get; set; }
        public string? HasExceptions { get; set; }

        /// <inheritdoc />
        public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns)
        {
            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();
            if (!long.TryParse(dataFileColumns.ElementAt(3), out long refillNbr))
                constraintViolations.Add("RefillNbr is not a long.");
            if (!DateTime.TryParse(dataFileColumns.ElementAt(4), out DateTime dateIn))
                constraintViolations.Add("DateIn is not a date.");
            if (!DateTime.TryParse(dataFileColumns.ElementAt(5), out DateTime dateOut))
                constraintViolations.Add("DateOut is not a date.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(6), out decimal daysOverall))
                constraintViolations.Add("DaysOverall is not a decimal.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(7), out decimal deductDaysIntervention))
                constraintViolations.Add("DeductDaysIntervention is not a decimal.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(8), out decimal deductDaysOffHours))
                constraintViolations.Add("DeductDaysOffHours is not a decimal.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(9), out decimal daysNetTat))
                constraintViolations.Add("DaysNetTat is not a decimal.");

            if (constraintViolations.ToArray().Length > 0)
            {
                //Note: This would only be a failure point during initial setup of a new job.
                throw new Exception(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                OrderNbr = dataFileColumns.ElementAt(0).Trim('"');
                Facility = dataFileColumns.ElementAt(1).Trim('"');
                RxNbr = dataFileColumns.ElementAt(2).Trim('"');
                RefillNbr = refillNbr;
                DateIn = dateIn;
                DateOut = dateOut;
                DaysOverall = daysOverall;
                DeductDaysIntervention = deductDaysIntervention;
                DeductDaysOffHours = deductDaysOffHours;
                DaysNetTat = daysNetTat;
                HasExceptions = dataFileColumns.ElementAt(10).Trim('"');
            }
        }

        /// <inheritdoc />
        public bool IsMatchingOnAllProperties(IDataRow recordToCompare)
        {
            if (recordToCompare is TurnaroundTimeSummaryRow turnaroundTimeRawDataRow)
            {
                return OrderNbr == turnaroundTimeRawDataRow.OrderNbr &&
                       Facility == turnaroundTimeRawDataRow.Facility &&
                       RxNbr == turnaroundTimeRawDataRow.RxNbr &&
                       RefillNbr == turnaroundTimeRawDataRow.RefillNbr &&
                       DateIn == turnaroundTimeRawDataRow.DateIn &&
                       DateOut == turnaroundTimeRawDataRow.DateOut &&
                       DaysOverall == turnaroundTimeRawDataRow.DaysOverall &&
                       DeductDaysIntervention == turnaroundTimeRawDataRow.DeductDaysIntervention &&
                       DeductDaysOffHours == turnaroundTimeRawDataRow.DeductDaysOffHours &&
                       DaysNetTat == turnaroundTimeRawDataRow.DaysNetTat &&
                       HasExceptions == turnaroundTimeRawDataRow.HasExceptions;
            }
            return false;
        }

        /// <inheritdoc />
        public bool IsMatchingOnAllNonderivedProperties(IDataRow recordToCompare)
        {
            if (recordToCompare is TurnaroundTimeSummaryRow turnaroundTimeRSummaryRow)
            {
                //These fields can be considered as the primary key fields for this data row, and are essentially populated from the data source rather than derived data elements.
                return OrderNbr == turnaroundTimeRSummaryRow.OrderNbr &&
                       Facility == turnaroundTimeRSummaryRow.Facility &&
                       RxNbr == turnaroundTimeRSummaryRow.RxNbr &&
                       RefillNbr == turnaroundTimeRSummaryRow.RefillNbr &&
                       DateIn == turnaroundTimeRSummaryRow.DateIn &&
                       DateOut == turnaroundTimeRSummaryRow.DateOut;
            }
            return false;
        }

        /// <inheritdoc />
        public List<string> GetListOfDerivedDataPropertyMismatches(IDataRow recordToCompare, bool endUsersApproveDecimalDifferencesWithinTolerances = false)
        {
            List<string> returnDerivedDataProperties = new List<string>();
            decimal maxDecimalDifference = endUsersApproveDecimalDifferencesWithinTolerances ? _maxDecimalRoundingDifference : 0M;

            if (recordToCompare is TurnaroundTimeSummaryRow turnaroundTimeRSummaryRow)
            {
                if (IsMatchingOnAllNonderivedProperties(recordToCompare))
                {
                    //All of these fields below are derived data fields that were populated by complex business logic programming that may need to be under development during the QA process.
                    if (Math.Abs((DaysOverall ?? 0M) - (turnaroundTimeRSummaryRow.DaysOverall ?? 0M)) > maxDecimalDifference)
                    {
                        returnDerivedDataProperties.Add($"DaysOverall=[{DaysOverall}] CompareToValue=[{turnaroundTimeRSummaryRow.DaysOverall}] difference is [{((DaysOverall ?? 0M) - (turnaroundTimeRSummaryRow.DaysOverall ?? 0M))}]");
                    }
                    if (Math.Abs((DeductDaysIntervention ?? 0M) - (turnaroundTimeRSummaryRow.DeductDaysIntervention ?? 0M)) > maxDecimalDifference)
                    {
                        returnDerivedDataProperties.Add($"DeductDaysIntervention=[{DeductDaysIntervention}] CompareToValue=[{turnaroundTimeRSummaryRow.DeductDaysIntervention}] difference is [{((DeductDaysIntervention ?? 0M) - (turnaroundTimeRSummaryRow.DeductDaysIntervention ?? 0M))}]");
                    }
                    if (Math.Abs((DeductDaysOffHours ?? 0M) - (turnaroundTimeRSummaryRow.DeductDaysOffHours ?? 0M)) > maxDecimalDifference)
                    {
                        returnDerivedDataProperties.Add($"DeductDaysOffHours=[{DeductDaysOffHours}] CompareToValue=[{turnaroundTimeRSummaryRow.DeductDaysOffHours}] difference is [{((DeductDaysOffHours ?? 0M) - (turnaroundTimeRSummaryRow.DeductDaysOffHours ?? 0M))}]");
                    }
                    if (Math.Abs((DaysNetTat ?? 0M) - (turnaroundTimeRSummaryRow.DaysNetTat ?? 0M)) > maxDecimalDifference)
                    {
                        returnDerivedDataProperties.Add($"DaysNetTat=[{DaysNetTat}] CompareToValue=[{turnaroundTimeRSummaryRow.DaysNetTat}] difference is [{((DaysNetTat ?? 0M) - (turnaroundTimeRSummaryRow.DaysNetTat ?? 0M))}]");
                    }
                }
            }

            return returnDerivedDataProperties;
        }
    }
}
