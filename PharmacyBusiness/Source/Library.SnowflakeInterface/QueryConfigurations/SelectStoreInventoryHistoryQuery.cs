using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class SelectStoreInventoryHistoryQuery : AbstractQueryConfiguration<SelectStoreInventoryHistoryRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/SelectStoreInventoryHistory.sql";

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
        public override SelectStoreInventoryHistoryRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating SelectStoreInventoryHistoryRow");
            return new SelectStoreInventoryHistoryRow {
                DateOfService = reader.GetNullableValueByIndex<DateTime>(0),
                StoreNum = reader.GetStringByIndex(1),
                NdcWithoutDashes = reader.GetStringByIndex(2),
                DrugName = reader.GetStringByIndex(3),
                Sdgi = reader.GetStringByIndex(4),
                Gcn = reader.GetStringByIndex(5),
                GcnSeqNum = reader.GetStringByIndex(6),
                OrangeBookCode = reader.GetStringByIndex(7),
                FormCode = reader.GetStringByIndex(8),
                PackSize = reader.GetNullableValueByIndex<decimal>(9),
                TruePack = reader.GetNullableValueByIndex<decimal>(10),
                Pm = reader.GetStringByIndex(11),
                IsPreferred = reader.GetStringByIndex(12),
                LastAcqCostPack = reader.GetNullableValueByIndex<decimal>(13),
                LastAcqCostUnit = reader.GetNullableValueByIndex<decimal>(14),
                LastAcqCostDate = reader.GetNullableValueByIndex<DateTime>(15),
                OnHandQty = reader.GetNullableValueByIndex<decimal>(16),
                OnHandValue = reader.GetNullableValueByIndex<decimal>(17),
                CommitedQty = reader.GetNullableValueByIndex<decimal>(18),
                CommitedValue = reader.GetNullableValueByIndex<decimal>(19),
                TotalQty = reader.GetNullableValueByIndex<decimal>(20),
                TotalValue = reader.GetNullableValueByIndex<decimal>(21),
                AcqCostPack = reader.GetNullableValueByIndex<decimal>(22),
                AcqCostUnit = reader.GetNullableValueByIndex<decimal>(23),
                PrimarySupplier = reader.GetStringByIndex(24),
                LastChangeDate = reader.GetNullableValueByIndex<DateTime>(25),
            };
        }
    }
}
