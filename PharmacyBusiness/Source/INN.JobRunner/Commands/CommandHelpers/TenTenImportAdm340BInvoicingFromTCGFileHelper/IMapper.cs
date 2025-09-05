using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BInvoicingFromTCGFileHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="Adm340BInvoicingRow"/> to a collection of <see cref="Adm340BInvoicing"/>
        /// </summary>
        IEnumerable<Adm340BInvoicing> MapToTenTenAdm340BInvoicing(IEnumerable<Adm340BInvoicingRow> adm340BInvoicingRows);
    }
}
