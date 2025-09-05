using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Configuration;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm
{
    public class CmmHttpClient
    {
        private readonly HttpClient _client;

        public CmmHttpClient(HttpClient client, IOptions<AppCmmClient> config)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _ = config.Value ?? throw new ArgumentNullException(nameof(config));

            _client.BaseAddress = new Uri(config.Value.BaseUrl);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{config.Value.Username}:{config.Value.Password}")));
        }

        public async Task SpecialtyDrugsPatientStatusUpdate(PatientStatus request, CancellationToken cancellationToken)
        {
            var response = await _client.PostAsync("service_request_events", new StringContent(JsonSerializer.Serialize<PatientStatus>(request), Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

            if(!response.IsSuccessStatusCode)
            {
                throw new CmmException($"Patient: {request.ServiceRequestEvent.SourceIdentifier} update to CMM failed with response code: {response.StatusCode}");
            }
        }
    }
}
