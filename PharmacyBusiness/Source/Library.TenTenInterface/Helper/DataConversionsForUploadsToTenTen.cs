namespace Library.TenTenInterface.Helper
{
    /// <summary>
    /// Handles data transformations unique to 1010data.
    /// </summary>
    public static class DataConversionsForUploadsToTenTen
    {
        /// <summary>
        /// Source DateTime values need to be converted to decimal values representing number of days from date 1/1/2035.
        /// </summary>
        /// <param name="dt">DateTime value to be converted.</param>
        /// <returns>Returns decimal value representing a date and a time of day.</returns>
        public static decimal FormatDateWithTime(DateTime dt)
        {
            return Convert.ToDecimal(Math.Round((dt - DateTime.Parse("01/01/2035")).TotalDays, 6).ToString("0.000000"));
        }

        /// <summary>
        /// Source DateTime values need to be converted to integer values of the date in YYYYMMDD format and time of day is omitted.
        /// </summary>
        /// <param name="dt">DateTime value to be converted.</param>
        /// <returns>Returns integer value representing just the date (not the time).</returns>
        public static int FormatDateWithoutTime(DateTime dt)
        {
            return Convert.ToInt32(dt.ToString("yyyyMMdd"));
        }
    }
}
