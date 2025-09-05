using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportConversationFactFromMcKessonCPSHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<ConversationFact> MapToTenTenConversationFact(IEnumerable<ConversationFactRow> conversationFactRows)
        {
            return conversationFactRows.Select(r => new ConversationFact
            {
                //TODO: UNDER CONSTRUCTION
                StoreNum = (long) r.MessageKey,
            });
        }
    }
}
