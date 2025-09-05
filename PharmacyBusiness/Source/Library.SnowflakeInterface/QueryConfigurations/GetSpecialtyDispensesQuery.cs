using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class GetSpecialtyDispensesQuery : AbstractQueryConfiguration<GetSpecialtyDispensesRow>
    {
        public required DateOnly StartDate { get; set; }

        public required DateOnly EndDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/GetSpecialtyDispenses.sql";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter paramStartdate = command.CreateParameter();
            paramStartdate.ParameterName = "STARTDATE";
            paramStartdate.DbType = DbType.Date;
            paramStartdate.Value = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day);
            command.Parameters.Add(paramStartdate);

            DbParameter paramEnddate = command.CreateParameter();
            paramEnddate.ParameterName = "ENDDATE";
            paramEnddate.DbType = DbType.Date;
            paramEnddate.Value = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day);
            command.Parameters.Add(paramEnddate);
        }

        /// <inheritdoc />
        public override GetSpecialtyDispensesRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating GetSpecialtyDispenses");
            throw new NotImplementedException();
        }
    }
}
