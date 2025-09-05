using Library.LibraryUtilities.DataFileWriter;
using Library.SnowflakeInterface.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeMailSalesExportHelper
{
    public class SnowflakeMailSalesExportHelperImp : ISnowflakeMailSalesExportHelper
    {
        private readonly IDataFileWriter _dataFileWriter;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;
        private readonly ILogger<SnowflakeMailSalesExportHelperImp> _logger;

        public SnowflakeMailSalesExportHelperImp(
            IDataFileWriter dataFileWriter,
            IOptions<SnowflakeDataOutputDirectories> snowflakeDataOutputDirectoriesOptions,
            ILogger<SnowflakeMailSalesExportHelperImp> logger
        )
        {
            _dataFileWriter = dataFileWriter ?? throw new ArgumentNullException(nameof(dataFileWriter));
            _snowflakeDataOutputDirectoriesOptions = snowflakeDataOutputDirectoriesOptions ?? throw new ArgumentNullException(nameof(snowflakeDataOutputDirectoriesOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task ExportMightBeRefundTransactionsAsync(DateOnly runDate, CancellationToken cancellationToken)
        {
            IEnumerable<MightBeRefundTransactionsRow> mightBeRefundTransactionsRows = []; // Do snowflake query here

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN546_MightBeRefund.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(mightBeRefundTransactionsRows, new DataFileWriterConfig<MightBeRefundTransactionsRow>
            {
                Header = "Col1|Col2|Col3",
                WriteDataLine = (MightBeRefundTransactionsRow c) => $"",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc />
        public async Task ExportMightBeSoldTransactionsAsync(DateOnly runDate, CancellationToken cancellationToken)
        {
            IEnumerable<MightBeSoldTransactionsRow> mightBeRefundTransactionsRows = []; // Do snowflake query here

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN546_MightBeSold.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(mightBeRefundTransactionsRows, new DataFileWriterConfig<MightBeSoldTransactionsRow>
            {
                Header = "Col1|Col2|Col3",
                WriteDataLine = (MightBeSoldTransactionsRow c) => $"",
                OutputFilePath = writePath,
            });
        }
    }
}
