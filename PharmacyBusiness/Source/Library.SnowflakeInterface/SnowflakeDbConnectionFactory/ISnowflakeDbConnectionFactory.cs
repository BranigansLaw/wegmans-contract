using Snowflake.Data.Client;

namespace Library.SnowflakeInterface.SnowflakeDbConnectionFactory
{
    public interface ISnowflakeDbConnectionFactory
    {
        /// <summary>
        /// Creates a new <see cref="SnowflakeDbConnection"/>
        /// </summary>
        Task<SnowflakeDbConnection> CreateAsync(CancellationToken cancellationToken);
    }
}
