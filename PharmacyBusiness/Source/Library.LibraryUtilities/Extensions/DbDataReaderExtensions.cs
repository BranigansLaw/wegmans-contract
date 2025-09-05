using System.Data.Common;

namespace Library.LibraryUtilities.Extensions
{
    public static class DbDataReaderExtensions
    {
        /// <summary>
        /// Attemptes to read a value of type <typeparamref name="T"/> at <paramref name="index"/> from <paramref name="reader"/>
        /// </summary>
        /// <returns>The value of type <typeparamref name="T"/> if correctly parsed and matched in type</returns>
        /// <exception cref="InvalidMappingException">If any type of mapping error occurs</exception>
        public static T GetValueByIndex<T>(this DbDataReader reader, int index) where T : struct
        {
            try
            {
                return reader.GetFieldValue<T>(index);
            }
            catch (Exception ex)
            {
                throw new InvalidMappingException(index, reader.GetName(index), reader.GetString(index), ex);
            }
        }

        /// <summary>
        /// Attemptes to read a string value at <paramref name="index"/> from <paramref name="reader"/>
        /// </summary>
        /// <returns>The string value of column index <paramref name="index"/></returns>
        /// <exception cref="InvalidMappingException">If any type of mapping error occurs</exception>
        public static string GetStringByIndex(this DbDataReader reader, int index)
        {
            try
            {
                return reader.GetString(index);
            }
            catch (Exception ex)
            {
                throw new InvalidMappingException(index, reader.GetName(index), reader.GetString(index), ex);
            }
        }

        /// <summary>
        /// Gets the nullable value in the <paramref name="reader"/> at <paramref name="index"/>
        /// </summary>
        /// <typeparam name="T">The type to parse</typeparam>
        /// <returns>The nullable value at <paramref name="index"/> in <paramref name="reader"/></returns>
        /// <exception cref="InvalidMappingException">If any type of mapping error occurs</exception>
        public static T? GetNullableValueByIndex<T>(this DbDataReader reader, int index) where T : struct
        {
            try
            {
                if (reader.IsDBNull(index))
                {
                    return null;
                }

                return (T) reader.GetValue(index);
            }
            catch (Exception ex)
            {
                throw new InvalidMappingException(index, reader.GetName(index), reader.GetString(index), ex);
            }
        }
    }
}
