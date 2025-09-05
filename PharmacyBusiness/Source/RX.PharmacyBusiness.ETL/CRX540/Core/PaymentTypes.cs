namespace RX.PharmacyBusiness.ETL.CRX540.Core
{
    /// <summary>
    /// The possible values for Payment Type Name.
    /// </summary>
    public static class PaymentTypes
    {
        public const string Cash = "CASH";
        public const string Check = "CHECK";
        public const string CreditCard = "CREDIT CARD";
        public const string CreditCardReversal = "CREDIT CARD REVERSAL";
        public const string CreditCardPartialReversal = "CC PARTIAL REVERSAL";
        public const string CustomerService = "CUSTOMER SERVICE";
        public const string ElectronicCheck = "E-CHECK";
        public const string Invoice = "INVOICE";
        public const string GetCreditCard = "GET CREDIT CARD";
        public const string Other = "";
    }
}
