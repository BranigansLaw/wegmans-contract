namespace Library.TenTenInterface.Exceptions
{
    public class LoginThrottlingException(string? message) : Exception(message) { }
}
