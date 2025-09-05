using INN.JobRunner;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeMedicareFeedsExportHelper;
using Library.LibraryUtilities.DataFileWriter;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers.SnowflakeMedicareFeedsExportHelper
{
    public class SnowflakeMedicareFeedsExportHelperImpTests
    {
        private readonly SnowflakeMedicareFeedsExportHelperImp _sut;

        private readonly ISnowflakeInterface _snowflakeInterfaceMock = Substitute.For<ISnowflakeInterface>();
        private readonly IDataFileWriter _dataFileWriterMock = Substitute.For<IDataFileWriter>();
        private readonly ILogger<SnowflakeMedicareFeedsExportHelperImp> _logger = Substitute.For<ILogger<SnowflakeMedicareFeedsExportHelperImp>>();
        private readonly IOptions<SnowflakeDataOutputDirectories> _options = Substitute.For<IOptions<SnowflakeDataOutputDirectories>>();

        public SnowflakeMedicareFeedsExportHelperImpTests()
        {
            _sut = new SnowflakeMedicareFeedsExportHelperImp(
                _snowflakeInterfaceMock,
                _dataFileWriterMock,
                _logger,
                _options);
        }

        /// <summary>
        /// Tests that <see cref="SnowflakeMedicareFeedsExportHelperImp.ExportFdsPharmaciesAsync(DateOnly, CancellationToken)"/> calls snowflake and then outputs the results into a file
        /// </summary>
        [Fact]
        public async Task ExportFdsPharmaciesAsync_CallsSnowflakeAndOutputsDataToDataWriter()
        {
            // Arrange
            DateOnly dateOnly = new(2024, 11, 12);
            CancellationToken c = new();

            IEnumerable<FdsPharmaciesRow> mockDataFromSnowflake = [];
            _snowflakeInterfaceMock.QuerySnowflakeAsync(Arg.Is<FdsPharmaciesQuery>(q => q.RunDate == dateOnly), Arg.Is(c))
                .Returns(mockDataFromSnowflake);

            string optionValue = Guid.NewGuid().ToString() + "_yyyyMMdd.txt";
            SnowflakeDataOutputDirectories mockOptionsOutput = Substitute.For<SnowflakeDataOutputDirectories>();
            mockOptionsOutput.INN545_FdsPharmacies = optionValue;
            _options.Value.Returns(mockOptionsOutput);

            // Act
            await _sut.ExportFdsPharmaciesAsync(dateOnly, c);

            // Assert
            await _dataFileWriterMock.Received()
                .WriteDataToFileAsync(Arg.Is(mockDataFromSnowflake), Arg.Is<DataFileWriterConfig<FdsPharmaciesRow>>(c =>
                    c.OutputFilePath == optionValue.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"))));
        }

        /// <summary>
        /// Tests that <see cref="SnowflakeMedicareFeedsExportHelperImp.ExportHpOnePharmaciesAsync(CancellationToken)"/> calls snowflake and then outputs the results into a file
        /// </summary>
        [Fact]
        public async Task ExportHpOnePharmaciesAsync_CallsSnowflakeAndOutputsDataToDataWriter()
        {
            // Arrange
            CancellationToken c = new();

            IEnumerable<HpOnePharmaciesRow> mockDataFromSnowflake = [];
            _snowflakeInterfaceMock.QuerySnowflakeAsync(Arg.Any<HpOnePharmaciesQuery>(), Arg.Is(c))
                .Returns(mockDataFromSnowflake);

            string optionValue = Guid.NewGuid().ToString() + "_yyyyMMdd.txt";
            SnowflakeDataOutputDirectories mockOptionsOutput = Substitute.For<SnowflakeDataOutputDirectories>();
            mockOptionsOutput.INN545_HpOnePharmacies = optionValue;
            _options.Value.Returns(mockOptionsOutput);

            // Act
            await _sut.ExportHpOnePharmaciesAsync(c);

            // Assert
            await _dataFileWriterMock.Received()
                .WriteDataToFileAsync(Arg.Is(mockDataFromSnowflake), Arg.Is<DataFileWriterConfig<HpOnePharmaciesRow>>(c =>
                    c.OutputFilePath == optionValue.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"))));
        }

        /// <summary>
        /// Tests that <see cref="SnowflakeMedicareFeedsExportHelperImp.ExportHpOnePrescriptionsExport(DateOnly, CancellationToken)"/> calls snowflake and then outputs the results into a file
        /// </summary>
        [Fact]
        public async Task ExportHpOnePrescriptionsExport_CallsSnowflakeAndOutputsDataToDataWriter()
        {
            // Arrange
            DateOnly dateOnly = new(2024, 11, 12);
            CancellationToken c = new();

            IEnumerable<WegmansHPOnePrescriptionsRow> mockDataFromSnowflake = [];
            _snowflakeInterfaceMock.QuerySnowflakeAsync(Arg.Is<WegmansHPOnePrescriptionsQuery>(q => q.RunDate == dateOnly), Arg.Is(c))
                .Returns(mockDataFromSnowflake);

            string optionValue = Guid.NewGuid().ToString() + "_yyyyMMdd.txt";
            SnowflakeDataOutputDirectories mockOptionsOutput = Substitute.For<SnowflakeDataOutputDirectories>();
            mockOptionsOutput.INN545_HpOnePerscriptions = optionValue;
            _options.Value.Returns(mockOptionsOutput);

            // Act
            await _sut.ExportHpOnePrescriptionsExport(dateOnly, c);

            // Assert
            await _dataFileWriterMock.Received()
                .WriteDataToFileAsync(Arg.Is(mockDataFromSnowflake), Arg.Is<DataFileWriterConfig<WegmansHPOnePrescriptionsRow>>(c =>
                    c.OutputFilePath == optionValue.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"))));
        }
    }
}
