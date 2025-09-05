using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportNetSalesFromNetezzaHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.NetezzaInterface;
using Library.NetezzaInterface.DataModel;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportNetSalesFromNetezza : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly INetezzaInterface _netezzaInterface;
        private readonly IMapper _mapper;
        private readonly ILogger<TenTenImportNetSalesFromNetezza> _logger;

        public TenTenImportNetSalesFromNetezza(
            ITenTenInterface tenTenInterface,
            INetezzaInterface netezzaInterface,
            IMapper mapper,
            ILogger<TenTenImportNetSalesFromNetezza> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _netezzaInterface = netezzaInterface ?? throw new ArgumentNullException(nameof(netezzaInterface));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "tenten-import-net-sales-from-netezza", 
            Description = "Import net sales from Netezza and send to TenTen. Control-M job INN501",
            Aliases = ["INN501"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _genericHelper.CheckRunForDate(runFor, new DateOnly(2024, 2, 13));

            await _genericHelper.RunFromToRunTillAsync(runFor, RunCommandLogicForDateAsync);
        }

        public async Task RunCommandLogicForDateAsync(DateOnly forDate)
        {
            _logger.LogInformation("Starting to import Netezza net sales rows for date {0}", forDate);
            IEnumerable<NetSaleRow> netezzaRows =
                await _netezzaInterface.GetNetSalesAsync(forDate, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection netezzaRows has: {netezzaRows.Count()} rows.");

            _logger.LogInformation("Upload Netezza Rows to TenTen");
            await _tenTenInterface.CreateOrAppendTenTenDataAsync(forDate, _mapper.MapToTenTenNetSales(netezzaRows), CancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Finished importing Netezza net sales rows");
        }
    }
}
