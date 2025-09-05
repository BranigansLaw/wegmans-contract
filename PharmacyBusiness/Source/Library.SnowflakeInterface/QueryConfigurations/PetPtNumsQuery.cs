using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations;

public class PetPtNumsQuery : AbstractQueryConfiguration<PetPtNumRow>
{
    /// <inheritdoc />
    public override string QueryFilePath => "./SnowflakeSQL/PetPtNums_YYYYMMDD.sql";

    /// <inheritdoc />
    public override PetPtNumRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
    {
        logDebug("Creating PetPtNum");

        return new PetPtNumRow
        {
            PatientNum = reader.GetNullableValueByIndex<long>(0),
            Species = reader.GetStringByIndex(1),
            CreateDate = reader.GetNullableValueByIndex<DateTime>(2),
            Pet = reader.GetStringByIndex(3),
        };
    }
}
