using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BEligibleClaimsFromTCGFileHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<Adm340BEligibleClaims> MapToTenTenAdm340BEligibleClaims(IEnumerable<Adm340BEligibleClaimsRow> adm340BEligibleClaimsRows)
        {
            return adm340BEligibleClaimsRows.Select(r => new Adm340BEligibleClaims
            {
                ClaimId = r.ClaimId,
                Type = r.Type,
                ClaimDate = r.ClaimDate,
                DateOfService = r.DateOfService,
                RefillNum = r.RefillNum,
                RxNum = r.RxNum,
                DrugName = r.DrugName,
                DrugPackSize = r.DrugPackSize,
                PackageQty = r.PackageQty,
                QtyPerUoi = r.QtyPerUoi,
                DrugNdcWo = r.DrugNdcWo,
                QtyDispensed = r.QtyDispensed,
                DaysSupply = r.DaysSupply,
                BrandGeneric = r.BrandGeneric,
                CashThirdParty = r.CashThirdParty,
                ClaimType = r.ClaimType,
                SalesTax = r.SalesTax,
                CoPay = r.CoPay,
                TpPay = r.TpPay,
                HcFacilityFee = r.HcFacilityFee,
                AmtDueHcFacility = r.AmtDueHcFacility,
                PharmName = r.PharmName,
                HcFacility = r.HcFacility,
                UniqueClaimId = r.UniqueClaimId,
                ContractId = r.ContractId,
                DerivedStoreNum = r.DerivedStoreNum,
                DerivedDrugNdc = r.DerivedDrugNdc,
                DerivedRunDate = r.DerivedRunDate,
                DerivedProcessDate = r.DerivedProcessDate
            });
        }
    }
}
