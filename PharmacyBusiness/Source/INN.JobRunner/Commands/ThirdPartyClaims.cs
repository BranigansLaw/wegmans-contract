using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.LibraryUtilities;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class ThirdPartyClaims : PharmacyCommandBase
    {
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly ILogger<ThirdPartyClaims> _logger;
        private readonly ILibraryUtilitiesFileCheckInterface _fileCheckInterface;


        public ThirdPartyClaims(
            IMcKessonDWInterface mcKessonDWInterface,
            ILibraryUtilitiesFileCheckInterface fileCheckInterface,
            ILogger<ThirdPartyClaims> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _mcKessonDWInterface = mcKessonDWInterface ?? throw new ArgumentNullException(nameof(mcKessonDWInterface));
            _fileCheckInterface = fileCheckInterface ?? throw new ArgumentNullException(nameof(fileCheckInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "third-party-claims",
            Description = "Gets Third Party Claim Extracts",
            Aliases = ["INN528"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            string fileName = $"ThirdPartyClaimRawData_{runFor.RunFor.ToString("yyyyMMdd")}.txt"; 

            _logger.LogInformation("Starting to export Third Party Claim data from McKesson DW to a data file.");
            IEnumerable<TpcDataRow> thirdPartyClaimRows = await _mcKessonDWInterface.GetThirdPartyClaimsBaseAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);

            string[] validShortFillStatuses = ["P", "C"];
            foreach (var row in thirdPartyClaimRows)
            {

                if (validShortFillStatuses.Contains(row.ShortFillStatus) && row.TpPlanNumber != default)
                {
                    //LookUpAcquisitionCost
                    row.LookUpAcquisitionCost = await _mcKessonDWInterface.GetThirdPartyClaimsAcquisitionCostAsync(row.RxFillSeq, row.RxRecordNum, row.FillStateKey, CancellationToken);
                }

                //Populate derived data fields.
                DateTime runDate = runFor.RunFor.ToDateTime(new TimeOnly(0));
                DateTime birthDate = (row.BirthDate.HasValue) ? row.BirthDate.GetValueOrDefault() : runDate;
                row.IsSenior = ((runDate - birthDate).TotalDays / 365) >= 60 ? "Y" : "N";
            }

            _logger.LogInformation($"Collection ThirdPartyClaims has {thirdPartyClaimRows.Count()} rows.");

            await _mcKessonDWInterface.WriteListToFileAsync(
                thirdPartyClaimRows.ToList(),
                fileName,
                true,
                "|",
                string.Empty,
                true,
                CancellationToken).ConfigureAwait(false);

            _fileCheckInterface.MoveFileToArchiveForQA(fileName);

            _logger.LogInformation("Finished exporting McKesson DW Third Party Claim Rows.");
        }
    }
}
