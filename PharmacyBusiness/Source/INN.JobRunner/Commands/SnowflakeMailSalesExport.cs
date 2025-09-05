using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeMailSalesExportHelper;
using INN.JobRunner.CommonParameters;

namespace INN.JobRunner.Commands
{
    public class SnowflakeMailSalesExport : PharmacyCommandBase
    {
        private readonly ISnowflakeMailSalesExportHelper _snowflakeMailSalesExportHelper;

        public SnowflakeMailSalesExport(
            ISnowflakeMailSalesExportHelper snowflakeMightBeExportHelper,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _snowflakeMailSalesExportHelper = snowflakeMightBeExportHelper ?? throw new ArgumentNullException(nameof(snowflakeMightBeExportHelper));
        }

        [Command(
            "snowflake-mail-sales-export",
            Description = "Runs snowflake query MightBeRefundTransactions.sql and MightBeSoldTransactions.sql",
            Aliases = ["INN546"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            await Task.WhenAll(
                _snowflakeMailSalesExportHelper.ExportMightBeSoldTransactionsAsync(runFor.RunFor, CancellationToken),
                _snowflakeMailSalesExportHelper.ExportMightBeRefundTransactionsAsync(runFor.RunFor, CancellationToken)
            );
        }
    }
}
