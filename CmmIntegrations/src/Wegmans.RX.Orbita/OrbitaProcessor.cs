using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Wegmans.RX.Orbita.Orbita;

namespace Wegmans.RX.Orbita
{
    public class OrbitaProcessor
    {
        private readonly OrbitaHttpClient _client;
        private static IMapper _mapper;
        
        public OrbitaProcessor(OrbitaHttpClient client, IMapper mapper)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Send patient information to Orbita
        /// </summary>
        /// <param name="patientToProcessQueueItem"></param>
        /// <param name="log"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <remarks>The JSON definition allows for multiple patients per packet. However only 1 is sent currently.</remarks>
        [FunctionName(nameof(OrbitaProcessor))]
        public async Task RunAsync(
            [QueueTrigger(OrbitaQueueBindingSettings.PatientsToProcessQueueName, Connection = OrbitaQueueBindingSettings.StorageAccountConnection)] PatientToOrbitaObject patientToProcessQueueItem,
            ILogger log,
            CancellationToken ct)
        {
            try
            {
                var orbitaPayload = new OrbitaJsonPayload()
                {
                    Patients = new List<OrbitaPatient>()
                    {
                        _mapper.Map<OrbitaPatient>(patientToProcessQueueItem)
                    }
                };

                List<OrbitaPatient> patientReturn = await _client.RemovePatient(orbitaPayload).ConfigureAwait(false);

                foreach(OrbitaPatient patient in patientReturn)
                    log.LogInformation($"Transaction {patientToProcessQueueItem.TransactionId} processed. Result: {patient.Result}");
            }
            catch (Exception ex)
            {
                log.LogError("Exception in OrbitaProcessor:RunAsync() - {ExceptionName}. TransID: {TransID}", ex.Message, patientToProcessQueueItem.TransactionId);
                throw;
            }
        }
    }
}