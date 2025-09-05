using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.DataModel.UploadRow.Implementation;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BOpportunityFromTCGFileHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="Adm340BOpportunityRow"/> to a collection of <see cref="Adm340BOpportunity"/>
        /// </summary>
        IOrderedEnumerable<Adm340BOpportunity> MapAndOrderToTenTenAdm340BOpportunity(IEnumerable<Adm340BOpportunityRow> adm340BOpportunityRows);

        /// <summary>
        /// Maps the collection of <see cref="Adm340BOpportunityRow"/> to a collection of <see cref="Adm340BOpportunityTenTenRow"/>
        /// </summary>
        IEnumerable<Adm340BOpportunityTenTenRow> MapAdm340BOpportunityRowToTenTenAzureRow(IEnumerable<Adm340BOpportunityRow> adm340BOpportunityRows);
    }
}
