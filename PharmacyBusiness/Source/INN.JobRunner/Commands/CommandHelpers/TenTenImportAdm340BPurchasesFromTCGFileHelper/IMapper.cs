using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BPurchasesFromTCGFileHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="Adm340BPurchasesRow"/> to a collection of <see cref="Adm340BPurchases"/>
        /// </summary>
        IEnumerable<Adm340BPurchases> MapToTenTenAdm340BPurchases(IEnumerable<Adm340BPurchasesRow> adm340BPurchasesRows);
    }
}
