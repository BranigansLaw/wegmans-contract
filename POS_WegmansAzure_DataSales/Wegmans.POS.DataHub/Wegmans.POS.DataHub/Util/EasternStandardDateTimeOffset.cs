using AutoMapper;
using System;

namespace Wegmans.POS.DataHub.Util
{
    public static class EasternStandardDateTimeOffset
    {
        public static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        /// <summary>
        ///     Converts a DateTime string, e.g. "2021-07-28 13:45:00", to a DateTimeOffset in the EST/EDT time zone.
        /// </summary>
        public static DateTimeOffset Parse(string value)
            => DateTime.SpecifyKind(DateTime.Parse(value), DateTimeKind.Unspecified).ToEasternStandardTime();
        public static DateTimeOffset ToEasternStandardTime(this DateTime value)
            => value.Kind == DateTimeKind.Unspecified
                ? ((DateTimeOffset)TimeZoneInfo.ConvertTimeToUtc(value, TimeZone)).ToEasternStandardTime()
                : TimeZoneInfo.ConvertTime(value, TimeZone);
        public static DateTimeOffset ToEasternStandardTime(this DateTimeOffset value)
            => TimeZoneInfo.ConvertTime(value, TimeZone);
    }
}