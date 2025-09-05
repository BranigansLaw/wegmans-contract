using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportMeasureFactFromMcKessonCPSHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.McKessonCPSInterface;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportMeasureFactFromMcKessonCPS : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IMcKessonCPSInterface _mcKessonCPSInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportMeasureFactFromMcKessonCPS> _logger;

        public TenTenImportMeasureFactFromMcKessonCPS(
            ITenTenInterface tenTenInterface,
            IMcKessonCPSInterface mcKessonCPSInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportMeasureFactFromMcKessonCPS> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _mcKessonCPSInterface = mcKessonCPSInterface ?? throw new ArgumentNullException(nameof(mcKessonCPSInterface));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "tenten-import-measure-fact-from-mckesson-cps", 
            Description = "Import Measure Fact from McKessonCPS and send to TenTen. Control-M job INN515",
            Aliases = ["INN515"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("UNDER CONSTRUCTION");
            await Task.CompletedTask; //To be deleted after implementation.
        }
    }
}
