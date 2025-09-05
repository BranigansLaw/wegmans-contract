using Library.SnowflakeInterface.QueryConfigurations;

namespace Library.SnowflakeInterface
{
    public interface ISnowflakeInterface
    {
        /// <summary>
        /// Run the given <paramref name="queryConfiguration"/> in snowflake
        /// </summary>
        /// <typeparam name="T">The type of data to map to</typeparam>
        Task<IEnumerable<T>> QuerySnowflakeAsync<T>(AbstractQueryConfiguration<T> queryConfiguration, CancellationToken c) where T : class;
    }
}
