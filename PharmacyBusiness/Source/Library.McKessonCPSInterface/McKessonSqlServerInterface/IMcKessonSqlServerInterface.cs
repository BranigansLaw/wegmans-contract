using System.Data;
using System.Data.SqlClient;

namespace Library.McKessonCPSInterface.McKessonSqlServerInterface
{
    public interface IMcKessonSqlServerInterface
    {
        Task<DataSet> RunQueryFileWithParamsToDataSetAsync(
            string sqlFileName,
            SqlParameter[] queryParams,
            CancellationToken cancellationToken);
    }
}
