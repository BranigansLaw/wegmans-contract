using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class OmnisysClaimQuery : AbstractQueryConfiguration<OmnisysClaimRow>
    {

        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/Omnisys_Claim_YYYYMMDD.sql";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter paramRunDate = command.CreateParameter();
            paramRunDate.ParameterName = "RUNDATE";
            paramRunDate.DbType = DbType.Date;
            paramRunDate.Value = new DateTime(RunDate.Year, RunDate.Month, RunDate.Day);
            command.Parameters.Add(paramRunDate);
        }

        /// <inheritdoc />
        public override OmnisysClaimRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating OmnisysClaims");
            string debug = (reader.GetDateTime(4)).ToString();
            return new OmnisysClaimRow
            {
                PharmacyNpi = reader.GetString(0),
                PrescriptionNbr = reader.GetString(1),
                RefillNumber = reader.GetString(2),
                SoldDate = DateOnly.FromDateTime(reader.GetDateTime(3)),
                DateOfService = DateOnly.FromDateTime(reader.GetDateTime(4)),
                NdcNumber = reader.GetString(5),
                CardholderId = reader.GetString(6),
                AuthorizationNumber = reader.GetString(7),
                ReservedForFutureUse = reader.GetString(8),
            };

        }
    }
}
