using INN.JobRunner.Commands;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeMedicareFeedsExportHelper;
using INN.JobRunner.CommonParameters;
using NSubstitute;
using ZZZTest.INN.JobRunner.Commands.CommandHelpers;

namespace ZZZTest.INN.JobRunner.Commands
{
    public class SnowflakeMedicareFeedsExportTests : PharmacyCommandBaseTests
    {
        private readonly SnowflakeMedicareFeedsExport _sut;

        private readonly ISnowflakeMedicareFeedsExportHelper _snowflakeMedicareFeedsExportHelperMock = Substitute.For<ISnowflakeMedicareFeedsExportHelper>();

        public SnowflakeMedicareFeedsExportTests() : base()
        {
            _sut = new SnowflakeMedicareFeedsExport(
                _snowflakeMedicareFeedsExportHelperMock,
                _mockGenericHelper,
                _mockCoconaContextWrapper
            );
        }

        /// <summary>
        /// Tests that <see cref="SnowflakeMedicareFeedsExport.RunAsync(RunForParameter)"/> calls all the helper methods
        /// </summary>
        [Fact]
        public async Task RunAsync_CallsAllHelperMethods()
        {
            // Arrange
            RunForParameter p = new RunForParameter
            {
                RunFor = new DateOnly(2024, 11, 11),
            };

            // Act
            await _sut.RunAsync(p);

            // Assert
            await _snowflakeMedicareFeedsExportHelperMock.Received(1)
                .ExportFdsPharmaciesAsync(Arg.Is(p.RunFor), TestCancellationToken);
            await _snowflakeMedicareFeedsExportHelperMock.Received(1)
                .ExportHpOnePharmaciesAsync(TestCancellationToken);
            await _snowflakeMedicareFeedsExportHelperMock.Received(1)
                .ExportHpOnePrescriptionsExport(Arg.Is(p.RunFor), TestCancellationToken);
        }
    }
}
