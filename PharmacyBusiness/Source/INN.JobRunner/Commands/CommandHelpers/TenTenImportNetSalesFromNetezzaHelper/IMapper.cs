using Library.NetezzaInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportNetSalesFromNetezzaHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="NetSaleRow"/> to a collection of <see cref="NetSales"/>
        /// </summary>
        IEnumerable<NetSales> MapToTenTenNetSales(IEnumerable<NetSaleRow> netSalesRows);
    }
}
