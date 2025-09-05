using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Wegmans.RX.Orbita.Orbita
{
    public static class IServiceCollectionOrbitaClient
    {
        public static IServiceCollection AddOrbitaClient(this IServiceCollection serviceDescriptors)
        {
            //NOTE: this is only being done because the IdentityModel SDK does not provide access to the ServiceCollection in AddClientAccessTokenManagement
            //So we needed to get this from our setup config (environment) and set this way which is not prefered
            var config = new ConfigurationBuilder().AddEnvironmentVariables().AddUserSecrets(Assembly.GetExecutingAssembly(), true).Build();
            var idOptions = new OrbitaClientOptions();
            config.GetSection(OrbitaClientOptions.SectionName).Bind(idOptions);
            _ = serviceDescriptors.AddOptions<OrbitaClientOptions>().Configure<IConfiguration>((options, configuration) =>
                configuration.GetSection(OrbitaClientOptions.SectionName).Bind(options));
            serviceDescriptors.AddHttpClient<OrbitaHttpClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<OrbitaClientOptions>>().Value;
                    client.BaseAddress = new Uri(options.ServerAddress, UriKind.Absolute);
                    client.Timeout = options.Timeout;
                }
                ).AddHttpMessageHandler(sp => {
                    var options = sp.GetRequiredService<IOptions<OrbitaClientOptions>>().Value;
                    return new OrbitaHeaderHandler(options.ApiKey);
                }
                )
               .AddTransientHttpErrorPolicy(builder => builder
                .Or<TooManyRequestsException>()
                .WaitAndRetryAsync(3,
                (_, response, context) =>
                {
                    if (response.Exception is TooManyRequestsException ex)
                    {
                        return ex.Response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(5);
                    }

                    return TimeSpan.FromMilliseconds(250);
                },
                (_, x, y, z) => Task.CompletedTask))
                .AddHttpMessageHandler(() => new TooManyRequestsHandler());
            return serviceDescriptors;
        }
    }

    public class TooManyRequestsHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);



            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                throw new TooManyRequestsException(request, response);
            }



            return response;
        }
    }

    public class TooManyRequestsException : Exception
    {
        public TooManyRequestsException(HttpRequestMessage request, HttpResponseMessage response)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            Response = response ?? throw new ArgumentNullException(nameof(response));
        }



        public HttpRequestMessage Request { get; }
        public HttpResponseMessage Response { get; }
    }
}
