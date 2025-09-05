namespace Library.LibraryUtilities.SqlFileReader
{
    public interface ISqlFileReader
    {
        /// <summary>
        /// Returns the contents of the SQL file at the given <paramref name="sqlFilePath"/>
        /// </summary>
        Task<string> GetSqlFileContentsAsync(string sqlFilePath, CancellationToken c);
    }
}
