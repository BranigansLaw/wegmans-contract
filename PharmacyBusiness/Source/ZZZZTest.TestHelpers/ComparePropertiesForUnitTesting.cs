using System.Reflection;
using Xunit;

namespace ZZZZTest.TestHelpers
{
    public class ComparePropertiesForUnitTesting
    {
        public static void AreAllPropertiesEqual<T>(T expectedResult, T res)
        {
            Assert.NotNull(expectedResult);
            Assert.NotNull(res);

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                var expectedValue = property.GetValue(expectedResult);
                var resValue = property.GetValue(res);

                Assert.Equal(expectedValue, resValue);
            }
        }
    }
}
