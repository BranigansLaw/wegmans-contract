using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BPurchasesFromTCGFileHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<Adm340BPurchases> MapToTenTenAdm340BPurchases(IEnumerable<Adm340BPurchasesRow> adm340BPurchasesRows)
        {
            return adm340BPurchasesRows.Select(r => new Adm340BPurchases
            {
                ContractId = r.ContractId,
                ContractName = r.ContractName,
                Ncpdp = r.Ncpdp,
                DatePurchased = r.DatePurchased,
                InvNum = r.InvNum,
                NdcWo = r.NdcWo,
                DrugPackSize = r.DrugPackSize,
                DrugName = r.DrugName,
                QtyPurchased = r.QtyPurchased,
                CostPkg = r.CostPkg,
                ExtCost = r.ExtCost,
                DerivedStoreNum = r.DerivedStoreNum,
                DerivedDrugNdc = r.DerivedDrugNdc,
                DerivedRunDate = r.DerivedRunDate,
                DerivedProcessDate = r.DerivedProcessDate
            });
        }
    }
}
