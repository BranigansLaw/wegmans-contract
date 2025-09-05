namespace Library.EmplifiInterface.Extensions
{
    public static class IBMDataConversionExtensions
    {
        /// <summary>
        /// Vendor IBM cannot process files containing any double quotes anywhere in the output file no matter how they are escaped or not - their mere existence anywhere in the file will crash IBM's file processing.
        /// Coincidentally, vendor 1010data also cannot process files via TenUp.exe containing any double quotes anywhere in the output file no matter how they are escaped or not - their mere existence anywhere in the file will crash TenUp.exe.
        /// If there are more special characters to be removed for IBM, see Library.TenTenInterface.Extensions.TenTenDataConversionExtensions.CleanStringForTenTenDataUpload for more coding ideas.
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string? CleanNullableStringForVendorFileLimitations(this string? sourceString)
        {
            if (sourceString == null)
                return null;
            
            if (string.IsNullOrWhiteSpace(sourceString))
                return string.Empty;

            return sourceString.Replace("\"", "");
        }

        /// <summary>
        /// Vendor IBM cannot process files containing any double quotes anywhere in the output file no matter how they are escaped or not - their mere existence anywhere in the file will crash IBM's file processing.
        /// Coincidentally, vendor 1010data also cannot process files via TenUp.exe containing any double quotes anywhere in the output file no matter how they are escaped or not - their mere existence anywhere in the file will crash TenUp.exe.
        /// If there are more special characters to be removed for IBM, see Library.TenTenInterface.Extensions.TenTenDataConversionExtensions.CleanStringForTenTenDataUpload for more coding ideas.
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string CleanNonNullableStringForVendorFileLimitations(this string sourceString)
        {
            if (string.IsNullOrWhiteSpace(sourceString))
                return string.Empty;

            return sourceString.Replace("\"", "");
        }
    }
}
