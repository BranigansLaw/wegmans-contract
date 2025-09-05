using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class GetSmartOrderMinMaxQuery : AbstractQueryConfiguration<GetSmartOrderMinMaxRow>
    {

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/GetSmartOrderPointsMinMax.sql";

        /// <inheritdoc />
        public override GetSmartOrderMinMaxRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating GetSmartOrderMinMaxRow for NDC==>> )" + reader.GetString(1));
            return new GetSmartOrderMinMaxRow
            {
                StoreNumber = reader.GetString(0),
                NdcWo = reader.GetString(1),
                MinQtyOverride = reader.GetNullableValueByIndex<long>(2),
                MaxQtyOverride = reader.GetNullableValueByIndex<long>(3),
                PurchasePlan = reader.GetString(4),
                LastUpdated = reader.GetNullableValueByIndex<long>(5)
            };

        }


    }
}
