using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class RxTransferQuery : AbstractQueryConfiguration<RxTransferRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/RxTransfer_YYYYMMDD.sql";

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
        public override RxTransferRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating RxTransferRow ==>> )" + reader.GetString(1));
            return new RxTransferRow
            {
                BaseStoreNum = reader.GetString(0),
                BaseStoreName = reader.GetString(1),
                ToStoreNum = reader.GetString(2),
                ToStore = reader.GetString(3),
                FromStoreNum = reader.GetString(4),
                FromStore = reader.GetString(5),
                TransferDest = reader.GetString(6),
                PatientNum = reader.GetNullableValueByIndex<long>(7),
                OrigRxNum = reader.GetString(8),
                RefillNum = reader.GetNullableValueByIndex<long>(9),
                TransferDate = reader.GetNullableValueByIndex<DateTime>(10),
                SoldDate = reader.GetNullableValueByIndex<DateTime>(11),
                ReadyDate = reader.GetNullableValueByIndex<DateTime>(12),
                CancelDate = reader.GetNullableValueByIndex<DateTime>(13),
                TransferTimeKey = reader.GetNullableValueByIndex<long>(14),
                WrittenNdcWo = reader.GetString(15),
                WrittenDrugName = reader.GetString(16),
                DispNdcWo = reader.GetString(17),
                DispDrugName = reader.GetString(18),
                QtyDispensed = reader.GetNullableValueByIndex<long>(19),
                Daw = reader.GetString(20),
                TransType = reader.GetString(21),
                TransMethod = reader.GetString(22),
                SigText = reader.GetString(23),
                PrescriberKey = reader.GetNullableValueByIndex<long>(24),
                RxRecordNum = reader.GetNullableValueByIndex<long>(25),
                RxFillSeq = reader.GetNullableValueByIndex<long>(26),
                PatientPay = reader.GetNullableValueByIndex<decimal>(27),
                TpPay = reader.GetNullableValueByIndex<decimal>(28),
                TxPrice = reader.GetNullableValueByIndex<decimal>(29),
                AcqCos = reader.GetNullableValueByIndex<decimal>(30),
                UCPrice = reader.GetNullableValueByIndex<decimal>(31),
                FirstFillDate = reader.GetNullableValueByIndex<DateTime>(32),
                LastFillDate = reader.GetNullableValueByIndex<DateTime>(33),
                SendingRph = reader.GetString(34),
                ReceiveRph = reader.GetString(35),
                XferAddr = reader.GetString(36),
                XferAddre = reader.GetString(37),
                XferCity = reader.GetString(38),
                XferSt = reader.GetString(39),
                XferZip = reader.GetString(40),
                XferPhone = reader.GetString(41),
                TransferReason = reader.GetString(42),
                NewRxRecordNum = reader.GetNullableValueByIndex<long>(43),
                CompetitorGroup = reader.GetString(44),
                CompetitorStoreNum = reader.GetString(45)
            };

        }


    }
}
