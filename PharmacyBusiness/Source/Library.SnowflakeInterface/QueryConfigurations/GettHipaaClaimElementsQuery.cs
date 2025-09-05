using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class GettHipaaClaimElementsQuery : AbstractQueryConfiguration<GettHipaaClaimElementsRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/GettHipaaClaimElements.sql";

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
        public override GettHipaaClaimElementsRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating GettHipaaClaimElements");
            throw new NotImplementedException();
        }
    }
}
