using System.Collections.ObjectModel;

namespace Library.McKessonDWInterface.Exceptions
{
    public class DataIntegrityException : Exception
    { 
        public string? DataSourceName { get; }

        public string? DataRowConstraintViolations { get; }

        public Collection<string>? DataRowConstraintViolationsCollection { get; }

        public string? DataSourceSummaryOfConstraintViolations { get; }

        public IEnumerable<string>? DataSourceConstraintViolations { get; }

        /// <summary>
        /// Data integrity issues with a single data line.
        /// </summary>
        /// <param name="dataRowConstraintViolations"></param>
        public DataIntegrityException(string dataRowConstraintViolations) : base(dataRowConstraintViolations)
        {
            DataRowConstraintViolations = dataRowConstraintViolations;
        }

        /// <summary>
        /// Data integrity issues with a single data line.
        /// </summary>
        /// <param name="dataRowConstraintViolationsCollection"></param>
        public DataIntegrityException(Collection<string> dataRowConstraintViolationsCollection) : base(string.Join(" ", dataRowConstraintViolationsCollection.ToArray()))
        {
            DataRowConstraintViolationsCollection = dataRowConstraintViolationsCollection;
            DataRowConstraintViolations = string.Join(" ", dataRowConstraintViolationsCollection.ToArray());
        }

        /// <summary>
        /// Data integrity issues with a data file.
        /// </summary>
        /// <param name="dataSourceDataRowCount"></param>
        /// <param name="dataSourceConstraintViolations"></param>
        public DataIntegrityException(string dataSourceName, long dataSourceDataRowCount, IEnumerable<string> dataSourceConstraintViolations) : base($"Data from [{dataSourceName}] has data integrity issues.")
        {
            DataSourceName = dataSourceName;
            int percentRejected = (int)Math.Round(((double)dataSourceConstraintViolations.ToList().Count / (double)dataSourceDataRowCount) * 100);
            DataSourceSummaryOfConstraintViolations = $"Data from [{dataSourceName}] has [{dataSourceConstraintViolations.ToList().Count}] data rows rejected due to data integrity issues, which is [{percentRejected}%] of the entire data feed.";
            DataSourceConstraintViolations = dataSourceConstraintViolations;
        }
    }
}
