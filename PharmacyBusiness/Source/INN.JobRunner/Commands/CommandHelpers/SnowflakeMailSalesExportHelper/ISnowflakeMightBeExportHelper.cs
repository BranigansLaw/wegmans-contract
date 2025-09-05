namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeMailSalesExportHelper
{
    public interface ISnowflakeMailSalesExportHelper
    {
        /// <summary>
        /// Runs the snowflake query MightBeRefundTransactions.sql and exports the results to a file
        /// </summary>
        Task ExportMightBeRefundTransactionsAsync(DateOnly runDate, CancellationToken cancellationToken);

        /// <summary>
        /// Runs the snowflake query MightBeSoldTransactions.sql and exports the results to a file
        /// </summary>
        Task ExportMightBeSoldTransactionsAsync(DateOnly runDate, CancellationToken cancellationToken);
    }
}
