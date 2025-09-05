using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportIvrOutboundCallsFromOmnicellFileHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="IvrOutboundCallsRow"/> to a collection of <see cref="IvrOutboundCalls"/>
        /// </summary>
        IEnumerable<IvrOutboundCalls> MapToTenTenIvrOutboundCalls(IEnumerable<IvrOutboundCallsRow> ivrOutboundCallsRows);
    }
}
