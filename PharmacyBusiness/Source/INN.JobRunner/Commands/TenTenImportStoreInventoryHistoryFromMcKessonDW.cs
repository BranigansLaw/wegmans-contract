using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportStoreInventoryHistoryFromMcKessonDWHelper;
using INN.JobRunner.CommonParameters;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Library.TenTenInterface;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportStoreInventoryHistoryFromMcKessonDW : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly IMapper _mapper;
        private readonly ILogger<TenTenImportStoreInventoryHistoryFromMcKessonDW> _logger;

        public TenTenImportStoreInventoryHistoryFromMcKessonDW(
            ITenTenInterface tenTenInterface,
            IMcKessonDWInterface mcKessonDWInterface,
            ISnowflakeInterface snowflakeInterface,
            IMapper mapper,
            ILogger<TenTenImportStoreInventoryHistoryFromMcKessonDW> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _mcKessonDWInterface = mcKessonDWInterface ?? throw new ArgumentNullException(nameof(mcKessonDWInterface));
            _snowflakeInterface = snowflakeInterface ?? throw new ArgumentNullException(nameof(snowflakeInterface));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "tenten-import-store-inventory-history-from-mckesson-dw",
            Description = "Import Store Inventory History from McKessonDW and send to TenTen. Control-M job INN520",
            Aliases = ["INN520"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _genericHelper.CheckRunForDate(runFor, new DateOnly(2024, 4, 22));

            _logger.LogInformation("Starting to import Store Inventory History rows");
            IEnumerable<StoreInventoryHistoryRow> storeInventoryHistoryRows =
                await _mcKessonDWInterface.GetStoreInventoryHistoryAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection Store Inventory History from McKesson has: {storeInventoryHistoryRows.Count()} rows.");

            // Snowflake Data Query
            //IEnumerable<SelectStoreInventoryHistoryRow> snowflakeResults = await _snowflakeInterface.QuerySnowflakeAsync(new SelectStoreInventoryHistoryQuery
            //{
            //    RunDate = runFor.RunFor,
            //}, CancellationToken).ConfigureAwait(false);
            //_logger.LogDebug($"Collection Store Inventory History from Snowflake has: {snowflakeResults.Count()} rows.");

            IEnumerable<StoreInventoryHistory> mappedMcKessonRows = _mapper.MapMcKessonToTenTenStoreInventoryHistory(storeInventoryHistoryRows);
            // Snowflake Data Transition
            //IEnumerable<StoreInventoryHistory> mappedSnowflakeRows = _mapper.MapSnowflakeToTenTenStoreInventoryHistory(snowflakeResults);

            _logger.LogInformation("Upload Store Inventory History rows to TenTen");
            await _tenTenInterface.CreateOrAppendTenTenDataAsync(runFor.RunFor, mappedMcKessonRows, CancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Finished importing Store Inventory History rows");
        }
    }
}
