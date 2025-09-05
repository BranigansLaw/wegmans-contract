namespace Library.LibraryUtilities.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Returns this <see cref="DateTimeOffset"/> as a <see cref="DateOnly"/>
        /// </summary>
        public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset)
        {
            return new DateOnly(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day);
        }
    }
}
