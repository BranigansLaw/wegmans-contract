using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.DataModel.UploadRow.Implementation;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BOpportunityFromTCGFileHelper
{
    public class MapperImp : IMapper
    {
        public IEnumerable<Adm340BOpportunityTenTenRow> MapAdm340BOpportunityRowToTenTenAzureRow(IEnumerable<Adm340BOpportunityRow> adm340BOpportunityRows)
        {
            return adm340BOpportunityRows.Select(r => new Adm340BOpportunityTenTenRow
            {
                ContractId = r.ContractId,
                AccountId = r.AccountId,
                ContractName = r.ContractName,
                IsPool = r.IsPool,
                WholesalerNum = r.WholesalerNum,
                WholesalerName = r.WholesalerName,
                NdcWo = r.NdcWo,
                DrugName = r.DrugName,
                DrugStrength = r.DrugStrength,
                DrugPackSize = r.DrugPackSize,
                OrdPkg = r.OrdPkg,
                ApprPkg = r.ApprPkg,
                OrderDate = r.OrderDate,
                DerivedStoreNum = r.DerivedStoreNum,
                DerivedDrugNdc = r.DerivedDrugNdc,
                DerivedRunDate = r.DerivedRunDate,
                DerivedProcessDate = r.DerivedProcessDate
            });
        }

        /// <inheritdoc />
        public IOrderedEnumerable<Adm340BOpportunity> MapAndOrderToTenTenAdm340BOpportunity(IEnumerable<Adm340BOpportunityRow> adm340BOpportunityRows)
        {
            return adm340BOpportunityRows.Select(r => new Adm340BOpportunity
            {
                ContractId = r.ContractId,
                AccountId = r.AccountId,
                ContractName = r.ContractName,
                IsPool = r.IsPool,
                WholesalerNum = r.WholesalerNum,
                WholesalerName = r.WholesalerName,
                NdcWo = r.NdcWo,
                DrugName = r.DrugName,
                DrugStrength = r.DrugStrength,
                DrugPackSize = r.DrugPackSize,
                OrdPkg = r.OrdPkg,
                ApprPkg = r.ApprPkg,
                OrderDate = r.OrderDate,
                DerivedStoreNum = r.DerivedStoreNum,
                DerivedDrugNdc = r.DerivedDrugNdc,
                DerivedRunDate = r.DerivedRunDate,
                DerivedProcessDate = r.DerivedProcessDate
            }).OrderBy(x => x.DerivedStoreNum);
        }
    }
}
