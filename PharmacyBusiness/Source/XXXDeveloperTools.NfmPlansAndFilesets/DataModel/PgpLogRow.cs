namespace XXXDeveloperTools.NfmPlansAndFilesets.DataModel
{
    public class PgpLogRow
    {
        /// <summary>
        /// //If true is decryption.
        /// //If false is encryption.
        /// </summary>
        public required bool IsDecryption { get; set; }

        /// <summary>
        /// Name of file that was encrypted/decrypted.
        /// </summary>
        public required string PgpFileName { get; set; }

        /// <summary>
        /// Path of file that was encrypted/decrypted.
        /// </summary>
        public required string PgpFilePath { get; set; }

        /// <summary>
        /// Date the file was encrypted/decrypted.
        /// </summary>
        public required DateTime PgpDate { get; set; }
    }
}
