using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class FdsPharmaciesQuery : AbstractQueryConfiguration<FdsPharmaciesRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/FDS_Pharmacies.sql";

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
        public override FdsPharmaciesRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating FdsPharmacies");
            return new FdsPharmaciesRow
            {
                NcpdpId = reader.GetStringByIndex(0),
                Npi = reader.GetStringByIndex(1),
                PharmacyName = reader.GetStringByIndex(2),
                AddressLine1 = reader.GetStringByIndex(3),
                AddressLine2 = reader.GetStringByIndex(4),
                City = reader.GetStringByIndex(5),
                State = reader.GetStringByIndex(6),
                ZipCode = reader.GetStringByIndex(7),
                StorePhoneNumber = reader.GetStringByIndex(8),
                FaxPhoneNumber = reader.GetStringByIndex(9),
                Banner = reader.GetStringByIndex(10),
            };
        }
    }
}
