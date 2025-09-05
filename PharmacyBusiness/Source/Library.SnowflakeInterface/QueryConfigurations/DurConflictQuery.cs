using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class DurConflictQuery : AbstractQueryConfiguration<DurConflictRow>
    {
        
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/DurConflict_YYYYMMDD.sql";

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
        public override DurConflictRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating DurConflictRow");
            return new DurConflictRow {
                StoreNumber = reader.GetStringByIndex(0),
                RxNumber = reader.GetStringByIndex(1),
                RefillNumber = reader.GetNullableValueByIndex<long>(2),
                PartSeqNumber = reader.GetNullableValueByIndex<long>(3),
                DurDate = reader.GetNullableValueByIndex<DateTime>(4),
                PatientNumber = reader.GetNullableValueByIndex<long>(5),
                NdcWo = reader.GetStringByIndex(6),
                DrugName = reader.GetStringByIndex(7),
                Sdgi = reader.GetStringByIndex(8),
                Gcn = reader.GetStringByIndex(9),
                GcnSequenceNumber = reader.GetStringByIndex(10),
                DeaClass = reader.GetStringByIndex(11),
                ConflictCode = reader.GetStringByIndex(12),
                ConflictDesc = reader.GetStringByIndex(13),
                ConflictType = reader.GetStringByIndex(14),
                SeverityDesc = reader.GetStringByIndex(15),
                ResultOfService = reader.GetStringByIndex(16),
                ProfService = reader.GetStringByIndex(17),
                LevelOfEffort = reader.GetNullableValueByIndex<long>(18),
                ReasonForService = reader.GetStringByIndex(19),
                IsCritical = reader.GetStringByIndex(20),
                IsException = reader.GetStringByIndex(21),
                RxFillSequence = reader.GetNullableValueByIndex<long>(22),
                RxRecordNumber = reader.GetNullableValueByIndex<long>(23),
                PrescriberKey = reader.GetNullableValueByIndex<long>(24),
                UserKey = reader.GetNullableValueByIndex<long>(25),
            };
        }
    }
}
