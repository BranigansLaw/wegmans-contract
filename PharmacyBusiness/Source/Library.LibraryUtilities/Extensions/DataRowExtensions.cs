using System.Data;
using System.Globalization;

namespace Library.LibraryUtilities.Extensions
{
    public static class DataRowExtensions
    {
        /// <summary>
        /// Returns the value of field <paramref name="index"/> or throws <see cref="ArgumentNullException"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">If the value of field <paramref name="index"/> is null</exception>
        public static T NonNullField<T>(this DataRow row, int index, int rowNumber)
        {
            try
            {
                return row.Field<T>(index) ?? throw new ArgumentNullException($"Value for index {index} is null for row {rowNumber}");
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException($"Invalid cast for index {index} on row {rowNumber}", e);
            }
        }

        /// <summary>
        /// Returns the value of field <paramref name="index"/> or returns the default value of <typeparamref name="T"/> if the value is null
        /// </summary>
        public static string? NullableString(this DataRow row, int index, int rowNumber)
        {
            if (string.IsNullOrWhiteSpace(row.ItemArray[index]?.ToString()))
                return null;
            else
                return row.ItemArray[index]?.ToString();
        }

        /// <summary>
        /// Returns the value of field <paramref name="index"/> or returns the default value of <typeparamref name="T"/> if the value is null
        /// </summary>
        public static T? NullableField<T>(this DataRow row, int index, int rowNumber) where T : struct
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row.ItemArray[index]?.ToString()))
                    return null;
                else
                {
                    if (typeof(T) == typeof(DateTime) && DateTime.TryParseExact(row.ItemArray[index]?.ToString(), "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                        return (T)(object)dt;
                    else if (typeof(T) == typeof(decimal) && decimal.TryParse(row.ItemArray[index]?.ToString(), out decimal dec))
                        return (T)(object)dec;
                    else if (typeof(T) == typeof(double) && double.TryParse(row.ItemArray[index]?.ToString(), out double dbl))
                        return (T)(object)dbl;
                    else if (typeof(T) == typeof(int) && int.TryParse(row.ItemArray[index]?.ToString(), out int i))
                        return (T)(object)i;
                    else
                        return (T?)row.ItemArray[index];
                }
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException($"Invalid cast for index {index} on row {rowNumber}, which has column name {row.Table.Columns[index].ColumnName}", e);
            }
        }
    }
}
