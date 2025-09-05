using Library.TenTenInterface.Helper;
using System.Text;
using System.Web;

namespace Library.TenTenInterface.Extensions
{
    public static class TenTenDataConversionExtensions
    {
        public static T? ConvertToTenTenDataType<T>(this DateTime? sourceDate) where T : struct
        {
            if (sourceDate == null)
                return null;
            if (typeof(T) == typeof(int))
                return (T)(object)DataConversionsForUploadsToTenTen.FormatDateWithoutTime((DateTime)sourceDate);
            if (typeof(T) == typeof(decimal))
                return (T)(object)DataConversionsForUploadsToTenTen.FormatDateWithTime((DateTime)sourceDate);

            throw new InvalidCastException("Invalid data type conversion");
        }
        public static string CleanStringForTenTenDataUpload(this string? sourceString)
        {
            if (sourceString == null || string.IsNullOrWhiteSpace(sourceString))
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            //Include alphanumeric characters(32 through 126)
            foreach (var c in sourceString.Trim())
            {
                if (c >= ' ' && c <= '~')
                    sb.Append(c);
            }
            return HttpUtility.HtmlEncode(sb.ToString());
        }
    }
}