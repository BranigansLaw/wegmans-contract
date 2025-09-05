using Library.SnowflakeInterface.Data;

namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeIqviaAndImiExportHelper
{
    public interface ISnowflakeIqviaAndImiExportHelper
    {
        /// <summary>
        /// Runs the snowflake query WEG_086_YYYYMMDD_01_For198.sql and exports the results to a file
        /// </summary>
        Task ExportInqviaAsync(DateOnly runDate, CancellationToken cancellationToken);

        /// <summary>
        /// Runs the snowflake query WEG_086_YYYYMMDD_01.sql and exports the results to a file
        /// </summary>
        Task ExportImiAsync(DateOnly runDate, CancellationToken cancellationToken);

        /// <summary>
        /// Handles fixed width for the data export.
        /// </summary>
        /// <param name="weg08601Row"></param>
        void HandleSetToFixedWidth(WEG08601Row weg08601Row);
    }
}
