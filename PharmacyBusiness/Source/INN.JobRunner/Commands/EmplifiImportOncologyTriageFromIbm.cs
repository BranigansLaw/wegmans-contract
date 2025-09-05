using Cocona;
using INN.JobRunner.Commands.CommandHelpers.EmplifiImportOncologyTriageFromIbmHelper;
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

public class EmplifiImportOncologyTriageFromIbm : PharmacyCommandBase
{
    private readonly ITenTenInterface _tenTenInterface;
    private readonly IEmplifiInterface _emplifiInterface;
    private readonly IDataFileInterface _dataFileInterface;
    private readonly ILogger<EmplifiImportOncologyTriageFromIbm> _logger;
    private readonly IHelper _helper;

    public EmplifiImportOncologyTriageFromIbm(
        ITenTenInterface tenTenInterface,
        IEmplifiInterface emplifiInterface,
        IDataFileInterface dataFileInterface,
        ILogger<EmplifiImportOncologyTriageFromIbm> logger,
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
        "emplifi-import-oncology-triage-from-ibm",
        Description = "Import Oncology triage from IBM. Control-M job INN608",
        Aliases = ["INN608"]
    )]
    public async Task RunAsync(RunForFileNameParameter runForFileName)
    {
        _logger.LogInformation($"Starting to import Oncology triages from IBM data file to Emplifi.");

        string batchJobName = "INN608";

        DateOnly runFor = DateOnly.FromDateTime(DateTime.Now);
        string emailSubject = $"{batchJobName} IBM CarePath Oncology Triage Reconciliation";
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
            fileNames = _dataFileInterface.GetFileNames("CarePath_DelayDenial_Triage_*.txt");
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

            IEnumerable<IbmOncologyTriageRow> ibmOncologyTriages = await _dataFileInterface.ReadFileToListAsync<IbmOncologyTriageRow>(
                file,
                "\r\n",
                "|",
                true,
                runFor,
                CancellationToken).ConfigureAwait(false);

            var oncologyTriage = ibmOncologyTriages.Select(ibmOncologyTriage => _helper.MapOncologyTriage(ibmOncologyTriage, completeSpecialtyItemRows));

            // Make API calls to import triage data to Emplifi
            var oncologyTirageNotification = await _emplifiInterface.ProcessOncologyTriageAsync(oncologyTriage, CancellationToken).ConfigureAwait(false);

            emailBody.AppendLine($"<br /><br />File: {Path.GetFileName(file)}");
            emailBody.AppendLine($"<br />Records Received: {oncologyTirageNotification.RecordsRead:#,##0}");
            emailBody.AppendLine($"<br />Records Loaded: {oncologyTirageNotification.RecordsLoaded:#,##0}");
            emailBody.AppendLine($"<br />Records Failed: {oncologyTirageNotification.RecordsFailed:#,##0}");

            if (oncologyTirageNotification.FailedImageFileNames.Count > 0)
            {
                foreach (var failedImageFileName in oncologyTirageNotification.FailedImageFileNames)
                {
                    _dataFileInterface.RejectImageFile(failedImageFileName);
                }

                emailBody.AppendLine($"<br />Images Failed: {oncologyTirageNotification.FailedImageFileNames.Count:#,##0}" +
                    $" - see \\\\wfm.wegmans.com\\departments\\Pharmacy\\HIPAARxOffice\\failed_images\\");
            }

            if (oncologyTirageNotification.ErrorMessages.Any())
            {
                emailBody.AppendLine($"<br />");
                oncologyTirageNotification.ErrorMessages.ForEach(error => emailBody.AppendLine($"<br />{error}"));
            }
        }

        if (dataFileCount > 0)
        {
            _emplifiInterface.SendTriageNotification(
                new EmailExceptionComposerImp(batchJobName, DateTime.Now, MailPriority.Normal),
                emailSubject,
                emailBody.ToString());
        }

        _logger.LogInformation($"Finished importing Oncology triage data to Emplifi");
    }
}