using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportCallRecordDetailFromCisco : PharmacyCommandBase
    {
        private readonly ILogger<TenTenImportCallRecordDetailFromCisco> _logger;

        public TenTenImportCallRecordDetailFromCisco(
            ILogger<TenTenImportCallRecordDetailFromCisco> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
           _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "tenten-import-call-record-detail-from-cisco", 
            Description = "Import call record detail from Cisco and send to TenTen. Control-M job INN508",
            Aliases = ["INN508"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("UNDER CONSTRUCTION");
            await Task.CompletedTask; //To be deleted after implementation.
        }
    }
}
