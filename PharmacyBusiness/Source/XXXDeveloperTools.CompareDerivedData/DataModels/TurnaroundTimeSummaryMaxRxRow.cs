using System.Collections.ObjectModel;

namespace XXXDeveloperTools.CompareDerivedData.DataModels
{
    public class TurnaroundTimeSummaryMaxRxRow : IDataRow
    {
        private const decimal _maxDecimalRoundingDifference = 0.000001M;

        public string? OrderNbr { get; set; }
        public string? Facility { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public decimal? DaysOverall { get; set; }
        public decimal? DeductDaysIntervention { get; set; }
        public decimal? DeductDaysOffHours { get; set; }
        public decimal? DaysNetTat { get; set; }
        public int? CountRxInOrder { get; set; }
        public string? HasExceptions { get; set; }

        /// <inheritdoc />
        public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns)
        {
            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();
            if (!DateTime.TryParse(dataFileColumns.ElementAt(2), out DateTime dateIn))
                constraintViolations.Add("DateIn is not a date.");
            if (!DateTime.TryParse(dataFileColumns.ElementAt(3), out DateTime dateOut))
                constraintViolations.Add("DateOut is not a date.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(4), out decimal daysOverall))
                constraintViolations.Add("DaysOverall is not a decimal.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(5), out decimal deductDaysIntervention))
                constraintViolations.Add("DeductDaysIntervention is not a decimal.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(6), out decimal deductDaysOffHours))
                constraintViolations.Add("DeductDaysOffHours is not a decimal.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(7), out decimal daysNetTat))
                constraintViolations.Add("DaysNetTat is not a decimal.");
            if (!int.TryParse(dataFileColumns.ElementAt(8), out int countRxInOrder))
                constraintViolations.Add("CountRxInOrder is not an int.");

            if (constraintViolations.ToArray().Length > 0)
            {
                //Note: This would only be a failure point during initial setup of a new job.
                throw new Exception(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                OrderNbr = dataFileColumns.ElementAt(0).Trim('"');
                Facility = dataFileColumns.ElementAt(1).Trim('"');
                DateIn = dateIn;
                DateOut = dateOut;
                DaysOverall = daysOverall;
                DeductDaysIntervention = deductDaysIntervention;
                DeductDaysOffHours = deductDaysOffHours;
                DaysNetTat = daysNetTat;
                CountRxInOrder = countRxInOrder;
                HasExceptions = dataFileColumns.ElementAt(9).Trim('"');
            }
        }

        /// <inheritdoc />
        public bool IsMatchingOnAllProperties(IDataRow recordToCompare)
        {
            if (recordToCompare is TurnaroundTimeSummaryMaxRxRow turnaroundTimeSummaryMaxRxRow)
            {
                return OrderNbr == turnaroundTimeSummaryMaxRxRow.OrderNbr &&
                       Facility == turnaroundTimeSummaryMaxRxRow.Facility &&
                       DateIn == turnaroundTimeSummaryMaxRxRow.DateIn &&
                       DateOut == turnaroundTimeSummaryMaxRxRow.DateOut &&
                       DaysOverall == turnaroundTimeSummaryMaxRxRow.DaysOverall &&
                       DeductDaysIntervention == turnaroundTimeSummaryMaxRxRow.DeductDaysIntervention &&
                       DeductDaysOffHours == turnaroundTimeSummaryMaxRxRow.DeductDaysOffHours &&
                       DaysNetTat == turnaroundTimeSummaryMaxRxRow.DaysNetTat &&
                       CountRxInOrder == turnaroundTimeSummaryMaxRxRow.CountRxInOrder &&
                       HasExceptions == turnaroundTimeSummaryMaxRxRow.HasExceptions;
            }
            return false;
        }

        /// <inheritdoc />
        public bool IsMatchingOnAllNonderivedProperties(IDataRow recordToCompare)
        {
            if (recordToCompare is TurnaroundTimeSummaryMaxRxRow turnaroundTimeSummaryMaxRxRow)
            {
                //These fields can be considered as the primary key fields for this data row, and are essentially populated from the data source rather than derived data elements.
                return OrderNbr == turnaroundTimeSummaryMaxRxRow.OrderNbr &&
                       Facility == turnaroundTimeSummaryMaxRxRow.Facility &&
                       DateIn == turnaroundTimeSummaryMaxRxRow.DateIn &&
                       DateOut == turnaroundTimeSummaryMaxRxRow.DateOut;
            }
            return false;
        }

        /// <inheritdoc />
        public List<string> GetListOfDerivedDataPropertyMismatches(IDataRow recordToCompare, bool endUsersApproveDecimalDifferencesWithinTolerances = false)
        {
            List<string> returnDerivedDataProperties = new List<string>();
            decimal maxDecimalDifference = endUsersApproveDecimalDifferencesWithinTolerances ? _maxDecimalRoundingDifference : 0M;

            if (recordToCompare is TurnaroundTimeSummaryMaxRxRow turnaroundTimeSummaryMaxRxRow)
            {
                if (IsMatchingOnAllNonderivedProperties(recordToCompare))
                {
                    //All of these fields below are derived data fields that were populated by complex business logic programming that may need to be under development during the QA process.
                    if (Math.Abs((DaysOverall ?? 0M) - (turnaroundTimeSummaryMaxRxRow.DaysOverall ?? 0M)) > maxDecimalDifference)
                    {
                        returnDerivedDataProperties.Add($"DaysOverall=[{DaysOverall}] CompareToValue=[{turnaroundTimeSummaryMaxRxRow.DaysOverall}] difference is [{((DaysOverall ?? 0M) - (turnaroundTimeSummaryMaxRxRow.DaysOverall ?? 0M))}]");
                    }
                    if (Math.Abs((DeductDaysIntervention ?? 0M) - (turnaroundTimeSummaryMaxRxRow.DeductDaysIntervention ?? 0M)) > maxDecimalDifference)
                    {
                        returnDerivedDataProperties.Add($"DeductDaysIntervention=[{DeductDaysIntervention}] CompareToValue=[{turnaroundTimeSummaryMaxRxRow.DeductDaysIntervention}] difference is [{((DeductDaysIntervention ?? 0M) - (turnaroundTimeSummaryMaxRxRow.DeductDaysIntervention ?? 0M))}]");
                    }
                    if (Math.Abs((DeductDaysOffHours ?? 0M) - (turnaroundTimeSummaryMaxRxRow.DeductDaysOffHours ?? 0M)) > maxDecimalDifference)
                    {
                        returnDerivedDataProperties.Add($"DeductDaysOffHours=[{DeductDaysOffHours}] CompareToValue=[{turnaroundTimeSummaryMaxRxRow.DeductDaysOffHours}] difference is [{((DeductDaysOffHours ?? 0M) - (turnaroundTimeSummaryMaxRxRow.DeductDaysOffHours ?? 0M))}]");
                    }
                    if (Math.Abs((DaysNetTat ?? 0M) - (turnaroundTimeSummaryMaxRxRow.DaysNetTat ?? 0M)) > maxDecimalDifference)
                    {
                        returnDerivedDataProperties.Add($"DaysNetTat=[{DaysNetTat}] CompareToValue=[{turnaroundTimeSummaryMaxRxRow.DaysNetTat}] difference is [{((DaysNetTat ?? 0M) - (turnaroundTimeSummaryMaxRxRow.DaysNetTat ?? 0M))}]");
                    }
                    if (CountRxInOrder != turnaroundTimeSummaryMaxRxRow.CountRxInOrder)
                    {
                        returnDerivedDataProperties.Add($"CountRxInOrder=[{CountRxInOrder}] CompareToValue=[{turnaroundTimeSummaryMaxRxRow.CountRxInOrder}]");
                    }
                    if (HasExceptions != turnaroundTimeSummaryMaxRxRow.HasExceptions)
                    {
                        returnDerivedDataProperties.Add($"IsException=[{HasExceptions}] CompareToValue=[{turnaroundTimeSummaryMaxRxRow.HasExceptions}]");
                    }
                }
            }

            return returnDerivedDataProperties;
        }
    }
}
