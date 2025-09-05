using Cocona.Application;
using INN.JobRunner.ErrorFormatter;
using NSubstitute;
using Polly;
using System.Runtime.CompilerServices;

namespace ZZZTest.INN.JobRunner.ExceptionFormatter
{
    public class ExceptionMapperImpTests
    {
        private readonly ExceptionMapperImp _sut;

        private readonly ICoconaConsoleProvider _mockConsoleProvider = Substitute.For<ICoconaConsoleProvider>();

        private readonly TextWriter _mockErrorStream = new StringWriter();

        public ExceptionMapperImpTests()
        {
            _mockConsoleProvider.Error.Returns(_mockErrorStream);

            _sut = new ExceptionMapperImp(_mockConsoleProvider);
        }

        [Theory]
        [InlineData(typeof(ArgumentException), "integration-tests")]
        [InlineData(typeof(DivideByZeroException), "integration-tests")]
        public async Task ExceptionToConsoleErrorAsync_LogsMessage_WhenCommandAndExceptionRecognized(Type exceptionType, string commandName)
        {
            // Arrange
            Exception exception = (Exception) (Activator.CreateInstance(exceptionType) ?? throw new ArgumentException(nameof(exceptionType)));

            // Act
            await _sut.ExceptionToConsoleErrorAsync(commandName, exception);

            // Assert
            string? streamContents = _mockErrorStream.ToString();
            Assert.NotNull(streamContents);
            Assert.NotEmpty(streamContents);
        }

        [Fact]
        public async Task ExceptionToConsoleErrorAsync_DoesNothing_WhenCommandIsNotRecognized()
        {
            // Act
            await _sut.ExceptionToConsoleErrorAsync("unknown-command", new DivideByZeroException());

            // Assert
            string? streamContents = _mockErrorStream.ToString();
            Assert.NotNull(streamContents);
            Assert.Empty(streamContents);
        }
    }
}
