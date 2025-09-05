namespace Library.LibraryUtilities.Extensions
{
    public static class SqlLiterals
    {
        /// <summary>
        /// Converts a date type bind variable in SQL to a date literal.
        /// </summary>
        public static string ToSqlDateLiteral(this DateOnly dateParameter)
        {
            return $"To_Date('{dateParameter.ToDateTime(new TimeOnly(0)).ToString("yyyyMMdd")}','YYYYMMDD')";
        }

        /// <summary>
        /// Converts a string type bind variable in SQL to a string literal.
        /// </summary>
        public static string ToSqlStringLiteral(this string stringParameter)
        {
            return $"'{stringParameter}'";
        }
    }
}
