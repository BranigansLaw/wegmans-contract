using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportMessageFactFromMcKessonCPSHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<MessageFact> MapToTenTenMessageFact(IEnumerable<MessageFactRow> messageFactRows)
        {
            return messageFactRows.Select(r => new MessageFact
            {
                //TODO: UNDER CONSTRUCTION
                StoreNum = r.StoreNum
            });
        }
    }
}
