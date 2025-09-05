using System.Text.RegularExpressions;
using XXXDeveloperTools.McKessonToSnowflakeMigrationFields;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string[] attributeMapLines = await File.ReadAllLinesAsync("./Attribute_Changes.csv");

        string currentTable = "";
        ICollection<ColumnMapSetting> columnMapSettings = [];
        ICollection<TableNameChangeSetting> tableNameChangeSettings = [];
        foreach (string attributeMapLine in attributeMapLines)
        {
            string[] columns = Regex.Split(attributeMapLine, "[,]{1}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            if (!string.IsNullOrEmpty(columns[0]) && columns.Length >= 5)
            {
                string tableName = columns[1];
                string mckessonColumnName = columns[2];
                string reason = columns[3];
                string? snowflakeColumnName = columns[4];

                if (!string.IsNullOrEmpty(tableName))
                {
                    currentTable = tableName;
                }

                if (reason == "table name changed")
                {
                    tableNameChangeSettings.Add(new TableNameChangeSetting
                    {
                        OriginalTableName = currentTable.Replace(" ", ""),
                        NewTableName = snowflakeColumnName.Replace(" ", ""),
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(snowflakeColumnName) && !string.IsNullOrEmpty(mckessonColumnName))
                    {
                        foreach (string snowflakeColumn in snowflakeColumnName.Split('\n'))
                        {
                            columnMapSettings.Add(new ColumnMapSetting
                            {
                                TableName = currentTable.Replace(" ", ""),
                                FieldName = mckessonColumnName.Replace(" ", ""),
                                Replacement = snowflakeColumn.Replace(" ", ""),
                            });
                        }
                    }
                }
            }
        }

        string[] files = Directory.GetFiles("../../../../Library.SnowflakeInterface/SnowflakeSQL");

        foreach (string file in files)
        {
            string fileContents = await File.ReadAllTextAsync(file);
            string originalFileContents = fileContents;

            foreach (ColumnMapSetting columnMapSetting in columnMapSettings)
            {
                Regex columnNameAlias = new($@"ERXDW_PLS_ARCHIVE_VIEW\.{columnMapSetting.TableName} ([A-Za-z0-9_\-]*)$");

                MatchCollection matches = columnNameAlias.Matches(fileContents);
                foreach (Match match in matches)
                {
                    string tableAlias = match.Groups[1].Value;
                    fileContents = fileContents.Replace($"{tableAlias}.{columnMapSetting.FieldName}", $"{tableAlias}.{columnMapSetting.Replacement}");
                }
            }

            foreach (TableNameChangeSetting tableNameMapping in tableNameChangeSettings)
            {
                fileContents = Regex.Replace(
                    fileContents, 
                    @$"ERXDW_PLS_ARCHIVE_VIEW\.{tableNameMapping.OriginalTableName}$", 
                    $"ERXDW_PLS_ARCHIVE_VIEW.{tableNameMapping.NewTableName}"
                );
            }

            if (fileContents != originalFileContents)
            {
                await File.WriteAllTextAsync(file, fileContents);
            }
        }
    }
}