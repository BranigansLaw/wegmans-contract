using Library.McKessonCPSInterface.DataModel;
using Library.McKessonCPSInterface.DataSetMapper;
using Library.McKessonCPSInterface.McKessonSqlServerInterface;
using System.Data;
using System.Data.SqlClient;

namespace Library.McKessonCPSInterface
{
    public class McKessonCPSInterfaceImp : IMcKessonCPSInterface
    {
        private readonly IMcKessonSqlServerInterface _mcKessonSqlServerInterface;
        private readonly IDataSetMapper _dataSetMapper;

        public McKessonCPSInterfaceImp(
            IMcKessonSqlServerInterface mcKessonSqlServerInterface,
            IDataSetMapper dataSetMapper)
        {
            _mcKessonSqlServerInterface = mcKessonSqlServerInterface ?? throw new ArgumentNullException(nameof(mcKessonSqlServerInterface));
            _dataSetMapper = dataSetMapper ?? throw new ArgumentNullException(nameof(dataSetMapper));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ConversationFactRow>> GetConversationFactsAsync(DateOnly runDate, CancellationToken c)
        {
            SqlParameter[] queryParams = [
                new SqlParameter("RunDate", SqlDbType.Date)
            ];
            queryParams[0].Value = runDate.ToDateTime(new TimeOnly(0));

            DataSet ds = await _mcKessonSqlServerInterface.RunQueryFileWithParamsToDataSetAsync(
                "SelectConversationFact",
                queryParams,
                c);

            return _dataSetMapper.MapConversationFact(ds);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ImmunizationQuestionnaireRow>> GetImmunizationQuestionnairesAsync(DateOnly runDate, CancellationToken c)
        {
            SqlParameter[] queryParams = [
                new SqlParameter("RunDate", SqlDbType.Date)
            ];
            queryParams[0].Value = runDate.ToDateTime(new TimeOnly(0));

            DataSet ds = await _mcKessonSqlServerInterface.RunQueryFileWithParamsToDataSetAsync(
                "SelectImmunizationQuestionnaire",
                queryParams,
                c);

            return _dataSetMapper.MapImmunizationQuestionnaires(ds);
        }
    }
}
