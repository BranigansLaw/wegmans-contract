using System.Data.OleDb;
using System.Data;
using Library.NetezzaInterface.DataModel;

namespace Library.NetezzaInterface
{
    public interface INetezzaInterface
    {
        Task<IEnumerable<NetSaleRow>> GetNetSalesAsync(DateOnly runFor, CancellationToken cancellationToken);

        Task<DataSet> DownloadQueryWithParamsToDataSetAsync(
           string sqlFile,
           OleDbParameter[] queryParams,
           CancellationToken cancellationToken
        );
    }
}
