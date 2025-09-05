using Microsoft.Extensions.Configuration;
using NSubstitute;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace ZZZZTest.TestHelpers
{
    public abstract class AbstractMyConsoleAppTest
    {
        protected readonly IConfiguration _configurationMock = Substitute.For<IConfiguration>();
        protected readonly ILoggerFactory _loggerFactoryMock = Substitute.For<ILoggerFactory>();
        protected readonly IHostApplicationLifetime _hostMock = Substitute.For<IHostApplicationLifetime>();
        protected readonly FakeTelemetryChannel _fakeTelemetryChannel = new FakeTelemetryChannel();

        protected TelemetryClient CreateFakeTelemetryClient()
        {
            return new TelemetryClient(
                new TelemetryConfiguration()
                {
                    TelemetryChannel = _fakeTelemetryChannel,
#pragma warning disable CS0618 // Type or member is obsolete
                    InstrumentationKey = "fakeKey"
#pragma warning restore CS0618 // Type or member is obsolete
                });
        }
    }
}
