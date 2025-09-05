using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class PrescriberAddressQuery : AbstractQueryConfiguration<PrescriberAddressRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/PRESCRIBERADDRESS_YYYYMMDD.sql";

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
        public override PrescriberAddressRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating PrescriberAddress");
            return new PrescriberAddressRow {
                PrescriberKey = reader.GetNullableValueByIndex<long>(0),
                PadrKey = reader.GetValueByIndex<long>(1),
                PrescribAddrNum = reader.GetValueByIndex<long>(2),
                AddrType = reader.GetStringByIndex(3),
                AddressOne = reader.GetStringByIndex(4),
                AddressTwo = reader.GetStringByIndex(5),
                AddrCity = reader.GetStringByIndex(6),
                Email = reader.GetStringByIndex(7),
                WebAddr = reader.GetStringByIndex(8),
                State = reader.GetStringByIndex(9),
                Zipcode = reader.GetStringByIndex(10),
                ZipExt = reader.GetStringByIndex(11),
                County = reader.GetStringByIndex(12),
                IsDefault = reader.GetStringByIndex(13),
                AddSource = reader.GetStringByIndex(14),
                DrugSched = reader.GetStringByIndex(15),
                PracticeName = reader.GetStringByIndex(16),
                AddrIdNum = reader.GetNullableValueByIndex<long>(17),
                PrefContact = reader.GetStringByIndex(18),
                AddrStatus = reader.GetStringByIndex(19),
                Npi = reader.GetStringByIndex(20),
                NpiBilling = reader.GetStringByIndex(21),
                NpiExpireDate = reader.GetNullableValueByIndex<DateTime>(22),
                StateLicNum = reader.GetStringByIndex(23),
                StateLicBilling = reader.GetStringByIndex(24),
                StateLicExpireDate = reader.GetNullableValueByIndex<DateTime>(25),
                LicenseState = reader.GetStringByIndex(26),
                DeaNum = reader.GetStringByIndex(27),
                DeaBilling = reader.GetStringByIndex(28),
                DeaExpireDate = reader.GetNullableValueByIndex<DateTime>(29),
                AreaCodePrim = reader.GetStringByIndex(30),
                PhoneNumPrim = reader.GetStringByIndex(31),
                ExtPrim = reader.GetStringByIndex(32),
                AreaCodeSec = reader.GetStringByIndex(33),
                PhoneNumSec = reader.GetStringByIndex(34),
                ExtSec = reader.GetStringByIndex(35),
                AreaCodeFax = reader.GetStringByIndex(36),
                PhoneNumFax = reader.GetStringByIndex(37),
                ExtFax = reader.GetStringByIndex(38),
                AreaCodeOther = reader.GetStringByIndex(39),
                PhoneNumOther = reader.GetStringByIndex(40),
                ExtOther = reader.GetStringByIndex(41),
                PhoneTypeOther2 = reader.GetStringByIndex(42),
                AreaCodeOther2 = reader.GetStringByIndex(43),
                PhoneNumOther2 = reader.GetStringByIndex(44),
                ExtOther2 = reader.GetStringByIndex(45),
                LastUpdate = reader.GetValueByIndex<DateTime>(46),
            };
        }
    }
}
