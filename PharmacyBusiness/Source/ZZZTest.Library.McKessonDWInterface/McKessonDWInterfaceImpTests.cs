using Castle.Core.Configuration;
using Library.LibraryUtilities.Extensions;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Library.McKessonDWInterface.DataSetMapper;
using Library.McKessonDWInterface.McKessonOracleInterface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ZZZTest.Library.McKessonDWInterface
{
    public class McKessonDWInterfaceImpTests
    {
        private readonly McKessonDWInterfaceImp _sut;

        private readonly IMcKessonOracleInterface _mcKessonOracleInterfaceMock = Substitute.For<IMcKessonOracleInterface>();
        private readonly IDataSetMapper _dataSetMapperMock = Substitute.For<IDataSetMapper>();
        private readonly IOptions<McKessonDWConfig> _configMock = Substitute.For<IOptions<McKessonDWConfig>>();
        private readonly ILogger<McKessonDWInterfaceImp> _loggerMock = Substitute.For<ILogger<McKessonDWInterfaceImp>>();

        public McKessonDWInterfaceImpTests()
        {
            _configMock.Value.Returns(new McKessonDWConfig());
            _sut = new McKessonDWInterfaceImp(_mcKessonOracleInterfaceMock, _dataSetMapperMock, _configMock, _loggerMock);
        }

        /// <summary>
        /// Test <see cref="McKessonDWInterfaceImp.GetNewTagPatientGroupsAsync(DateOnly, CancellationToken)"/> correctly transfers arguments to <see cref="OracleParameter"/>s and calls the oracle interface library with the correct parameters, then passes the data set to the <see cref="IDataSetMapper"/> dependency
        /// </summary>
        [Fact]
        public async Task GetNewTagPatientGroupsAsync_PassesCorrectParametersToDependencies()
        {
            // Arrange
            DateOnly requestDate = new DateOnly(2024, 03, 15);
            CancellationToken cancellationToken = CancellationToken.None;
            DataSet returnedDataSet = new DataSet();
            OracleParameter[] passedParameters = [];
            _mcKessonOracleInterfaceMock.RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectNewTagPatientGroups",
                Arg.Do<OracleParameter[]>(p => passedParameters = p),
                cancellationToken).Returns(returnedDataSet);
            IEnumerable<NewTagPatientGroupsRow> mapperReturn = [];
            _dataSetMapperMock.MapNewTagPatientGroups(returnedDataSet).Returns(mapperReturn);

            // Act
            IEnumerable<NewTagPatientGroupsRow> result = await _sut.GetNewTagPatientGroupsAsync(requestDate, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mapperReturn, result);

            await _mcKessonOracleInterfaceMock.Received(1).RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectNewTagPatientGroups",
                Arg.Any<OracleParameter[]>(),
                cancellationToken);
            // Check OracleParamer array passed to method had correctly configured arguments
            OracleParameter singleParameter = Assert.Single(passedParameters);
            Assert.NotNull(singleParameter);
            Assert.Equal("RunDate", singleParameter.ParameterName);
            Assert.Equal(DbType.Date, singleParameter.DbType);
            Assert.Equal(ParameterDirection.Input, singleParameter.Direction);
            string? valueString = singleParameter.Value.ToString();
            Assert.NotNull(valueString);
            Assert.Equal(requestDate.Year, DateTime.Parse(valueString).Year);
            Assert.Equal(requestDate.Month, DateTime.Parse(valueString).Month);
            Assert.Equal(requestDate.Day, DateTime.Parse(valueString).Day);

            _dataSetMapperMock.Received(1).MapNewTagPatientGroups(returnedDataSet);
        }

        /// <summary>
        /// Test <see cref="McKessonDWInterfaceImp.GetRxErpAsync(DateOnly, CancellationToken)"/> correctly transfers arguments to <see cref="OracleParameter"/>s and calls the oracle interface library with the correct parameters, then passes the data set to the <see cref="IDataSetMapper"/> dependency
        /// </summary>
        [Fact]
        public async Task GetRxErpAsync_PassesCorrectParametersToDependencies()
        {
            // Arrange
            DateOnly requestDate = new DateOnly(2024, 03, 15);
            CancellationToken cancellationToken = CancellationToken.None;
            DataSet returnedDataSet = new DataSet();
            OracleParameter[] passedParameters = [];
            _mcKessonOracleInterfaceMock.RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectRxErp",
                Arg.Do<OracleParameter[]>(p => passedParameters = p),
                cancellationToken).Returns(returnedDataSet);
            IEnumerable<RxErpRow> mapperReturn = [];
            _dataSetMapperMock.MapRxErp(returnedDataSet).Returns(mapperReturn);

            // Act
            IEnumerable<RxErpRow> result = await _sut.GetRxErpAsync(requestDate, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mapperReturn, result);

            await _mcKessonOracleInterfaceMock.Received(1).RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectRxErp",
                Arg.Any<OracleParameter[]>(),
                cancellationToken);
            // Check OracleParamer array passed to method had correctly configured arguments
            OracleParameter singleParameter = Assert.Single(passedParameters);
            Assert.NotNull(singleParameter);
            Assert.Equal("RunDate", singleParameter.ParameterName);
            Assert.Equal(DbType.Date, singleParameter.DbType);
            Assert.Equal(ParameterDirection.Input, singleParameter.Direction);
            string? valueString = singleParameter.Value.ToString();
            Assert.NotNull(valueString);
            Assert.Equal(requestDate.Year, DateTime.Parse(valueString).Year);
            Assert.Equal(requestDate.Month, DateTime.Parse(valueString).Month);
            Assert.Equal(requestDate.Day, DateTime.Parse(valueString).Day);

            _dataSetMapperMock.Received(1).MapRxErp(returnedDataSet);
        }

        /// <summary>
        /// Test <see cref="McKessonDWInterfaceImp.GetSoldDetailAsync(DateOnly, CancellationToken)"/> correctly transfers arguments to <see cref="OracleParameter"/>s and calls the oracle interface library with the correct parameters, then passes the data set to the <see cref="IDataSetMapper"/> dependency
        /// </summary>
        [Fact]
        public async Task GetSoldDetailAsync_PassesCorrectParametersToDependencies()
        {
            // Arrange
            DateOnly requestDate = new DateOnly(2024, 03, 15);
            CancellationToken cancellationToken = CancellationToken.None;
            DataSet returnedDataSet = new DataSet();
            OracleParameter[] passedParameters = [];
            _mcKessonOracleInterfaceMock.RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectSoldDetail",
                Arg.Do<OracleParameter[]>(p => passedParameters = p),
                cancellationToken).Returns(returnedDataSet);
            IEnumerable<SoldDetailRow> mapperReturn = [];
            _dataSetMapperMock.MapSoldDetail(returnedDataSet).Returns(mapperReturn);

            // Act
            IEnumerable<SoldDetailRow> result = await _sut.GetSoldDetailAsync(requestDate, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mapperReturn, result);

            await _mcKessonOracleInterfaceMock.Received(1).RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectSoldDetail",
                Arg.Any<OracleParameter[]>(),
                cancellationToken);
            // Check OracleParamer array passed to method had correctly configured arguments
            OracleParameter singleParameter = Assert.Single(passedParameters);
            Assert.NotNull(singleParameter);
            Assert.Equal("RunDate", singleParameter.ParameterName);
            Assert.Equal(DbType.Date, singleParameter.DbType);
            Assert.Equal(ParameterDirection.Input, singleParameter.Direction);
            string? valueString = singleParameter.Value.ToString();
            Assert.NotNull(valueString);
            Assert.Equal(requestDate.Year, DateTime.Parse(valueString).Year);
            Assert.Equal(requestDate.Month, DateTime.Parse(valueString).Month);
            Assert.Equal(requestDate.Day, DateTime.Parse(valueString).Day);

            _dataSetMapperMock.Received(1).MapSoldDetail(returnedDataSet);
        }

        /// <summary>
        /// Test <see cref="McKessonDWInterfaceImp.GetStoreInventoryHistoryAsync(DateOnly, CancellationToken)"/> correctly transfers arguments to <see cref="OracleParameter"/>s and calls the oracle interface library with the correct parameters, then passes the data set to the <see cref="IDataSetMapper"/> dependency
        /// </summary>
        [Fact]
        public async Task GetStoreInventoryHistoryAsync_PassesCorrectParametersToDependencies()
        {
            // Arrange
            DateOnly requestDate = new DateOnly(2024, 03, 15);
            CancellationToken cancellationToken = CancellationToken.None;
            DataSet returnedDataSet = new DataSet();
            OracleParameter[] passedParameters = [];
            _mcKessonOracleInterfaceMock.RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectStoreInventoryHistory",
                Arg.Do<OracleParameter[]>(p => passedParameters = p),
                cancellationToken).Returns(returnedDataSet);
            IEnumerable<StoreInventoryHistoryRow> mapperReturn = [];
            _dataSetMapperMock.MapStoreInventoryHistory(returnedDataSet).Returns(mapperReturn);

            // Act
            IEnumerable<StoreInventoryHistoryRow> result = await _sut.GetStoreInventoryHistoryAsync(requestDate, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mapperReturn, result);

            await _mcKessonOracleInterfaceMock.Received(1).RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectStoreInventoryHistory",
                Arg.Any<OracleParameter[]>(),
                cancellationToken);
            // Check OracleParamer array passed to method had correctly configured arguments
            OracleParameter singleParameter = Assert.Single(passedParameters);
            Assert.NotNull(singleParameter);
            Assert.Equal("RunDate", singleParameter.ParameterName);
            Assert.Equal(DbType.Date, singleParameter.DbType);
            Assert.Equal(ParameterDirection.Input, singleParameter.Direction);
            string? valueString = singleParameter.Value.ToString();
            Assert.NotNull(valueString);
            Assert.Equal(requestDate.Year, DateTime.Parse(valueString).Year);
            Assert.Equal(requestDate.Month, DateTime.Parse(valueString).Month);
            Assert.Equal(requestDate.Day, DateTime.Parse(valueString).Day);

            _dataSetMapperMock.Received(1).MapStoreInventoryHistory(returnedDataSet);
        }

        /// <summary>
        /// Test <see cref="McKessonDWInterfaceImp.GetOmnisysClaimAsync(DateOnly, CancellationToken)"/> correctly transfers arguments to <see cref="OracleParameter"/>s and calls the oracle interface library with the correct parameters, then passes the data set to the <see cref="IDataSetMapper"/> dependency
        /// </summary>
        [Fact]
        public async Task GetOmnisysClaimAsync_PassesCorrectParametersToDependencies()
        {
            // Arrange
            DateOnly requestDate = new DateOnly(2024, 03, 15);
            string expectedDateLiteral = "To_Date('20240315','YYYYMMDD')";
            Dictionary<string, string> expectedFindReplaceLiteralParam = new Dictionary<string, string> { { ":RunDate", expectedDateLiteral } };
            CancellationToken cancellationToken = CancellationToken.None;
            DataSet returnedDataSet = new DataSet();
            Dictionary<string, string> findReplaceLiteralParams = [];
            _mcKessonOracleInterfaceMock.RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectOmnisysClaim",
                Arg.Do<Dictionary<string, string>>(p => findReplaceLiteralParams = p),
                cancellationToken).Returns(returnedDataSet);
            IEnumerable<OmnisysClaimRow> mapperReturn = [];
            _dataSetMapperMock.MapOmnisysClaim(returnedDataSet).Returns(mapperReturn);

            // Act
            IEnumerable<OmnisysClaimRow> result = await _sut.GetOmnisysClaimsAsync(requestDate, cancellationToken);
            string actualDateLiteral = requestDate.ToSqlDateLiteral();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mapperReturn, result);
            Assert.Equal(expectedDateLiteral, actualDateLiteral);

            await _mcKessonOracleInterfaceMock.Received(1).RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectOmnisysClaim",
                Arg.Any<Dictionary<string, string>>(),
                cancellationToken);

            // Check Dictionary passed to method had correctly configured arguments
            Assert.NotNull(findReplaceLiteralParams);
            Assert.Equal(expectedFindReplaceLiteralParam, findReplaceLiteralParams);

            _dataSetMapperMock.Received(1).MapOmnisysClaim(returnedDataSet);
        }
    }
}
