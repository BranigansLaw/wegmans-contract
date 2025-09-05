namespace YYYQATools.CompareOutputFiles.Helper
{
    /// <summary>
    /// This enum is used to return the results of the comparison of two files.
    /// We clearly have a success when the two files are identical.
    /// We might have a conditional success when the lines from the files match but have different sort order and end user approval for each reason why the two files are not perfectly identical, like sort order.
    /// We clearly have a failure when the record values within the files are not identical with or without a set sort order.
    /// </summary>
    public enum CompareReturnCodes
    {
        Success_FilesArePerfectlyIdentical = 0,
        ConditionalSuccess_LinesFromFilesMatchButHaveDifferentSortOrder = 1,
        Failure_IrreconcilableDifferencesWereFound = 11,
        Failure_UnableToCompareFiles = 12
    }
}
