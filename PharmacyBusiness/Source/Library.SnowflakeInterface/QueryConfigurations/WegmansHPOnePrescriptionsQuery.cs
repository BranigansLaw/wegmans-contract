using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class WegmansHPOnePrescriptionsQuery : AbstractQueryConfiguration<WegmansHPOnePrescriptionsRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/Wegmans_HPOne_Prescriptions_YYYYMMDD.sql";

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
        public override WegmansHPOnePrescriptionsRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating WegmansHPOnePrescriptions");
            return new WegmansHPOnePrescriptionsRow
            {
                Version = reader.GetValueByIndex<long>(0),
                ClientPatientId = reader.GetNullableValueByIndex<long>(1),
                LastName = reader.GetStringByIndex(2),
                FirstName = reader.GetStringByIndex(3),
                Gender = reader.GetStringByIndex(4),
                AddressLine1 = reader.GetStringByIndex(5),
                AddressLine2 = reader.GetStringByIndex(6),
                City = reader.GetStringByIndex(7),
                State = reader.GetStringByIndex(8),
                Zipcode = reader.GetStringByIndex(9),
                Dob = reader.GetStringByIndex(10),
                Email = reader.GetStringByIndex(11),
                Phone = reader.GetStringByIndex(12),
                PhoneType = reader.GetStringByIndex(13),
                Language = reader.GetStringByIndex(14),
                Ndc = reader.GetStringByIndex(15),
                Qty = reader.GetValueByIndex<decimal>(16),
                DaysSupply = reader.GetValueByIndex<decimal>(17),
                ProductName = reader.GetStringByIndex(18),
                FillDate = reader.GetNullableValueByIndex<long>(19),
                FillNum = reader.GetNullableValueByIndex<long>(20),
                AuthorizedRefills = reader.GetValueByIndex<decimal>(21),
                Npi = reader.GetStringByIndex(22),
                MedicareContractId = reader.GetStringByIndex(23),
                MedicarePlanId = reader.GetStringByIndex(24),
                Bin = reader.GetStringByIndex(25),
                Pcn = reader.GetStringByIndex(26),
                GroupId = reader.GetStringByIndex(27),
                Plan = reader.GetStringByIndex(28),
                PrescriberNpi = reader.GetStringByIndex(29),
                Medicared = reader.GetStringByIndex(30),
                MedicareId = reader.GetStringByIndex(31),
            };
        }
    }
}
