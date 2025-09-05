using Library.TenTenInterface.DataModel;
using Library.TenTenInterface.UploadXmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportProjectionDetailHelper;

public interface IMapper
{
    /// <summary>
    /// Maps the collection of <see cref="ProjectionDetailRow"/> to a collection of <see cref="ProjectionDetail"/>
    /// </summary>
    /// <param name="projectionDetailRows"></param>
    /// <returns></returns>
    IEnumerable<ProjectionDetail> MapToTenTenProjectionDetail(IEnumerable<ProjectionDetailRow> projectionDetailRows);

    /// <summary>
    /// Convert a 1010data string value to a nullable int
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int? Convert1010StringToNullableInt(string? value);

    /// <summary>
    /// Convert a 1010data string value to a nullable decimal
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public decimal? Convert1010StringToNullableDecimal(string? value);

    /// <summary>
    /// Convert a 1010data string value to a nullable double
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public double? Convert1010StringToNullableDouble(string? value);

    /// <summary>
    /// Derive a clean string from a 1010data string value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string? Derive1010String(string? value);

    /// <summary>
    /// Convert a 1010data string value to a nullable DateTime
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public DateTime? Convert1010StringToNullableDateTime(string? value);

    /// <summary>
    /// Derive an int value from a 1010data string with a decimal format specifier
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int? DeriveIntFrom1010DecimalString(string? value);
}
