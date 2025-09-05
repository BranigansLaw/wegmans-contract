using System;
namespace RX.PharmacyBusiness.ETL
{
    public static class ExtensionsHelper
    {
        /// <summary>
        /// Rounds to nearest minute.
        /// https://stackoverflow.com/questions/24006244/roundoff-timespan-to-15-min-interval
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minutes"></param>
        /// <returns>Interval clock time</returns>
        /// <example>
        /// var span1 = new TimeSpan(0, 10, 37, 00).RoundToNearestMinutes(15); //Returns 10:30
        /// var span2 = new TimeSpan(0, 10, 38, 00).RoundToNearestMinutes(15); //Returns 10:45
        /// </example>
        public static TimeSpan RoundToNearestMinutes(this TimeSpan input, int minutes)
        {
            var totalMinutes = (int)(input + new TimeSpan(0, minutes / 2, 0)).TotalMinutes;

            return new TimeSpan(0, totalMinutes - totalMinutes % minutes, 0);
        }
    }
}