using Library.TenTenInterface.DataModel;
using Library.TenTenInterface.UploadXmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportProjectionDetailHelper;

public class MapperImp : IMapper
{
    /// <inheritdoc />
    public IEnumerable<ProjectionDetail> MapToTenTenProjectionDetail(IEnumerable<ProjectionDetailRow> projectionDetailRows)
    {
        return projectionDetailRows.Select(r => new ProjectionDetail
        {
            StoreNumber = Convert1010StringToNullableInt(r.StoreNumber),
            RxNumber = Convert1010StringToNullableInt(r.RxNumber),
            RefillNumber = Convert1010StringToNullableInt(r.RefillNumber),
            PartialFillNumber = Convert1010StringToNullableInt(r.PartialFillNumber),
            TransactionNumber = Convert1010StringToNullableInt(r.TransactionNumber),
            StoreGenericSales = Convert1010StringToNullableDecimal(r.StoreGenericSales),
            StoreGenericCost = Convert1010StringToNullableDecimal(r.StoreGenericCost),
            StoreGenericCount = DeriveIntFrom1010DecimalString(r.StoreGenericCount),
            StoreBrandSales = Convert1010StringToNullableDecimal(r.StoreBrandSales),
            StoreBrandCost = Convert1010StringToNullableDecimal(r.StoreBrandCost),
            StoreBrandCount = DeriveIntFrom1010DecimalString(r.StoreBrandCount),
            CfGenericSales = Convert1010StringToNullableDecimal(r.CfGenericSales),
            CfGenericCost = Convert1010StringToNullableDecimal(r.CfGenericCost),
            CfGenericCount = DeriveIntFrom1010DecimalString(r.CfGenericCount),
            CfBrandSales = Convert1010StringToNullableDecimal(r.CfBrandSales),
            CfBrandCost = Convert1010StringToNullableDecimal(r.CfBrandCost),
            CfBrandCount = DeriveIntFrom1010DecimalString(r.CfBrandCount),
            Discount = Convert1010StringToNullableInt(r.Discount),
            BillIndicator = Derive1010String(r.BillIndicator),
            RefundPrice = Convert1010StringToNullableDecimal(r.RefundPrice),
            RefundYouPay = Convert1010StringToNullableDecimal(r.RefundYouPay),
            DataFileSource = Derive1010String(r.DataFileSource),
            SoldDate = Convert1010StringToNullableDateTime(r.SoldDate),
            RxId = Convert1010StringToNullableDouble(r.RxId)
        });
    }

    /// <inheritdoc />
    public int? Convert1010StringToNullableInt(string? value) => string.IsNullOrWhiteSpace(value) ? null : int.Parse(value.Trim());

    /// <inheritdoc />
    public decimal? Convert1010StringToNullableDecimal(string? value) => string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value.Trim());

    /// <inheritdoc />
    public double? Convert1010StringToNullableDouble(string? value) => string.IsNullOrWhiteSpace(value) ? null : double.Parse(value.Trim());

    /// <inheritdoc />
    public string? Derive1010String(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <inheritdoc />
    public DateTime? Convert1010StringToNullableDateTime(string? value) => string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value.Trim());

    /// <inheritdoc />
    public int? DeriveIntFrom1010DecimalString(string? value)
    {
        var decimalValue = Convert1010StringToNullableDecimal(value);

        return decimalValue.HasValue ? (int)Math.Floor(decimalValue.Value) : null;
    }
}
