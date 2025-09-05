using INN.JobRunner.Commands;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportNetSalesFromNetezzaHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.NetezzaInterface;
using Library.NetezzaInterface.DataModel;
using Library.TenTenInterface;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ZZZTest.INN.JobRunner.Commands.CommandHelpers;

namespace ZZZTest.INN.JobRunner.Commands
{
    public class TenTenImportNetSalesFromNetezzaTests : PharmacyCommandBaseTests
    {
        private readonly TenTenImportNetSalesFromNetezza _sut;

        private readonly ITenTenInterface _tenTenInterfaceMock = Substitute.For<ITenTenInterface>();
        private readonly INetezzaInterface _netezzaInterfaceMock = Substitute.For<INetezzaInterface>();
        private readonly IMapper _mapperMock = Substitute.For<IMapper>();
        private readonly ILoggerFactory _loggerFactoryMock = Substitute.For<ILoggerFactory>();

        public TenTenImportNetSalesFromNetezzaTests() : base()
        {
            _sut = new TenTenImportNetSalesFromNetezza(
                _tenTenInterfaceMock,
                _netezzaInterfaceMock,
                _mapperMock,
                _loggerFactoryMock.CreateLogger<TenTenImportNetSalesFromNetezza>(),
                _mockGenericHelper,
                _mockCoconaContextWrapper
            );
        }

        [Fact]
        public async Task RunAsync_CallsGenericLooper_WithCorrectParameters()
        {
            // Arrange
            RunForParameter runForParameter = new()
            {
                RunFor = new DateOnly(2024, 2, 20)
            };

            _mockGenericHelper.RunFromToRunTillAsync(runForParameter, Arg.Any<Func<DateOnly, Task>>()).Returns(Task.CompletedTask);

            await _sut.RunAsync(runForParameter);

            // Assert
            _mockGenericHelper.Received(1).CheckRunForDate(runForParameter, Arg.Is<DateOnly>(a => a.Equals(new DateOnly(2024, 2, 13))));
            await _mockGenericHelper.Received(1).RunFromToRunTillAsync(runForParameter, Arg.Any<Func<DateOnly, Task>>());
        }

        [Fact]
        public async Task RunCommandLogicForDateAsync_CallsAllDependencies_WhenSuccessful()
        {
            // Arrange
            DateOnly runFor = new DateOnly(2024, 2, 20);

            IEnumerable<NetSaleRow> getNetSalesResult = new List<NetSaleRow>();
            _netezzaInterfaceMock.GetNetSalesAsync(Arg.Any<DateOnly>(), TestCancellationToken).Returns(getNetSalesResult);

            IEnumerable<NetSales> mapToTenTenNetSalesResult = new List<NetSales>();
            _mapperMock.MapToTenTenNetSales(getNetSalesResult).Returns(mapToTenTenNetSalesResult);

            _tenTenInterfaceMock.CreateOrAppendTenTenDataAsync(Arg.Any<DateOnly>(), mapToTenTenNetSalesResult, TestCancellationToken).Returns(Task.CompletedTask);

            await _sut.RunCommandLogicForDateAsync(runFor);

            // Assert
            await _netezzaInterfaceMock.Received(1).GetNetSalesAsync(Arg.Any<DateOnly>(), TestCancellationToken);
            _mapperMock.Received(1).MapToTenTenNetSales(getNetSalesResult);
            await _tenTenInterfaceMock.Received(1).CreateOrAppendTenTenDataAsync(Arg.Any<DateOnly>(), mapToTenTenNetSalesResult, TestCancellationToken);
        }
    }
}
