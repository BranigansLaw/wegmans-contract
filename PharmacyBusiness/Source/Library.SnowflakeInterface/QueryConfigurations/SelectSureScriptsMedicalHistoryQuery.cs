using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class SelectSureScriptsMedicalHistoryQuery : AbstractQueryConfiguration<SelectSureScriptsMedicalHistoryRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/SelectSureScriptsMedicalHistory.sql";

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
        public override SelectSureScriptsMedicalHistoryRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating SelectSureScriptsMedicalHistoryRow");
            return new SelectSureScriptsMedicalHistoryRow {
                Recordsequencenumber = reader.GetValueByIndex<long>(0),
                Participantpatientid = reader.GetNullableValueByIndex<long>(1),
                Patientlastname = reader.GetStringByIndex(2),
                Patientfirstname = reader.GetStringByIndex(3),
                Patientmiddlename = reader.GetStringByIndex(4),
                Patientprefix = reader.GetStringByIndex(5),
                Patientsufix = reader.GetStringByIndex(6),
                Patientdateofbirth = reader.GetNullableValueByIndex<DateTime>(7),
                Patientgender = reader.GetStringByIndex(8),
                Patientaddress1 = reader.GetStringByIndex(9),
                Patientcity = reader.GetStringByIndex(10),
                Patientstate = reader.GetStringByIndex(11),
                Patientzipcode = reader.GetStringByIndex(12),
                Ncpdpid = reader.GetStringByIndex(13),
                Chainsiteid = reader.GetStringByIndex(14),
                Pharmacyname = reader.GetStringByIndex(15),
                Facilityaddress1 = reader.GetStringByIndex(16),
                Facilitycity = reader.GetStringByIndex(17),
                Facilitystate = reader.GetStringByIndex(18),
                Facilityzipcode = reader.GetStringByIndex(19),
                Facilityphonenumber = reader.GetStringByIndex(20),
                Fprimarycareproviderlastname = reader.GetStringByIndex(21),
                Primarycareproviderfirstname = reader.GetStringByIndex(22),
                Primarycareprovideraddress1 = reader.GetStringByIndex(23),
                Primarycareprovidercity = reader.GetStringByIndex(24),
                Primarycareproviderstate = reader.GetStringByIndex(25),
                Primarycareproviderzipcode = reader.GetStringByIndex(26),
                Primarycareproviderareacode = reader.GetStringByIndex(27),
                Primarycareproviderphonenumber = reader.GetStringByIndex(28),
                Prescriptionnumber = reader.GetStringByIndex(29),
                Fillnumber = reader.GetNullableValueByIndex<long>(30),
                Ndcnumberdispensed = reader.GetStringByIndex(31),
                Medicationname = reader.GetStringByIndex(32),
                Quantityprescribed = reader.GetNullableValueByIndex<decimal>(33),
                Quantitydispensed = reader.GetNullableValueByIndex<decimal>(34),
                Dayssupply = reader.GetNullableValueByIndex<long>(35),
                Sigtext = reader.GetStringByIndex(36),
                Datewritten = reader.GetNullableValueByIndex<long>(37),
                Datefilled = reader.GetNullableValueByIndex<long>(38),
                Datepickedupdispensed = reader.GetNullableValueByIndex<long>(39),
                Refillsoriginallyauthorized = reader.GetNullableValueByIndex<decimal>(40),
                Refillsremaining = reader.GetNullableValueByIndex<decimal>(41),
                LogicFillfactkey = reader.GetValueByIndex<long>(42),
                LogicPdpatientkey = reader.GetNullableValueByIndex<long>(43),
                LogicPatientaddressusage = reader.GetStringByIndex(44),
                LogicPatientaddresscreatedate = reader.GetNullableValueByIndex<DateTime>(45),
                LogicPresphonekey = reader.GetNullableValueByIndex<long>(46),
                LogicPresphonestatus = reader.GetStringByIndex(47),
                LogicPresphonesourcecode = reader.GetStringByIndex(48),
                LogicPresphonehlevel = reader.GetNullableValueByIndex<DateTime>(49),
            };
        }
    }
}
