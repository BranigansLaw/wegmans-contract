namespace Library.TenTenInterface.Exceptions
{
    public class UnknownReturnCodeException : Exception
    {
        public UnknownReturnCodeException(int returnCode, string? tentenMessage) : 
            base($"Unknown TenTen response code ({returnCode}){(!string.IsNullOrEmpty(tentenMessage) ? ": " : "")}{tentenMessage}")
        {
            ReturnCode = returnCode;
            TenTenMessage = tentenMessage;
        }

        public int ReturnCode { get; }

        public string? TenTenMessage { get; }
    }
}
