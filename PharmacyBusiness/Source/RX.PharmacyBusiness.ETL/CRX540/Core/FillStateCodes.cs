namespace RX.PharmacyBusiness.ETL.CRX540.Core
{
    public static class FillStateCodes
    {
        public const string BankingIdentificationNumberEntry = "7";
        public const string BankingIdentificationNumberExit = "8";
        public const string Cancelled = "10";
        public const string CustomerReturned = "13";
        public const string DeclineRTP = "15";
        public const string DeclineSold = "16";
        public const string Dispensed = "1";
        public const string Edit = "11";
        public const string FinancialFeedReleased = "19";
        public const string FinancialFeedSold = "17";
        public const string FinancialFeedUnreleased = "20";
        public const string FinancialFeedUnsold = "18";
        public const string ItemDeclined = "9";

        /// <summary>
        /// Identifies a reversal type claim - think of it as a Mulligan because previous claim data entry was wrong in some way (pricing or address, etc).
        /// </summary>
        public const string NegativeAdjudication = "4";
        public const string PointOfSaleComlete = "5";

        /// <summary>
        /// Identifies a billing type claim - this is when an insurance company owes money to Wegmans and that relationship is managed by Inmar via the SmartRx data feed (aka, IDCF).
        /// </summary>
        public const string PositiveAdjudication = "3";
        public const string Sold = "14";
        public const string Transferred = "6";
        public const string VerficiationDenied = "12";
        public const string Verified = "2";
    }
}
