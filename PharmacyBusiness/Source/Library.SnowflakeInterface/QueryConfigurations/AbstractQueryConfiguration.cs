using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public abstract class AbstractQueryConfiguration<T> where T : class
    {
        /// <summary>
        /// The path to the query file that should be run
        /// </summary>
        public abstract string QueryFilePath { get; }

        /// <summary>
        /// Maps the given <see cref="DataSet"/> into type <typeparamref name="T"/>
        /// </summary>
        public abstract T MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug);

        /// <summary>
        /// Add parameters to the command before it runs
        /// </summary>
        public virtual void AddParameters(DbCommand command, Action<string> logDebug) { }
    }
}
