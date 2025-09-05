using INN.JobRunner.Commands;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper;
using INN.JobRunner.CommonParameters;
using NSubstitute;
using ZZZTest.INN.JobRunner.Commands.CommandHelpers;

namespace ZZZTest.INN.JobRunner.Commands
{
    public class SnowflakeToTenTenDataSftpExportTests : PharmacyCommandBaseTests
    {
        private readonly SnowflakeToTenTenDataSftpExport _sut;

        private readonly ISnowflakeToTenTenDataSftpExportHelper _snowflakeToTenTenDataSftpExportHelperMock = Substitute.For<ISnowflakeToTenTenDataSftpExportHelper>();

        public SnowflakeToTenTenDataSftpExportTests() : base()
        {
            _sut = new SnowflakeToTenTenDataSftpExport(
                _snowflakeToTenTenDataSftpExportHelperMock,
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
            await _snowflakeToTenTenDataSftpExportHelperMock.Received(1)
                .ExportDurConflictAsync(Arg.Is(p.RunFor), TestCancellationToken);
            await _snowflakeToTenTenDataSftpExportHelperMock.Received(1)
                .ExportPrescribersAsync(Arg.Is(p.RunFor), TestCancellationToken);
            await _snowflakeToTenTenDataSftpExportHelperMock.Received(1)
                .ExportPrescriberAddressAsync(Arg.Is(p.RunFor), TestCancellationToken);
            await _snowflakeToTenTenDataSftpExportHelperMock.Received(1)
                .ExportRxTransferAsync(Arg.Is(p.RunFor), TestCancellationToken);
            await _snowflakeToTenTenDataSftpExportHelperMock.Received(1)
                .ExportSupplierPriceDrugFileExportAsync(Arg.Is(p.RunFor), TestCancellationToken);
            await _snowflakeToTenTenDataSftpExportHelperMock.Received(1)
                .ExportPetPtNumsAsync(Arg.Is(p.RunFor), TestCancellationToken);
            await _snowflakeToTenTenDataSftpExportHelperMock.Received(1)
                .ExportInvAdjAsync(Arg.Is(p.RunFor), TestCancellationToken);
        }
    }
}
