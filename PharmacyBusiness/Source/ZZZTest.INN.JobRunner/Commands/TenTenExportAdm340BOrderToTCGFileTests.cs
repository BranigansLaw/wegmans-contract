using INN.JobRunner.Commands;
using INN.JobRunner.CommonParameters;
using Library.TenTenInterface;
using Library.TenTenInterface.DownloadsFromTenTen;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ZZZTest.INN.JobRunner.Commands.CommandHelpers;

namespace ZZZTest.INN.JobRunner.Commands
{
    public class TenTenExportAdm340BOrderToTCGFileTests : PharmacyCommandBaseTests
    {
        private readonly TenTenExportAdm340BOrderToTCGFile _sut;

        private readonly ITenTenInterface _tenTenInterfaceMock = Substitute.For<ITenTenInterface>();
        private readonly ILogger<TenTenExportAdm340BOrderToTCGFile> _loggerMock = Substitute.For<ILogger<TenTenExportAdm340BOrderToTCGFile>>();

        public TenTenExportAdm340BOrderToTCGFileTests() : base()
        {
            _sut = new TenTenExportAdm340BOrderToTCGFile(
                _tenTenInterfaceMock,
                _loggerMock,
                _mockGenericHelper,
                _mockCoconaContextWrapper
            );
        }

        [Theory]
        [InlineData(2024, 2, 20)]
        public async Task RunAsync_HasRequiredParameters_WhenSuccessful(int year, int month, int day)
        {
            // Arrange
            RunForParameter runForParameter = new()
            {
                RunFor = new DateOnly(year, month, day)
            };
            TenTenDataExtracts? passedInParam = null;
            _tenTenInterfaceMock.OutputDataExtractQueryResultsTenTenAsync(Arg.Do<TenTenDataExtracts>(p => passedInParam = p), TestCancellationToken).Returns(Task.CompletedTask);

            // Act
            await _sut.RunAsync(runForParameter);

            // Assert
            await _tenTenInterfaceMock.Received(1).OutputDataExtractQueryResultsTenTenAsync(Arg.Any<TenTenDataExtracts>(), TestCancellationToken);
            Assert.NotNull(passedInParam);
            Assert.NotNull(passedInParam.ColumnNames);
            Assert.True(passedInParam.ColumnNames.Length > 0);
            Assert.False(string.IsNullOrWhiteSpace(passedInParam.TenTenDownloadQueryXml));
            Assert.False(string.IsNullOrWhiteSpace(passedInParam.OutputFileSpecifications.FieldDelimiter));
            Assert.False(string.IsNullOrWhiteSpace(passedInParam.OutputFileSpecifications.ReplacementHeader));
            Assert.False(string.IsNullOrWhiteSpace(passedInParam.OutputFileSpecifications.FileName));
            Assert.Contains(runForParameter.RunFor.ToString("yyyyMMdd"), passedInParam.OutputFileSpecifications.FileName);
        }
    }
}
