using Informix.Net.Core;
using Library.InformixInterface;
using Library.InformixInterface.DataModel;
using Library.InformixInterface.Extensions;
using Library.InformixInterface.InformixDatabaseConnection;
using Library.InformixInterface.Mapper;
using Library.LibraryUtilities.SqlFileReader;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ZZZTest.Library.InformixInterface
{
    public class InformixInterfaceImpTests
    {
        private readonly InformixInterfaceImp _sut;

        private readonly IInformixDatabaseConnection _informixDatabaseConnectionMock = Substitute.For<IInformixDatabaseConnection>();
        private readonly ISqlFileReader _sqlFileReaderMock = Substitute.For<ISqlFileReader>();
        private readonly IMapper _mapperMock = Substitute.For<IMapper>();
        private readonly ILogger<InformixInterfaceImp> _loggerMock = Substitute.For<ILogger<InformixInterfaceImp>>();

        public InformixInterfaceImpTests()
        {
            _sut = new InformixInterfaceImp(_informixDatabaseConnectionMock, _sqlFileReaderMock, _mapperMock, _loggerMock);
        }

        /// <summary>
        /// Tests that <see cref="InformixInterfaceImp.GetCsqAgentAsync(DateOnly, CancellationToken)"/> passes parameters to required parameters
        /// </summary>
        [Fact]
        public async Task GetCsqAgentAsync_PassesCorrectParametersToDependencies()
        {
            // Arrange
            DateOnly runFor = new DateOnly(2024, 3, 15);
            CancellationToken c = new CancellationToken();
            string mockSqlFileContents = Guid.NewGuid().ToString();
            _sqlFileReaderMock
                .GetSqlFileContentsAsync("SQL/callcsqagent_YYYYMMDD.sql", c)
                .Returns(mockSqlFileContents);
            IDictionary<string, string> passedToRunQueryAsync = new Dictionary<string, string>();
            List<object[]> runQueryAsyncMockReturn =
            [
                [ Guid.NewGuid() ],
                [ Guid.NewGuid() ]
            ];
            List<CsqAgentRow> mappedReturns = [
                GenerateRandomCsqAgentRow(),
                GenerateRandomCsqAgentRow(),
            ];
            _informixDatabaseConnectionMock
                .RunQueryAsync(mockSqlFileContents, Arg.Do<IDictionary<string, string>>(a => passedToRunQueryAsync = a), c)
                .Returns(runQueryAsyncMockReturn);
            _mapperMock.ParseCsqAgentRow(runQueryAsyncMockReturn.ElementAt(0)).Returns(mappedReturns.ElementAt(0));
            _mapperMock.ParseCsqAgentRow(runQueryAsyncMockReturn.ElementAt(1)).Returns(mappedReturns.ElementAt(1));

            // Act
            IEnumerable<CsqAgentRow> result = (await _sut.GetCsqAgentAsync(runFor, c)).ToList();

            // Assert
            await _sqlFileReaderMock.Received(1).GetSqlFileContentsAsync("SQL/callcsqagent_YYYYMMDD.sql", c);
            await _informixDatabaseConnectionMock.Received(1)
                .RunQueryAsync(mockSqlFileContents, Arg.Any<IDictionary<string, string>>(), c);
            var receivedCalls = _mapperMock.ReceivedCalls();
            _mapperMock.Received(1).ParseCsqAgentRow(runQueryAsyncMockReturn.ElementAt(0));
            _mapperMock.Received(1).ParseCsqAgentRow(runQueryAsyncMockReturn.ElementAt(1));

            Assert.True(passedToRunQueryAsync.ContainsKey("RunFor"));
            string singleParam = passedToRunQueryAsync["RunFor"];
            Assert.Equal(runFor.ToInformixDateFormat(), singleParam);

            Assert.Equal(mappedReturns.Count, result.Count());
            Assert.Contains(mappedReturns.ElementAt(0), result);
            Assert.Contains(mappedReturns.ElementAt(1), result);
        }

        /// <summary>
        /// Tests that <see cref="InformixInterfaceImp.GetRecDetailAsync(DateOnly, CancellationToken)"/> passes parameters to required parameters
        /// </summary>
        [Fact]
        public async Task GetRecDetailAsync_PassesCorrectParametersToDependencies()
        {
            // Arrange
            DateOnly runFor = new DateOnly(2024, 3, 15);
            CancellationToken c = new CancellationToken();
            string mockSqlFileContents = Guid.NewGuid().ToString();
            _sqlFileReaderMock
                .GetSqlFileContentsAsync("SQL/callrecdetail_YYYYMMDD.sql", c)
                .Returns(mockSqlFileContents);
            IDictionary<string, string> passedToRunQueryAsync = new Dictionary<string, string>();
            List<object[]> runQueryAsyncMockReturn =
            [
                [ Guid.NewGuid() ],
                [ Guid.NewGuid() ]
            ];
            List<RecDetailRow> mappedReturns = [
                GenerateRandomRecDetailRow(),
                GenerateRandomRecDetailRow(),
            ];
            _informixDatabaseConnectionMock
                .RunQueryAsync(mockSqlFileContents, Arg.Do<IDictionary<string, string>>(a => passedToRunQueryAsync = a), c)
                .Returns(runQueryAsyncMockReturn);
            _mapperMock.ParseRecDetailRow(runQueryAsyncMockReturn.ElementAt(0)).Returns(mappedReturns.ElementAt(0));
            _mapperMock.ParseRecDetailRow(runQueryAsyncMockReturn.ElementAt(1)).Returns(mappedReturns.ElementAt(1));

            // Act
            IEnumerable<RecDetailRow> result = (await _sut.GetRecDetailAsync(runFor, c)).ToList();

            // Assert
            await _sqlFileReaderMock.Received(1).GetSqlFileContentsAsync("SQL/callrecdetail_YYYYMMDD.sql", c);
            await _informixDatabaseConnectionMock.Received(1)
                .RunQueryAsync(mockSqlFileContents, Arg.Any<IDictionary<string, string>>(), c);
            var receivedCalls = _mapperMock.ReceivedCalls();
            _mapperMock.Received(1).ParseRecDetailRow(runQueryAsyncMockReturn.ElementAt(0));
            _mapperMock.Received(1).ParseRecDetailRow(runQueryAsyncMockReturn.ElementAt(1));

            Assert.True(passedToRunQueryAsync.ContainsKey("RunFor"));
            string singleParam = passedToRunQueryAsync["RunFor"];
            Assert.Equal(runFor.ToInformixDateFormat(), singleParam);

            Assert.Equal(mappedReturns.Count, result.Count());
            Assert.Contains(mappedReturns.ElementAt(0), result);
            Assert.Contains(mappedReturns.ElementAt(1), result);
        }

        /// <summary>
        /// Tests that <see cref="InformixInterfaceImp.GetStateDetailAsync(DateOnly, CancellationToken)"/> passes parameters to required parameters
        /// </summary>
        [Fact]
        public async Task GetStateDetailAsync_PassesCorrectParametersToDependencies()
        {
            // Arrange
            DateOnly runFor = new DateOnly(2024, 3, 15);
            CancellationToken c = new CancellationToken();
            string mockSqlFileContents = Guid.NewGuid().ToString();
            _sqlFileReaderMock
                .GetSqlFileContentsAsync("SQL/callstatedetail_YYYYMMDD.sql", c)
                .Returns(mockSqlFileContents);
            IDictionary<string, string> passedToRunQueryAsync = new Dictionary<string, string>();
            List<object[]> runQueryAsyncMockReturn =
            [
                [ Guid.NewGuid() ],
                [ Guid.NewGuid() ]
            ];
            List<StateDetailRow> mappedReturns = [
                GenerateRandomStateDetailRow(),
                GenerateRandomStateDetailRow(),
            ];
            _informixDatabaseConnectionMock
                .RunQueryAsync(mockSqlFileContents, Arg.Do<IDictionary<string, string>>(a => passedToRunQueryAsync = a), c)
                .Returns(runQueryAsyncMockReturn);
            _mapperMock.ParseStateDetailRow(runQueryAsyncMockReturn.ElementAt(0)).Returns(mappedReturns.ElementAt(0));
            _mapperMock.ParseStateDetailRow(runQueryAsyncMockReturn.ElementAt(1)).Returns(mappedReturns.ElementAt(1));

            // Act
            IEnumerable<StateDetailRow> result = (await _sut.GetStateDetailAsync(runFor, c)).ToList();

            // Assert
            await _sqlFileReaderMock.Received(1).GetSqlFileContentsAsync("SQL/callstatedetail_YYYYMMDD.sql", c);
            await _informixDatabaseConnectionMock.Received(1)
                .RunQueryAsync(mockSqlFileContents, Arg.Any<IDictionary<string, string>>(), c);
            var receivedCalls = _mapperMock.ReceivedCalls();
            _mapperMock.Received(1).ParseStateDetailRow(runQueryAsyncMockReturn.ElementAt(0));
            _mapperMock.Received(1).ParseStateDetailRow(runQueryAsyncMockReturn.ElementAt(1));

            Assert.True(passedToRunQueryAsync.ContainsKey("RunFor"));
            string singleParam = passedToRunQueryAsync["RunFor"];
            Assert.Equal(runFor.ToInformixDateFormat(), singleParam);

            Assert.Equal(mappedReturns.Count, result.Count());
            Assert.Contains(mappedReturns.ElementAt(0), result);
            Assert.Contains(mappedReturns.ElementAt(1), result);
        }

        private CsqAgentRow GenerateRandomCsqAgentRow()
        {
            return new CsqAgentRow
            {
                agent_name = Guid.NewGuid().ToString(),
                application_name = Guid.NewGuid().ToString(),
                called_number = Guid.NewGuid().ToString(),
                contact_disposition = 0,
                csq_names = Guid.NewGuid().ToString(),
                destination_dn = Guid.NewGuid().ToString(),
                end_time = null,
                latestsynchedtime = null,
                node_id = null,
                originator_dn = Guid.NewGuid().ToString(),
                qindex = null,
                queue_time = null,
                ring_time = null,
                sequence_num = null,
                session_id = null,
                session_id_seq = null,
                start_time = null,
                talk_time = null,
                work_time = null,
            };
        }

        private RecDetailRow GenerateRandomRecDetailRow()
        {
            return new RecDetailRow
            {
                agent_extension = Guid.NewGuid().ToString(),
                agent_login_id = Guid.NewGuid().ToString(),
                agent_name = null,
                agent_nonipcc_extn = null,
                call_ani = null,
                call_duration = null,
                called_number = null,
                call_end_time = null,
                call_routed_csq = null,
                call_skills = null,
                call_start_time = null,
                hold_time = null,
                latestsynchedtime = null,
                other_csqs = null,
                talk_time = null,
                type_call = null,
                wrapup_time = null,
            };
        }

        private StateDetailRow GenerateRandomStateDetailRow()
        {
            return new StateDetailRow
            {
                agent_extension = Guid.NewGuid().ToString(),
                agent_login_id = Guid.NewGuid().ToString(),
                agent_name = null,
                agent_state = null,
                duration = null,
                latestsynchedtime = null,
                reason_code = null,
                transition_time = null,
            };
        }
    }
}
