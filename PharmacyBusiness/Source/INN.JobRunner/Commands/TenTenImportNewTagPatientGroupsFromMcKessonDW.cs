using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportNewTagPatientGroupsFromMcKessonDWHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportNewTagPatientGroupsFromMcKessonDW : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportNewTagPatientGroupsFromMcKessonDW> _logger;

        public TenTenImportNewTagPatientGroupsFromMcKessonDW(
            ITenTenInterface tenTenInterface,
            IMcKessonDWInterface mcKessonDWInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportNewTagPatientGroupsFromMcKessonDW> logger,
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
            "tenten-import-new-tag-patient-groups-from-mckesson-dw", 
            Description = "Import New Tag Patient Groups from McKessonDW and send to TenTen. Control-M job INN510",
            Aliases = ["INN510"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            //_genericHelper.CheckRunForDate(runFor, new DateOnly(2024, 4, 22));
            _genericHelper.CheckRunForDate(runFor, new DateOnly(2010, 4, 22));

            _logger.LogInformation("Starting to import New Tag Patient Groups rows");
            IEnumerable<NewTagPatientGroupsRow> newTagPatientGroupsRows =
                await _mcKessonDWInterface.GetNewTagPatientGroupsAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection New Tag Patient Groups has: {newTagPatientGroupsRows.Count()} rows.");

            _logger.LogInformation("Upload New Tag Patient Groups rows to TenTen");
            await _tenTenInterface.CreateOrAppendTenTenDataAsync(runFor.RunFor, _mapper.MapToTenTenNewTagPatientGroups(newTagPatientGroupsRows), CancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Finished importing New Tag Patient Groups rows");
        }
    }
}
