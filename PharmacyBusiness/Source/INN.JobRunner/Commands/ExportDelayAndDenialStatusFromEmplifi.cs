using CaseServiceWrapper;
using Cocona;
using INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper;
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

namespace INN.JobRunner.Commands
{
    public class ExportDelayAndDenialStatusFromEmplifi : PharmacyCommandBase
    {
        private readonly IEmplifiInterface _emplifiInterface;
        private readonly IDelayAndDenialHelper _delayAndDenialHelperImp;
        private readonly IHelper _helper;
        private readonly ILogger<ExportDelayAndDenialStatusFromEmplifi> _logger;

        public ExportDelayAndDenialStatusFromEmplifi(
            IEmplifiInterface emplifiInterface,
            IDelayAndDenialHelper delayAndDenialHelperImp,
            ILogger<ExportDelayAndDenialStatusFromEmplifi> logger,
            IHelper helper,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _emplifiInterface = emplifiInterface ?? throw new ArgumentNullException(nameof(emplifiInterface));
            _delayAndDenialHelperImp = delayAndDenialHelperImp ?? throw new ArgumentNullException(nameof(delayAndDenialHelperImp));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "export-delay-and-denial-status-from-emplifi", 
            Description = "Export Delay and Denial Status from Emplifi and send to vendor. Control-M job INN602", 
            Aliases = ["INN602"]
        )]
        public async Task RunAsync(RunForDateTimeParameter runForDateTime)
        {
            string batchJobName = "INN602";
            _logger.LogInformation($"Starting batch job [{batchJobName}] to export Delay and Denial Status from Emplifi to a data file for RunDate and Time [{runForDateTime.RunFor}].");

            var dateParameters = _delayAndDenialHelperImp.GetDateRangeForOutboundStatus(runForDateTime.RunFor);

            IEnumerable<Case> importRows =
                await _emplifiInterface.GetDelayAndDenialOutboundStatusAsync(
                    dateParameters.startDateTime, 
                    dateParameters.endDateTime,
                    CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection delayAndDenialRows has: {importRows.Count()} rows.");

            var exportRows = _helper.SelectDataRowsMatchingBusinessRules(
                dateParameters.startDateTime, 
                dateParameters.endDateTime, 
                importRows,
                out List<EmplifiRecordReportingStatus> recordReportingStatus);

            //Setting the date time in the filename to the end date time of the date range so that if there are multiple reruns we will overwrite the one file rather than stacking up multiple files varying timestamps.
            string outputFileName = "Wegmans_DelayDenial_Status_Dispense_Data_" + dateParameters.endDateTime.ToString("yyyyMMddHHmmss") + ".txt";
            _logger.LogDebug($"Collection exportRows has: {exportRows.Count()} rows which will be rwritten to file named [{outputFileName}].");
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
                    new DataIntegrityException(outputFileName, (exportRows.Count() + notifyDataCorrections.Count()), notifyDataCorrections),
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

            _logger.LogInformation("Finished exporting Delay and Denial Status Rows.");
        }
    }
}
