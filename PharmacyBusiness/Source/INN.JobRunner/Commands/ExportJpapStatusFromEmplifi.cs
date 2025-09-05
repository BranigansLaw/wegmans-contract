using CaseServiceWrapper;
using Cocona;
using INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.EmplifiInterface;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.EmailSender;
using Library.EmplifiInterface.Exceptions;
using Library.EmplifiInterface.Helper;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Text.Json;

namespace INN.JobRunner.Commands;

public class ExportJpapStatusFromEmplifi : PharmacyCommandBase
{
    private readonly IEmplifiInterface _emplifiInterface;
    private readonly IJpapDispenseAndStatusHelper _jpapDispenseAndStatusHelper;
    private readonly IHelper _helper;
    private readonly ILogger<ExportJpapStatusFromEmplifi> _logger;

    public ExportJpapStatusFromEmplifi(
        IEmplifiInterface emplifiInterface,
        IJpapDispenseAndStatusHelper jpapDispenseAndStatusHelper,
        ILogger<ExportJpapStatusFromEmplifi> logger,
        IHelper helper,
        IGenericHelper genericHelper,
        ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
    {
        _emplifiInterface = emplifiInterface ?? throw new ArgumentNullException(nameof(emplifiInterface));
        _jpapDispenseAndStatusHelper = jpapDispenseAndStatusHelper ?? throw new ArgumentNullException(nameof(jpapDispenseAndStatusHelper));
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Command(
        "export-jpap-status-from-emplifi",
        Description = "Export JPAP Status from Emplifi and send to vendor. Control-M job INN622",
        Aliases = ["INN622"]
    )]
    public async Task RunAsync(RunForDateTimeParameter runForDateTime)
    {
        string batchJobName = "INN622";
        _logger.LogInformation($"Starting batch job [{batchJobName}] to export JPAP Status from Emplifi to a data file for RunDate and Time [{runForDateTime.RunFor}].");

        var dateParameters = _jpapDispenseAndStatusHelper.GetDateRangeForOutboundStatus(runForDateTime.RunFor);

        IEnumerable<Case> importRows =
            await _emplifiInterface.GetJpapOutboundStatusAsync(
                dateParameters.startDateTime,
                dateParameters.endDateTime,
                CancellationToken).ConfigureAwait(false);
        _logger.LogDebug($"Collection jpapRows has: {importRows.Count()} rows.");

        var exportRows = _helper.SelectDataRowsMatchingBusinessRules(
            dateParameters.startDateTime,
            dateParameters.endDateTime,
            importRows,
            out List<EmplifiRecordReportingStatus> recordReportingStatus);

        //Setting the date time in the filename to the end date time of the date range so that if there are multiple reruns we will overwrite the one file rather than stacking up multiple files varying timestamps.
        string outputFileName = "Wegmans_PDE_Status_Dispense_Data_" + dateParameters.endDateTime.ToString("yyyyMMddHHmmss") + ".txt";
        _logger.LogDebug($"Collection exportRows has: {exportRows.Count()} rows which will be rewritten to file named [{outputFileName}].");
        await _emplifiInterface.WriteListToFileAsync(
            exportRows.ToList(),
            outputFileName,
            true,
            "|",
            "",
            true,
            CancellationToken).ConfigureAwait(false);

        List<string> notifyDataCorrections = recordReportingStatus
            .Where(r => r.NotifyEndUsersForCorrection == true)
            .Select(r => $"CaseId: {r.CaseId}, IssueSeq: {r.IssueSeq}, IsValidForReporting: {r.IsValidForReporting}, ReportingStatusDescription: {r.ReportingStatusDescription}")
            .ToList();
        _logger.LogDebug($"There are {notifyDataCorrections.Count} rows for notification to end users.");

        //Run API to update ExportedDate for all valid reported records.
        //Do not fail job if this API call fails, but log the error and continue.
        //TODO: Ask Melissa if she wants a notification that this API to update ExportDate failed and which records failed to update.
        List<string> exceptions = await _emplifiInterface.SetExtractedDateAsync(
            recordReportingStatus,
            dateParameters.endDateTime,
            CancellationToken).ConfigureAwait(false);

        if (notifyDataCorrections.Any())
        {
            _emplifiInterface.SendDataIntegrityNotification(
                new EmailExceptionComposerImp(batchJobName, runForDateTime.RunFor, MailPriority.High),
                new DataIntegrityException(outputFileName, (exportRows.Count() + notifyDataCorrections.Count), notifyDataCorrections),
                exceptions);
        }


        //TODO: Remove this next line after testing...or, maybe Melissa might want to keep it.
        await _emplifiInterface.WriteListToFileAsync(
            recordReportingStatus.OrderBy(c => c.CaseId).ThenBy(i => i.IssueSeq).ToList(),
            "DEBUG_" + outputFileName,
            true,
            "|",
            "",
            true,
            CancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Finished exporting Jpap Status Rows.");
    }
}
