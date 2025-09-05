namespace RX.PharmacyBusiness.ETL.CRX540.Core
{
    /// <summary>
    /// The possible values returned by the ERx TREXONE_DW_DATA.FILL_STATUS_TYPE table.
    /// </summary>
    public static class FillStatusTypes
    {
        public static string Cancelled = "Cancelled";
        public static string Deleted = "Deleted";
        public static string Denied = "Denied";
        public static string InProcess = "In-Process";
        public static string NA = "NA";
        public static string Returned = "Returned";
        public static string ReturnedPartial = "Returned(Partial)";
        public static string SameDayReturn = "Same Day Return";
        public static string Sold = "Sold";
        public static string Suspended = "Suspended";
        public static string Transferred = "Transferred";
    }
}
