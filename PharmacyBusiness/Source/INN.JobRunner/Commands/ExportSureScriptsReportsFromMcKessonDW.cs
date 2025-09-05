using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.LibraryUtilities;
using Library.LibraryUtilities.DataFileWriter;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Library.McKessonDWInterface.Helper;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class ExportSureScriptsReportsFromMcKessonDW : PharmacyCommandBase
    {
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly ILogger<ExportSureScriptsReportsFromMcKessonDW> _logger;
        private readonly ILibraryUtilitiesFileCheckInterface _fileCheckInterface;
        private readonly ISureScriptsHelper _sureScriptsHelper;
        private readonly IDataFileWriter _dataFileWriter;

        public ExportSureScriptsReportsFromMcKessonDW(
            IMcKessonDWInterface mcKessonDWInterface,
            ISnowflakeInterface snowflakeInterface,
            ILibraryUtilitiesFileCheckInterface fileCheckInterface,
            ILogger<ExportSureScriptsReportsFromMcKessonDW> logger,
            IGenericHelper genericHelper,
            ISureScriptsHelper sureScriptsHelper,
            IDataFileWriter dataFileWriter,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _mcKessonDWInterface = mcKessonDWInterface ?? throw new ArgumentNullException(nameof(mcKessonDWInterface));
            _snowflakeInterface = snowflakeInterface ?? throw new ArgumentNullException(nameof(snowflakeInterface));
            _fileCheckInterface = fileCheckInterface ?? throw new ArgumentNullException(nameof(fileCheckInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sureScriptsHelper = sureScriptsHelper ?? throw new ArgumentNullException(nameof(sureScriptsHelper));
            _dataFileWriter = dataFileWriter ?? throw new ArgumentNullException(nameof(dataFileWriter));
        }

        [Command(
            "export-sure-scripts-reports-from-mckesson-dw",
            Description = "Export Sure Scripts reports from McKesson DW and send to vendor Sure Scripts. Control-M job INN603",
            Aliases = ["INN603"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogDebug($"Begin making three data extracts for Sure Scripts with Run Date [{runFor.RunFor}].");
            string fileNameMedicalHistory = "PEFWegmans" + runFor.RunFor.ToString("yyyyMMdd") + "01.txt";
            string fileNamePhysicianNotificationLetterAllStates = "IMMWegmans" + runFor.RunFor.ToString("yyyyMMdd") + "01.txt";
            string fileNamePhysicianNotificationLetterPAState = "IMMWegmansPA" + runFor.RunFor.ToString("yyyyMMdd") + "01.txt";

            _logger.LogDebug($"Begin query of Medical History raw data McKesson DW.");
            IEnumerable<SureScriptsMedicalHistoryRawDataRow> medicalHistoryRawDataRows = 
                await _mcKessonDWInterface.GetSureScriptsMedicalHistoryRawDataAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection medicalHistoryRawDataRows has: {medicalHistoryRawDataRows.Count()} rows.");

            var medicalHistoryDeDuplicatedDataRows = _sureScriptsHelper.DeDuplicateMedHistoryRows(medicalHistoryRawDataRows);

            var medicalHistoryRankedDataRows = _sureScriptsHelper.RankMedHistoryByPrescriptionFill(medicalHistoryDeDuplicatedDataRows);

            // Snowflake Query (does not return correct row count, left in so there is a link from this command to the correct Snowflake query
            //IEnumerable<SelectSureScriptsMedicalHistoryRow> snowflakeData = await _snowflakeInterface.QuerySnowflakeAsync(new SelectSureScriptsMedicalHistoryQuery
            //{
            //    RunDate = runFor.RunFor,
            //}, CancellationToken).ConfigureAwait(false);

            await _mcKessonDWInterface.WriteListToFileAsync(
                medicalHistoryRawDataRows.ToList(),
                "RawData_MedicalHistory_" + runFor.RunFor.ToString("yyyyMMdd") + ".txt",
                false,
                "|",
                string.Empty,
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.MoveFileToArchiveForQA("RawData_MedicalHistory_" + runFor.RunFor.ToString("yyyyMMdd") + ".txt");

            _logger.LogDebug($"Begin applying business rules to Medical History raw data to get final Medical History report.");
            IEnumerable<SureScriptsMedicalHistoryReportRow> medicalHistoryReportRows = _mcKessonDWInterface.DeriveSureScriptsMedicalHistoryReport(medicalHistoryRankedDataRows);

            var medicalHistorySortedDataRows = medicalHistoryReportRows
                .OrderBy(s1 => s1.PrescriptionNumber).ThenBy(s2 => s2.FillNumber).ThenBy(s3 => s3.CompoundIngredientSequenceNumber);

            _logger.LogDebug($"Begin writing data extract 1 of 3 with file name [{fileNameMedicalHistory}] having [{medicalHistoryReportRows.Count()}] rows.");
            await _mcKessonDWInterface.WriteListToFileAsync(
                [_sureScriptsHelper.GetMedHistoryHeaderRecord(runFor.RunFor)],
                fileNameMedicalHistory,
                false,
                "|",
                string.Empty,
                true,
                false,
                CancellationToken).ConfigureAwait(false);
            await _mcKessonDWInterface.WriteListToFileAsync(
                medicalHistorySortedDataRows.ToList(),
                fileNameMedicalHistory,
                false,
                "|",
                string.Empty,
                true,
                true,
                CancellationToken).ConfigureAwait(false);
            await _mcKessonDWInterface.WriteListToFileAsync(
                [_sureScriptsHelper.GetMedHistoryTrailerRecord(medicalHistoryReportRows.Count())],
                fileNameMedicalHistory,
                false,
                "|",
                string.Empty,
                true,
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.CopyFileToArchiveForQA(fileNameMedicalHistory);

            _logger.LogDebug($"Begin query of Physician Notification Letters raw data from McKesson DW.");
            IEnumerable<SureScriptsPhysicianNotificationLetterRawDataRow> pnlRawDataRows = await _mcKessonDWInterface.GetSureScriptsPhysicianNotificationLettersRawDataAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection pnlRawDataRows has: {pnlRawDataRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                pnlRawDataRows.ToList(),
                "RawData_PhysicianNotificationLetters_" + runFor.RunFor.ToString("yyyyMMdd") + ".txt",
                false,
                "|",
                string.Empty,
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.MoveFileToArchiveForQA("RawData_PhysicianNotificationLetters_" + runFor.RunFor.ToString("yyyyMMdd") + ".txt");

            _logger.LogDebug($"Begin applying business rules to Physician Notification Letters raw data to get final reports (one for All States and another for State of PA).");
            IEnumerable<SureScriptsPhysicianNotificationLetterReportRow> pnlPAStateReportRows = _mcKessonDWInterface.DeriveSureScriptsPhysicianNotificationLetterReport(pnlRawDataRows, "PA");
            IEnumerable<SureScriptsPhysicianNotificationLetterReportRow> pnlAllStateReportRows = _mcKessonDWInterface.DeriveSureScriptsPhysicianNotificationLetterReport(pnlRawDataRows, "ALL");

            _logger.LogDebug($"Begin writing data extract 2 of 3 with file name [{fileNamePhysicianNotificationLetterPAState}] having [{pnlPAStateReportRows.Count()}] rows.");
            await _dataFileWriter.WriteDataToFileAsync(_sureScriptsHelper.GetPNLHeaderRecord(runFor.RunFor, "|"), new DataFileWriterConfig<char>
            {
                OutputFilePath = fileNamePhysicianNotificationLetterPAState,
                Header = null,
                WriteDataLine = (char c) => $"{c}"
            });
            await _mcKessonDWInterface.WriteListToFileAsync(
                pnlPAStateReportRows.ToList(),
                fileNamePhysicianNotificationLetterPAState,
                false,
                "|",
                string.Empty,
                true,
                CancellationToken).ConfigureAwait(false);
            await _dataFileWriter.WriteDataToFileAsync(_sureScriptsHelper.GetPNLTrailerRecord(pnlPAStateReportRows.Count(), "|"), new DataFileWriterConfig<char>
            {
                OutputFilePath = fileNamePhysicianNotificationLetterPAState,
                Header = null,
                WriteDataLine = (char c) => $"{c}"
            });
            _fileCheckInterface.CopyFileToArchiveForQA(fileNamePhysicianNotificationLetterPAState);

            _logger.LogDebug($"Begin writing data extract 3 of 3 with file name [{fileNamePhysicianNotificationLetterAllStates}] having [{pnlAllStateReportRows.Count()}] rows.");
            await _dataFileWriter.WriteDataToFileAsync(_sureScriptsHelper.GetPNLHeaderRecord(runFor.RunFor, "|"), new DataFileWriterConfig<char>
            {
                OutputFilePath = fileNamePhysicianNotificationLetterPAState,
                Header = null,
                WriteDataLine = (char c) => $"{c}"
            });
            await _mcKessonDWInterface.WriteListToFileAsync(
                pnlAllStateReportRows.ToList(),
                fileNamePhysicianNotificationLetterAllStates,
                false,
                "|",
                string.Empty,
                true,
                CancellationToken).ConfigureAwait(false);
            await _dataFileWriter.WriteDataToFileAsync(_sureScriptsHelper.GetPNLTrailerRecord(pnlAllStateReportRows.Count(), "|"), new DataFileWriterConfig<char>
            {
                OutputFilePath = fileNamePhysicianNotificationLetterPAState,
                Header = null,
                WriteDataLine = (char c) => $"{c}"
            });
            _fileCheckInterface.CopyFileToArchiveForQA(fileNamePhysicianNotificationLetterAllStates);

            _logger.LogInformation("Finished making data extracts from McKesson DW to vendor Sure Scripts.");
        }
    }
}
