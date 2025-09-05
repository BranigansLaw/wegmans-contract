using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportMessageFactFromMcKessonCPSHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="MessageFactRow"/> to a collection of <see cref="MessageFact"/>
        /// </summary>
        IEnumerable<MessageFact> MapToTenTenMessageFact(IEnumerable<MessageFactRow> messageFactRows);
    }
}
