using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.DataFileInterface;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class ExportOmnisysClaimFromMcKessonDW : PharmacyCommandBase
    {
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly IDataFileInterface _dataFileInterface;
        private readonly ILogger<ExportOmnisysClaimFromMcKessonDW> _logger;

        public ExportOmnisysClaimFromMcKessonDW(
            IMcKessonDWInterface mcKessonDWInterface,
            IDataFileInterface dataFileInterface,
            ILogger<ExportOmnisysClaimFromMcKessonDW> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _mcKessonDWInterface = mcKessonDWInterface ?? throw new ArgumentNullException(nameof(mcKessonDWInterface));
            _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "export-omnisys-claim-from-mckesson-dw", 
            Description = "Export Omnisys Claim from McKesson DW and send to vendor. Control-M job INN601", 
            Aliases = ["INN601"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation($"Starting to export Omnisys Claim from McKesson DW to a data file for Run Date [{runFor.RunFor}].");

            IEnumerable<OmnisysClaimRow> omnisysClaimRows =
                await _mcKessonDWInterface.GetOmnisysClaimsAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection omnisysClaimRows has: {omnisysClaimRows.Count()} rows.");

            await _dataFileInterface.WriteListToFileAsync(
                omnisysClaimRows.ToList(),
                "Wegmans_SOLDDATE_REQ_" + runFor.RunFor.AddDays(-1).ToString("yyyyMMdd") + ".TXT",
                true,
                "\r",
                string.Empty,
                true,
                CancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Finished exporting McKesson DW Omnisys Claim Rows.");
        }
    }
}
