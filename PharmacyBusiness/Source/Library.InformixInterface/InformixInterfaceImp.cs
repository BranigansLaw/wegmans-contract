using Library.InformixInterface.DataModel;
using Library.InformixInterface.Extensions;
using Library.InformixInterface.InformixDatabaseConnection;
using Library.InformixInterface.Mapper;
using Library.LibraryUtilities.SqlFileReader;
using Microsoft.Extensions.Logging;

namespace Library.InformixInterface
{
    public class InformixInterfaceImp : IInformixInterface
    {
        private readonly IInformixDatabaseConnection _informixDatabaseConnection;
        private readonly ISqlFileReader _sqlFileReader;
        private readonly IMapper _mapper;
        private readonly ILogger<InformixInterfaceImp> _logger;

        public InformixInterfaceImp(
            IInformixDatabaseConnection informixDatabaseConnection,
            ISqlFileReader sqlFileReader,
            IMapper mapper,
            ILogger<InformixInterfaceImp> logger
        )
        {
            _informixDatabaseConnection = informixDatabaseConnection ?? throw new ArgumentNullException(nameof(informixDatabaseConnection));
            _sqlFileReader = sqlFileReader ?? throw new ArgumentNullException(nameof(sqlFileReader));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CsqAgentRow>> GetCsqAgentAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogDebug("Start GetCsqAgentAsync");

            IEnumerable<object[]> rows = await _informixDatabaseConnection.RunQueryAsync(
                await _sqlFileReader.GetSqlFileContentsAsync("SQL/callcsqagent_YYYYMMDD.sql", c),
                new Dictionary<string, string>
                {
                    { "RunFor", runFor.ToInformixDateFormat() }
                }, c);

            return rows.Select(_mapper.ParseCsqAgentRow);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RecDetailRow>> GetRecDetailAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogDebug("Start GetRecDetailAsync");

            IEnumerable<object[]> rows = await _informixDatabaseConnection.RunQueryAsync(
                await _sqlFileReader.GetSqlFileContentsAsync("SQL/callrecdetail_YYYYMMDD.sql", c),
                new Dictionary<string, string>
                {
                    { "RunFor", runFor.ToInformixDateFormat() }
                }, c);

            return rows.Select(_mapper.ParseRecDetailRow);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<StateDetailRow>> GetStateDetailAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogDebug("Start GetStateDetailAsync");

            IEnumerable<object[]> rows = await _informixDatabaseConnection.RunQueryAsync(
                await _sqlFileReader.GetSqlFileContentsAsync("SQL/callstatedetail_YYYYMMDD.sql", c),
                new Dictionary<string, string>
                {
                    { "RunFor", runFor.ToInformixDateFormat() }
                }, c);

            return rows.Select(_mapper.ParseStateDetailRow);
        }
    }
}
