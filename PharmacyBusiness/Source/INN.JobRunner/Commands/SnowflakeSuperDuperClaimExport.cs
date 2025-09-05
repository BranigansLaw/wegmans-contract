using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.LibraryUtilities.DataFileWriter;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace INN.JobRunner.Commands
{
    public class SnowflakeSuperDuperClaimExport : PharmacyCommandBase
    {
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly IDataFileWriter _dataFileWriter;
        private readonly ILogger<SnowflakeDeceasedExport> _logger;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;

        public SnowflakeSuperDuperClaimExport(
            ISnowflakeInterface snowflakeInterface,
            IDataFileWriter dataFileWriter,
            ILogger<SnowflakeDeceasedExport> logger,
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
            "snowflake-super-duper-claim-export",
            Description = "Runs snowflake query, Super_Duper_Claim_YYYYMMDD.sql",
            Aliases = ["INN617"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("Gathering AtebMaster data");
            IEnumerable<DeceasedRow> deceasedData = []; // Call Super_Duper_Claim_YYYYMMDD.sql

            _logger.LogInformation($"Returned {deceasedData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN617.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(deceasedData, new DataFileWriterConfig<DeceasedRow>
            {
                Header = "Col1|Col2|Col3",
                WriteDataLine = (DeceasedRow c) => $"",
                OutputFilePath = writePath,
            });
        }
    }
}
