using Library.TenTenInterface.Response;

namespace Library.TenTenInterface.TenTenResponseParser
{
    public interface ITenTenResponseParser
    {
        /// <summary>
        /// Parse TenTen <see cref="HttpRequestMessage"/>, handle error codes, and return the desired objest
        /// </summary>
        T ParseTenTenResponse<T>(HttpResponseMessage res) where T : class, ITenTenResponse;
    }
}
