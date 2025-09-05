namespace XXXDeveloperTools.CompareDerivedData.Helper
{
    /// <summary>
    /// This enum is used to return the results of the comparison of two files.
    /// If the files are perfectly identical, or only different by sort order, then see Project "YYYQATools.CompareOutputFiles".
    /// One reason while your files might not be match using "YYYQATools.CompareOutputFiles" is due to data formatting, so now this tool "XXXDeveloperTools.CompareDerivedData" can compare values using a variety of formatting.
    /// We might have a conditional success when the values have different formatting, and/or different decimal rounding behavior.
    /// The success is conditionalIF we have end user approval for each reason why the two files are not perfectly identical.
    /// We clearly have a failure when the record values within the files are not identical.
    /// </summary>
    public enum CompareDerivedDataReturnCodes
    {
        /* If the files are perfectly identical, or only different by sort order, then see Project "YYYQATools.CompareOutputFiles".
        Success_FilesArePerfectlyIdentical = 0,
        ConditionalSuccess_LinesFromFilesMatchButHaveDifferentSortOrder = 1,
        */
        ConditionalSuccess_DataValuesMatchWithSameSortOrder = 2,
        ConditionalSuccess_DataValuesMatchWithDifferentSortOrder = 3,
        ConditionalSuccess_DataValuesMatchWithCloseDecimalRounding = 4,
        Failure_IrreconcilableDifferencesWereFound = 11,
        Failure_UnableToCompareFiles = 12
    }
}
