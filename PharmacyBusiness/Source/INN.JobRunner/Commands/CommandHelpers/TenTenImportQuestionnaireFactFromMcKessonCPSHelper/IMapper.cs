using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportQuestionnaireFactFromMcKessonCPSHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="QuestionnaireFactRow"/> to a collection of <see cref="QuestionnaireFact"/>
        /// </summary>
        IEnumerable<QuestionnaireFact> MapToTenTenQuestionnaireFact(IEnumerable<QuestionnaireFactRow> questionnaireFactRows);
    }
}
