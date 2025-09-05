using System;

namespace RX.PharmacyBusiness.ETL.RXS710
{
    public static class ExtensionMethods
    {
        public static string TruncateValue(this string value, int start, int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            else
            {
                return value.PadRight(start + length).Substring(start, length).Trim();
            }
        }

        public static string PadLeftAndValidate(this string value, int length, char paddingChar)
        {
            return value.ValidateLength(length).PadLeft(length, paddingChar);
        }

        public static string PadRightAndValidate(this string value, int length, char paddingChar)
        {
            return value.ValidateLength(length).PadRight(length, paddingChar);
        }

        public static string ValidateLength(this string value, int length)
        {
            if (value.Length > length)
            {
                throw new Exception($"Field length is [{value.Length}], but max length is [{length}], value [{value}]");
            }

            return value;
        }

        public static DateTime StartOfWeek(this DateTime value, DayOfWeek startOfWeek)
        {
            int diff = (7 + (value.DayOfWeek - startOfWeek)) % 7;
            return value.AddDays(-1 * diff).Date;
        }
    }
}