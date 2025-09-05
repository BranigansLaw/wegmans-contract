using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper;
using INN.JobRunner.CommonParameters;

namespace INN.JobRunner.Commands
{
    public class SnowflakeToTenTenDataSftpExport : PharmacyCommandBase
    {
        private readonly ISnowflakeToTenTenDataSftpExportHelper _snowflakeToTenTenDataSftpExportHelper;

        public SnowflakeToTenTenDataSftpExport(
            ISnowflakeToTenTenDataSftpExportHelper snowflakeToTenTenDataSftpExportHelper,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _snowflakeToTenTenDataSftpExportHelper = snowflakeToTenTenDataSftpExportHelper ?? throw new ArgumentNullException(nameof(snowflakeToTenTenDataSftpExportHelper));
        }

        [Command(
            "snowflake-to-tenten-data-sftp-export",
            Description = "Runs various snowflake queries and exports files to be ready for upload to TenTen",
            Aliases = ["INN544"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            await Task.WhenAll([
                _snowflakeToTenTenDataSftpExportHelper.ExportDurConflictAsync(runFor.RunFor, CancellationToken),
                _snowflakeToTenTenDataSftpExportHelper.ExportPrescriberAddressAsync(runFor.RunFor, CancellationToken),
                _snowflakeToTenTenDataSftpExportHelper.ExportPrescribersAsync(runFor.RunFor, CancellationToken),
                _snowflakeToTenTenDataSftpExportHelper.ExportRxTransferAsync(runFor.RunFor, CancellationToken),
                _snowflakeToTenTenDataSftpExportHelper.ExportSupplierPriceDrugFileExportAsync(runFor.RunFor, CancellationToken),
                _snowflakeToTenTenDataSftpExportHelper.ExportPetPtNumsAsync(runFor.RunFor, CancellationToken),
                _snowflakeToTenTenDataSftpExportHelper.ExportInvAdjAsync(runFor.RunFor, CancellationToken),
                // Future Snowflake exports go here
            ]);
        }
    }
}
