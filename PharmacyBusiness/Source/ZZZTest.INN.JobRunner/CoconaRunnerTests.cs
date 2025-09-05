using Cocona.Application;
using INN.JobRunner;
using INN.JobRunner.ErrorFormatter;
using INN.JobRunner.Utility;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ZZZZTest.TestHelpers;

namespace ZZZTest.INN.JobRunner
{
    public class CoconaRunnerTests
    {
        private readonly CoconaRunner _sut;

        private readonly ICoconaConsoleProvider _mockCoconaConsoleProvider = Substitute.For<ICoconaConsoleProvider>();
        private readonly IExceptionMapper _mockExceptionMapper = Substitute.For<IExceptionMapper>();
        private readonly IConfiguration _mockConfiguration = Substitute.For<IConfiguration>();
        private readonly FakeTelemetryChannel _fakeTelemetryChannel = new FakeTelemetryChannel();
        private readonly ILogger<CoconaRunner> _mockLogger = Substitute.For<ILogger<CoconaRunner>>();
        private readonly IUtility _mockUtility = Substitute.For<IUtility>();

        private TextWriter _mockOutputStream = new StringWriter();

        public CoconaRunnerTests()
        {
            _mockUtility.Delay(Arg.Any<int>()).Returns(Task.CompletedTask);
            _mockCoconaConsoleProvider.Output.Returns(_mockOutputStream);
            _sut = new CoconaRunner(
                coconaConsoleProvider: _mockCoconaConsoleProvider,
                exceptionMapper: _mockExceptionMapper,
                configuration: _mockConfiguration,
                telemetryClient: new TelemetryClient(
                new TelemetryConfiguration()
                {
                    TelemetryChannel = _fakeTelemetryChannel,
#pragma warning disable CS0618 // Type or member is obsolete
                    InstrumentationKey = "fakeKey"
#pragma warning restore CS0618 // Type or member is obsolete
                }),
                utility: _mockUtility,
                logger: _mockLogger
            );
        }
        
        /// <summary>
        /// Tests that <see cref="CoconaRunner.TaskRunnerAsync"/> returns the same return code as the delegate.
        /// </summary>
        [Fact]
        public async Task TaskRunnerAsync_Returns_SameReturnCodeAsDelegate()
        {
            // Arrange
            int returnCode = 0;

            // Act
            int result = await _sut.TaskRunnerAsync(async () =>
            {
                await Task.CompletedTask;

                return returnCode;
            }, "CommandName", ["TST900"], []);

            // Assert
            Assert.Equal(returnCode, result);
        }

        /// <summary>
        /// Tests that <see cref="CoconaRunner.TaskRunnerAsync"/> catches thrown exceptions and logs them to the logger.
        /// </summary>
        [Fact]
        public async Task TaskRunnerAsync_CatchesThrownExceptions_AndLogsToLogger()
        {
            // Arrange
            string commandName = "CommandName";
            Exception e = new("message");

            // Act
            int result = await _sut.TaskRunnerAsync(async () =>
            {
                await Task.CompletedTask;
                throw e;
            }, commandName, ["TST900"], []);

            // Assert
            _mockLogger.Received(1).Log(LogLevel.Error, e, e.Message);
            await _mockExceptionMapper.Received(1).ExceptionToConsoleErrorAsync(commandName, e);
            Assert.Equal(1, result);
        }
    }
}
