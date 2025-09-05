namespace XXXDeveloperTools.CompareDerivedData.DataModels
{
    public interface IDataRow
    {
        /// <summary>
        /// Set the properties of the record from the file row.
        /// </summary>
        /// <param name="dataFileColumns">Array of data fields from vendor data file.</param>
        void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns);

        /// <summary>
        /// Compare the properties of the record to the properties of another record.
        /// If they all match, return true.
        /// </summary>
        /// <param name="recordToCompare"></param>
        /// <returns></returns>
        bool IsMatchingOnAllProperties(IDataRow recordToCompare);

        /// <summary>
        /// Compare only raw data (nonderived data) properties of the record to the properties of another record.
        /// The primary properties are the properties that uniquely identify the record, like a primary key or a combination of properties.
        /// Properties populated by derived values based on business logic are excluded as these tend to be the biggest cause of mismatches.
        /// </summary>
        /// <param name="recordToCompare"></param>
        /// <returns></returns>
        bool IsMatchingOnAllNonderivedProperties(IDataRow recordToCompare);

        /// <summary>
        /// Compare the derived data properties of the record to the properties of another record.
        /// </summary>
        /// <param name="recordToCompare"></param>
        /// <param name="endUsersApproveDecimalDifferencesWithinTolerances">If true, then rounding differences are ignored.</param>
        /// <returns></returns>
        List<string> GetListOfDerivedDataPropertyMismatches(IDataRow recordToCompare, bool endUsersApproveDecimalDifferencesWithinTolerances);
    }
}
