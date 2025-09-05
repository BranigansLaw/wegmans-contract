namespace Library.LibraryUtilities.GetNow
{
    public class GetNowImp : IGetNow
    {
        private static readonly TimeZoneInfo EasternStandardTimezone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        /// <inheritdoc />
        public DateTimeOffset GetNow() => DateTimeOffset.Now;

        /// <inheritdoc />
        public DateTimeOffset GetNowEasternStandardTime()
        {
            return TimeZoneInfo.ConvertTime(GetNow(), EasternStandardTimezone);
        }
    }
}
