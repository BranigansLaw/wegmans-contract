using INN.JobRunner.Commands;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BOpportunityFromTCGFileHelper;
using INN.JobRunner.CommonParameters;
using Library.DataFileInterface;
using Library.DataFileInterface.EmailSender;
using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface;
using Library.TenTenInterface.DataModel.UploadRow.Implementation;
using Library.TenTenInterface.DownloadsFromTenTen;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Linq;
using ZZZTest.INN.JobRunner.Commands.CommandHelpers;

namespace ZZZTest.INN.JobRunner.Commands
{
    public class TenTenImportAdm340BOpportunityFromTCGFileTests : PharmacyCommandBaseTests
    {
        private readonly TenTenImportAdm340BOpportunityFromTCGFile _sut;

        private readonly ITenTenInterface _tenTenInterfaceMock = Substitute.For<ITenTenInterface>();
        private readonly IDataFileInterface _dataFileInterfaceMock = Substitute.For<IDataFileInterface>();
        private readonly IMapper _mapperMock = Substitute.For<IMapper>();
        private readonly ILogger<TenTenImportAdm340BOpportunityFromTCGFile> _loggerFactoryMock = Substitute.For<ILogger<TenTenImportAdm340BOpportunityFromTCGFile>>();

        public TenTenImportAdm340BOpportunityFromTCGFileTests() : base()
        {
            _sut = new TenTenImportAdm340BOpportunityFromTCGFile(
                _tenTenInterfaceMock,
                _dataFileInterfaceMock,
                _mapperMock,
                _loggerFactoryMock,
                _mockGenericHelper,
                _mockCoconaContextWrapper
            );
        }

        /// <summary>
        /// Tests that <see cref="TenTenImportAdm340BOpportunityFromTCGFile.RunAsync(RunForParameter)"/> calls all required dependencies and passes correct referenced variables as required
        /// </summary>
        [Fact]
        public async Task RunAsync_CallsAllDependencies()
        {
            // Arrange
            RunForParameter runForParameter = new()
            {
                RunFor = new DateOnly(2024, 2, 20)
            };

            IEnumerable<Adm340BOpportunityRow> readFromFile = [new Adm340BOpportunityRow()];
            _dataFileInterfaceMock.ReadFileToListAndNotifyExceptionsAsync<Adm340BOpportunityRow>(
                Arg.Any<EmailExceptionComposerImp>(),
                $"340B_WegmansShelter_OrderOpportunity_{runForParameter.RunFor:yyyyMMdd}.txt",
                "\n",
                "|",
                true,
                runForParameter.RunFor,
                TestCancellationToken).Returns(readFromFile);

            IOrderedEnumerable<Adm340BOpportunity> returnedFromMapper = new List<Adm340BOpportunity>().OrderBy(s => s.DerivedDrugNdc);
            _mapperMock.MapAndOrderToTenTenAdm340BOpportunity(readFromFile).Returns(returnedFromMapper);

            IEnumerable<Adm340BOpportunityTenTenRow> returnedFromAzureMapper = [];
            _mapperMock.MapAdm340BOpportunityRowToTenTenAzureRow(readFromFile).Returns(returnedFromAzureMapper);

            // Act
            await _sut.RunAsync(runForParameter);

            // Assert
            _mapperMock.Received(1).MapAndOrderToTenTenAdm340BOpportunity(readFromFile);
            _mapperMock.Received(1).MapAdm340BOpportunityRowToTenTenAzureRow(readFromFile);
            await _tenTenInterfaceMock.Received(1).UploadDataAsync(returnedFromAzureMapper, TestCancellationToken, runForParameter.RunFor);
            await _tenTenInterfaceMock.Received(1).CreateOrAppendTenTenDataAsync(runForParameter.RunFor, returnedFromMapper, TestCancellationToken);
        }
    }
}
