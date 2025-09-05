using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class McKessonDWNaloxoneQuery : AbstractQueryConfiguration<McKessonDWNaloxoneRow>
    {
        public required string PrescriberNPicsV { get; set; }

        public required string NdcCsv { get; set; }

        public required string ReportingName { get; set; }

        public required string ReportingPhone { get; set; }

        public required string ReportingEmail { get; set; }

        public required DateOnly StartDate { get; set; }

        public required DateOnly EndDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/McKesson_DW_Naloxone.sql";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter paramPrescribernpicsv = command.CreateParameter();
            paramPrescribernpicsv.ParameterName = "PRESCRIBERNPICSV";
            paramPrescribernpicsv.DbType = DbType.String;
            paramPrescribernpicsv.Value = PrescriberNPicsV;
            command.Parameters.Add(paramPrescribernpicsv);

            DbParameter paramNdccsv = command.CreateParameter();
            paramNdccsv.ParameterName = "NDCCSV";
            paramNdccsv.DbType = DbType.String;
            paramNdccsv.Value = NdcCsv;
            command.Parameters.Add(paramNdccsv);

            DbParameter paramReportingname = command.CreateParameter();
            paramReportingname.ParameterName = "REPORTINGNAME";
            paramReportingname.DbType = DbType.String;
            paramReportingname.Value = ReportingName;
            command.Parameters.Add(paramReportingname);

            DbParameter paramReportingphone = command.CreateParameter();
            paramReportingphone.ParameterName = "REPORTINGPHONE";
            paramReportingphone.DbType = DbType.String;
            paramReportingphone.Value = ReportingPhone;
            command.Parameters.Add(paramReportingphone);

            DbParameter paramReportingemail = command.CreateParameter();
            paramReportingemail.ParameterName = "REPORTINGEMAIL";
            paramReportingemail.DbType = DbType.String;
            paramReportingemail.Value = ReportingEmail;
            command.Parameters.Add(paramReportingemail);

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
        public override McKessonDWNaloxoneRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating McKessonDWNaloxone");
            throw new NotImplementedException();
        }
    }
}
