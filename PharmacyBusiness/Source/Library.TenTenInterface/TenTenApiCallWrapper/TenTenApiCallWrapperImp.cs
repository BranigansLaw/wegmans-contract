using Library.TenTenInterface.Exceptions;
using Library.TenTenInterface.Response;
using Library.TenTenInterface.TenTenResponseParser;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Net.Sockets;

namespace Library.TenTenInterface.TenTenApiCallWrapper
{
    public class TenTenApiCallWrapperImp : ITenTenApiCallWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<TenTenConfig> _config;
        private readonly ITenTenResponseParser _tenTenResponseParser;
        private readonly ILogger<TenTenApiCallWrapperImp> _logger;

        public TenTenApiCallWrapperImp(
            HttpClient httpClient,
            IOptions<TenTenConfig> config,
            ITenTenResponseParser tenTenResponseParser,
            ILogger<TenTenApiCallWrapperImp> logger
        )
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _tenTenResponseParser = tenTenResponseParser ?? throw new ArgumentNullException(nameof(tenTenResponseParser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            TenTenRetryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(8, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), onRetry: LogRetry); // retry after 2s, 4s, ... 4 minutes

            HandleTenTenReturnCodesPolicy = Policy
                .Handle<TooManyActiveLoginsException>()
                .Or<InvalidUsernameOrPasswordException>()
                .Or<MaterializeQueryFailedException>()
                .Or<DuplicateSessionException>()
                .Or<LoginThrottlingException>()
                .Or<TimeoutException>()
                .Or<SocketException>() // Network error
                .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(30 * retryAttempt), onRetry: LogRetry); // retry after 30s, 60s, ... 5 minutes
        }

        /// <summary>
        /// Retry logic for HTTP Exceptions
        /// </summary>
        private readonly AsyncRetryPolicy TenTenRetryPolicy;

        /// <summary>
        /// Retry logic for 1010 Exceptions
        /// </summary>
        private readonly AsyncRetryPolicy HandleTenTenReturnCodesPolicy;

        private void LogRetry(Exception ex, TimeSpan waitTime)
        {
            _logger.LogWarning(ex, $"Retrying in {waitTime.Seconds} seconds");
        }

        /// <summary>
        /// Wrapper for TenTen API requests
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="action">The action to wrap</param>
        private Task<T> TenTenRetryWrapperAsync<T>(Func<Task<T>> action) =>
            TenTenRetryPolicy.ExecuteAsync(() =>
                HandleTenTenReturnCodesPolicy.ExecuteAsync(action));

        /// <inheritdoc />
        public Task<T> CallTenTenApiAsync<T>(Func<SessionCredentials, HttpClient, Task<HttpResponseMessage>> apiWrapper, CancellationToken c) where T : class, ITenTenResponse
        {
            return TenTenRetryWrapperAsync(async () =>
            {
                LoginResponse? loginResponse = null;
                try
                {
                    _logger.LogDebug("Logging into TenTen");
                    loginResponse = _tenTenResponseParser.ParseTenTenResponse<LoginResponse>(
                        await _httpClient.GetAsync($"gw.k?api=login&apiversion=3&uid={_config.Value.Username}&pswd={_config.Value.Password}{_config.Value.KillSettings}",
                        c).ConfigureAwait(false));
                    _logger.LogDebug("Login Completed Successfully");

                    return _tenTenResponseParser.ParseTenTenResponse<T>(await apiWrapper(
                        new SessionCredentials(
                            username: _config.Value.Username,
                            sessionId: loginResponse.SessionId,
                            password: loginResponse.Password
                        ),
                        _httpClient).ConfigureAwait(false));
                }
                finally
                {
                    if (loginResponse != null)
                    {
                        _logger.LogDebug("Logging out of TenTen");
                        _tenTenResponseParser.ParseTenTenResponse<LogoutResponse>(
                            await _httpClient.GetAsync($"gw.k?api=logout&apiversion=3&uid={_config.Value.Username}&pswd={loginResponse.Password}&sid={loginResponse.SessionId}{_config.Value.KillSettings}",
                            c).ConfigureAwait(false));
                        _logger.LogDebug("Logout complete. Waiting for 5 seconds for TenTen session to be recycled");

                        // Wait 5 seconds for the logout to resolve on the TenTen side
                        await Task.Delay(5000);
                        _logger.LogDebug("Logout wait completed");
                    }
                }
            });
        }

        public Task<T> CallTenTenApiWithoutLoginAsync<T>(Func<HttpClient, Task<HttpResponseMessage>> apiWrapper, CancellationToken c) where T : class, ITenTenResponse
        {
            return TenTenRetryWrapperAsync(async () =>
            {
                return _tenTenResponseParser.ParseTenTenResponse<T>(await apiWrapper(_httpClient).ConfigureAwait(false));
            });
        }
    }
}
