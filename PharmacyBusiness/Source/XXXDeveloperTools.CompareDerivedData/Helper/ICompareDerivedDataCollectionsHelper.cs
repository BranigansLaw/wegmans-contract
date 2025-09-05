namespace XXXDeveloperTools.CompareDerivedData.Helper
{
    public interface ICompareDerivedDataCollectionsHelper
    {
        /// <summary>
        /// Compare old to new, and new to old collections.
        /// The maximum number of issues to appear in the logging because without this comparing hundreds of thousands of records can be overwhelming.
        /// Also, it is a good practice to resolve one issue at a time. So, keep fixing things until you get zero mismatches.
        /// </summary>
        void CompareCollections();
    }
}
