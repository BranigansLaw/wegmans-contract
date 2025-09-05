using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeIqviaAndImiExportHelper;
using INN.JobRunner.CommonParameters;
using Library.SnowflakeInterface.Data;

namespace INN.JobRunner.Commands
{
    public class SnowflakeIqviaAndImiExport : PharmacyCommandBase
    {
        private readonly ISnowflakeIqviaAndImiExportHelper _snowflakeIqviaAndImiExportHelper;

        public SnowflakeIqviaAndImiExport(
            ISnowflakeIqviaAndImiExportHelper snowflakeIqviaAndImiExportHelper,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _snowflakeIqviaAndImiExportHelper = snowflakeIqviaAndImiExportHelper ?? throw new ArgumentNullException(nameof(snowflakeIqviaAndImiExportHelper));
        }

        [Command(
            "snowflake-inqvia-and-imi-export",
            Description = "Runs snowflake query WEG_086_YYYYMMDD_01.sql and WEG_086_YYYYMMDD_01_For198.sql",
            Aliases = ["INN547"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            //TODO: Implement fixed width for the data export. Here are some sample lines of code to demonstrate how to do all that.
            IEnumerable<WEG08601Row> weg08601Rows = new List<WEG08601Row>();
            foreach (var weg08601Row in weg08601Rows)
            {
                _snowflakeIqviaAndImiExportHelper.HandleSetToFixedWidth(weg08601Row);
            }

            await Task.WhenAll(
                _snowflakeIqviaAndImiExportHelper.ExportImiAsync(runFor.RunFor, CancellationToken),
                _snowflakeIqviaAndImiExportHelper.ExportInqviaAsync(runFor.RunFor, CancellationToken)
            );
        }
    }
}
