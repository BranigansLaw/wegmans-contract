namespace Library.InformixInterface.Extensions
{
    public static class DateOnlyExtensions
    {
        /// <summary>
        /// Formats this <see cref="DateOnly"/> as an Informix SQL date
        /// </summary>
        public static string ToInformixDateFormat(this DateOnly dateOnly) => $"DATE('{dateOnly:MM-dd-yyyy}')";
    }
}
