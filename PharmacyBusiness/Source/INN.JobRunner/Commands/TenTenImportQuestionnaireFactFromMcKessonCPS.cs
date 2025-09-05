using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportQuestionnaireFactFromMcKessonCPSHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.McKessonCPSInterface;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportQuestionnaireFactFromMcKessonCPS : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IMcKessonCPSInterface _mcKessonCPSInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportQuestionnaireFactFromMcKessonCPS> _logger;

        public TenTenImportQuestionnaireFactFromMcKessonCPS(
            ITenTenInterface tenTenInterface,
            IMcKessonCPSInterface mcKessonCPSInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportQuestionnaireFactFromMcKessonCPS> logger,
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
            "tenten-import-questionnaire-fact-from-mckesson-cps", 
            Description = "Import Questionnaire Fact from McKessonCPS and send to TenTen. Control-M job INN519",
            Aliases = ["INN519"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("UNDER CONSTRUCTION");
            await Task.CompletedTask; //To be deleted after implementation.
        }
    }
}
