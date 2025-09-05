using Library.LibraryUtilities.Extensions;

namespace Library.SnowflakeInterface.Exceptions
{
    public class RowMapperFailedException : Exception
    {
        public int RowFailed { get; }

        public RowMapperFailedException(int rowFailed, InvalidMappingException ex) : 
            base(
                message: $"Row number {rowFailed} encountered an error in mapping for column {ex.ColumnIndex}: {ex.ColumnName}{(ex.InnerException != null ? $" with error {ex.InnerException.Message}" : "")}",
                innerException: ex
            )
        {
            RowFailed = rowFailed;
        }
    }
}
