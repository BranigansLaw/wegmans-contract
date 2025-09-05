using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class WorkersCompMonthlyQuery : AbstractQueryConfiguration<WorkersCompMonthly>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/WorkersCompMonthly_YYYYMMDD.sql";

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
        public override WorkersCompMonthly MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating WorkersCompMonthly");
            return new WorkersCompMonthly {
                FacilityIdNumber = reader.GetStringByIndex(0),
                RxNumber = reader.GetStringByIndex(1),
                DateFilled = reader.GetStringByIndex(2),
                PatientLastName = reader.GetStringByIndex(3),
                PatientFirstName = reader.GetStringByIndex(4),
                CardholderId = reader.GetStringByIndex(5),
                PatientDob = reader.GetStringByIndex(6),
                DrugNdcNumber = reader.GetStringByIndex(7),
                DrugName = reader.GetStringByIndex(8),
                QtyDrugDispensed = reader.GetNullableValueByIndex<long>(9),
                TxPrice = reader.GetValueByIndex<decimal>(10),
                PatientPay = reader.GetValueByIndex<decimal>(11),
                AdjAmt = reader.GetValueByIndex<decimal>(12),
                ThirdPartyName = reader.GetStringByIndex(13),
                ThirdPartyCode = reader.GetStringByIndex(14),
                SplitBillIndicator = reader.GetStringByIndex(15),
                PrescriberName = reader.GetStringByIndex(16),
                DateSold = reader.GetNullableValueByIndex<DateTime>(17),
                ClaimNumber = reader.GetValueByIndex<long>(18),
            };
        }
    }
}
