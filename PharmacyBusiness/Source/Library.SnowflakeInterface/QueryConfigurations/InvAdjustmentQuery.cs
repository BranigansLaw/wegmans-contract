using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations;

public class InvAdjustmentQuery : AbstractQueryConfiguration<InvAdjustmentRow>
{
    /// <summary>
    /// The run date specified for this query
    /// </summary>
    public required DateOnly RunDate { get; set; }

    /// <inheritdoc />
    public override string QueryFilePath => "./SnowflakeSQL/InvAdj_YYYYMMDD.sql";

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
    public override InvAdjustmentRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
    {
        logDebug("Creating Inventory Adjustment");
        return new InvAdjustmentRow
        {
            DateKey = reader.GetValueByIndex<long>(0),
            StoreNumber = reader.GetString(1),
            DrugLabelName = reader.GetString(2),
            DrugNDC = reader.GetString(3),
            AdjustmentQuantity = reader.GetNullableValueByIndex<decimal>(4),
            AdjustmentCost = reader.GetNullableValueByIndex<decimal>(5),
            AdjustmentExtendedCost = reader.GetNullableValueByIndex<decimal>(6),
            AdjustmentTypeCode = reader.GetString(7),
            InventoryAdjustmentNumber = reader.GetValueByIndex<long>(8),
            AdjustmentReasonCode = reader.GetString(9),
            Description = reader.GetString(10),
            SystemUserFirstName = reader.GetString(11),
            SystemUserLastName = reader.GetString(12),
            SystemUserKey = reader.GetString(13),
            NDC = reader.GetString(14),
            ReferenceNumber = reader.GetString(15)
        };
    }
}
