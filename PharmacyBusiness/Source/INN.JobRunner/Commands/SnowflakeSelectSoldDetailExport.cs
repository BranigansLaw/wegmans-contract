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
    public class SnowflakeSelectSoldDetailExport : PharmacyCommandBase
    {
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly IDataFileWriter _dataFileWriter;
        private readonly ILogger<SnowflakeSelectSoldDetailExport> _logger;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;

        public SnowflakeSelectSoldDetailExport(
            ISnowflakeInterface snowflakeInterface,
            IDataFileWriter dataFileWriter,
            ILogger<SnowflakeSelectSoldDetailExport> logger,
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
            "snowflake-select-sold-detail-export",
            Description = "Runs snowflake query, SelectSoldDetail.sql",
            Aliases = ["INN539"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("Gathering Snowflake Select Sold Detail data");
            IEnumerable<SelectSoldDetailRow> SelectSoldDetailData = await _snowflakeInterface.QuerySnowflakeAsync(
                new SelectSoldDetailQuery
                {
                    RunDate = runFor.RunFor
                }, CancellationToken);

            _logger.LogInformation($"Returned {SelectSoldDetailData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN539.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(SelectSoldDetailData, new DataFileWriterConfig<SelectSoldDetailRow>
            {
                Header = "StoreNumber|RxNumber|RefillNumber|PartSequenceNumber|SoldDate|OrderNumber|QtyDispensed|NdcWo|AdqCost|TpPay|PatientPay|TxPrice|",
                WriteDataLine = (SelectSoldDetailRow c) => $"{c.StoreNumber}|{c.RxNumber}|{c.RefillNumber}|{c.PartSequenceNumber}|{c.SoldDate}|{c.OrderNumber}|"
                                                          +$"{c.QtyDispensed}|{c.NdcWo}|{c.AdqCost}|{c.TpPay}|{c.PatientPay}|{c.TxPrice}",
                OutputFilePath = writePath,
            });
        }
    }
}
