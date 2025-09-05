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
    public class SnowflakeWorkersCompMonthlyExport : PharmacyCommandBase
    {
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly IDataFileWriter _dataFileWriter;
        private readonly ILogger<SnowflakeDeceasedExport> _logger;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;

        public SnowflakeWorkersCompMonthlyExport(
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
            "snowflake-workers-comp-monthly-export",
            Description = "Runs snowflake query, WorkersCompMonthly_YYYYMMDD.sql",
            Aliases = ["INN801"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("Gathering WorkersComp data");
            IEnumerable<WorkersCompMonthly> workersCompData = await _snowflakeInterface.QuerySnowflakeAsync(
                new WorkersCompMonthlyQuery 
                {
                    RunDate = runFor.RunFor
                }, 
                CancellationToken);

            _logger.LogInformation($"Returned {workersCompData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN801.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(workersCompData, new DataFileWriterConfig<WorkersCompMonthly>
            {
                Header = "FacilityIdNumber|RxNumber|DateFilled|PatientLastName|PatientFirstName|CardholderId|PatientDob|DrugNdcNumber|DrugName|QtyDrugDispensed|TxPrice|PatientPay|AdjAmt|" +
                         "ThirdPartyName|ThirdPartyCode|SplitBillIndicator|PrescriberName|DateSold|ClaimNumber",
                WriteDataLine = (WorkersCompMonthly c) => $"{c.FacilityIdNumber}|{c.RxNumber}|{c.DateFilled}|{c.PatientLastName}|{c.PatientFirstName}|{c.CardholderId}|"
                                                          + $"{c.PatientDob}|{c.DrugNdcNumber}|{c.DrugName}|{c.QtyDrugDispensed}|{c.TxPrice}|{c.PatientPay}|{c.AdjAmt}|"
                                                          + $"{c.ThirdPartyName}|{c.ThirdPartyCode}|{c.SplitBillIndicator}|{c.PrescriberName}|{c.DateSold}|{c.ClaimNumber}",
                OutputFilePath = writePath,
            });
        }
    }
}
