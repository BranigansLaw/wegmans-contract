using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportCallStateDetailFromCiscoHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.InformixInterface;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportCallStateDetailFromCisco : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IInformixInterface _informixInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportCallStateDetailFromCisco> _logger;

        public TenTenImportCallStateDetailFromCisco(
            ITenTenInterface tenTenInterface,
            IInformixInterface ciscoInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportCallStateDetailFromCisco> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _informixInterface = ciscoInterface ?? throw new ArgumentNullException(nameof(ciscoInterface));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "tenten-import-call-state-detail-from-cisco", 
            Description = "Import call state detail from Cisco and send to TenTen. Control-M job INN509",
            Aliases = ["INN509"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("UNDER CONSTRUCTION");
            await Task.CompletedTask; //To be deleted after implementation.
        }
    }
}
