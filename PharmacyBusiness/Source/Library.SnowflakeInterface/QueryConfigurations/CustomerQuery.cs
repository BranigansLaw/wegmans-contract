using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class CustomerQuery : AbstractQueryConfiguration<TestCustomerRow>
    {
        /// <summary>
        /// The run data specified for this query
        /// </summary>
        public required DateOnly RunDate { get; set; }

        /// <summary>
        /// The number of records to return
        /// </summary>
        public required int Limit { get; set; }

        /// <summary>
        /// Return only customers who's first name contains the following string
        /// </summary>
        public required string FirstNameContains { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/CustomerQuery.sql";

        /// <inheritdoc />
        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter runDataParam = command.CreateParameter();
            runDataParam.ParameterName = "RunDate";
            runDataParam.DbType = DbType.Date;
            runDataParam.Value = new DateTime(RunDate.Year, RunDate.Month, RunDate.Day);
            command.Parameters.Add(runDataParam);

            DbParameter limitParam = command.CreateParameter();
            limitParam.ParameterName = "Limit";
            limitParam.DbType = DbType.Int32;
            limitParam.Value = Limit;
            command.Parameters.Add(limitParam);

            DbParameter firstNameContainsParam = command.CreateParameter();
            firstNameContainsParam.ParameterName = "FirstNameContains";
            firstNameContainsParam.DbType = DbType.String;
            firstNameContainsParam.Value = FirstNameContains;
            command.Parameters.Add(firstNameContainsParam);
        }

        /// <inheritdoc />
        public override TestCustomerRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating TestCustomer");
            return new TestCustomerRow
            {
                Id = reader.GetString(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Email = reader.GetString(3),
            };
        }
    }
}
