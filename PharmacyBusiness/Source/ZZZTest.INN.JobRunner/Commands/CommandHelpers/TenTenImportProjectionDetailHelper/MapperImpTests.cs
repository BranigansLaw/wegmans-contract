using INN.JobRunner.Commands.CommandHelpers.TenTenImportProjectionDetailHelper;
using Library.TenTenInterface.DataModel;

namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers.TenTenImportProjectionDetailHelper;

public class MapperImpTests
{
    private readonly MapperImp _sut;

    public MapperImpTests()
    {
        _sut = new MapperImp();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Convert1010StringToNullableInt_ShouldReturnNull_WhenValueIsNullOrWhitespace(string? value)
    {
        // Arrange

        // Act
        var result = _sut.Convert1010StringToNullableInt(value);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("123", 123)]
    [InlineData(" 123 ", 123)]
    public void Convert1010StringToNullableInt_ShouldReturnInt_WhenValueIsValid(string value, int expectedResult)
    {
        // Arrange

        // Act
        var result = _sut.Convert1010StringToNullableInt(value);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Convert1010StringToNullableDecimal_ShouldReturnNull_WhenValueIsNullOrWhitespace(string? value)
    {
        // Arrange

        // Act
        var result = _sut.Convert1010StringToNullableDecimal(value);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("123.45", 123.45)]
    [InlineData(" 123.45 ", 123.45)]
    public void Convert1010StringToNullableDecimal_ShouldReturnDecimal_WhenValueIsValid(string value, decimal expectedResult)
    {
        // Arrange

        // Act
        var result = _sut.Convert1010StringToNullableDecimal(value);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Convert1010StringToNullableDouble_ShouldReturnNull_WhenValueIsNullOrWhitespace(string? value)
    {
        // Arrange

        // Act
        var result = _sut.Convert1010StringToNullableDouble(value);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("123.45", 123.45)]
    [InlineData(" 123.45 ", 123.45)]
    public void Convert1010StringToNullableDouble_ShouldReturnDouble_WhenValueIsValid(string value, double expectedResult)
    {
        // Arrange

        // Act
        var result = _sut.Convert1010StringToNullableDouble(value);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Derive1010String_ShouldReturnNull_WhenValueIsNullOrWhitespace(string? value)
    {
        // Arrange

        // Act
        var result = _sut.Derive1010String(value);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("test", "test")]
    [InlineData("  test  ", "test")]
    public void Derive1010String_ShouldReturnTrimmedString_WhenValueIsValid(string value, string expectedResult)
    {
        // Arrange

        // Act
        var result = _sut.Derive1010String(value);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Convert1010StringToNullableDateTime_ShouldReturnNull_WhenValueIsNullOrWhitespace(string? value)
    {
        // Arrange

        // Act
        var result = _sut.Convert1010StringToNullableDateTime(value);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("10/27/24", 10, 27, 2024)]
    [InlineData(" 10/27/24 ", 10, 27, 2024)]
    public void Convert1010StringToNullableDateTime_ShouldReturnDateTime_WhenValueIsValid(string value, int month, int day, int year)
    {
        // Arrange
        var expectedResult = new DateTime(year, month, day);

        // Act
        var result = _sut.Convert1010StringToNullableDateTime(value);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void DeriveIntFrom1010DecimalString_ShouldReturnNull_WhenValueIsNullOrWhitespace(string? value)
    {
        // Arrange

        // Act
        var result = _sut.DeriveIntFrom1010DecimalString(value);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("1.00", 1)]
    [InlineData(" 1.00 ", 1)]
    public void DeriveIntFrom1010DecimalString_ShouldReturnInt_WhenValueIsValid(string value, int expectedResult)
    {
        // Arrange

        // Act
        var result = _sut.DeriveIntFrom1010DecimalString(value);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void MapToTenTenProjectionDetail_ShouldMapCorrectly()
    {
        // Arrange
        var projectionDetailRows = new List<ProjectionDetailRow>
            {
                new(
                    "    1",
                    "  20319207",
                    " 0",
                    " 0",
                    "",
                    "       21.88",
                    "       12.36",
                    "        1.00",
                    "            ",
                    "            ",
                    "            ",
                    "            ",
                    "            ",
                    "            ",
                    "            ",
                    "            ",
                    "            ",
                    "",
                    "Third Party ",
                    "",
                    "",
                    "POS1010ProjectionDtlERx_20241028   ",
                    "10/27/24",
                    "134518925133316"
                    )
            };

        // Act
        var result = _sut.MapToTenTenProjectionDetail(projectionDetailRows).ToList();

        // Assert
        Assert.Single(result);
        var detail = result.First();
        Assert.Equal(1, detail.StoreNumber);
        Assert.Equal(20319207, detail.RxNumber);
        Assert.Equal(0, detail.RefillNumber);
        Assert.Equal(0, detail.PartialFillNumber);
        Assert.Null(detail.TransactionNumber);
        Assert.Equal(21.88m, detail.StoreGenericSales);
        Assert.Equal(12.36m, detail.StoreGenericCost);
        Assert.Equal(1, detail.StoreGenericCount);
        Assert.Null(detail.StoreBrandSales);
        Assert.Null(detail.StoreBrandCost);
        Assert.Null(detail.StoreBrandCount);
        Assert.Null(detail.CfGenericSales);
        Assert.Null(detail.CfGenericCost);
        Assert.Null(detail.CfGenericCount);
        Assert.Null(detail.CfBrandSales);
        Assert.Null(detail.CfBrandCost);
        Assert.Null(detail.CfBrandCount);
        Assert.Null(detail.Discount);
        Assert.Equal("Third Party", detail.BillIndicator);
        Assert.Null(detail.RefundPrice);
        Assert.Null(detail.RefundYouPay);
        Assert.Equal("POS1010ProjectionDtlERx_20241028", detail.DataFileSource);
        Assert.Equal(new DateTime(2024, 10, 27), detail.SoldDate);
        Assert.Equal(134518925133316, detail.RxId);
    }
}
