using Library.TenTenInterface.Response;

namespace Library.TenTenInterface.TenTenApiCallWrapper
{
    public interface ITenTenApiCallWrapper
    {
        /// <summary>
        /// Handles the login/logout as well as retry logic while running <paramref name="apiWrapper"/>
        /// </summary>
        Task<T> CallTenTenApiAsync<T>(Func<SessionCredentials, HttpClient, Task<HttpResponseMessage>> apiWrapper, CancellationToken c) where T : class, ITenTenResponse;

        /// <summary>
        /// Call the TenTen API without the login/logout handling, but with retry logic and error code handling
        /// </summary>
        Task<T> CallTenTenApiWithoutLoginAsync<T>(Func<HttpClient, Task<HttpResponseMessage>> apiWrapper, CancellationToken c) where T : class, ITenTenResponse;
    }
}
