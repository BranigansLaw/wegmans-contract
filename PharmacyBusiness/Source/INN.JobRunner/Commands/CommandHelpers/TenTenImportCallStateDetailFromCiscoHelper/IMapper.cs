using Library.InformixInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportCallStateDetailFromCiscoHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="StateDetailRow"/> to a collection of <see cref="CallStateDetail"/>
        /// </summary>
        IEnumerable<CallStateDetail> MapToTenTenCallStateDetail(IEnumerable<StateDetailRow> callStateDetailRows);
    }
}
