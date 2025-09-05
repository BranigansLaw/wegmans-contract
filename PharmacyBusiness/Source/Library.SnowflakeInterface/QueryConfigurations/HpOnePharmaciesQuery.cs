using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class HpOnePharmaciesQuery : AbstractQueryConfiguration<HpOnePharmaciesRow>
    {

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/Wegmans_HPOne_Pharmacies_YYYYMMDD.sql";

        /// <inheritdoc />
        public override HpOnePharmaciesRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating HpOnePharmacies");
            return new HpOnePharmaciesRow
            {
                Npi = reader.GetStringByIndex(0),
                PharmacyName = reader.GetStringByIndex(1),
                AddressLine1 = reader.GetStringByIndex(2),
                AddressLine2 = reader.GetStringByIndex(3),
                City = reader.GetStringByIndex(4),
                State = reader.GetStringByIndex(5),
                ZipCode = reader.GetStringByIndex(6),
                StorePhoneNumber = reader.GetStringByIndex(7),
                FaxPhoneNumber = reader.GetStringByIndex(8),
                PaidSearchRadius = reader.GetStringByIndex(9),
                Banner = reader.GetStringByIndex(10),
            };
        }
    }
}
