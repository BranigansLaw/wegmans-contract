using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.DataFileInterface;
using Library.LibraryUtilities;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class ScriptsWorkloadBalancing : PharmacyCommandBase
    {
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly IDataFileInterface _dataFileInterface;
        private readonly ILogger<ScriptsWorkloadBalancing> _logger;
        private readonly ILibraryUtilitiesFileCheckInterface _fileCheckInterface;

        public ScriptsWorkloadBalancing(
            IMcKessonDWInterface mcKessonDWInterface,
            ILibraryUtilitiesFileCheckInterface fileCheckInterface,
            IDataFileInterface dataFileInterface,
            ILogger<ScriptsWorkloadBalancing> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _mcKessonDWInterface = mcKessonDWInterface ?? throw new ArgumentNullException(nameof(mcKessonDWInterface));
            _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
            _fileCheckInterface = fileCheckInterface ?? throw new ArgumentNullException(nameof(fileCheckInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "scripts-workload-balancing", 
            Description = "Gets the Scripts Workload Balancing Step rows",
            Aliases = ["INN529"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            string filename = $"WorkloadBalanceRawData_{runFor.RunFor.ToString("yyyyMMdd")}.txt";
            _logger.LogInformation("Starting to export Scripts Workload Balancing data from McKesson DW to a data file");

            IEnumerable<WorkloadBalanceRow> scriptsWorkloadBalanceRows =
                await _mcKessonDWInterface.GetWorkloadBalanceRowsAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);

            decimal?[] skipPreVerCodes = [4, 8];
            foreach (var row in scriptsWorkloadBalanceRows)
            {
                if(row.WorkflowStep == "Drug Utilization Review" && skipPreVerCodes.Contains(row.SkipPreVerCode))
                {
                    row.WorkflowStep = "Pre-Verification Verified";
                }
            }
            _logger.LogDebug($"Collection scriptsWorkloadBalanceRows has: {scriptsWorkloadBalanceRows.Count()} rows.");

             await _mcKessonDWInterface.WriteListToFileAsync(
                scriptsWorkloadBalanceRows.ToList(),
                filename,
                true,
                "|",
                string.Empty,
                true,
                CancellationToken).ConfigureAwait(false);

            _fileCheckInterface.MoveFileToArchiveForQA(filename);
            _logger.LogInformation("Finished exporting Scripts Workload Balancing Step rows");
        }
    }
}
