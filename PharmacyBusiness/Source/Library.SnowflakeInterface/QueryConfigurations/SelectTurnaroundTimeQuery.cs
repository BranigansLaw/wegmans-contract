using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class SelectTurnaroundTimeQuery : AbstractQueryConfiguration<SelectTurnaroundTimeRow>
    {
        public required DateOnly StartDate { get; set; }

        public required DateOnly EndDate { get; set; }

        public required string TatTarget { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/SelectTurnaroundTime.sql";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter paramStart_date = command.CreateParameter();
            paramStart_date.ParameterName = "START_DATE";
            paramStart_date.DbType = DbType.Date;
            paramStart_date.Value = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day);
            command.Parameters.Add(paramStart_date);

            DbParameter paramEnd_date = command.CreateParameter();
            paramEnd_date.ParameterName = "END_DATE";
            paramEnd_date.DbType = DbType.Date;
            paramEnd_date.Value = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day);
            command.Parameters.Add(paramEnd_date);

            DbParameter paramTat_target = command.CreateParameter();
            paramTat_target.ParameterName = "TAT_TARGET";
            paramTat_target.DbType = DbType.String;
            paramTat_target.Value = TatTarget;
            command.Parameters.Add(paramTat_target);
        }

        /// <inheritdoc />
        public override SelectTurnaroundTimeRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating SelectTurnaroundTime");
            throw new NotImplementedException();
        }
    }
}
