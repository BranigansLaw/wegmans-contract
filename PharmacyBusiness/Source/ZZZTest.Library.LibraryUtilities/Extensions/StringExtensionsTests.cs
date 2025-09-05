using Library.LibraryUtilities.Extensions;
using static Library.LibraryUtilities.Extensions.StringExtensions;

namespace ZZZTest.Library.LibraryUtilities.Extensions
{
    public class StringExtensionsTests
    {
        public StringExtensionsTests() {}

        [Theory]
        [InlineData("A", 4, "LEFT", "A   ")]
        [InlineData("ABCD", 4, "LEFT", "ABCD")]
        [InlineData("ABCDEFG", 4, "LEFT", "ABCD")]
        [InlineData("1", 4, "RIGHT", "0001")]
        [InlineData("1234", 4, "RIGHT", "1234")]
        [InlineData("1234567", 4, "RIGHT", "1234")]
        public void SetToFixedWidth_ShouldReturnExpectedResult(string rawData, int fixedWidth, string alignment, string expectedResult)
        {
            // Setup
            var justification = alignment == "LEFT" ? FixedWidthJustification.Left : FixedWidthJustification.Right;
            var paddingChar = alignment == "LEFT" ? StringExtensions.BlankSpaceChar : StringExtensions.ZeroChar;

            // Act
            string actualResult = rawData.SetToFixedWidth(fixedWidth, justification, paddingChar);

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("AB{0}DE", (char)13, "ABDE")]
        public void KeepOnlyAlphaNumericCharacters_ShouldReturnExpectedResult(string rawDataTemplate, char oddCharactersToAddIn, string expectedResult)
        {
            // Setup
            var testString = string.Format(rawDataTemplate, oddCharactersToAddIn);

            // Act
            string actualResult = testString.KeepOnlyAlphaNumericCharacters();

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
