using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class WEG08601For198Query : AbstractQueryConfiguration<WEG08601For198Row>
    {
        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/WEG_086_YYYYMMDD_01_For198.sql";

        /// <inheritdoc />
        public override WEG08601For198Row MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating WEG08601For198");
            throw new NotImplementedException();
        }
    }
}
