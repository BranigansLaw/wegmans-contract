using AstuteCaseService;
using AstutePatientService;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MtomEncoder;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Wegmans.RX.Orbita.Astute;
using Wegmans.RX.Orbita.Astute.Configuration;
using Wegmans.RX.Orbita.Orbita;

[assembly: FunctionsStartup(typeof(Wegmans.RX.Orbita.Startup))]
namespace Wegmans.RX.Orbita
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            using var serviceProvider = builder.Services.BuildServiceProvider();
            var hostConfiguration = serviceProvider.GetRequiredService<IConfiguration>();
            
            builder.Services.AddOptions();
            builder.Services.AddHttpClient();
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            _ = builder.Services.AddHealthChecks().AddAzureStorageQueueDepthHealthCheck<IConfiguration>(nameof(OrbitaQueueBindingSettings.PatientsToProcessQueueName) + "-poison", 
                c => c.GetValue<string>(OrbitaQueueBindingSettings.StorageAccountConnection),
                c => c.GetValue<string>(nameof(OrbitaQueueBindingSettings.PatientsToProcessQueueName)) + "-poison",
                ignoreQueueNotExists: true)
                .AddApplicationInsightsPublisher(options => options.ApplicationName = "Orbita");

            var healthCheckPublisherHostedService = builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IHostedService) && x.ImplementationType?.Name == "HealthCheckPublisherHostedService");

            if (!(healthCheckPublisherHostedService is null))
            {
                builder.Services.Remove(healthCheckPublisherHostedService);
            }

            builder.Services.Configure<AppAstuteClientSecurity>(hostConfiguration.GetSection(nameof(AppAstuteClientSecurity)));

            var astuteClientSettings = hostConfiguration.GetSection(nameof(AppAstuteClient)).Get<AppAstuteClient>();

            builder.Services.AddSingleton<ICaseService>(new CaseServiceClient(this.GetMtomCustomBinding(), new EndpointAddress(astuteClientSettings.CaseUrl)));
            builder.Services.AddSingleton<IAddressService>(new AddressServiceClient(AddressServiceClient.EndpointConfiguration.WSHttpBinding_IAddressService, astuteClientSettings.AddressUrl));
            builder.Services.AddTransient(typeof(IAstuteSoapProxy), typeof(AstuteSoapProxy));

            builder.Services.AddOrbitaClient();
        }

        private CustomBinding GetMtomCustomBinding()
        {
            var enc = new TextMessageEncodingBindingElement
            {
                MessageVersion = MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap11, AddressingVersion.None),
                ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                {
                    MaxBytesPerRead = 20000000
                }
            };
            var encoding = new MtomMessageEncoderBindingElement(enc);
            var transport = new HttpsTransportBindingElement
            {
                MaxReceivedMessageSize = 20000000,
                MaxBufferSize = 20000000
            };
            return new CustomBinding(encoding, transport);
        }
    }
}
