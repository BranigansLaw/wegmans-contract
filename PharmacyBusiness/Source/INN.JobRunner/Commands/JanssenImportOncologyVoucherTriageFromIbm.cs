using Cocona;
using INN.JobRunner.Commands.CommandHelpers.JanssenImportOncologyVoucherTriageFromIbmHelper;
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

public class JanssenImportOncologyVoucherTriageFromIbm : PharmacyCommandBase
{
    private readonly ITenTenInterface _tenTenInterface;
    private readonly IEmplifiInterface _emplifiInterface;
    private readonly IDataFileInterface _dataFileInterface;
    private readonly ILogger<JanssenImportOncologyVoucherTriageFromIbm> _logger;
    private readonly IHelper _helper;

    public JanssenImportOncologyVoucherTriageFromIbm(
        ITenTenInterface tenTenInterface,
        IEmplifiInterface emplifiInterface,
        IDataFileInterface dataFileInterface,
        ILogger<JanssenImportOncologyVoucherTriageFromIbm> logger,
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
        "janssen-import-oncology-voucher-triage-from-ibm",
        Description = "Import Oncology voucher triage from IBM. Control-M job INN609",
        Aliases = ["INN609"]
    )]
    public async Task RunAsync(RunForFileNameParameter runForFileName)
    {
        _logger.LogInformation($"Starting to import Oncology voucher triages from IBM data file to Emplifi.");

        string batchJobName = "INN609";

        DateOnly runFor = DateOnly.FromDateTime(DateTime.Now);
        string emailSubject = $"{batchJobName} IBM CarePath Oncology Voucher Triage Reconciliation";
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
            fileNames = _dataFileInterface.GetFileNames("Wegmans_CarePath_Voucher_Triage_*.txt");
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

            IEnumerable<IbmOncologyVoucherTriageRow> ibmOncologyVoucherTriageRows = await _dataFileInterface.ReadFileToListAsync<IbmOncologyVoucherTriageRow>(
                file,
                "\n",
                "|",
                true,
                runFor,
                CancellationToken).ConfigureAwait(false);

            var oncologyVoucherTriage = ibmOncologyVoucherTriageRows.Select(ibmOncologyVoucherTriage => _helper.MapOncologyVoucherTriage(ibmOncologyVoucherTriage, completeSpecialtyItemRows));

            // Make API calls to import triage data to Emplifi
            var oncologyVoucherTirageNotification = await _emplifiInterface.ProcessOncologyVoucherTriageAsync(oncologyVoucherTriage, CancellationToken).ConfigureAwait(false);

            emailBody.AppendLine($"<br /><br />File: {Path.GetFileName(file)}");
            emailBody.AppendLine($"<br />Records Received: {oncologyVoucherTirageNotification.RecordsRead:#,##0}");
            emailBody.AppendLine($"<br />Records Loaded: {oncologyVoucherTirageNotification.RecordsLoaded:#,##0}");
            emailBody.AppendLine($"<br />Records Failed: {oncologyVoucherTirageNotification.RecordsFailed:#,##0}");

            if (oncologyVoucherTirageNotification.FailedImageFileNames.Count > 0)
            {
                foreach (var failedImageFileName in oncologyVoucherTirageNotification.FailedImageFileNames)
                {
                    _dataFileInterface.RejectImageFile(failedImageFileName);
                }

                emailBody.AppendLine($"<br />Images Failed: {oncologyVoucherTirageNotification.FailedImageFileNames.Count:#,##0}" +
                    $" - see \\\\wfm.wegmans.com\\departments\\Pharmacy\\HIPAARxOffice\\failed_images\\");
            }

            if (oncologyVoucherTirageNotification.ErrorMessages.Any())
            {
                emailBody.AppendLine($"<br />");
                oncologyVoucherTirageNotification.ErrorMessages.ForEach(error => emailBody.AppendLine($"<br />{error}"));
            }
        }

        if (dataFileCount > 0)
        {
            _emplifiInterface.SendTriageNotification(
                new EmailExceptionComposerImp(batchJobName, DateTime.Now, MailPriority.Normal),
                emailSubject,
                emailBody.ToString());
        }

        _logger.LogInformation($"Finished importing Oncology voucher triage data to Emplifi");
    }
}