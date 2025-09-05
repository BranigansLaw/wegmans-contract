using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BOpportunityFromTCGFileHelper;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.DataFileInterface;
using Library.DataFileInterface.EmailSender;
using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace INN.JobRunner.Commands
{
    public class TenTenImportAdm340BOpportunityFromTCGFile : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IDataFileInterface _dataFileInterface;
        private readonly IMapper _mapper;
        private readonly ILogger<TenTenImportAdm340BOpportunityFromTCGFile> _logger;

        public TenTenImportAdm340BOpportunityFromTCGFile(
            ITenTenInterface tenTenInterface,
            IDataFileInterface dataFileInterface,
            IMapper mapper,
            ILogger<TenTenImportAdm340BOpportunityFromTCGFile> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "import-adm-340b-opportunity-from-tcg-file", 
            Description = "Import Adm 340B Opportunity from TCG file and send to TenTen. Control-M job INN524", 
            Aliases = ["INN524"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            string batchJobName = "INN524";
            string importFileName = $"340B_WegmansShelter_OrderOpportunity_{runFor.RunFor.ToString("yyyyMMdd")}.txt";
            _logger.LogInformation($"Starting job [{batchJobName}] with Run For Date [{runFor.RunFor.ToShortDateString()}] to import Adm 340B Opportunity file [{importFileName}] from TCG file and send to TenTen.");
            IEnumerable<Adm340BOpportunityRow> adm340BOpportunityRow = await _dataFileInterface.ReadFileToListAndNotifyExceptionsAsync<Adm340BOpportunityRow>(
                new EmailExceptionComposerImp(batchJobName, runFor.RunFor, MailPriority.High),
                importFileName,
                "\n",
                "|",
                true,
                runFor.RunFor,
                CancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Uploading to TenTen Azure Blob");
            // Run asynchronously while API call completes
            ConfiguredTaskAwaitable azureUploadTask = _tenTenInterface.UploadDataAsync(
                toUpload: _mapper.MapAdm340BOpportunityRowToTenTenAzureRow(adm340BOpportunityRow), 
                c: CancellationToken, 
                runDate: runFor.RunFor).ConfigureAwait(false);

            _logger.LogDebug($"Collection Adm 340B Opportunity has: [{adm340BOpportunityRow.Count()}] rows after processing data file.");

            if (adm340BOpportunityRow.Any())
            {
                _logger.LogInformation("Upload Adm 340B Opportunity rows to TenTen");
                await _tenTenInterface.CreateOrAppendTenTenDataAsync(
                    runFor.RunFor, 
                    _mapper.MapAndOrderToTenTenAdm340BOpportunity(adm340BOpportunityRow), 
                    CancellationToken).ConfigureAwait(false);
            }
            else
            {
                _logger.LogWarning("No Adm 340B Opportunity rows to import to 1010.");
            }

            // Wait for Azure Blob upload task to complete
            await azureUploadTask;

            _logger.LogInformation("Finished importing Adm 340B Opportunity rows to 1010");
        }
    }
}
