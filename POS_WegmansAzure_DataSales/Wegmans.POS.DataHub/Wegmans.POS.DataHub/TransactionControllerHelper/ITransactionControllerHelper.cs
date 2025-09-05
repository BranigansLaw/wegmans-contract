using System.Threading.Tasks;
using System.Xml.Linq;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.PriceData;

namespace Wegmans.POS.DataHub.TransactionControllerHelper;

public interface ITransactionControllerHelper
{
    /// <Summary>
    ///     Updates Transaction record
    /// </Summary>
    /// <param name="record">  <see cref="XElement"/> ACE data serialized as XElement</param>
    /// <param name="outputTransaction">  <see cref="Transaction"/> object being modified to contain the JSON output data</param>
    void UpdateTransactionData(XElement record, Transaction outputTransaction);

    /// <summary>
    /// Check Item Sold By LB
    /// </summary>
    /// <param name="itemnumber"></param>
    /// <returns>True if item is sold by LB, false otherwise</returns>
    Task<bool> CheckItemSoldByLB(int? itemnumber);

    /// <summary>
    /// Check Price by SKu and Store Number
    /// </summary>
    /// <param name="itemnumber"></param>
    /// <param name="storenumber"></param>
    /// <returns>Price if item found, false otherwise</returns>
    Task<PriceDataController.PriceData> GetPriceByItemStore(int? itemnumber, int? storenumber);
}
