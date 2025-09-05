using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportConversationFactFromMcKessonCPSHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="ConversationFactRow"/> to a collection of <see cref="ConversationFact"/>
        /// </summary>
        IEnumerable<ConversationFact> MapToTenTenConversationFact(IEnumerable<ConversationFactRow> conversationFactRows);
    }
}
