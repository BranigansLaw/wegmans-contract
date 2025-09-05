using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportConversationFactFromMcKessonCPSHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.McKessonCPSInterface;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportConversationFactFromMcKessonCPS : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IMcKessonCPSInterface _mcKessonCPSInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportConversationFactFromMcKessonCPS> _logger;

        public TenTenImportConversationFactFromMcKessonCPS(
            ITenTenInterface tenTenInterface,
            IMcKessonCPSInterface mcKessonCPSInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportConversationFactFromMcKessonCPS> logger,
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
            "tenten-import-conversation-fact-from-mckesson-cps", 
            Description = "Import Conversation Fact from McKessonCPS and send to TenTen. Control-M job INN513",
            Aliases = ["INN513"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            //TODO: Make history table and set that table date here
            //_genericHelper.CheckRunForDate(runFor, new DateOnly(2024, 2, 13));

            _logger.LogInformation("Starting to import CPS Conversation Fact rows");
            // TODO: Mapping error on row that is found. Commenting out for now.
            //IEnumerable<ConversationFactRow> conversationFactRows =
            //    await _mcKessonCPSInterface.GetConversationFactsAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);
            //_logger.LogDebug($"Collection conversationFactRows has: {conversationFactRows.Count()} rows.");

            //TODO: Write mapper code.
            //_logger.LogInformation("Upload Conversation Fact Rows to TenTen");
            //await _tenTenInterface.CreateOrAppendTenTenDataAsync(runFor.RunFor, _mapper.MapToTenTenConversationFact(conversationFactRows), CancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Finished importing Netezza net sales rows");

            await Task.CompletedTask; //To be deleted after implementation.
        }
    }
}
