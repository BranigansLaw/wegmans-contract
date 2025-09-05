using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class PatientAddressesQuery : AbstractQueryConfiguration<PatientAddressesRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/Patient_Addresses_YYYYMMDD.sql";

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
        public override PatientAddressesRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating PatientAddressesRow");
            return new PatientAddressesRow
            {
                PadrKey = reader.GetValueByIndex<long>(0),
                PatientNum = reader.GetNullableValueByIndex<long>(1),
                AddressOne = reader.GetStringByIndex(2),
                AddressTwo = reader.GetStringByIndex(3),
                AddrCity = reader.GetStringByIndex(4),
                AddrState = reader.GetStringByIndex(5),
                AddrZipcode = reader.GetStringByIndex(6),
                AddressZipext = reader.GetStringByIndex(7),
                County = reader.GetStringByIndex(8),
                AddressType = reader.GetStringByIndex(9),
                AddressUsage = reader.GetStringByIndex(10),
                AddrStatus = reader.GetStringByIndex(11),
                AreaCode = reader.GetStringByIndex(12),
                PhoneNum = reader.GetStringByIndex(13),
                PhoneExt = reader.GetStringByIndex(14),
                AddressUpdated = reader.GetValueByIndex<long>(15),
                PhoneUpdated = reader.GetValueByIndex<long>(16),
                Dwaddressnum = reader.GetNullableValueByIndex<long>(17),
                Dwphonenum = reader.GetNullableValueByIndex<long>(18),
            };
        }
    }
}
