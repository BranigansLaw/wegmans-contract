using INN.JobRunner.Commands.CommandHelpers.SnowflakeMailSalesExportHelper;
using Library.LibraryUtilities.DataFileWriter;
using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Library.LibraryUtilities.Extensions.StringExtensions;

namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeIqviaAndImiExportHelper
{
    public class SnowflakeIqviaAndImiExportHelperImp : ISnowflakeIqviaAndImiExportHelper
    {
        private readonly IDataFileWriter _dataFileWriter;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;
        private readonly ILogger<SnowflakeMailSalesExportHelperImp> _logger;

        public SnowflakeIqviaAndImiExportHelperImp(
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
        public async Task ExportImiAsync(DateOnly runDate, CancellationToken cancellationToken)
        {
            IEnumerable<MightBeRefundTransactionsRow> mightBeRefundTransactionsRows = []; // Do snowflake query here

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN547_Imi.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(mightBeRefundTransactionsRows, new DataFileWriterConfig<MightBeRefundTransactionsRow>
            {
                Header = "Col1|Col2|Col3",
                WriteDataLine = (c) => $"",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc />
        public async Task ExportInqviaAsync(DateOnly runDate, CancellationToken cancellationToken)
        {
            IEnumerable<MightBeRefundTransactionsRow> mightBeRefundTransactionsRows = []; // Do snowflake query here

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN547_Inqvia.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(mightBeRefundTransactionsRows, new DataFileWriterConfig<MightBeRefundTransactionsRow>
            {
                Header = "Col1|Col2|Col3",
                WriteDataLine = (c) => $"",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc />
        public void HandleSetToFixedWidth(WEG08601Row weg08601Row)
        {
            //TODO: Implement fixed width justification for the data. Here are some sample lines of code to demonstrate how to do all that.
            //      Specifications for widths and justifications can be found here "Source\RX.PharmacyBusiness.ETL\CRX572\WEG_086_YYYYMMDD_01.sql"
            /*
            
            weg08601Row.FIELD01 = weg08601Row.FIELD01.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD02 = weg08601Row.FIELD02.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD03 = weg08601Row.FIELD03.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD04 = weg08601Row.FIELD04.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD05 = weg08601Row.FIELD05.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD06 = weg08601Row.FIELD06.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD07 = weg08601Row.FIELD07.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD08 = weg08601Row.FIELD08.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD09 = weg08601Row.FIELD09.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD10 = weg08601Row.FIELD10.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD11 = weg08601Row.FIELD11.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD12 = weg08601Row.FIELD12.KeepOnlyAlphaNumericCharacters();
            weg08601Row.FIELD13 = weg08601Row.FIELD13.KeepOnlyAlphaNumericCharacters();
            //add more fields up to Field108 here...

            weg08601Row.FIELD01 = weg08601Row.FIELD01.SetToFixedWidth(2, FixedWidthJustification.Left, StringExtensions.BlankSpaceChar);
            weg08601Row.FIELD02 = weg08601Row.FIELD02.SetToFixedWidth(1, FixedWidthJustification.Left, StringExtensions.BlankSpaceChar);
            weg08601Row.FIELD03 = weg08601Row.FIELD03.SetToFixedWidth(8, FixedWidthJustification.Left, StringExtensions.BlankSpaceChar);
            weg08601Row.FIELD04 = weg08601Row.FIELD04.SetToFixedWidth(6, FixedWidthJustification.Left, StringExtensions.BlankSpaceChar);
            weg08601Row.FIELD05 = weg08601Row.FIELD05.SetToFixedWidth(15, FixedWidthJustification.Left, StringExtensions.BlankSpaceChar);
            weg08601Row.FIELD06 = weg08601Row.FIELD06.SetToFixedWidth(10, FixedWidthJustification.Left, StringExtensions.BlankSpaceChar);
            weg08601Row.FIELD07 = weg08601Row.FIELD07.SetToFixedWidth(6, FixedWidthJustification.Left, StringExtensions.BlankSpaceChar);
            weg08601Row.FIELD08 = weg08601Row.FIELD08.SetToFixedWidth(15, FixedWidthJustification.Left, StringExtensions.BlankSpaceChar);
            weg08601Row.FIELD09 = weg08601Row.FIELD09.SetToFixedWidth(8, FixedWidthJustification.Left, StringExtensions.BlankSpaceChar);
            weg08601Row.FIELD10 = weg08601Row.FIELD10.SetToFixedWidth(1, FixedWidthJustification.Right, StringExtensions.ZeroChar);
            weg08601Row.FIELD11 = weg08601Row.FIELD11.SetToFixedWidth(1, FixedWidthJustification.Right, StringExtensions.ZeroChar);
            weg08601Row.FIELD12 = weg08601Row.FIELD12.SetToFixedWidth(2, FixedWidthJustification.Right, StringExtensions.ZeroChar);
            weg08601Row.FIELD13 = weg08601Row.FIELD13.SetToFixedWidth(2, FixedWidthJustification.Right, StringExtensions.ZeroChar);
            //add more fields up to Field108 here...

            */
        }
    }
}
