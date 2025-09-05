using System;

namespace Wegmans.POS.DataHub.ReprocessTransactionsNew
{
    public class ReprocessingCursor
    {
        /// <summary>
        /// The date currently being reprocessed
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// The hour currently being reprocessed
        /// </summary>
        public int Hour { get; set; }

        public ReprocessingCursor(DateOnly date)
        {
            Date = date;
            Hour = 23;
        }

        /// <summary>
        /// Increment the cursor to the next sequential folder
        /// </summary>
        public void Decrement()
        {
            if (Hour == 0)
            {
                Date = Date.AddDays(-1);
                Hour = 23;
            }
            else
            {
                Hour--;
            }
        }

        /// <summary>
        /// The the Azure blob path for this cursor
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            return $"{Date:yyyy'/'MM'/'dd}/{Hour.ToString().PadLeft(2, '0')}/";
        }
    }
}
