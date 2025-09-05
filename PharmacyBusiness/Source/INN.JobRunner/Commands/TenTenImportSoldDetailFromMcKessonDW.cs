using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportSoldDetailFromMcKessonDWHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportSoldDetailFromMcKessonDW : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportSoldDetailFromMcKessonDW> _logger;

        public TenTenImportSoldDetailFromMcKessonDW(
            ITenTenInterface tenTenInterface,
            IMcKessonDWInterface mcKessonDWInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportSoldDetailFromMcKessonDW> logger,
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
            "tenten-import-sold-detail-from-mckesson-dw", 
            Description = "Import Sold Detail from McKessonDW and send to TenTen. Control-M job INN512",
            Aliases = ["INN512"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _genericHelper.CheckRunForDate(runFor, new DateOnly(2024, 4, 22));

            _logger.LogInformation("Starting to import Sold Detail rows");
            IEnumerable<SoldDetailRow> soldDetailRows =
                await _mcKessonDWInterface.GetSoldDetailAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection Sold Detail has: {soldDetailRows.Count()} rows.");

            _logger.LogInformation("Upload Sold Detail rows to TenTen");
            await _tenTenInterface.CreateOrAppendTenTenDataAsync(runFor.RunFor, _mapper.MapToTenTenSoldDetail(soldDetailRows), CancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Finished importing Sold Detail rows");
        }
    }
}
