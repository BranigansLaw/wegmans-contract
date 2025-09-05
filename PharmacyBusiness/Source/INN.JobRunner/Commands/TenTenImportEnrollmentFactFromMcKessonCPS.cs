using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportEnrollmentFactFromMcKessonCPSHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.McKessonCPSInterface;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportEnrollmentFactFromMcKessonCPS : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IMcKessonCPSInterface _mcKessonCPSInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportEnrollmentFactFromMcKessonCPS> _logger;

        public TenTenImportEnrollmentFactFromMcKessonCPS(
            ITenTenInterface tenTenInterface,
            IMcKessonCPSInterface mcKessonCPSInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportEnrollmentFactFromMcKessonCPS> logger,
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
            "tenten-import-enrollment-fact-from-mckesson-cps", 
            Description = "Import Enrollment Fact from McKessonCPS and send to TenTen. Control-M job INN514",
            Aliases = ["INN514"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            // TODO: Set cannotRunBeforeOrOn once historical rollup created in TenTen
            _genericHelper.CheckRunForDate(runFor, cannotRunBeforeOrOn: new DateOnly(2020, 1, 1));

            await _genericHelper.RunFromToRunTillAsync(runFor, RunCommandLogicForDateAsync);
        }

        public async Task RunCommandLogicForDateAsync(DateOnly forDate)
        {
            _logger.LogInformation("UNDER CONSTRUCTION");

            await Task.CompletedTask; //To be deleted after implementation.
        }
    }
}
