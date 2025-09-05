using Library.LibraryUtilities.Extensions;

namespace ZZZTest.Library.LibraryUtilities.Extensions
{
    public class DecimalExtensionsTests
    {
        public DecimalExtensionsTests() { }

        [Theory]
        [InlineData("1.230000000", "1.23")]
        [InlineData(".0000", "0.0")]
        [InlineData("0.00000", "0.0")]
        [InlineData("00000", "0")]
        public void TrimTrailingZeroes_ShouldReturnExpectedResult_WhenWorkingWithDecimals(string rawDataString, string expectedResultString)
        {
            // Setup
            decimal rawData = decimal.Parse(rawDataString);
            decimal expectedResult = decimal.Parse(expectedResultString);

            // Assert
            if (rawDataString.Contains('.'))
                Assert.NotEqual(expectedResult.ToString(), rawData.ToString());

            // Act
            decimal actualResult = rawData.TrimTrailingZeroes();

            // Assert
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedResult.ToString(), actualResult.ToString());
        }
    }
}
