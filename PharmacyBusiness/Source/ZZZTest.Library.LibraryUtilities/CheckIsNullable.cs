using Library.LibraryUtilities;

namespace ZZZTest.Library.LibraryUtilities
{
    public class CheckIsNullableTest
    {
        /// <summary>
        /// Tests that <see cref="CheckIsNullable.IsNullable{T}"/> returns true for type <see cref="int?"/>
        /// </summary>
        [Fact]
        public void IsNullable_ReturnsTrue_ForNullableInt()
        {
            // Act
            bool res = CheckIsNullable.IsNullable<int?>();

            // Assert
            Assert.True(res);
        }

        /// <summary>
        /// Tests that <see cref="CheckIsNullable.IsNullable{T}"/> returns true for type <see cref="DateTime?"/>
        /// </summary>
        [Fact]
        public void IsNullable_ReturnsTrue_ForNullableDateTime()
        {
            // Act
            bool res = CheckIsNullable.IsNullable<DateTime?>();

            // Assert
            Assert.True(res);
        }

        /// <summary>
        /// Tests that <see cref="CheckIsNullable.IsNullable{T}"/> returns false for type <see cref="DateTime"/>
        /// </summary>
        [Fact]
        public void IsNullable_ReturnsFalse_ForDateTime()
        {
            // Act
            bool res = CheckIsNullable.IsNullable<DateTime>();

            // Assert
            Assert.False(res);
        }
    }
}