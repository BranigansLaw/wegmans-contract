using Cocona;
using INN.JobRunner.Commands.CommandHelpers.EmplifiImportJpapTriageFromIbmHelper;
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

public class EmplifiImportJpapTriageFromIbm : PharmacyCommandBase
{
    private readonly ITenTenInterface _tenTenInterface;
    private readonly IEmplifiInterface _emplifiInterface;
    private readonly IDataFileInterface _dataFileInterface;
    private readonly ILogger<EmplifiImportJpapTriageFromIbm> _logger;
    private readonly IHelper _helper;

    public EmplifiImportJpapTriageFromIbm(
        ITenTenInterface tenTenInterface,
        IEmplifiInterface emplifiInterface,
        IDataFileInterface dataFileInterface,
        ILogger<EmplifiImportJpapTriageFromIbm> logger,
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
        "emplifi-import-jpap-triage-from-ibm",
        Description = "Import JPAP triage from IBM. Control-M job INN606",
        Aliases = ["INN606"]
    )]
    public async Task RunAsync(RunForFileNameParameter runForFileName)
    {
        _logger.LogInformation($"Starting to import JPAP triages from IBM data file to Emplifi.");

        string batchJobName = "INN606";

        DateOnly runFor = DateOnly.FromDateTime(DateTime.Now);
        string emailSubject = $"{batchJobName} IBM CarePath JPAP Triage Reconciliation";
        StringBuilder emailBody = new();
        emailBody.AppendLine($"IBM reconciliation reporting for {runFor:MM/dd/yyyy}");

        // Comment this out if trying to run locally
        _logger.LogInformation($"Run query in 1010data to get Complete Specialty Item List.");
        TenTenDataQuery queryCompleteSpecialtyItem = new TenTenDataQuery(
            $"<base table=\"wegmans.shared.lists.ndc.specialty_item_list_devpharm\"/>",
            ["ndc_wo", "program_header", "actual_drug_name"],
            runFor);
        IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows =
            await _tenTenInterface.GetQueryResultsForTransformingToCollectionsAsync<CompleteSpecialtyItemRow>(queryCompleteSpecialtyItem, CancellationToken).ConfigureAwait(false);
        _logger.LogDebug($"Collection completeSpecialtyItemRows has: [{completeSpecialtyItemRows.Count()}] rows.");
        if (!completeSpecialtyItemRows.Any())
        {
            throw new Exception($"No rows found in the Complete Specialty Item List. Exiting the job.");
        }

        // If a file name is specified, only process that file. Otherwise, process all files in the input directory matching the specified file name pattern
        IEnumerable<string> fileNames;
        if (string.IsNullOrWhiteSpace(runForFileName.RunFor))
        {
            fileNames = _dataFileInterface.GetFileNames("Wegmans_CarePath_PDE_Triage_*.txt");
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

            IEnumerable<IbmJpapTriage> ibmJpapTriages = await _dataFileInterface.ReadFileToListAsync<IbmJpapTriage>(
                file,
                "\n",
                "|",
                true,
                runFor,
                CancellationToken).ConfigureAwait(false);

            var jpapTriages = ibmJpapTriages.Select(ibmJpapTriage => _helper.MapJpapTriage(ibmJpapTriage, completeSpecialtyItemRows));

            // Make API calls to import triage data to Emplifi
            var jpapTriageNotification = await _emplifiInterface.ProcessJpapTriageAsync(jpapTriages, CancellationToken).ConfigureAwait(false);

            emailBody.AppendLine($"<br /><br />File: {Path.GetFileName(file)}");
            emailBody.AppendLine($"<br />Records Received: {jpapTriageNotification.RecordsRead:#,##0}");
            emailBody.AppendLine($"<br />Records Loaded: {jpapTriageNotification.RecordsLoaded:#,##0}");
            emailBody.AppendLine($"<br />Records Failed: {jpapTriageNotification.RecordsFailed:#,##0}");

            if (jpapTriageNotification.FailedImageFileNames.Count > 0)
            {
                foreach (var failedImageFileName in jpapTriageNotification.FailedImageFileNames)
                {
                    _dataFileInterface.RejectImageFile(failedImageFileName);
                }

                emailBody.AppendLine($"<br />Images Failed: {jpapTriageNotification.FailedImageFileNames.Count:#,##0}" +
                    $" - see \\\\wfm.wegmans.com\\departments\\Pharmacy\\HIPAARxOffice\\failed_images\\");
            }

            if (jpapTriageNotification.ErrorMessages.Any())
            {
                emailBody.AppendLine($"<br />");
                jpapTriageNotification.ErrorMessages.ForEach(error => emailBody.AppendLine($"<br />{error}"));
            }
        }

        if (dataFileCount > 0)
        {
            _emplifiInterface.SendTriageNotification(
                new EmailExceptionComposerImp(batchJobName, DateTime.Now, MailPriority.Normal),
                emailSubject,
                emailBody.ToString());
        }

        _logger.LogInformation($"Finished importing JPAP triage data to Emplifi");
    }
}