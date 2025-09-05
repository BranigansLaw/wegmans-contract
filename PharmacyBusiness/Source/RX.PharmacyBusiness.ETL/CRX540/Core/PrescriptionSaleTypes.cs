namespace RX.PharmacyBusiness.ETL.CRX540.Core
{
    public enum PrescriptionSaleTypes
    {
        Undefined = 0,
        Sold = 1,
        CreditCardFullRefund = 2,
        CreditCardPartialRefund = 3,
        CreditCardAnyRefund = 4,
        AlternativePaymentRefund = 5
    }
}
