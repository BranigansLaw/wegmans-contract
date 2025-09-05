using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wegmans.RX.Orbita.Orbita
{
    public class OrbitaHeaderHandler : DelegatingHandler
    {
        private readonly string _apiKey;

        public OrbitaHeaderHandler(string apiKey)
        {
            _apiKey = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKey)) ?? throw new ArgumentNullException(nameof(apiKey));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("api-key-authorization", $"Basic {_apiKey}");
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}