namespace Library.LibraryUtilities.Extensions
{
    public class ColumnIndexOutOfBoundsException : Exception
    {
        public int ColumnIndex { get; }

        public ColumnIndexOutOfBoundsException(int columnIndex)
        {
            ColumnIndex = columnIndex;
        }
    }
}
