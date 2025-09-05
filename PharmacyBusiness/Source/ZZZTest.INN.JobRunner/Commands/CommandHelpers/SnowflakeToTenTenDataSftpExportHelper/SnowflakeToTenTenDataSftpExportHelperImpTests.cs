using INN.JobRunner;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper;
using Library.LibraryUtilities.DataFileWriter;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers.SnowflakeMedicareFeedsExportHelper
{
    public class SnowflakeToTenTenDataSftpExportHelperImpTests
    {
        private readonly SnowflakeToTenTenDataSftpExportHelperImp _sut;

        private readonly ISnowflakeInterface _snowflakeInterfaceMock = Substitute.For<ISnowflakeInterface>();
        private readonly IDataFileWriter _dataFileWriterMock = Substitute.For<IDataFileWriter>();
        private readonly ITenTenInterface _tenTenInterfaceMock = Substitute.For<ITenTenInterface>();
        private readonly IMapper _mapperMock = Substitute.For<IMapper>();
        private readonly ILogger<SnowflakeToTenTenDataSftpExportHelperImp> _logger = Substitute.For<ILogger<SnowflakeToTenTenDataSftpExportHelperImp>>();
        private readonly IOptions<SnowflakeDataOutputDirectories> _options = Substitute.For<IOptions<SnowflakeDataOutputDirectories>>();

        public SnowflakeToTenTenDataSftpExportHelperImpTests()
        {
            _sut = new SnowflakeToTenTenDataSftpExportHelperImp(
                snowflakeInterface: _snowflakeInterfaceMock,
                tenTenInterface: _tenTenInterfaceMock,
                dataFileWriter: _dataFileWriterMock,
                mapper: _mapperMock,
                logger: _logger,
                snowflakeDataOutputDirectoriesOptions: _options);
        }

        /// <summary>
        /// Tests that <see cref="SnowflakeToTenTenDataSftpExportHelperImp.ExportDurConflictAsync(DateOnly, CancellationToken)"/> calls snowflake and then outputs the results into a file
        /// </summary>
        [Fact]
        public async Task ExportDurConflictAsync_CallsSnowflakeAndOutputsDataToDataWriter()
        {
            // Arrange
            DateOnly dateOnly = new(2024, 11, 12);
            CancellationToken c = new();

            IEnumerable<DurConflictRow> mockDataFromSnowflake = [];
            _snowflakeInterfaceMock.QuerySnowflakeAsync(Arg.Is<DurConflictQuery>(q => q.RunDate == dateOnly), Arg.Is(c))
                .Returns(mockDataFromSnowflake);

            IEnumerable<DurConflictTenTenRow> returnedByMapper = [];
            _mapperMock.MapFromDurConflictRow(mockDataFromSnowflake).Returns(returnedByMapper);

            string optionValue = Guid.NewGuid().ToString() + "_yyyyMMdd.txt";
            SnowflakeDataOutputDirectories mockOptionsOutput = Substitute.For<SnowflakeDataOutputDirectories>();
            mockOptionsOutput.INN544_DurConflict = optionValue;
            _options.Value.Returns(mockOptionsOutput);

            // Act
            await _sut.ExportDurConflictAsync(dateOnly, c);

            // Assert
            await _tenTenInterfaceMock.Received(1)
                .UploadDataAsync(returnedByMapper, c, dateOnly);
            await _dataFileWriterMock.Received()
                .WriteDataToFileAsync(Arg.Is(mockDataFromSnowflake), Arg.Is<DataFileWriterConfig<DurConflictRow>>(c =>
                    c.OutputFilePath == optionValue.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"))));
        }

        /// <summary>
        /// Tests that <see cref="SnowflakeToTenTenDataSftpExportHelperImp.ExportPrescriberAddressAsync(DateOnly, CancellationToken)"/> calls snowflake and then outputs the results into a file
        /// </summary>
        [Fact]
        public async Task ExportPrescriberAddressAsync_CallsSnowflakeAndOutputsDataToDataWriter()
        {
            // Arrange
            DateOnly dateOnly = new(2024, 11, 12);
            CancellationToken c = new();

            IEnumerable<PrescriberAddressRow> mockDataFromSnowflake = [];
            _snowflakeInterfaceMock.QuerySnowflakeAsync(Arg.Is<PrescriberAddressQuery>(q => q.RunDate == dateOnly), Arg.Is(c))
                .Returns(mockDataFromSnowflake);

            string optionValue = Guid.NewGuid().ToString() + "_yyyyMMdd.txt";
            SnowflakeDataOutputDirectories mockOptionsOutput = Substitute.For<SnowflakeDataOutputDirectories>();
            mockOptionsOutput.INN544_PrescriberAddress = optionValue;
            _options.Value.Returns(mockOptionsOutput);

            // Act
            await _sut.ExportPrescriberAddressAsync(dateOnly, c);

            // Assert
            await _dataFileWriterMock.Received()
                .WriteDataToFileAsync(Arg.Is(mockDataFromSnowflake), Arg.Is<DataFileWriterConfig<PrescriberAddressRow>>(c =>
                    c.OutputFilePath == optionValue.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"))));
        }

        /// <summary>
        /// Tests that <see cref="SnowflakeToTenTenDataSftpExportHelperImp.ExportPrescribersAsync(DateOnly, CancellationToken)"/> calls snowflake and then outputs the results into a file
        /// </summary>
        [Fact]
        public async Task ExportPrescribersAsync_CallsSnowflakeAndOutputsDataToDataWriter()
        {
            // Arrange
            DateOnly dateOnly = new(2024, 11, 12);
            CancellationToken c = new();

            IEnumerable<PrescriberRow> mockDataFromSnowflake = [];
            _snowflakeInterfaceMock.QuerySnowflakeAsync(Arg.Is<PrescriberQuery>(q => q.RunDate == dateOnly), Arg.Is(c))
                .Returns(mockDataFromSnowflake);

            string optionValue = Guid.NewGuid().ToString() + "_yyyyMMdd.txt";
            SnowflakeDataOutputDirectories mockOptionsOutput = Substitute.For<SnowflakeDataOutputDirectories>();
            mockOptionsOutput.INN544_Prescriber = optionValue;
            _options.Value.Returns(mockOptionsOutput);

            // Act
            await _sut.ExportPrescribersAsync(dateOnly, c);

            // Assert
            await _dataFileWriterMock.Received()
                .WriteDataToFileAsync(Arg.Is(mockDataFromSnowflake), Arg.Is<DataFileWriterConfig<PrescriberRow>>(c =>
                    c.OutputFilePath == optionValue.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"))));
        }

        /// <summary>
        /// Tests that <see cref="SnowflakeToTenTenDataSftpExportHelperImp.ExportRxTransferAsync(DateOnly, CancellationToken)"/> calls snowflake and then outputs the results into a file
        /// </summary>
        [Fact]
        public async Task ExportRxTransferAsync_CallsSnowflakeAndOutputsDataToDataWriter()
        {
            // Arrange
            DateOnly dateOnly = new(2024, 11, 12);
            CancellationToken c = new();

            IEnumerable<RxTransferRow> mockDataFromSnowflake = [];
            _snowflakeInterfaceMock.QuerySnowflakeAsync(Arg.Is<RxTransferQuery>(q => q.RunDate == dateOnly), Arg.Is(c))
                .Returns(mockDataFromSnowflake);

            string optionValue = Guid.NewGuid().ToString() + "_yyyyMMdd.txt";
            SnowflakeDataOutputDirectories mockOptionsOutput = Substitute.For<SnowflakeDataOutputDirectories>();
            mockOptionsOutput.INN544_RxTransfer = optionValue;
            _options.Value.Returns(mockOptionsOutput);

            // Act
            await _sut.ExportRxTransferAsync(dateOnly, c);

            // Assert
            await _dataFileWriterMock.Received()
                .WriteDataToFileAsync(Arg.Is(mockDataFromSnowflake), Arg.Is<DataFileWriterConfig<RxTransferRow>>(c =>
                    c.OutputFilePath == optionValue.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"))));
        }

        [Fact]
        public async Task ExportSupplierPriceDrugFileAsync_CallsSnowflakeAndOutputsDataToDataWriter()
        {
            // Arrange
            DateOnly dateOnly = new(2024, 11, 27);
            CancellationToken c = new();

            IEnumerable<SupplierPriceDrugFileRow> mockSupplierPriceDrugFileRows = [];
            _snowflakeInterfaceMock.QuerySnowflakeAsync(Arg.Is<SupplierPriceDrugFileQuery>(
                q => q.RunDate == dateOnly), Arg.Is(c))
                .Returns(mockSupplierPriceDrugFileRows);

            string optionValue = Guid.NewGuid().ToString() + "_yyyyMMdd.txt";

            SnowflakeDataOutputDirectories mockOptionsOutput = Substitute.For<SnowflakeDataOutputDirectories>();
            mockOptionsOutput.INN544_SupplierPriceDrugFile = optionValue;
            _options.Value.Returns(mockOptionsOutput);

            // Act
            await _sut.ExportSupplierPriceDrugFileExportAsync(dateOnly, c);

            // Assert 
            await _dataFileWriterMock.Received()
                .WriteDataToFileAsync(Arg.Is(mockSupplierPriceDrugFileRows), Arg.Is<DataFileWriterConfig<SupplierPriceDrugFileRow>>(
                    c => c.OutputFilePath == optionValue.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"))));

        }
    }
}
