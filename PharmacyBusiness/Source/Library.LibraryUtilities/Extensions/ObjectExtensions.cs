namespace Library.LibraryUtilities.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Parse this <see cref="object"/> and return a null if null, or parse if not null or empty
        /// </summary>
        public static DateTime? ToDateTime(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string? s = obj.ToString();

            return s == null || string.IsNullOrEmpty(s) ? null : DateTime.Parse(s);
        }

        /// <summary>
        /// Parse this <see cref="int"/> and return a null if null, or parse if not null or empty
        /// </summary>
        public static int? ToInt(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string? s = obj.ToString();

            return s == null || string.IsNullOrEmpty(s) ? null : int.Parse(s);
        }

        /// <summary>
        /// Parse this <see cref="short"/> and return a null if null, or parse if not null or empty
        /// </summary>
        public static short? ToShort(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string? s = obj.ToString();

            return s == null || string.IsNullOrEmpty(s) ? null : short.Parse(s);
        }

        /// <summary>
        /// Parse this <see cref="short"/> and return a null if null, or parse if not null or empty
        /// </summary>
        public static decimal? ToDecimal(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string? s = obj.ToString();

            return s == null || string.IsNullOrEmpty(s) ? null : decimal.Parse(s);
        }

        /// <summary>
        /// Parse this <see cref="string"/> and return a null if null, or parse if not null or empty
        /// </summary>
        public static string? ToNullableString(this object obj)
        {    
            if (obj == null)
            {
                return null;
            }

            return obj.ToString();
        }
    }
}
