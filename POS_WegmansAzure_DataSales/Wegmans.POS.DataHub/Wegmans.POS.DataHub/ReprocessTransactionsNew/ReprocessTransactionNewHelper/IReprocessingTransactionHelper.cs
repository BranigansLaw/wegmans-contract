namespace Wegmans.POS.DataHub.ReprocessTransactionsNew.ReprocessTransactionNewHelper
{
    public interface IReprocessingTransactionHelper
    {
        /// <summary>
        /// Checks the given <paramref name="schedule"/> and returns the <see cref="Schedule.NumberToQueuePerInterval"/> based on the current time in Rochester. If no matching time is found, 0 is returned.
        /// </summary>
        int GetMaxTransactionsToQueue(ReprocessingSchedule schedule);
    }
}
