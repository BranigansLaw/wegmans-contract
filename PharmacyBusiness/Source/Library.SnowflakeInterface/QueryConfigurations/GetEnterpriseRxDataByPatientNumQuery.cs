using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class GetEnterpriseRxDataByPatientNumQuery : AbstractQueryConfiguration<GetEnterpriseRxDataByPatientNumRow>
    {
        public required string PatientNumList { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/GetEnterpriseRxDataByPatientNum.sql";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter paramPatientnumlist = command.CreateParameter();
            paramPatientnumlist.ParameterName = "PATIENTNUMLIST";
            paramPatientnumlist.DbType = DbType.String;
            paramPatientnumlist.Value = PatientNumList;
            command.Parameters.Add(paramPatientnumlist);
        }

        /// <inheritdoc />
        public override GetEnterpriseRxDataByPatientNumRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating GetEnterpriseRxDataByPatientNum");
            throw new NotImplementedException();
        }
    }
}
