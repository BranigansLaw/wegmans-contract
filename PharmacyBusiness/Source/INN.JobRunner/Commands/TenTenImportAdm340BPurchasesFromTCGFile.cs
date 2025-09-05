using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BPurchasesFromTCGFileHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.DataFileInterface;
using Library.DataFileInterface.EmailSender;
using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;
using System.Net.Mail;

namespace INN.JobRunner.Commands
{
    public class TenTenImportAdm340BPurchasesFromTCGFile : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IDataFileInterface _dataFileInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportAdm340BPurchasesFromTCGFile> _logger;

        public TenTenImportAdm340BPurchasesFromTCGFile(
            ITenTenInterface tenTenInterface,
            IDataFileInterface dataFileInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportAdm340BPurchasesFromTCGFile> logger,
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
            "import-adm-340b-purchases-from-tcg-file", 
            Description = "Import Adm 340B Purchases from TCG file and send to TenTen. Control-M job INN525", 
            Aliases = ["INN525"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            string batchJobName = "INN525";
            string importFileName = $"Wegmans_Purchase_{runFor.RunFor.ToString("yyyyMMdd")}.txt";
            _logger.LogInformation($"Starting job [{batchJobName}] with Run For Date [{runFor.RunFor.ToShortDateString()}] to import Adm 340B Purchases file [{importFileName}] from TCG file and send to TenTen.");
            IEnumerable<Adm340BPurchasesRow> adm340BPurchasesRow = await _dataFileInterface.ReadFileToListAndNotifyExceptionsAsync<Adm340BPurchasesRow>(
                new EmailExceptionComposerImp(batchJobName, runFor.RunFor, MailPriority.High),
                importFileName,
                "\n",
                "|",
                true,
                runFor.RunFor,
                CancellationToken).ConfigureAwait(false);

            _logger.LogDebug($"Collection Adm 340B Purchases has: [{adm340BPurchasesRow.Count()}] rows after processing data file.");

            if (adm340BPurchasesRow.Any())
            {
                _logger.LogInformation("Upload Adm 340B Purchases rows to TenTen");
                await _tenTenInterface.CreateOrAppendTenTenDataAsync(runFor.RunFor, _mapper.MapToTenTenAdm340BPurchases(adm340BPurchasesRow)
                    .OrderBy(x => x.DerivedStoreNum), CancellationToken).ConfigureAwait(false);
            }
            else
            {
                _logger.LogWarning("No Adm 340B Purchases rows to import to 1010.");
            }

            _logger.LogInformation("Finished importing Adm 340B Purchases rows to 1010");
        }
    }
}
