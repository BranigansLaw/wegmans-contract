namespace RX.PharmacyBusiness.ETL.CRX582.Contracts
{
    using RX.PharmacyBusiness.ETL.CRX582.Core;

    public interface IInsurancePayerFactory
    {
        InsurancePayer Create(FillFact fact);
    }
}
