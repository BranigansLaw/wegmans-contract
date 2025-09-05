namespace Library.TenTenInterface.Exceptions
{
    public class RowCountMismatchException(int expected, int actual) : Exception($"TenTen row count mismatch. Expected: {expected}. Actual: {actual}") { }
}
