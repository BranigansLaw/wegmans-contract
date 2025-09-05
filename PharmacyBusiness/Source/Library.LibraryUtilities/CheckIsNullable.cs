namespace Library.LibraryUtilities
{
    public static class CheckIsNullable
    {
        public static bool IsNullable<T>()
        {
            Type type = typeof(T);
            if (!type.IsValueType)
            {
                return true; // ref-type
            }

            if (Nullable.GetUnderlyingType(type) != null)
            {
                return true; // Nullable<T>
            }

            return false; // value-type
        }
    }
}
