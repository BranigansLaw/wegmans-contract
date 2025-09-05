using Library.McKessonCPSInterface.DataModel;
using System.Data;

namespace Library.McKessonCPSInterface.DataSetMapper
{
    public interface IDataSetMapper
    {
        /// <summary>
        /// Maps the conversation fact data set to a collection of conversation fact rows
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        IEnumerable<ConversationFactRow> MapConversationFact(DataSet ds);

        /// <summary>
        /// Maps the immunization questionnaire data set to a collection of immunization questionnaire rows
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        IEnumerable<ImmunizationQuestionnaireRow> MapImmunizationQuestionnaires(DataSet ds);
    }
}
