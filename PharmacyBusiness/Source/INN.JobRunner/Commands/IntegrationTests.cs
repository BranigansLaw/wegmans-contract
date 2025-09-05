using CaseServiceWrapper;
using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.IntegrationTests;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.DataFileInterface;
using Library.EmplifiInterface;
using Library.InformixInterface;
using Library.InformixInterface.DataModel;
using Library.LibraryUtilities.DataFileWriter;
using Library.McKessonCPSInterface;
using Library.McKessonCPSInterface.DataModel;
using Library.McKessonDWInterface;
using Library.NetezzaInterface;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Library.TenTenInterface;
using Library.TenTenInterface.DataModel.UploadRow.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace INN.JobRunner.Commands
{
    public class IntegrationTests : PharmacyCommandBase
    {
        private readonly IEmplifiInterface _emplifiInterface;
        private readonly IDataFileInterface _dataFileInterface;
        private readonly IDataFileWriter _dataFileWriter;
        private readonly INetezzaInterface _netezzaDownload;
        private readonly IMcKessonDWInterface _mcKessonDWDownload;
        private readonly IMcKessonCPSInterface _mcKessonCPSInterface;
        private readonly ITenTenInterface _tenTenDataUpload;
        private readonly IInformixInterface _informixInterface;
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly IConfiguration _configuration;
        private readonly IUtility _utility;
        private readonly ILogger<IntegrationTests> _logger;
        private readonly IOptions<TenTenConfig> _tenTenConfig;

        public IntegrationTests(
            IEmplifiInterface emplifiInterface,
            IDataFileInterface dataFileInterface,
            IDataFileWriter dataFileWriter,
            INetezzaInterface netezzaDownload,
            IMcKessonDWInterface mcKessonDWDownload,
            IMcKessonCPSInterface mcKessonCPSInterface,
            ITenTenInterface tenTenDataUpload,
            IInformixInterface informixInterface,
            ISnowflakeInterface snowflakeInterface,
            IConfiguration configuration,
            IUtility utility,
            ILogger<IntegrationTests> logger,
            IGenericHelper services,
            ICoconaContextWrapper coconaContextWrapper,
            IOptions<TenTenConfig> tenTenConfig
        ) : base(services, coconaContextWrapper)
        {
            _emplifiInterface = emplifiInterface ?? throw new ArgumentNullException(nameof(emplifiInterface));
            _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
            _dataFileWriter = dataFileWriter ?? throw new ArgumentNullException(nameof(dataFileWriter));
            _netezzaDownload = netezzaDownload ?? throw new ArgumentNullException(nameof(netezzaDownload));
            _mcKessonDWDownload = mcKessonDWDownload ?? throw new ArgumentNullException(nameof(mcKessonDWDownload));
            _mcKessonCPSInterface = mcKessonCPSInterface ?? throw new ArgumentNullException(nameof(mcKessonCPSInterface));
            _tenTenDataUpload = tenTenDataUpload ?? throw new ArgumentNullException(nameof(tenTenDataUpload));
            _informixInterface = informixInterface ?? throw new ArgumentNullException(nameof(informixInterface));
            _snowflakeInterface = snowflakeInterface ?? throw new ArgumentNullException(nameof(snowflakeInterface));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tenTenConfig = tenTenConfig ?? throw new ArgumentNullException(nameof(tenTenConfig));
        }

        [Command(
            "integration-tests",
            Description = "Run integration tests. Control-M job INN900",
            Aliases = ["INN900"]
        )]
        public async Task RunAsync(RunForParameter r)
        {
            await Parallel.ForEachAsync<Func<CancellationToken, Task>>(
                [
                    RunForTestAsync,
                    ReadTestFileAsync,
                    TestNetSalesEtlAsync,
                    AppInsightsSanitizesSensitiveInformationTestAsync,
                    TestInformixTestMethodsAsync,
                    TryEmplifi,
                    (c) => TestSnowflakeConnectionAsync(r.RunFor, c),
                    TestMcKessonCPSConnectionAsync,
                    TestTenTenAzureBlobUploadAsync,
                ],
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = 20,
                    CancellationToken = CancellationToken,
                },
                async (s, c) => await s(c).ConfigureAwait(false)
            ).ConfigureAwait(false);
        }

        private Task RunForTestAsync(CancellationToken cancellationToken)
        {
            DateOnly runFor = DateOnly.FromDateTime(DateTime.Now);
            if (DateOnly.TryParse(_configuration.GetValue<string>("runFor"), out DateOnly runForParsed))
            {
                runFor = runForParsed;
            }

            _logger.LogDebug($"Running for: {runFor}");

            return Task.CompletedTask;
        }

        private async Task ReadTestFileAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Current directory: {Directory.GetCurrentDirectory()}");
            _logger.LogDebug($"Utility.GetCurrentExecutableDirectory: {_utility.GetCurrentExecutableDirectory()}");

            string readWriteTestFileName = $"{_utility.GetCurrentExecutableDirectory()}test-read-write.txt";
            _logger.LogDebug($"readWriteTestFileName: {readWriteTestFileName}");
            using (StreamWriter sw = File.CreateText(readWriteTestFileName))
            {
                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine(Guid.NewGuid().ToString());
                await sw.WriteLineAsync(stringBuilder, cancellationToken).ConfigureAwait(false);
            }

            string readLine = string.Empty;
            using (StreamReader sr = new(File.OpenRead(readWriteTestFileName)))
            {
                readLine += await sr.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            }
            _logger.LogDebug($"readLine: {readLine}");
        }

        private async Task TestNetSalesEtlAsync(CancellationToken cancellationToken)
        {
            DateOnly runFor = DateOnly.FromDateTime(DateTime.Now);

            /******************************************************************************/
            //TODO: Maybe we could reserve INN5## jobs for daily ETLs, and INN6## jobs for daily Data Extracts.
            _logger.LogDebug($"Begin ETL Process for job INN501 Net Sales.");
            var feed = new NetSalesEtl(runFor, "DEV");
            feed.DataToBeUploadedToTenTen = await _netezzaDownload.GetNetSalesAsync(runFor, cancellationToken).ConfigureAwait(false);

            /******************************************************************************/
            //TODO: Maybe we could reserve INN5## jobs for daily ETLs, and INN6## jobs for daily Data Extracts.
            _logger.LogDebug($"Begin Data Extract Process for job INN601 Mail Sales Extract Summary.");
            string outboxPath = $"{_utility.GetCurrentExecutableDirectory()}Outbox"; //TODO: Discuss data extract options with the team, such as emails or APIs.
                                                                                     //        var feed = new TenTenDownloads.MailSalesSummary(runFor);
                                                                                     //        TenTenDataExtracts dataExtract = new TenTenDataExtracts(runFor, "INN601");
                                                                                     //        await _tenTenDataUpload.DataExtractQueryTenTenAsync(dataExtract, outboxPath, cancellationToken).ConfigureAwait(false);
        }

        private async Task AppInsightsSanitizesSensitiveInformationTestAsync(CancellationToken cancellationToken)
        {
            string url = "https://6e93f960-d22f-465e-96e3-321268d36982.mock.pstmn.io/SendSensitiveInfo?pswd=dkjfghdkfgh&sid=439853409&param=normal";

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        private async Task TestInformixTestMethodsAsync(CancellationToken c)
        {
            IEnumerable<RecDetailRow> recDetail = await _informixInterface.GetRecDetailAsync(new DateOnly(2024, 3, 15), c);
            _logger.LogInformation($"Returned {recDetail.Count()} recDetail rows");

            IEnumerable<CsqAgentRow> csAgent = await _informixInterface.GetCsqAgentAsync(new DateOnly(2024, 3, 15), c);
            _logger.LogInformation($"Returned {csAgent.Count()} csAgent rows");

            IEnumerable<StateDetailRow> stateDetail = await _informixInterface.GetStateDetailAsync(new DateOnly(2024, 3, 15), c);
            _logger.LogInformation($"Returned {stateDetail.Count()} stateDetail rows");
        }

        private async Task TestMcKessonCPSConnectionAsync(CancellationToken c)
        {
            DateOnly testDate = new DateOnly(2023, 3, 16);

            IEnumerable<ImmunizationQuestionnaireRow> conversationFacts = await _mcKessonCPSInterface.GetImmunizationQuestionnairesAsync(
                testDate,
                c).ConfigureAwait(false);

            _logger.LogDebug($"Received {conversationFacts.Count()} claims from McKessonDW interface");
        }

        /// <summary>
        /// Test the Snowflake interface connection
        /// </summary>
        /// <param name="c">The <see cref="CancellationToken"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        private async Task TestSnowflakeConnectionAsync(DateOnly runDate, CancellationToken c)
        {
            _logger.LogInformation("Testing Snowflake FillFact query");
            IEnumerable<FillFactRow> fillFactData = await _snowflakeInterface.QuerySnowflakeAsync(
                new FillFactQuery
                {
                    ReadyDateKey = 20240808,
                    RxNumber = "60012478",
                    TotalPricePaid = 38.57M,
                    DispensedItemExpirationDate = new DateOnly(2025, 8, 8),
                    LocalTransactionDate = new DateTime(2024, 8, 8, 15, 56, 50)
                }, c);
            _logger.LogInformation($"Returned {fillFactData.Count()} rows");

            foreach (FillFactRow fillFact in fillFactData)
            {
                _logger.LogInformation($"{fillFact.FillFactKey}, {fillFact.OrderDateKey}, {fillFact.Source}, {fillFact.FullPackageUnc}");
            }
        }

        private async Task TryEmplifi(CancellationToken c)
        {
            _logger.LogInformation("TryEmplifi");
            IEnumerable<Case> mySearchResults = await _emplifiInterface.GetDelayAndDenialOutboundStatusAsync(DateTime.Now.AddDays(-1), DateTime.Now, c).ConfigureAwait(false);

            _logger.LogInformation($"mySearchResults has [{mySearchResults.Count()}] rows");
        }

        /// <summary>
        /// Tests uploading to the TenTen Azure Blob
        /// </summary>
        private async Task TestTenTenAzureBlobUploadAsync(CancellationToken c)
        {
            IEnumerable<TestTenTenRow> testData = Enumerable.Range(0, 10).Select(i =>
                new TestTenTenRow
                {
                    Id = i,
                    DontWrite = "I won't be written to file",
                    Price = 12.54f,
                    Message = "Hello, TenTen",
                });

            await _tenTenDataUpload.UploadDataAsync(testData, c);
        }
    }
}
