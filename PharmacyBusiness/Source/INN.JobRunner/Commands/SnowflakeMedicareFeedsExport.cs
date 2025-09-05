using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeMedicareFeedsExportHelper;
using INN.JobRunner.CommonParameters;

namespace INN.JobRunner.Commands
{
    public class SnowflakeMedicareFeedsExport : PharmacyCommandBase
    {
        private readonly ISnowflakeMedicareFeedsExportHelper _snowflakeMedicareFeedsExportHelper;

        public SnowflakeMedicareFeedsExport(
            ISnowflakeMedicareFeedsExportHelper snowflakeMedicareFeedsExportHelper,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _snowflakeMedicareFeedsExportHelper = snowflakeMedicareFeedsExportHelper ?? throw new ArgumentNullException(nameof(snowflakeMedicareFeedsExportHelper));
        }

        [Command(
            "snowflake-medicare-feeds-export",
            Description = "Exports various Medicare data from Snowflake",
            Aliases = ["INN545"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            await Task.WhenAll([
                _snowflakeMedicareFeedsExportHelper.ExportHpOnePrescriptionsExport(runFor.RunFor, CancellationToken),
                _snowflakeMedicareFeedsExportHelper.ExportHpOnePharmaciesAsync(CancellationToken),
                _snowflakeMedicareFeedsExportHelper.ExportFdsPharmaciesAsync(runFor.RunFor, CancellationToken),
                _snowflakeMedicareFeedsExportHelper.ExportFdsPrescriptionsAsync(runFor.RunFor, CancellationToken),
            ]);
        }
    }
}
