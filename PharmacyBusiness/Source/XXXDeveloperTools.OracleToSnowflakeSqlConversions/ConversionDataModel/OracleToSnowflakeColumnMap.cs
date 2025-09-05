namespace XXXDeveloperTools.OracleToSnowflakeSqlConversions.ConversionDataModel
{
    public class OracleToSnowflakeColumnMap
    {
        public required SchemaDetails OracleSchemaDetails { get; set; }
        public required bool IsOracleColumNameDeprecatedInSnowflake { get; set; }
        public string? RevisedSnowflakeColumName { get; set; }
    }

}
