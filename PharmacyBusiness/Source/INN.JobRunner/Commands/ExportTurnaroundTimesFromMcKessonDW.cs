using Cocona;
using FluentDateTime;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.LibraryUtilities;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Library.McKessonDWInterface.Helper;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class ExportTurnaroundTimesFromMcKessonDW : PharmacyCommandBase
    {
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly ILogger<ExportTurnaroundTimesFromMcKessonDW> _logger;
        private readonly ILibraryUtilitiesFileCheckInterface _fileCheckInterface;

        public ExportTurnaroundTimesFromMcKessonDW(
            IMcKessonDWInterface mcKessonDWInterface,
            ILibraryUtilitiesFileCheckInterface fileCheckInterface,
            ILogger<ExportTurnaroundTimesFromMcKessonDW> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _mcKessonDWInterface = mcKessonDWInterface ?? throw new ArgumentNullException(nameof(mcKessonDWInterface));
            _fileCheckInterface = fileCheckInterface ?? throw new ArgumentNullException(nameof(fileCheckInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "export-turnaround-times-from-mckesson-dw", 
            Description = "Export Turnaround Times from McKesson DW and send to Pharmacy Business team. Control-M job INN701", 
            Aliases = ["INN701"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("Starting to export Turnaround Times from McKesson DW to data files.");

            //This weekly job runs on Mondays and exports data from the previous week Sunday through Saturday.
            //If there is an outage on Monday and we need to run on the next day or so, this job can still report on the prior week with the following date range logi and a RunFor date parameter is not needed to be passed in.
            DateTime runDate = runFor.RunFor.ToDateTime(new TimeOnly(0));
            DateOnly previousSunday = DateOnly.FromDateTime(runDate.AddDays(-7).Previous(DayOfWeek.Sunday));
            DateOnly previousSaturday = DateOnly.FromDateTime(runDate.Previous(DayOfWeek.Saturday));

            _logger.LogDebug($"Executing TAT Report 1 of 9 [TAT_Excellus_Raw_Data_Prior_Week_{runDate.ToString("yyyyMMdd")}.csv] from [{previousSunday}] to [{previousSaturday}]...");

            IEnumerable<TatRawDataRow> tatExecellusRawDataRows =
                await _mcKessonDWInterface.GetTatRawDataAsync(previousSunday, previousSaturday, TurnaroundTimeHelperImp.TAT_Target_EXCELLUS, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection tatExecellusRawDataRows has: {tatExecellusRawDataRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatExecellusRawDataRows.ToList(),
                "RawData_TAT_Excellus_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.MoveFileToArchiveForQA("RawData_TAT_Excellus_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            IEnumerable<TatDetailsRow> tatExecellusDetailRows = _mcKessonDWInterface.GetTatDetailsReport(tatExecellusRawDataRows, TurnaroundTimeHelperImp.TAT_Target_EXCELLUS);
            _logger.LogDebug($"Collection tatExecellusDetailRows has: {tatExecellusDetailRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatExecellusDetailRows.ToList(),
                "TAT_Excellus_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.CopyFileToArchiveForQA("TAT_Excellus_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            _logger.LogDebug($"Executing TAT Report 2 of 9 [TAT_Excellus_Prior_Week_{runDate.ToString("yyyyMMdd")}.csv] from [{previousSunday}] to [{previousSaturday}]...");
            IEnumerable<TatSummaryRow> tatExecellusSummaryRows = _mcKessonDWInterface.DeriveTatSummaryReport(tatExecellusDetailRows, TurnaroundTimeHelperImp.TAT_Target_EXCELLUS);
            _logger.LogDebug($"Collection tatExecellusSummaryRows has: {tatExecellusSummaryRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatExecellusSummaryRows.ToList(),
                "TAT_Excellus_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.CopyFileToArchiveForQA("TAT_Excellus_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            _logger.LogDebug($"Executing TAT Report 3 of 9 [TAT_Excellus_MaxRx_Prior_Week_{runDate.ToString("yyyyMMdd")}.csv] from [{previousSunday}] to [{previousSaturday}]...");
            IEnumerable<TatSummaryMaxRxRow> tatExecellusSummaryMaxRxRows = _mcKessonDWInterface.DeriveTatSummaryMaxRxReport(tatExecellusSummaryRows);
            _logger.LogDebug($"Collection tatExecellusSummaryMaxRxRows has: {tatExecellusSummaryMaxRxRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatExecellusSummaryMaxRxRows.ToList(),
                "TAT_Excellus_MaxRx_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.CopyFileToArchiveForQA("TAT_Excellus_MaxRx_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            _logger.LogDebug($"Executing TAT Report 4 of 9 [TAT_IHA_Raw_Data_Prior_Week_{runDate.ToString("yyyyMMdd")}.csv] from [{previousSunday}] to [{previousSaturday}]...");
            IEnumerable<TatRawDataRow> tatIhaRawDataRows =
                await _mcKessonDWInterface.GetTatRawDataAsync(previousSunday, previousSaturday, TurnaroundTimeHelperImp.TAT_Target_IHA, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection tatIhaRawDataRows has: {tatIhaRawDataRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatIhaRawDataRows.ToList(),
                "RawData_TAT_IHA_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.MoveFileToArchiveForQA("RawData_TAT_IHA_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            IEnumerable<TatDetailsRow> tatIhaDetailRows = _mcKessonDWInterface.GetTatDetailsReport(tatIhaRawDataRows, TurnaroundTimeHelperImp.TAT_Target_IHA);
            _logger.LogDebug($"Collection tatIhaDetailRows has: {tatIhaDetailRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatIhaDetailRows.ToList(),
                "TAT_IHA_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.CopyFileToArchiveForQA("TAT_IHA_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            _logger.LogDebug($"Executing TAT Report 5 of 9 [TAT_IHA_Prior_Week_{runDate.ToString("yyyyMMdd")}.csv] from [{previousSunday}] to [{previousSaturday}]...");
            IEnumerable<TatSummaryRow> tatIhaSummaryRows = _mcKessonDWInterface.DeriveTatSummaryReport(tatIhaDetailRows, TurnaroundTimeHelperImp.TAT_Target_IHA);
            _logger.LogDebug($"Collection tatIhaSummaryRows has: {tatIhaSummaryRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatIhaSummaryRows.ToList(),
                "TAT_IHA_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.CopyFileToArchiveForQA("TAT_IHA_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            _logger.LogDebug($"Executing TAT Report 6 of 9 [TAT_IHA_MaxRx_Prior_Week_{runDate.ToString("yyyyMMdd")}.csv] from [{previousSunday}] to [{previousSaturday}]...");
            IEnumerable<TatSummaryMaxRxRow> tatIhaSummaryMaxRxRows = _mcKessonDWInterface.DeriveTatSummaryMaxRxReport(tatIhaSummaryRows);
            _logger.LogDebug($"Collection tatIhaSummaryMaxRxRows has: {tatIhaSummaryMaxRxRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatIhaSummaryMaxRxRows.ToList(),
                "TAT_IHA_MaxRx_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.CopyFileToArchiveForQA("TAT_IHA_MaxRx_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            _logger.LogDebug($"Executing TAT Report 7 of 9 [TAT_Specialty_Raw_Data_Prior_Week_{runDate.ToString("yyyyMMdd")}.csv] from [{previousSunday}] to [{previousSaturday}]...");
            IEnumerable<TatRawDataRow> tatSpecialtyRawDataRows =
                await _mcKessonDWInterface.GetTatRawDataAsync(previousSunday, previousSaturday, TurnaroundTimeHelperImp.TAT_Target_SPECIALTY, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection tatSpecialtyRawDataRows has: {tatSpecialtyRawDataRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatSpecialtyRawDataRows.ToList(),
                "RawData_TAT_Specialty_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.MoveFileToArchiveForQA("RawData_TAT_Specialty_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            IEnumerable<TatDetailsRow> tatSpecialtyDetailRows = _mcKessonDWInterface.GetTatDetailsReport(tatSpecialtyRawDataRows, TurnaroundTimeHelperImp.TAT_Target_SPECIALTY);
            _logger.LogDebug($"Collection  tatSpecialtyDetailRows has: {tatSpecialtyDetailRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatSpecialtyDetailRows.ToList(),
                "TAT_Specialty_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.CopyFileToArchiveForQA("TAT_Specialty_Raw_Data_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            _logger.LogDebug($"Executing TAT Report 8 of 9 [TAT_Specialty_Prior_Week_{runDate.ToString("yyyyMMdd")}.csv] from [{previousSunday}] to [{previousSaturday}]...");
            IEnumerable<TatSummaryRow> tatSpecialtySummaryRows = _mcKessonDWInterface.DeriveTatSummaryReport(tatSpecialtyDetailRows, TurnaroundTimeHelperImp.TAT_Target_SPECIALTY);
            _logger.LogDebug($"Collection tatSpecialtySummaryRows has: {tatSpecialtySummaryRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatSpecialtySummaryRows.ToList(),
                "TAT_Specialty_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.CopyFileToArchiveForQA("TAT_Specialty_Prior_Week_" + runDate.ToString("yyyyMMdd") + ".csv");

            DateOnly startYearToDate = DateOnly.FromDateTime(new DateTime(runDate.Year, 1, 1));
            DateOnly endYearToDate = previousSaturday;

            if (runDate >= new DateTime(runDate.Year, 1, 1) && runDate <= new DateTime(runDate.Year, 1, 15))
            {
                startYearToDate = DateOnly.FromDateTime(new DateTime(runDate.Year - 1, 1, 1));
                endYearToDate = DateOnly.FromDateTime(new DateTime(runDate.Year - 1, 12, 31));
            }
            _logger.LogDebug($"Executing TAT Report 9 of 9 [TAT_Specialty_YTD_{runDate.ToString("yyyyMMdd")}.csv] from [{startYearToDate}] to [{endYearToDate}]...");

            IEnumerable<TatRawDataRow> tatSpecialtyRawDataRowsYTD =
                await _mcKessonDWInterface.GetTatRawDataAsync(startYearToDate, endYearToDate, TurnaroundTimeHelperImp.TAT_Target_SPECIALTY, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection tatSpecialtyRawDataRowsYTD has: {tatSpecialtyRawDataRowsYTD.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatSpecialtyRawDataRowsYTD.ToList(),
                "RawData_TAT_Specialty_YTD_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.MoveFileToArchiveForQA("RawData_TAT_Specialty_YTD_" + runDate.ToString("yyyyMMdd") + ".csv");

            IEnumerable<TatDetailsRow> tatSpecialtyDetailsRowsYTD = _mcKessonDWInterface.GetTatDetailsReport(tatSpecialtyRawDataRowsYTD, TurnaroundTimeHelperImp.TAT_Target_SPECIALTY);
            _logger.LogDebug($"Collection tatSpecialtyDetailsRowsYTD has: {tatSpecialtyDetailsRowsYTD.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatSpecialtyDetailsRowsYTD.ToList(),
                "DetailsRows_TAT_Specialty_YTD_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.MoveFileToArchiveForQA("DetailsRows_TAT_Specialty_YTD_" + runDate.ToString("yyyyMMdd") + ".csv");

            IEnumerable<TatSummaryRow> tatSpecialtySummaryRowsYTD = _mcKessonDWInterface.DeriveTatSummaryReport(tatSpecialtyDetailsRowsYTD, TurnaroundTimeHelperImp.TAT_Target_SPECIALTY);
            _logger.LogDebug($"Collection tatSpecialtySummaryRowsYTD has: {tatSpecialtySummaryRowsYTD.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                tatSpecialtySummaryRowsYTD
                    .OrderByDescending(r => r.DaysNetTat)
                    .ThenBy(r => r.OrderNbr)
                    .ThenBy(r => r.Facility)
                    .ThenBy(r => r.RxNbr)
                    .ThenBy(r => r.RefillNbr)
                    .ToList(),
                "TAT_Specialty_YTD_" + runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                "\"",
                true,
                CancellationToken).ConfigureAwait(false);
            _fileCheckInterface.CopyFileToArchiveForQA("TAT_Specialty_YTD_" + runDate.ToString("yyyyMMdd") + ".csv");

            _logger.LogInformation("Finished exporting Turnaround Times from McKesson DW.");
        }
    }
}
