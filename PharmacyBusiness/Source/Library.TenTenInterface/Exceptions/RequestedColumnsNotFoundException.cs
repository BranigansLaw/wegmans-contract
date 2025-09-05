namespace Library.TenTenInterface.Exceptions
{
    public class RequestedColumnsNotFoundException(string message) : Exception($"Failed due to missing columns with message: {message}") { }
}
