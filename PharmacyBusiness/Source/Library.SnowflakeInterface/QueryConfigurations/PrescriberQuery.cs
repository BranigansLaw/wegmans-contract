using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class PrescriberQuery : AbstractQueryConfiguration<PrescriberRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/PRESCRIBER_YYYYMMDD.sql";

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
        public override PrescriberRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating Prescriber");
            return new PrescriberRow {
                PrescriberKey = reader.GetValueByIndex<long>(0),
                PrescriberNum = reader.GetNullableValueByIndex<long>(1),
                ActivePrescriberNum = reader.GetNullableValueByIndex<long>(2),
                TitleAbbr = reader.GetStringByIndex(3),
                FirstName = reader.GetStringByIndex(4),
                MiddleName = reader.GetStringByIndex(5),
                LastName = reader.GetStringByIndex(6),
                SuffixAbbr = reader.GetStringByIndex(7),
                GenderCode = reader.GetStringByIndex(8),
                Status = reader.GetStringByIndex(9),
                AmaActivity = reader.GetStringByIndex(10),
                InactiveCode = reader.GetStringByIndex(11),
                PrefGeneric = reader.GetStringByIndex(12),
                PrefTherSub = reader.GetStringByIndex(13),
                CreateUserKey = reader.GetNullableValueByIndex<long>(14),
                AddSource = reader.GetStringByIndex(15),
                SupvPrsNum = reader.GetNullableValueByIndex<long>(16),
                UniqPrsNum = reader.GetNullableValueByIndex<long>(17),
                PrescriberId = reader.GetStringByIndex(18),
                EsvMatch = reader.GetStringByIndex(19),
                IsEsvValid = reader.GetStringByIndex(20),
                CreateDate = reader.GetNullableValueByIndex<DateTime>(21),
                BirthDate = reader.GetNullableValueByIndex<DateTime>(22),
                DeceasedDate = reader.GetNullableValueByIndex<DateTime>(23),
                InactiveDate = reader.GetNullableValueByIndex<DateTime>(24),
                PpiEnabled = reader.GetStringByIndex(25),
                Npi = reader.GetStringByIndex(26),
                NpiBilling = reader.GetStringByIndex(27),
                NpiExpireDate = reader.GetNullableValueByIndex<DateTime>(28),
                StateLicNum = reader.GetStringByIndex(29),
                StateLicBilling = reader.GetStringByIndex(30),
                LicenseState = reader.GetStringByIndex(31),
                StateLicExpireDate = reader.GetNullableValueByIndex<DateTime>(32),
                DeaNum = reader.GetStringByIndex(33),
                DeaBilling = reader.GetStringByIndex(34),
                DeaExpireDate = reader.GetNullableValueByIndex<DateTime>(35),
                MedicaidNum = reader.GetStringByIndex(36),
                FedtaxidNum = reader.GetStringByIndex(37),
                StateissueNum = reader.GetStringByIndex(38),
                NarcdeaNum = reader.GetStringByIndex(39),
                NcpdpNum = reader.GetStringByIndex(40),
                LastUpdate = reader.GetNullableValueByIndex<DateTime>(41),
            };
        }
    }
}
