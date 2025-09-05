namespace YYYQATools.CompareOutputFiles.Helper
{
    public interface ICompareCollectionsHelper
    {
        /// <summary>
        /// Compare the two collections.
        /// </summary>
        /// <returns></returns>
        int CompareCollections();

        /// <summary>
        /// Get a summary report of the comparison.
        /// </summary>
        /// <param name="limitReportingOfMismatches"></param>
        /// <returns></returns>
        string GetCompareSummaryReport(int limitReportingOfMismatches = 100);
    }
}
