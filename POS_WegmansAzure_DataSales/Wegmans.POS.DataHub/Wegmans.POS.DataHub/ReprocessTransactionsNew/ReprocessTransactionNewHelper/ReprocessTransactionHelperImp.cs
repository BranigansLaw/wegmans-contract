using System;
using Wegmans.POS.DataHub.Util.EasternStandardTimeGenerator;

namespace Wegmans.POS.DataHub.ReprocessTransactionsNew.ReprocessTransactionNewHelper
{
    public class ReprocessTransactionHelperImp : IReprocessingTransactionHelper
    {
        private readonly IEasternStandardTimeGenerator _timeGenerator;

        public ReprocessTransactionHelperImp(IEasternStandardTimeGenerator timeGenerator)
        {
            _timeGenerator = timeGenerator ?? throw new ArgumentNullException(nameof(timeGenerator));
        }

        /// <inheritdoc />
        public int GetMaxTransactionsToQueue(ReprocessingSchedule schedule)
        {
            DateTimeOffset currentTime = _timeGenerator.GetCurrentEasternStandardTime();

            foreach (Schedule s in schedule.Schedules)
            {
                if (s.StartTime.ToTimeSpan() <= currentTime.TimeOfDay && 
                    (s.EndTime == TimeOnly.MaxValue || s.EndTime.ToTimeSpan() > currentTime.TimeOfDay))
                {
                    return s.NumberToQueuePerInterval;
                }
            }

            return 0;
        }
    }
}
