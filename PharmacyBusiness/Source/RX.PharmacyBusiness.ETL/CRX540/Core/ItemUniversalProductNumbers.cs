namespace RX.PharmacyBusiness.ETL.CRX540.Core
{
    /// <summary>
    /// The possible values for Payment Type Name.
    /// </summary>
    public static class ItemUniversalProductNumbers
    {
        public const long UndefinedItem = 0;
        public const long ShippingChargeItem = 7789025991;
        public const long PrescriptionItem = 7789025014;
        public const long ThirdPartyRefundItem = 7789025027;
        public const long ThirdPartyPaymentItem = 7789025030;

        public static readonly long[] ValidItems = {
            PrescriptionItem,
            ThirdPartyPaymentItem,
            ThirdPartyRefundItem,
            ShippingChargeItem
        };
    }
}
