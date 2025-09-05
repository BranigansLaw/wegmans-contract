using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class FillFactQuery : AbstractQueryConfiguration<FillFactRow>
    {
        /// <summary>
        /// The Ready Date Key as a integer.
        /// Snowflake data type: NUMBER(8,0)
        /// </summary>
        public required int ReadyDateKey { get; set; }

        /// <summary>
        /// The Prescription Number as a string.
        /// Snowflake data type: VARCHAR(20)
        /// </summary>
        public required string RxNumber { get; set; }

        /// <summary>
        /// The Total Price Paid as a decimal.
        /// Snowflake data type: NUMBER(18,6)
        /// </summary>
        public required decimal TotalPricePaid { get; set; }

        /// <summary>
        /// The Dispensed Item Expiration Date as a DateOnly
        /// Snowflake data type: TIMESTAMP_NTZ(9)
        /// </summary>
        public required DateOnly DispensedItemExpirationDate { get; set; }

        /// <summary>
        /// The Local Transaction Date as a DateTime
        /// Snowflake data type: TIMESTAMP_NTZ(9)
        /// </summary>
        public required DateTime LocalTransactionDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/FillFactTest.sql";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter paramReadyDateKey = command.CreateParameter();
            paramReadyDateKey.ParameterName = "ReadyDateKey";
            paramReadyDateKey.DbType = DbType.Int32;
            paramReadyDateKey.Value = ReadyDateKey;
            command.Parameters.Add(paramReadyDateKey);

            DbParameter paramRxNumber = command.CreateParameter();
            paramRxNumber.ParameterName = "RxNumber";
            paramRxNumber.DbType = DbType.String;
            paramRxNumber.Value = RxNumber;
            command.Parameters.Add(paramRxNumber);

            DbParameter paramTotalPricePaid = command.CreateParameter();
            paramTotalPricePaid.ParameterName = "TotalPricePaid";
            paramTotalPricePaid.DbType = DbType.String;
            paramTotalPricePaid.Value = TotalPricePaid;
            command.Parameters.Add(paramTotalPricePaid);

            DbParameter paramDispensedItemExpirationDate = command.CreateParameter();
            paramDispensedItemExpirationDate.ParameterName = "DispensedItemExpirationDate";
            paramDispensedItemExpirationDate.DbType = DbType.Date;
            paramDispensedItemExpirationDate.Value = DispensedItemExpirationDate.ToDateTime(new TimeOnly(0));
            command.Parameters.Add(paramDispensedItemExpirationDate);

            DbParameter paramLocalTransactionDate = command.CreateParameter();
            paramLocalTransactionDate.ParameterName = "LocalTransactionDate";
            paramLocalTransactionDate.DbType = DbType.DateTime;
            paramLocalTransactionDate.Value = LocalTransactionDate;
            command.Parameters.Add(paramLocalTransactionDate);
        }

        /// <inheritdoc />
        public override FillFactRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating FillFact");
            return new FillFactRow
            {
                FillFactKey = reader.GetValueByIndex<long>(0),
                Source = reader.GetStringByIndex(1),
                OrderDateKey = reader.GetValueByIndex<long>(2),
                RefillQuantity = reader.GetValueByIndex<decimal>(3),
                FullPackageUnc = reader.GetNullableValueByIndex<decimal>(4),
            };
        }
    }
}
