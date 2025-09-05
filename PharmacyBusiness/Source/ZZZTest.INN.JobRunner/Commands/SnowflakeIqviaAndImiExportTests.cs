using INN.JobRunner.Commands;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeIqviaAndImiExportHelper;
using INN.JobRunner.CommonParameters;
using NSubstitute;
using ZZZTest.INN.JobRunner.Commands.CommandHelpers;

namespace ZZZTest.INN.JobRunner.Commands
{
    public class SnowflakeIqviaAndImiExportTests : PharmacyCommandBaseTests
    {
        private readonly SnowflakeIqviaAndImiExport _sut;

        private readonly ISnowflakeIqviaAndImiExportHelper _snowflakeMedicareFeedsExportHelperMock = Substitute.For<ISnowflakeIqviaAndImiExportHelper>();

        public SnowflakeIqviaAndImiExportTests() : base()
        {
            _sut = new SnowflakeIqviaAndImiExport(
                _snowflakeMedicareFeedsExportHelperMock,
                _mockGenericHelper,
                _mockCoconaContextWrapper
            );
        }

        /// <summary>
        /// Tests that <see cref="SnowflakeMailSalesExport.RunAsync(RunForParameter)"/> calls all the helper methods
        /// </summary>
        [Fact]
        public async Task RunAsync_CallsAllHelperMethods()
        {
            // Arrange
            RunForParameter p = new()
            {
                RunFor = new DateOnly(2024, 11, 11),
            };

            // Act
            await _sut.RunAsync(p);

            // Assert
            await _snowflakeMedicareFeedsExportHelperMock.Received(1)
                .ExportImiAsync(Arg.Is(p.RunFor), TestCancellationToken);
            await _snowflakeMedicareFeedsExportHelperMock.Received(1)
                .ExportInqviaAsync(Arg.Is(p.RunFor), TestCancellationToken);
        }
    }
}
