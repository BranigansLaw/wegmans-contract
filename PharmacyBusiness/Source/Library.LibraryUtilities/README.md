# Library Utilities

This project is used by other `Library` projects and contains shared utilities.

## DbDataReader to C# code

The following code was used to generate C# code from `DbDataReader`:

```csharp
ReadOnlyCollection<DbColumn> columnSchema = await dr.GetColumnSchemaAsync();

string classProperties = "";
string mappingProperties = "";
int colCount = 0;
foreach (DbColumn col in columnSchema)
{
    // The new class
    classProperties += $"public required {col.DataType.Name}{(col.AllowDBNull == true ? "?" : "")} {col.ColumnName} {{ get; set; }}\n";

    // The mapping method
    string getMethod = $"dr.GetNullableString({colCount}),";
    if (col.DataType == typeof(DateTime))
    {
        getMethod = $"dr.GetDateTimeFromString({colCount}),";
    }
    else if (col.DataType == typeof(int))
    {
        getMethod = $"dr.GetIntFromString({colCount}),";
    }
    else if (col.DataType == typeof(short))
    {
        getMethod = $"dr.GetShortFromString({colCount}),";
    }
    else if (col.DataType == typeof(long))
    {
        getMethod = $"dr.GetLongFromString({colCount}),";
    }
    else if (col.DataType == typeof(decimal))
    {
        getMethod = $"dr.GetDecimalFromString({colCount}),";
    }
    mappingProperties += $"{col.ColumnName} = {getMethod}\n";

    colCount++;
}

_logger.LogDebug($"public class NewClass {{\n{classProperties}}}");
_logger.LogDebug($"return new NewClass {{\n{mappingProperties}}}");
_logger.LogDebug(JsonConvert.SerializeObject(columnSchema));
```