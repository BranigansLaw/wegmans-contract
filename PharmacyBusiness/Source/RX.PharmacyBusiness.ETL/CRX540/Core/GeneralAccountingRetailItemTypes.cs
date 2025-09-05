namespace RX.PharmacyBusiness.ETL.CRX540.Core
{
    public enum GeneralAccountingRetailTypes
    {
        Undefined = 0,
        PrescriptionSale = 1,
        ShippingCharge = 2,

        /// <summary>
        /// The difference of full retail price minus patient copay.
        /// </summary>
        ThirdPartyPayment = 3
    }
}

