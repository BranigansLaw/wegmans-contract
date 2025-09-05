using CaseServiceWrapper;
using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.EmplifiInterface;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.EmailSender;
using Library.EmplifiInterface.Exceptions;
using Library.LibraryUtilities;
using Library.LibraryUtilities.Extensions;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Library.McKessonDWInterface.Helper;
using Library.TenTenInterface;
using Library.TenTenInterface.DataModel;
using Library.TenTenInterface.DownloadsFromTenTen;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Net.Mail;

namespace INN.JobRunner.Commands
{
    public class ExportAstuteAdherenceCallsFromMcKesson : PharmacyCommandBase
    {
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IEmplifiInterface _emplifiInterface;
        private readonly ILibraryUtilitiesFileCheckInterface _fileCheckInterface;
        private readonly ILogger<ExportAstuteAdherenceCallsFromMcKesson> _logger;
        private readonly IGenericHelper _helper;

        public ExportAstuteAdherenceCallsFromMcKesson(
            IMcKessonDWInterface mcKessonDWInterface,
            ITenTenInterface tenTenInterface,
            IEmplifiInterface emplifiInterface,
            ILibraryUtilitiesFileCheckInterface fileCheckInterface,
            ILogger<ExportAstuteAdherenceCallsFromMcKesson> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _mcKessonDWInterface = mcKessonDWInterface ?? throw new ArgumentNullException(nameof(mcKessonDWInterface));
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _emplifiInterface = emplifiInterface ?? throw new ArgumentNullException(nameof(emplifiInterface));
            _fileCheckInterface = fileCheckInterface ?? throw new ArgumentNullException(nameof(fileCheckInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _helper = genericHelper ?? throw new ArgumentNullException(nameof(genericHelper));
        }

        [Command(
            "export-astute-adherence-calls-from-mckesson",
            Description = "Export Astute Adherence Calls From McKesson. Control-M job INN605",
            Aliases = ["INN605"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation($"Starting to export Astute Adherence Calls from McKesson to a data file for Run Date [{runFor.RunFor}].");

            string batchJobName = "INN605";
            EmailAttributeData emailAttributeData = new EmailAttributeData()
            {
                OutputFileName = batchJobName + ".csv"
            };

            DateOnly startDate = runFor.RunFor.AddDays(-5);
            DateOnly endDate = runFor.RunFor;

            _logger.LogInformation($"Run query in 1010data to get Complete Specialty Item List.");
            // Comment this out if trying to run locally
            TenTenDataQuery queryCompleteSpecialtyItem = new TenTenDataQuery(
                $"<base table=\"wegmans.shared.lists.ndc.specialty_item_list_devpharm\"/>",
                ["ndc_wo", "program_header", "actual_drug_name"],
                runFor.RunFor);
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows = 
              await _tenTenInterface.GetQueryResultsForTransformingToCollectionsAsync<CompleteSpecialtyItemRow>(queryCompleteSpecialtyItem, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection completeSpecialtyItemRows has: [{completeSpecialtyItemRows.Count()}] rows.");
            if (!completeSpecialtyItemRows.Any())
            {
                throw new Exception($"No rows found in the Complete Specialty Item List. Exiting the job.");
            }

            _logger.LogInformation($"Run query in 1010data to get Specialty Dispense Exclusion List.");
            TenTenDataQuery querySpecialtyDispenseExclusionRows = new TenTenDataQuery(
                $"<base table=\"wegmans.shared.lists.ndc.specialty_dispense_exclusions\"/>",
                ["ndc_wo", "medication"],
                runFor.RunFor);
            IEnumerable<SpecialtyDispenseExclusionRow> specialtyDispenseExclusionRows =
               await _tenTenInterface.GetQueryResultsForTransformingToCollectionsAsync<SpecialtyDispenseExclusionRow>(querySpecialtyDispenseExclusionRows, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection specialtyDispenseExclusionRows has: [{specialtyDispenseExclusionRows.Count()}] rows.");
            if (!specialtyDispenseExclusionRows.Any())
            {
                throw new Exception($"No rows found in the Complete Specialty Item List. Exiting the job.");
            }

            _logger.LogInformation($"Run query in McKesson from Start Date [{startDate:MM/dd/yyyy}] to End Date [{endDate:MM/dd/yyyy}].");
            IEnumerable<AstuteAdherenceDispenseRawDataRow> astuteAdherenceDispenseRawDataRows =
                await _mcKessonDWInterface.GetAstuteAdherenceDispensesAsync(startDate, endDate, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection astuteAdherenceDispenseRawDataRows has: {astuteAdherenceDispenseRawDataRows.Count()} rows.");

            string rawDataFileName = "RawData_AstuteAdherenceDispenses_" + runFor.RunFor.ToString("yyyyMMdd") + ".txt";
            _logger.LogDebug($"Write astuteAdherenceDispenseRows to a raw data file for QA purposes when we move to Snowflake with filename [{rawDataFileName}].");
            await _mcKessonDWInterface.WriteListToFileAsync(
                astuteAdherenceDispenseRawDataRows.ToList(),
                rawDataFileName,
                true,
                "|",
                string.Empty,
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.MoveFileToArchiveForQA(rawDataFileName);

            _logger.LogInformation($"Populate derived data properties for reporting.");
            IEnumerable<AstuteAdherenceDispenseReportRow> astuteAdherenceDispenseReportRows = 
                _mcKessonDWInterface.GetAstuteAdherenceDispensesReport(
                    astuteAdherenceDispenseRawDataRows,
                    completeSpecialtyItemRows,
                    specialtyDispenseExclusionRows,
                    out Dictionary<decimal, List<string>> constraintViolations
                );
            _logger.LogDebug($"Collection astuteAdherenceDispenseReportRows has: {astuteAdherenceDispenseReportRows.Count()} rows.");


            _logger.LogInformation($"Make API calls for astuteAdherenceDispenseReportRows collection.");
             List<string> exceptions = await _emplifiInterface.ProcessAstuteAdherenceDispenseAsync(
                    astuteAdherenceDispenseReportRows,
                    CancellationToken
                ).ConfigureAwait(false);


            if (constraintViolations.Any() || exceptions.Any())
            {
                _emplifiInterface.SendDataNotification(
                    new EmailExceptionComposerImp(batchJobName, DateTime.Now, MailPriority.High), 
                    constraintViolations,
                    emailAttributeData,
                    exceptions);
            }

            _logger.LogInformation($"Finished exporting Astute Adherence Calls");
        }
    }
}
