using Library.McKessonCPSInterface;
using Library.McKessonCPSInterface.DataModel;
using Library.McKessonCPSInterface.DataSetMapper;
using Library.McKessonCPSInterface.McKessonSqlServerInterface;
using NSubstitute;
using System.Data;
using System.Data.SqlClient;

namespace ZZZTest.Library.McKessonCPSInterface
{
    public class McKessonCPSInterfaceImpTests
    {
        private readonly McKessonCPSInterfaceImp _sut;

        private readonly IMcKessonSqlServerInterface _mcKessonSqlServerInterfaceMock = Substitute.For<IMcKessonSqlServerInterface>();
        private readonly IDataSetMapper _dataSetMapperMock = Substitute.For<IDataSetMapper>();

        public McKessonCPSInterfaceImpTests() {
            _sut = new McKessonCPSInterfaceImp(_mcKessonSqlServerInterfaceMock, _dataSetMapperMock);
        }

        [Fact]
        public async Task GetConversationFactsAsync_PassesCorrectParametersToDependencies()
        {
            // Arrange
            DateOnly requestDate = new DateOnly(2024, 03, 15);
            string sqlFileName = "SelectConversationFact";
            CancellationToken cancellationToken = CancellationToken.None;
            DataSet returnedDataSet = new();
            SqlParameter[] passedParameters = [];
            _mcKessonSqlServerInterfaceMock.RunQueryFileWithParamsToDataSetAsync(
                sqlFileName,
                Arg.Do<SqlParameter[]>(p => passedParameters = p),
                cancellationToken).Returns(returnedDataSet);
            IEnumerable<ConversationFactRow> mapperReturn = [];
            _dataSetMapperMock.MapConversationFact(returnedDataSet).Returns(mapperReturn);

            // Act
            IEnumerable<ConversationFactRow> result = await _sut.GetConversationFactsAsync(requestDate, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mapperReturn, result);

            await _mcKessonSqlServerInterfaceMock.Received(1).RunQueryFileWithParamsToDataSetAsync(
                sqlFileName,
                Arg.Any<SqlParameter[]>(),
                cancellationToken);
            // Check OracleParamer array passed to method had correctly configured arguments
            SqlParameter singleParameter = Assert.Single(passedParameters);
            Assert.NotNull(singleParameter);
            Assert.Equal("RunDate", singleParameter.ParameterName);
            Assert.Equal(DbType.Date, singleParameter.DbType);
            Assert.Equal(ParameterDirection.Input, singleParameter.Direction);
            string? valueString = singleParameter.Value.ToString();
            Assert.NotNull(valueString);
            Assert.Equal(requestDate.Year, DateTime.Parse(valueString).Year);
            Assert.Equal(requestDate.Month, DateTime.Parse(valueString).Month);
            Assert.Equal(requestDate.Day, DateTime.Parse(valueString).Day);

            _dataSetMapperMock.Received(1).MapConversationFact(returnedDataSet);
        }
    }
}