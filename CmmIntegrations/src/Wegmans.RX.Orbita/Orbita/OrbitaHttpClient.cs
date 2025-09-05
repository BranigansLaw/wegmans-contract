using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Wegmans.RX.Orbita.Orbita
{
    public class OrbitaHttpClient
    {
        private readonly HttpClient _client;
        private IOptions<OrbitaClientOptions> _options;
        private static string orbitaSoftError = "the user was found, but was not removed from further communications";

        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true
        };
        
        public OrbitaHttpClient(HttpClient client, IOptions<OrbitaClientOptions> options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<List<OrbitaPatient>> RemovePatient(OrbitaJsonPayload orbitaPayload)
        {
            HttpContent httpContent = new StringContent(JsonSerializer.Serialize(orbitaPayload, jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);
            
            var orbitaResponse = await _client.PostAsync("/oeapi/communication/remove", httpContent).ConfigureAwait(false);
            
            orbitaResponse.EnsureSuccessStatusCode();

            var response = await orbitaResponse.Content.ReadAsAsync<List<OrbitaPatient>>().ConfigureAwait(false);

            /*              
             Orbita "Soft Error" handling and retries
            
            In the returned JSON payload from Orbita they can return a result that indicates Orbita had a internal error and should be retried.
            In the code below, if that result is returned a custom SoftErrorException will be thrown.
            That exception bubbles up to SendToOrbita(OrbitaJsonPayload payload) which in turn bubbles up to OrbitaProcessor:RunAsync().
            The exception gets caught and logged there, then rethrown.
            There is nothing left to remove that message from the queue, so the lease on the queue message being read (and removed) expires.
            The message will then get reprocessed.
            The HTTPClient code (boilerplate code) will recognize if that same message gets reprocessed 5 times.
            And if it does, the message gets put into the "poison queue" just like any hard HTTP errors would.
            The poison queue gets monitored for items being in it, and if there is, a Remedy ticket gets generated then cleared when the poison queue is empty.          
             */
            if (response == null || response.Any(r => r.Result.Equals(orbitaSoftError, StringComparison.InvariantCultureIgnoreCase)) == true)
            {
                // TODO: Locate patient(s) instead of assuming only one
                throw new SoftErrorException($"Soft Error returned from Orbita. TransID: {orbitaPayload.Patients[0].TransactionID}");
            }

            return response;
        }
    }

    public class SoftErrorException : Exception
    {
        public SoftErrorException()
        {
        }

        public SoftErrorException(string message) : base(message)
        {
        }

        public SoftErrorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}