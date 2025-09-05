using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BEligibleClaimsFromTCGFileHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="Adm340BEligibleClaimsRow"/> to a collection of <see cref="Adm340BEligibleClaims"/>
        /// </summary>
        IEnumerable<Adm340BEligibleClaims> MapToTenTenAdm340BEligibleClaims(IEnumerable<Adm340BEligibleClaimsRow> adm340BEligibleClaimsRows);
    }
}
