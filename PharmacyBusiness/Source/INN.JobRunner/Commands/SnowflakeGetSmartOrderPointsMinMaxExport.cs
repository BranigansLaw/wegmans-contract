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
    public class SnowflakeGetSmartOrderPointsMinMaxExport : PharmacyCommandBase
    {
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly IDataFileWriter _dataFileWriter;
        private readonly ILogger<SnowflakeGetSmartOrderPointsMinMaxExport> _logger;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;
         
        public SnowflakeGetSmartOrderPointsMinMaxExport(
            ISnowflakeInterface snowflakeInterface,
            IDataFileWriter dataFileWriter,
            ILogger<SnowflakeGetSmartOrderPointsMinMaxExport> logger,
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
            "snowflake-get-smart-order-points-min-max-export",
            Description = "Runs snowflake query, GetSmartOrderPointsMinMax.sql",
            Aliases = ["INN536"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("Gathering Snowflake Get Smart Order Points Min Max data");
            IEnumerable<GetSmartOrderMinMaxRow> GetSmartOrderData = await _snowflakeInterface.QuerySnowflakeAsync(
                new GetSmartOrderMinMaxQuery(), CancellationToken);

            _logger.LogInformation($"Returned {GetSmartOrderData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN536.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(GetSmartOrderData, new DataFileWriterConfig<GetSmartOrderMinMaxRow>
            {
                Header = "StoreNumber|NdcWo|MinQtyOverride|MaxQtyOverride|PurchasePlan|LastUpdated",
                WriteDataLine = (GetSmartOrderMinMaxRow c) => $"{c.StoreNumber}|{c.NdcWo}|{c.MinQtyOverride}|{c.MaxQtyOverride}|{c.PurchasePlan}|{c.LastUpdated}",
                OutputFilePath = writePath,
            });
        }
    }
}
