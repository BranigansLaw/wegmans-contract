namespace XXXDeveloperTools.OracleToSnowflakeSqlConversions.ConversionDataModel
{
    public class JobToBeConvertedFromOracleToSnowflake
    {
        public required string JobNumber { get; set; }
        public required string SqlFileName { get; set; }
        public required string SqlFileRelativePath { get; set; }
        public IEnumerable<OracleToSnowflakeColumnMap>? ColumnMaps { get; set; }
    }
}
