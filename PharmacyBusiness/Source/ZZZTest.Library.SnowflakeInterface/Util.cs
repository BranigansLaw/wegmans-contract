using Library.LibraryUtilities.Extensions;
using NSubstitute;
using System.Data.Common;

namespace ZZZTest.Library.SnowflakeInterface
{
    public static class Util
    {
        /// <summary>
        /// Setup the <see cref="DbDataReader.IsDBNull(int)"/> or <see cref="DbDataReader.GetValueByIndex(int)"/> depending if <paramref name="value"/> is null or not
        /// </summary>
        public static void SetupNullableReturn(DbDataReader mockedReader, object[] values, int index)
        {
            if (values[index] == null)
            {
                mockedReader.IsDBNull(index).Returns(true);
            }
            else
            {
                mockedReader.IsDBNull(index).Returns(false);
                mockedReader.GetValue(index).Returns(values[index]);
            }
        }
    }
}
