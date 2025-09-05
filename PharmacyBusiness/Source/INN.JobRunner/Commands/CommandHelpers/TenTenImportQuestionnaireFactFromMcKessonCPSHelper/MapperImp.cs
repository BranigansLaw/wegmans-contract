using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportQuestionnaireFactFromMcKessonCPSHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<QuestionnaireFact> MapToTenTenQuestionnaireFact(IEnumerable<QuestionnaireFactRow> questionnaireFactRows)
        {
            return questionnaireFactRows.Select(r => new QuestionnaireFact
            {
                //TODO: UNDER CONSTRUCTION
                StoreNum = r.StoreNum
            });
        }
    }
}
