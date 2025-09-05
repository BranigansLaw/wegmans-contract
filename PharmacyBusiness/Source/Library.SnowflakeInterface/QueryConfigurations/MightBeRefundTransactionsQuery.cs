using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class MightBeRefundTransactionsQuery : AbstractQueryConfiguration<MightBeRefundTransactionsRow>
    {
        public required string QueryDateString { get; set; }

        public required string QueryDateDateType { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/MightBeRefundTransactions.sql";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter paramQuerydatestr = command.CreateParameter();
            paramQuerydatestr.ParameterName = "QUERYDATESTR";
            paramQuerydatestr.DbType = DbType.String;
            paramQuerydatestr.Value = QueryDateString;
            command.Parameters.Add(paramQuerydatestr);

            DbParameter paramQuerydatedt = command.CreateParameter();
            paramQuerydatedt.ParameterName = "QUERYDATEDT";
            paramQuerydatedt.DbType = DbType.String;
            paramQuerydatedt.Value = QueryDateDateType;
            command.Parameters.Add(paramQuerydatedt);
        }

        /// <inheritdoc />
        public override MightBeRefundTransactionsRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating MightBeRefundTransactions");
            throw new NotImplementedException();
        }
    }
}
