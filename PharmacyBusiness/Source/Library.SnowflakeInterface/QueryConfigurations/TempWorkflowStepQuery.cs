using Library.SnowflakeInterface.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class TempWorkflowStepQuery : AbstractQueryConfiguration<TempWorkflowStepRow>
    {
        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/TEMP_WORKFLOW_STEP.sql";

        /// <inheritdoc />
        public override TempWorkflowStepRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating TempWorkflowStep");
            throw new NotImplementedException();
        }
    }
}
