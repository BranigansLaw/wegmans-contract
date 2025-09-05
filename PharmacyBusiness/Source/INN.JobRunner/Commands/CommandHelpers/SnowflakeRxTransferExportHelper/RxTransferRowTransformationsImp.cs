using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;

namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeRxTransferExportHelper
{
    public class RxTransferRowTransformationsImp : IRxTransferRowTransformations
    {
        public void HandleTrimTrailingZeroes(RxTransferRow rxTransferRow)
        {
            rxTransferRow.QtyDispensed = rxTransferRow.QtyDispensed.TrimTrailingZeroes();
            rxTransferRow.PatientPay = rxTransferRow.PatientPay.TrimTrailingZeroes();
            rxTransferRow.TpPay = rxTransferRow.TpPay.TrimTrailingZeroes();
            rxTransferRow.TxPrice = rxTransferRow.TxPrice.TrimTrailingZeroes();
            rxTransferRow.AcqCos = rxTransferRow.AcqCos.TrimTrailingZeroes();
            rxTransferRow.UCPrice = rxTransferRow.UCPrice.TrimTrailingZeroes();
        }
    }
}
