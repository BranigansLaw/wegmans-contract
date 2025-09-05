namespace XXXDeveloperTools.McKessonToSnowflakeMigrationFields
{
    public class ColumnMapSetting
    {
        public required string TableName { get; set; }

        public required string FieldName { get; set; }

        public required string? Replacement { get; set; }
    }
}
