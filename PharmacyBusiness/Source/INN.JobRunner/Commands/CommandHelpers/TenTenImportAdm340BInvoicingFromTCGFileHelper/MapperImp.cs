using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BInvoicingFromTCGFileHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<Adm340BInvoicing> MapToTenTenAdm340BInvoicing(IEnumerable<Adm340BInvoicingRow> adm340BInvoicingRows)
        {
            return adm340BInvoicingRows.Select(r => new Adm340BInvoicing
            {
                ClaimId = r.ClaimId,
                ClaimDate = r.ClaimDate,
                DateOfService = r.DateOfService,
                RefillNum = r.RefillNum,
                RxNum = r.RxNum,
                DrugName = r.DrugName,
                DrugPackSize = r.DrugPackSize,
                DrugNdc = r.DrugNdc,
                QtyDispensed = r.QtyDispensed,
                BrandGeneric = r.BrandGeneric,
                DrugCost = r.DrugCost,
                CoPay = r.CoPay,
                TpPay = r.TpPay,
                HcFacilityFee = r.HcFacilityFee,
                PercentReplenished = r.PercentReplenished,
                AmtDueHcFacility = r.AmtDueHcFacility,
                Ncpdp = r.Ncpdp,
                PharmName = r.PharmName,
                HcFacility = r.HcFacility,
                ContractId = r.ContractId,
                DerivedStoreNum = r.DerivedStoreNum,
                DerivedDrugNdcWo = r.DerivedDrugNdcWo,
                DerivedRunDate = r.DerivedRunDate,
                DerivedProcessDate = r.DerivedProcessDate
            });
        }
    }
}
