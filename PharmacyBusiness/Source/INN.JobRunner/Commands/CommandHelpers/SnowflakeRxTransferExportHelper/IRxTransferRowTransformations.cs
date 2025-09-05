using Library.SnowflakeInterface.Data;

namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeRxTransferExportHelper
{
    public interface IRxTransferRowTransformations
    {
        /// <summary>
        /// Handles trimming trailing zeroes for RxTransferRow
        /// </summary>
        /// <param name="rxTransferRow"></param>
        void HandleTrimTrailingZeroes(RxTransferRow rxTransferRow);
    }
}
