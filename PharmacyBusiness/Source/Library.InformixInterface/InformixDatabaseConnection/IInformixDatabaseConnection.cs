namespace Library.InformixInterface.InformixDatabaseConnection
{
    public interface IInformixDatabaseConnection
    {
        Task<IEnumerable<object[]>> RunQueryAsync(string sql, IDictionary<string, string> parameters, CancellationToken c);
    }
}
