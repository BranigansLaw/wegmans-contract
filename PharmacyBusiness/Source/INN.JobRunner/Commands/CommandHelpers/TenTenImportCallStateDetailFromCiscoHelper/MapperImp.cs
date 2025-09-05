using Library.InformixInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportCallStateDetailFromCiscoHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<CallStateDetail> MapToTenTenCallStateDetail(IEnumerable<StateDetailRow> callStateDetailRows)
        {
            return callStateDetailRows.Select(r => new CallStateDetail
            {
                
            });
        }
    }
}
