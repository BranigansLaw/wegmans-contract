namespace Library.LibraryUtilities.Extensions
{
    public class InvalidMappingException : Exception
    {
        public int ColumnIndex { get; }

        public string ColumnName { get; set; }

        public string? Value { get; }

        public InvalidMappingException(int columnIndex, string columnName, string? value, Exception innerException) : base(
            message: $"Mapping failed for value {value} in column index {columnIndex} due to error {innerException.Message}",
            innerException: innerException
        )
        {
            ColumnIndex = columnIndex;
            ColumnName = columnName;
            Value = value;
        }
    }
}
