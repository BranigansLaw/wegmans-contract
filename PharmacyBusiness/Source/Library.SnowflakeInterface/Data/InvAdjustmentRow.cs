namespace Library.SnowflakeInterface.Data;

public class InvAdjustmentRow
{
    public required long DateKey { get; set; }
    public required string? StoreNumber { get; set; }
    public required string? DrugLabelName { get; set; }
    public required string? DrugNDC { get; set; }
    public required decimal? AdjustmentQuantity { get; set; }
    public required decimal? AdjustmentCost { get; set; }
    public required decimal? AdjustmentExtendedCost { get; set; }
    public required string? AdjustmentTypeCode { get; set; }
    public required long InventoryAdjustmentNumber { get; set; }
    public required string? AdjustmentReasonCode { get; set; }
    public required string? Description { get; set; }
    public required string? SystemUserFirstName { get; set; }
    public required string? SystemUserLastName { get; set; }
    public required string? SystemUserKey { get; set; }
    public required string? NDC { get; set; }
    public required string? ReferenceNumber { get; set; }
}
