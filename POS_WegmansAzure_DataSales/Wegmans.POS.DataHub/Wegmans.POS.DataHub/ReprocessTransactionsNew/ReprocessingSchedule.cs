using System;
using System.Collections.Generic;

namespace Wegmans.POS.DataHub.ReprocessTransactionsNew
{
    public class ReprocessingSchedule
    {
        public IEnumerable<Schedule> Schedules { get; set; }

        public static ReprocessingSchedule Default = new ReprocessingSchedule
        {
            Schedules = new List<Schedule> {
                new Schedule {
                    StartTime = new TimeOnly(0, 0),
                    EndTime = new TimeOnly(6, 0),
                    NumberToQueuePerInterval = 100000
                },
                new Schedule {
                    StartTime = new TimeOnly(6, 0),
                    EndTime = new TimeOnly(21, 0),
                    NumberToQueuePerInterval = 50000
                },
                new Schedule {
                    StartTime = new TimeOnly(21, 0),
                    EndTime = TimeOnly.MaxValue,
                    NumberToQueuePerInterval = 100000
                },
            }
        };
    }

    /// <summary>
    /// Schedule to run different NumberToQueuePerInterval
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// The starting time of this interval, inclusive, in Rochester time
        /// </summary>
        public TimeOnly StartTime { get; set; }

        /// <summary>
        /// The ending time of this interval, exclusive, in Rochester time
        /// </summary>
        public TimeOnly EndTime { get; set; }

        /// <summary>
        /// The configured maximum number of transactions to queue during this interval
        /// </summary>
        public int NumberToQueuePerInterval { get; set; }
    }
}