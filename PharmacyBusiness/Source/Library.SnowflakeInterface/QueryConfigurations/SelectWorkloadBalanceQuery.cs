using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class SelectWorkloadBalanceQuery : AbstractQueryConfiguration<SelectWorkloadBalanceRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/SelectWorkloadBalance.sql";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter paramRunDate = command.CreateParameter();
            paramRunDate.ParameterName = "RUNDATE";
            paramRunDate.DbType = DbType.Date;
            paramRunDate.Value = new DateTime(RunDate.Year, RunDate.Month, RunDate.Day);
            command.Parameters.Add(paramRunDate);
        }

        /// <inheritdoc />
        public override SelectWorkloadBalanceRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating SelectWorkloadBalance");
            throw new NotImplementedException();
        }
    }
}
