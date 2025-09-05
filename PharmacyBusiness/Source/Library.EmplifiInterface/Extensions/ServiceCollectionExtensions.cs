using CaseServiceWrapper;
using AddressServiceWrapper;
using CaseStreamServiceWrapper;
using Library.EmplifiInterface.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.ServiceModel;

namespace Library.EmplifiInterface.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IEmplifiInterface"/> interface to the <see cref="IServiceCollection"/>
        /// </summary>
        public static IServiceCollection AddEmplifiInterface(this IServiceCollection services, Action<EmplifiConfig> getConfig)
        {
            services.Configure(getConfig);
            services.AddTransient<IEmplifiInterface, EmplifiInterfaceImp>();
            services.AddTransient<IDelayAndDenialHelper, DelayAndDenialHelperImp>();
            services.AddTransient<IJpapDispenseAndStatusHelper, JpapDispenseAndStatusHelperImp>();
            services.AddTransient<IOncologyTriageHelper, OncologyTriageHelperImp>();
            services.AddTransient<IJpapTriageHelper, JpapTriageHelperImp>();
            services.AddTransient<IJpapEligibilityHelper, JpapEligibilityHelperImp>();
            services.AddTransient<IAstuteAdherenceDispenseHelper, AstuteAdherenceHelperImp>();
            services.AddTransient<IVerificationOfBenefitsHelper, VerificationOfBenefitsHelperImp>();
            services.AddTransient<ICaseService, CaseServiceClient>(s =>
                new CaseServiceClient(new BasicHttpsBinding
                {
                    Security = new BasicHttpsSecurity
                    {
                        Mode = BasicHttpsSecurityMode.Transport
                    },
                    MessageEncoding = WSMessageEncoding.Mtom,

                    //These two properties are needed to increase the message size limit from the default 64KB to 20MB because these have maturity - they have been in use since 2020 without issue.
                    MaxReceivedMessageSize = 20000000,
                    MaxBufferSize = 20000000
                },
                new EndpointAddress(string.Concat(s.GetRequiredService<IOptions<EmplifiConfig>>().Value.Url, "/CaseService.svc"))));
            services.AddTransient<IAddressService, AddressServiceClient>(s =>
                new AddressServiceClient(new WSHttpBinding
                {
                    Security = new WSHttpSecurity
                    {
                        Mode = SecurityMode.Transport
                    }
                },
                new EndpointAddress(string.Concat(s.GetRequiredService<IOptions<EmplifiConfig>>().Value.Url, "/AddressService.svc"))));
            services.AddTransient<ICaseStreamService, CaseStreamServiceClient>(s =>
                new CaseStreamServiceClient(new BasicHttpsBinding
                {
                    Security = new BasicHttpsSecurity
                    {
                        Mode = BasicHttpsSecurityMode.Transport
                    },
                    MessageEncoding = WSMessageEncoding.Mtom
                },
                new EndpointAddress(string.Concat(s.GetRequiredService<IOptions<EmplifiConfig>>().Value.Url, "/CaseStreamService.svc"))));

            return services;
        }

        // might be needed for custom binding later
        //private CustomBinding GetMtomCustomBinding()
        //{
        //    var enc = new TextMessageEncodingBindingElement
        //    {
        //        MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap11, AddressingVersion.None),
        //        ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
        //        {
        //            MaxBytesPerRead = 20000000
        //        }
        //    };
        //    var encoding = new MtomMessageEncoderBindingElement(enc);
        //    var transport = new HttpsTransportBindingElement
        //    {
        //        MaxReceivedMessageSize = 20000000,
        //        MaxBufferSize = 20000000
        //    };
        //    return new CustomBinding(encoding, transport);
        //}

    }
}
