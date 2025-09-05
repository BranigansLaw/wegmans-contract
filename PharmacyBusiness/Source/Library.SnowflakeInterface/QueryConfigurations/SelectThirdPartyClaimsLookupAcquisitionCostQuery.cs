using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class SelectThirdPartyClaimsLookupAcquisitionCostQuery : AbstractQueryConfiguration<SelectThirdPartyClaimsLookupAcquisitionCostRow>
    {
        public required string RxRecordNumber { get; set; }

        public required string RxFillSequence { get; set; }

        public required string FillStateKey { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/SelectThirdPartyClaimsLookupAcquisitionCost.sql";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter paramRx_record_num = command.CreateParameter();
            paramRx_record_num.ParameterName = "RX_RECORD_NUM";
            paramRx_record_num.DbType = DbType.String;
            paramRx_record_num.Value = RxRecordNumber;
            command.Parameters.Add(paramRx_record_num);

            DbParameter paramRx_fill_seq = command.CreateParameter();
            paramRx_fill_seq.ParameterName = "RX_FILL_SEQ";
            paramRx_fill_seq.DbType = DbType.String;
            paramRx_fill_seq.Value = RxFillSequence;
            command.Parameters.Add(paramRx_fill_seq);

            DbParameter paramFill_state_key = command.CreateParameter();
            paramFill_state_key.ParameterName = "FILL_STATE_KEY";
            paramFill_state_key.DbType = DbType.String;
            paramFill_state_key.Value = FillStateKey;
            command.Parameters.Add(paramFill_state_key);
        }

        /// <inheritdoc />
        public override SelectThirdPartyClaimsLookupAcquisitionCostRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating SelectThirdPartyClaimsLookupAcquisitionCost");
            throw new NotImplementedException();
        }
    }
}
