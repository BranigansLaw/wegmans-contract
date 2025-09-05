using Library.SnowflakeInterface.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class ETLInfoForLoggingOracleDWQuery : AbstractQueryConfiguration<ETLInfoForLoggingDWRow>
    {

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/ETLInfoForLogging_Oracle_DW.sql";

        /// <inheritdoc />
        public override ETLInfoForLoggingDWRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating ETLInfoForLoggingOracleDW");
            throw new NotImplementedException();
        }
    }
}
