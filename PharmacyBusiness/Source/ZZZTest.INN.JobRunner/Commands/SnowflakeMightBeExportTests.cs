using INN.JobRunner.Commands;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeMailSalesExportHelper;
using INN.JobRunner.CommonParameters;
using NSubstitute;
using ZZZTest.INN.JobRunner.Commands.CommandHelpers;

namespace ZZZTest.INN.JobRunner.Commands
{
    public class SnowflakeMightBeExportTests : PharmacyCommandBaseTests
    {
        private readonly SnowflakeMailSalesExport _sut;

        private readonly ISnowflakeMailSalesExportHelper _snowflakeMedicareFeedsExportHelperMock = Substitute.For<ISnowflakeMailSalesExportHelper>();

        public SnowflakeMightBeExportTests() : base()
        {
            _sut = new SnowflakeMailSalesExport(
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
                .ExportMightBeRefundTransactionsAsync(Arg.Is(p.RunFor), TestCancellationToken);
            await _snowflakeMedicareFeedsExportHelperMock.Received(1)
                .ExportMightBeSoldTransactionsAsync(Arg.Is(p.RunFor), TestCancellationToken);
        }
    }
}
