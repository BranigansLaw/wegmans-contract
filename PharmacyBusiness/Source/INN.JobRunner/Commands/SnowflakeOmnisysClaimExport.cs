using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.LibraryUtilities.DataFileWriter;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace INN.JobRunner.Commands
{
    public class SnowflakeOmnisysClaimExport : PharmacyCommandBase
    {
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly IDataFileWriter _dataFileWriter;
        private readonly ILogger<SnowflakeOmnisysClaimExport> _logger;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;

        public SnowflakeOmnisysClaimExport(
            ISnowflakeInterface snowflakeInterface,
            IDataFileWriter dataFileWriter,
            ILogger<SnowflakeOmnisysClaimExport> logger,
            IOptions<SnowflakeDataOutputDirectories> snowflakeDataOutputDirectoriesOptions,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _snowflakeInterface = snowflakeInterface ?? throw new ArgumentNullException(nameof(snowflakeInterface));
            _dataFileWriter = dataFileWriter ?? throw new ArgumentNullException(nameof(dataFileWriter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _snowflakeDataOutputDirectoriesOptions = snowflakeDataOutputDirectoriesOptions ?? throw new ArgumentNullException(nameof(snowflakeDataOutputDirectoriesOptions));
        }

        [Command(
            "snowflake-omnisys-claim-export",
            Description = "Runs snowflake query, Omnisys_Claim_YYYYMMDD.sql",
            Aliases = ["INN531"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("Gathering Snowflake Omnisys Claim data");
            //IEnumerable<OmnisysClaimRow> OmnisysClaimData = await _snowflakeInterface.QuerySnowflakeAsync(
            //    new OmnisysClaimQuery
            //    {
            //        RunDate = runFor.RunFor
            //    }, CancellationToken);

            //_logger.LogInformation($"Returned {OmnisysClaimData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN531.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync([]/*OmnisysClaimData*/, new DataFileWriterConfig<OmnisysClaimRow>
            {
                Header = "PharmacyNpi|PrescriptionNbr|RefillNumber|SoldDate|DateOfService|NdcNumber|CardholderId|AuthorizationNumber|ReservedForFutureUse|",
                WriteDataLine = (OmnisysClaimRow c) => $"{c.PharmacyNpi}|{c.PrescriptionNbr}|{c.RefillNumber}|{c.SoldDate}|{c.DateOfService}|{c.NdcNumber}|{c.CardholderId}|{c.AuthorizationNumber}|{c.ReservedForFutureUse}",
                OutputFilePath = writePath,
            });
        }
    }
}
