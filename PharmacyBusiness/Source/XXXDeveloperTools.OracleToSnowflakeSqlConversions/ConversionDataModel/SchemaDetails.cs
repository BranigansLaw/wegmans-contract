namespace XXXDeveloperTools.OracleToSnowflakeSqlConversions.ConversionDataModel
{
    public class SchemaDetails
    {
        public required McKessonSchemaNickName SchemaName { get; set; }
        public required string TableName { get; set; }
        public required string ColumName { get; set; }
    }
}
