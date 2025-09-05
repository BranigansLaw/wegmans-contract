using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportMedicareAutoshipFromEnlivenHealthHelper;
using INN.JobRunner.CommonParameters;
using Library.DataFileInterface;
using Library.DataFileInterface.EmailSender;
using Library.DataFileInterface.VendorFileDataModels;
using Library.LibraryUtilities.Extensions;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.IO;
using System.Net.Mail;

namespace INN.JobRunner.Commands
{
    public class TenTenImportMedicareAutoshipFromEnlivenHealth : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IDataFileInterface _dataFileInterface;
        private readonly IMapper _mapper;
        private readonly ILogger<TenTenImportMedicareAutoshipFromEnlivenHealth> _logger;

        public TenTenImportMedicareAutoshipFromEnlivenHealth(
            ITenTenInterface tenTenInterface,
            IDataFileInterface dataFileInterface,
            IMapper mapper,
            ILogger<TenTenImportMedicareAutoshipFromEnlivenHealth> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "tenten-import-medicare-autoship-from-enlivenhealth",
            Description = "Import Medicare Autoship from EnlivenHealth and send to TenTen. Control-M job INN607",
            Aliases = ["INN607"]
        )]
        public async Task RunAsync(RunForDateTimeParameter runForDateTime)
        {
            string batchJobName = "INN607";
            string uploadFileNamePattern = $"Wegmans_Medicare_Autoship_DNC_PhoneNum_*.csv";
            IEnumerable<string> uploadFileNames = _dataFileInterface.GetFileNamesFromImportsAndArchives(uploadFileNamePattern, runForDateTime.RunFor);
            List<string> rejectFileNames = new List<string>();
            _logger.LogInformation($"Starting job [{batchJobName}] to import [{uploadFileNames.Count()}] Medicare Autoship file(s) matching this file name pattern [{uploadFileNamePattern}] from EnlivenHealth file and upload to TenTen.");

            foreach (string uploadFileName in uploadFileNames)
            {
                _logger.LogDebug($"Begin processing data file [{uploadFileName}].");
                DateTime? fileNameDate = _dataFileInterface.ExtractDateFromFileName(uploadFileName, uploadFileNamePattern);
                if (!fileNameDate.HasValue)
                {
                    _dataFileInterface.RejectFile(uploadFileName);
                    rejectFileNames.Add(uploadFileName);
                    continue;
                }

                IEnumerable<MedicareAutoshipRow> medicareAutoshipRows = await _dataFileInterface.ReadFileToListAndNotifyExceptionsAsync<MedicareAutoshipRow>(
                    new EmailExceptionComposerImp(batchJobName, DateOnly.FromDateTime(fileNameDate.Value), MailPriority.High),
                    uploadFileName,
                    "\n",
                    ",",
                    true,
                    DateOnly.FromDateTime(fileNameDate.Value),
                    CancellationToken).ConfigureAwait(false);

                _logger.LogDebug($"Collection medicareAutoshipRows has: [{medicareAutoshipRows.Count()}] rows after processing data file [{uploadFileName}].");

                if (medicareAutoshipRows.Any())
                {
                    _logger.LogInformation($"Upload [{medicareAutoshipRows.Count()}] rows to TenTen from data file [{uploadFileName}]");
                    await _tenTenInterface.CreateOrAppendTenTenDataAsync(DateOnly.FromDateTime(fileNameDate.Value), _mapper.MapToTenTenMedicareAutoship(medicareAutoshipRows), CancellationToken)
                        .ConfigureAwait(false);
                }
                else
                {
                    _logger.LogWarning($"No Medicare Autoship rows to import to 1010 from data file [{uploadFileName}].");
                }
            }

            if (rejectFileNames.Any())
            {
                throw new Exception($"The following files were rejected and moved to the Rejects folder: [{string.Join(", ", rejectFileNames)}]");
            }

            _logger.LogInformation("Finished importing Medicare Autoship files to 1010");
        }
    }
}
