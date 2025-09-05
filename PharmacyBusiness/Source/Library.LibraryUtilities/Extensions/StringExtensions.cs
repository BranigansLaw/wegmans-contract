using System.Text;

namespace Library.LibraryUtilities.Extensions
{
    public static class StringExtensions
    {
        public const char BlankSpaceChar = ' ';
        public const char ZeroChar = '0';
        public enum FixedWidthJustification
        {
            Left,
            Right
        }

        /// <summary>
        /// Removing text qualifier with Trim(textQualifier) removes more than just the first and last characters if that character
        /// repeats consecutively.
        /// This method only removes first and last positions.
        /// </summary>
        public static string RemoveFirstAndLastCharacters(this string inputString, int count = 1)
        {
            return RemoveLastCharacters(RemoveFirstCharacters(inputString, count), count);
        }

        public static string RemoveFirstCharacters(this string inputString, int count = 1)
        {
            if (inputString.Length >= count)
            {
                return inputString.Remove(0, count);
            }

            return inputString;
        }

        public static string RemoveLastCharacters(this string inputString, int count = 1)
        {
            if (inputString.Length >= count)
            {
                return inputString.Remove(inputString.Length - count, count);
            }

            return inputString;
        }

        public static string SurroundWith(this string inputString, string surround)
        {
            return surround + inputString + surround;
        }

        public static string First(this string inputString, int count)
        {
            return inputString.Substring(0, count);
        }

        public static string Last(this string inputString, int count)
        {
            return inputString.Substring(inputString.Length - count, count);
        }

        public static string Last2(this string inputString, int count)
        {
            return inputString.Substring(inputString.Length - count, count);
        }

        public static string KeepOnlyAlphaNumericCharacters(this string? sourceString)
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
            return sb.ToString();
        }

        public static string TrimTrailingZeroes(this string? inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
                return inputValue ?? string.Empty;

            string numberAsString = inputValue.Trim();
            int indexOfDot = numberAsString.IndexOf('.');

            while (numberAsString.EndsWith("0") && //Check if there are more zeroes that can be removed.
                    numberAsString != "0") //Do not remove the last zero if the number is zero
            {
                //Do not remove the last digit adjacent to the decimal point if there is a decimal point
                if (inputValue.Contains('.') &&
                    (indexOfDot + 2) == numberAsString.Length)
                    break;

                numberAsString = numberAsString.Substring(0, numberAsString.Length - 1);
            }

            return numberAsString;
        }

        public static string SetToFixedWidth(this string? sourceString, int fixedStringLength, FixedWidthJustification leftOrRightJustification, char paddingCharacter, bool shouldThrowExceptionWhenTruncationOccurs = false)
        {
            string result = sourceString?.Trim() ?? string.Empty;

            if (result.Length > fixedStringLength)
            {
                if (shouldThrowExceptionWhenTruncationOccurs)
                    throw new System.Exception($"The string [{result}] is too long to fit in a fixed-width string of length [{fixedStringLength}].");

                result = result.Substring(0, fixedStringLength);
            }

            if (leftOrRightJustification == FixedWidthJustification.Right)
                result = result.PadLeft(fixedStringLength, paddingCharacter);
            else
                result = result.PadRight(fixedStringLength, paddingCharacter);

            return result;
        }
    }
}
