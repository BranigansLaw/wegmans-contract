using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportRxErpFromMcKessonDWHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportRxErpFromMcKessonDW : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportRxErpFromMcKessonDW> _logger;

        public TenTenImportRxErpFromMcKessonDW(
            ITenTenInterface tenTenInterface,
            IMcKessonDWInterface mcKessonDWInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportRxErpFromMcKessonDW> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _mcKessonDWInterface = mcKessonDWInterface ?? throw new ArgumentNullException(nameof(mcKessonDWInterface));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "tenten-import-rx-erp-from-mckesson-dw", 
            Description = "Import Rx Erp from McKessonDW and send to TenTen. Control-M job INN511",
            Aliases = ["INN511"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _genericHelper.CheckRunForDate(runFor, new DateOnly(2024, 4, 22));

            _logger.LogInformation("Starting to import Rx Erp rows");
            IEnumerable<RxErpRow> rxErpRows =
                await _mcKessonDWInterface.GetRxErpAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection Rx Erp has: {rxErpRows.Count()} rows.");

            _logger.LogInformation("Upload Rx Erp rows to TenTen");
            await _tenTenInterface.CreateOrAppendTenTenDataAsync(runFor.RunFor, _mapper.MapToTenTenRxErp(rxErpRows), CancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Finished importing Rx Erp rows");
        }
    }
}
