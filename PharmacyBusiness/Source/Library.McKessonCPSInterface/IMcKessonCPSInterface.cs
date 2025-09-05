using Library.McKessonCPSInterface.DataModel;

namespace Library.McKessonCPSInterface
{
    public interface IMcKessonCPSInterface
    {
        /// <summary>
        /// Returns a list of conversation facts for the given run date.
        /// </summary>
        /// <param name="runDate"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<ConversationFactRow>> GetConversationFactsAsync(DateOnly runDate, CancellationToken c);

        /// <summary>
        /// Returns a list of immunization questionnaires for the given run date.
        /// </summary>
        /// <param name="runDate"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<ImmunizationQuestionnaireRow>> GetImmunizationQuestionnairesAsync(DateOnly runDate, CancellationToken c);
    }
}
