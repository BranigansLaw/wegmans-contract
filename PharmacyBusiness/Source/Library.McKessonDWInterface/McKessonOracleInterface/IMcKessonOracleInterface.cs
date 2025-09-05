using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Library.McKessonDWInterface.McKessonOracleInterface
{
    public interface IMcKessonOracleInterface
    {
        /// <summary>
        /// This method utilizes Oracle bind variables.
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="sqlFileName"></param>
        /// <param name="queryParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DataSet> RunQueryFileWithParamsToDataSetAsync(
            CommandType commandType,
            string sqlFileName,
            OracleParameter[] queryParams,
            CancellationToken cancellationToken);

        /// <summary>
        /// This method does a find replace so that literals can be replaced in the SQL file.
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="sqlFileName"></param>
        /// <param name="findReplaceLiteralParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DataSet> RunQueryFileWithLiteralsToDataSetAsync(
            CommandType commandType,
            string sqlFileName,
            Dictionary<string, string> findReplaceLiteralParams,
            CancellationToken cancellationToken);
    }
}
