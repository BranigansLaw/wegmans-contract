using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportIvrOutboundCallsFromOmnicellFileHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.DataFileInterface;
using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportIvrOutboundCallsFromOmnicellFile : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IDataFileInterface _dataFileInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportIvrOutboundCallsFromOmnicellFile> _logger;

        public TenTenImportIvrOutboundCallsFromOmnicellFile(
            ITenTenInterface tenTenInterface,
            IDataFileInterface dataFileInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportIvrOutboundCallsFromOmnicellFile> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "import-ivr-outbound-calls-from-omnicell-file", 
            Description = "Import IVR Outbound Calls from Omnicell file and send to TenTen. Control-M job INN502",
            Aliases = ["INN502"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _genericHelper.CheckRunForDate(runFor, new DateOnly(2024, 4, 22));

            string importFileName = $"IVROutboundReport_{runFor.RunFor.ToString("yyyyMMdd")}.txt";
            _logger.LogInformation($"Starting to import IVR Outbound Calls file [{importFileName}] from Omnicell file and send to TenTen.");
            IEnumerable<IvrOutboundCallsRow> ivrOutboundCallsRows = await _dataFileInterface.ReadFileToListAsync<IvrOutboundCallsRow>(
               importFileName,
                "\n",
                "|",
                true,
                runFor.RunFor,
                CancellationToken).ConfigureAwait(false);

            _logger.LogDebug($"Collection IVR Outbound Calls has: {ivrOutboundCallsRows.Count()} rows.");

            if (ivrOutboundCallsRows.Any())
            {
                _logger.LogInformation("Upload IVR Outbound Calls rows to TenTen");
                await _tenTenInterface.CreateOrAppendTenTenDataAsync(runFor.RunFor, _mapper.MapToTenTenIvrOutboundCalls(ivrOutboundCallsRows), CancellationToken).ConfigureAwait(false);
            }
            else
            {
                _logger.LogWarning("No IVR Outbound Calls rows to import to 1010.");
            }

            _logger.LogInformation("Finished importing IVR Outbound Calls rows to 1010");
        }
    }
}
