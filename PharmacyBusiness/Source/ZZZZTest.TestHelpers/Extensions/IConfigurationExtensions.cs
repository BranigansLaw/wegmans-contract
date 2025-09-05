using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace ZZZZTest.TestHelpers.Extensions
{
    public static class IConfigurationExtensions
    {
        public static void MockGetValue(this IConfiguration configuration, string key, string value)
        {
            IConfigurationSection s = Substitute.For<IConfigurationSection>();
            s.Value.Returns(value);

            configuration.GetSection(key).Returns(s);
        }
    }
}
