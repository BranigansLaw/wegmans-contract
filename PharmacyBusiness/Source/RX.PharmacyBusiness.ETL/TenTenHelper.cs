namespace RX.PharmacyBusiness.ETL
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Handles data transformations unique to 1010data.
    /// </summary>
    public static class TenTenHelper
    {
        /// <summary>
        /// When data within text files is TenUp'd, all date with time values need to be converted to decimal number of days from year 2035.
        /// </summary>
        /// <param name="dt">DateTime value to be converted.</param>
        /// <returns>Returns string intended for output to a text file that will get TenUp'd.</returns>
        public static string FormatDateWithTimeForTenUp(DateTime dt)
        {
            return Math.Round((dt - DateTime.Parse("01/01/2035")).TotalDays, 6).ToString("0.000000");
        }

        /// <summary>
        /// When data within text files is TenUp'd, all date without time values need to be converted to integers in YYYYMMDD format.
        /// </summary>
        /// <param name="dt">DateTime value to be converted.</param>
        /// <returns>Returns string intended for output to a text file that will get TenUp'd.</returns>
        public static string FormatDateWithoutTimeForTenUp(DateTime dt)
        {
            return dt.ToString("yyyyMMdd");
        }

        /// <summary>
        /// TenUp of files has character limitations, so this methods removes offending characters.
        /// </summary>
        /// <param name="originalString"></param>
        /// <returns></returns>
        public static string CleanStringForTenUpOld(string originalString)
        {
            byte[] asciiBytesDirty = Encoding.ASCII.GetBytes(originalString.Trim());
            byte[] asciiBytesCleaned = new byte[] { };
            List<byte> bytesCleanedList = new List<byte>();

            foreach (var asciiByte in asciiBytesDirty)
            {
                //Include alphanumeric characters(32 through 126)
                //Exclude Double Quote(34) and Vertical Bar(124) that would break executing TenUp from a file.
                if (asciiByte >= 32 && 
                    asciiByte <= 126 &&
                    asciiByte != 34 &&
                    asciiByte != 124)
                {
                    //asciiBytesCleaned[asciiBytesCleaned.Length] = asciiByte;
                    bytesCleanedList.Add(asciiByte);
                }
            }

            asciiBytesCleaned = bytesCleanedList.ToArray();
            return Encoding.UTF8.GetString(asciiBytesCleaned);
        }

        /// <summary>
        /// TenUp of files has character limitations, so this method removes offending characters.
        /// </summary>
        /// <param name="originalString"></param>
        /// <returns>String clean enough even for 1010.</returns>
        public static string CleanStringForTenUp(string originalString)
        {
            if (string.IsNullOrEmpty(originalString?.Trim()))
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            //Include alphanumeric characters(32 through 126)
            //Exclude Double Quote(34) and Vertical Bar(124) that would break executing TenUp from a file.
            foreach (var c in originalString?.Trim())
            {
                if ((c >= ' ' && c <= '~') && c != '"' && c != '|')
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
