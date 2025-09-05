using Cocona;
using INN.JobRunner.Commands.CommandHelpers.EmplifiImportJpapEligibilityFromIbmHelper;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.DataFileInterface;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface;
using Library.EmplifiInterface.EmailSender;
using Library.TenTenInterface;
using Library.TenTenInterface.DataModel;
using Library.TenTenInterface.DownloadsFromTenTen;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Text;

namespace INN.JobRunner.Commands;

public class EmplifiImportJpapEligibilityFromIbm : PharmacyCommandBase
{
    private readonly ITenTenInterface _tenTenInterface;
    private readonly IEmplifiInterface _emplifiInterface;
    private readonly IDataFileInterface _dataFileInterface;
    private readonly ILogger<EmplifiImportJpapEligibilityFromIbm> _logger;
    private readonly IHelper _helper;

    public EmplifiImportJpapEligibilityFromIbm(
        ITenTenInterface tenTenInterface,
        IEmplifiInterface emplifiInterface,
        IDataFileInterface dataFileInterface,
        ILogger<EmplifiImportJpapEligibilityFromIbm> logger,
        IHelper helper,
        IGenericHelper genericHelper,
        ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
    {
        _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
        _emplifiInterface = emplifiInterface ?? throw new ArgumentNullException(nameof(emplifiInterface));
        _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
    }

    [Command(
        "emplifi-jpap-eligibility-from-ibm",
        Description = "Import Jpap Eligibility from IBM. Control-M job INN611",
        Aliases = ["INN611"]
    )]
    public async Task RunAsync(RunForFileNameParameter runForFileName)
    {
        _logger.LogInformation($"Starting to import Jpap Eligibility from IBM data file to Emplifi.");

        string batchJobName = "INN611";

        DateOnly runFor = DateOnly.FromDateTime(DateTime.Now);
        string emailSubject = $"{batchJobName} IBM CarePath JPAP Eligibility Reconciliation";
        StringBuilder emailBody = new();
        emailBody.AppendLine($"IBM reconciliation reporting for {runFor:MM/dd/yyyy}");

        // If a file name is specified, only process that file. Otherwise, process all files in the input directory matching the specified file name pattern
        IEnumerable<string> fileNames;
        if (string.IsNullOrWhiteSpace(runForFileName.RunFor))
        {
            fileNames = _dataFileInterface.GetFileNames("CarePath_Wegmans_EligibilityFeed_*.txt");
        }
        else
        {
            fileNames = [runForFileName.RunFor];
        }

        int dataFileCount = 0;
        foreach (var file in fileNames)
        {
            // Archive control files, which are not used
            if (file.Contains("_Control", StringComparison.OrdinalIgnoreCase))
            {
                _dataFileInterface.ArchiveFile(file);
                continue;
            }
            dataFileCount++;
            _logger.LogInformation($"Processing file [{file}].");

            IEnumerable<IbmJpapEligibilityRow> ibmJpapEligiblityRows = await _dataFileInterface.ReadFileToListAsync<IbmJpapEligibilityRow>(
                file,
                "\n",
                "|",
                true,
                runFor,
                CancellationToken).ConfigureAwait(false);

            var jpapEligibilityRows = ibmJpapEligiblityRows.Select(_helper.MapJpapEligibility);

            // Make API calls to import eligibility data to Emplifi
            var jpapEligibilityNotification = await _emplifiInterface.ProcessJpapEligibilityAsync(jpapEligibilityRows, CancellationToken).ConfigureAwait(false);

            emailBody.AppendLine($"<br /><br />File: {Path.GetFileName(file)}");
            emailBody.AppendLine($"<br />Records Received: {jpapEligibilityNotification.RecordsRead:#,##0}");
            emailBody.AppendLine($"<br />Records Loaded: {jpapEligibilityNotification.RecordsLoaded:#,##0}");
            emailBody.AppendLine($"<br />Records Failed: {jpapEligibilityNotification.RecordsFailed:#,##0}");
            emailBody.AppendLine($"<br />Records Skipped: {jpapEligibilityNotification.RecordsSkipped:#,##0}");

            if (jpapEligibilityNotification.FailedFileNames.Count > 0)
            {
                emailBody.AppendLine($"<br />File Failed: {jpapEligibilityNotification.FailedFileNames.Count:#,##0}");
            }

            if (jpapEligibilityNotification.ErrorMessages.Any())
            {
                emailBody.AppendLine($"<br />");
                jpapEligibilityNotification.ErrorMessages.ForEach(error => emailBody.AppendLine($"<br />{error}"));
            }
        }

        if (dataFileCount > 0)
        {
            _emplifiInterface.SendEligibilityNotification(
                new EmailExceptionComposerImp(batchJobName, DateTime.Now, MailPriority.Normal),
                emailSubject,
                emailBody.ToString());
        }

        _logger.LogInformation($"Finished importing Jpap Eligibility data to Emplifi");
    }
}