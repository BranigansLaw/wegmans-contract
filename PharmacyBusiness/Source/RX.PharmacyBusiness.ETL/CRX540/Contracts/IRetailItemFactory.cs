namespace RX.PharmacyBusiness.ETL.CRX540.Contracts
{
    using System;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    public interface IRetailItemFactory
    {
        GeneralAccountingRetailItem[] Create(PrescriptionSale sale, DateTime date);
    }
}
