using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BInvoicingFromTCGFileHelper;
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
    public class TenTenImportAdm340BInvoicingFromTCGFile : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IDataFileInterface _dataFileInterface;
        private readonly IMapper _mapper;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportAdm340BInvoicingFromTCGFile> _logger;

        public TenTenImportAdm340BInvoicingFromTCGFile(
            ITenTenInterface tenTenInterface,
            IDataFileInterface dataFileInterface,
            IMapper mapper,
            IUtility utility,
            ILogger<TenTenImportAdm340BInvoicingFromTCGFile> logger,
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
            "import-adm-340b-invoicing-from-tcg-file", 
            Description = "Import Adm 340B Invoicing from TCG file and send to TenTen. Control-M job INN523", 
            Aliases = ["INN523"]
        )]
        public async Task RunAsync(RunForParameter runFor )
        {
            string batchJobName = "INN523";
            string importFileNamePattern = $"Wegmans_Pharmacy_Invoices_{runFor.RunFor:yyyyMMdd}*.txt";
            IEnumerable<string> uploadFileNames = _dataFileInterface.GetFileNames(importFileNamePattern);
            uploadFileNames = uploadFileNames.OrderBy(x => x.Length);
            _logger.LogInformation($"Starting job [{batchJobName}] to import [{uploadFileNames.Count()}] import Adm 340B Invoicing file(s) matching this file name pattern [{importFileNamePattern}] from TCG file and send to TenTen");

            foreach (string uploadFileName in uploadFileNames)
            {
                _logger.LogDebug($"Begin processing data file [{uploadFileName}].");

                IEnumerable<Adm340BInvoicingRow> adm340BInvoicingRow = await _dataFileInterface.ReadFileToListAndNotifyExceptionsAsync<Adm340BInvoicingRow>(
                    new EmailExceptionComposerImp(batchJobName, runFor.RunFor, MailPriority.High),
                    uploadFileName,
                    "\n",
                    "|",
                    true,
                    runFor.RunFor,
                    CancellationToken).ConfigureAwait(false);

                _logger.LogDebug($"Collection Adm 340B Invoicing has: [{adm340BInvoicingRow.Count()}] rows after processing data file.");

                if (adm340BInvoicingRow.Any())
                {
                    _logger.LogInformation("Upload Adm 340B Invoicing rows to TenTen");
                    await _tenTenInterface.CreateOrAppendTenTenDataAsync(runFor.RunFor, _mapper.MapToTenTenAdm340BInvoicing(adm340BInvoicingRow)
                        .OrderBy(x => x.ClaimDate)
                        .ThenBy(x => x.DerivedStoreNum)
                        .ThenBy(x => x.RxNum)
                        .ThenBy(x => x.RefillNum), CancellationToken).ConfigureAwait(false);
                }
                else
                {
                    _logger.LogWarning("No Adm 340B Invoicing rows to import to 1010.");
                }

            }
            
            _logger.LogInformation("Finished importing Adm 340B Invoicing rows to 1010");
        }
    }
}
