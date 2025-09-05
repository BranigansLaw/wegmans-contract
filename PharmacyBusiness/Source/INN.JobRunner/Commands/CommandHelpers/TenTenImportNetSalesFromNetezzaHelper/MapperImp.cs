using Library.NetezzaInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportNetSalesFromNetezzaHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<NetSales> MapToTenTenNetSales(IEnumerable<NetSaleRow> netSalesRows)
        {
            return netSalesRows.Select(r => new NetSales
            {
                CashierNumber = r.CashNum,
                CouponDescriptionMFG = r.CouponDescMfg,
                CouponDescriptionWP = r.CouponDescWp,
                DateSold = r.DateSold,
                DepartmentName = r.DeptName,
                DepartmentNumber = r.DeptNum,
                DisplayTime = r.DisplayTime,
                GPAmount = r.GpAmt,
                ItemDescription = r.ItemDesc,
                ItemNumber = r.ItemNum,
                NetItemCount = r.NetCnt,
                NetSalesAmount = r.NetSalesAmt,
                PLDepartmentCode = r.PlCode,
                PLDepartmentName = r.PlName,
                Quantity = r.Qty,
                RefundAmount = r.RefundAmt,
                RegisterDescription = r.RegDesc,
                RegisterNumber = r.RegNum,
                StoreName = r.StoreName,
                StoreNum = r.StoreNum,
                TenderAmount = r.TenderAmt,
                TenderStatusDescription = r.TenderDesc,
                TenderTypeDescription = r.TenderTypeDesc,
                TenderType = r.TenderType,
                TransactionNumber = r.TxNum,
                TxTypeCode = r.TxType,
                TxTypeDescription = r.TxTypeDesc,
            });
        }
    }
}
