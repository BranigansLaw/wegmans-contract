using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class SelectSoldDetailQuery : AbstractQueryConfiguration<SelectSoldDetailRow>
    {

        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/SelectSoldDetail.sql";

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
        public override SelectSoldDetailRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating Sold Detail Row");
            string debug = reader.GetDecimal(8).ToString();
            return new SelectSoldDetailRow
            {
                StoreNumber = reader.GetString(0),
                RxNumber = reader.GetString(1),
                RefillNumber = reader.GetFieldValue<int?>(2),
                PartSequenceNumber = reader.GetFieldValue<int?>(3),
                SoldDate = reader.GetDateTime(4),
                OrderNumber = reader.GetString(5),
                QtyDispensed = reader.GetFieldValue<decimal?>(6),
                NdcWo = reader.GetString(7),
                AdqCost = reader.GetFieldValue<decimal?>(8),
                TpPay = reader.GetFieldValue<decimal?>(9),
                PatientPay = reader.GetFieldValue<decimal?>(10),
                TxPrice = reader.GetFieldValue<decimal?>(11)
            };

        }
    }
}
