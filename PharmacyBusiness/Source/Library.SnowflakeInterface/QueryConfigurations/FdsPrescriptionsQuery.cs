using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class FdsPrescriptionsQuery : AbstractQueryConfiguration<FdsPrescriptionsRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/FDS_Prescriptions.SQL";

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
        public override FdsPrescriptionsRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating FdsPrescriptionsRow");
            return new FdsPrescriptionsRow
            {
                Version = reader.GetValueByIndex<long>(0),
                Clientpatientid = reader.GetNullableValueByIndex<long>(1),
                Lastname = reader.GetStringByIndex(2),
                Firstname = reader.GetStringByIndex(3),
                Gender = reader.GetStringByIndex(4),
                Addressline1 = reader.GetStringByIndex(5),
                Addressline2 = reader.GetStringByIndex(6),
                City = reader.GetStringByIndex(7),
                State = reader.GetStringByIndex(8),
                Zipcode = reader.GetStringByIndex(9),
                Dob = reader.GetStringByIndex(10),
                Email = reader.GetStringByIndex(11),
                Phone = reader.GetStringByIndex(12),
                Phonetype = reader.GetStringByIndex(13),
                Language = reader.GetStringByIndex(14),
                Ndc = reader.GetStringByIndex(15),
                Qty = reader.GetValueByIndex<decimal>(16),
                Dayssupply = reader.GetValueByIndex<decimal>(17),
                Productname = reader.GetStringByIndex(18),
                Filldate = reader.GetNullableValueByIndex<long>(19),
                Fillnum = reader.GetNullableValueByIndex<long>(20),
                Authorizedrefills = reader.GetValueByIndex<decimal>(21),
                Npi = reader.GetStringByIndex(22),
                Bin = reader.GetStringByIndex(23),
                Pcn = reader.GetStringByIndex(24),
                Groupid = reader.GetStringByIndex(25),
                Plan = reader.GetStringByIndex(26),
                Prescribernpi = reader.GetStringByIndex(27),
                Patientmbi = reader.GetStringByIndex(28),
                Copay = reader.GetNullableValueByIndex<decimal>(29),
                Reimbursement = reader.GetNullableValueByIndex<decimal>(30),
            };
        }
    }
}
