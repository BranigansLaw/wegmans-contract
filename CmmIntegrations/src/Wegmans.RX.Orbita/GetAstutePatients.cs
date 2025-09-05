using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.RX.Orbita.Astute;
using Wegmans.RX.Orbita.Orbita;

namespace Wegmans.RX.Orbita
{
    public class GetAstutePatients
    {
        private readonly AstuteSoapProxy _astuteSoapProxy;
        private static IMapper _mapper;

        public GetAstutePatients(IAstuteSoapProxy astuteSoapProxy, IMapper mapper)
        {
            _astuteSoapProxy = (AstuteSoapProxy)astuteSoapProxy ?? throw new ArgumentNullException(nameof(astuteSoapProxy));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [FunctionName(nameof(GetAstutePatients))]
        public async Task RunAsync(
            [TimerTrigger("%GetAstutePatientsTimerTrigger%")] TimerInfo myTimer,
            [Queue(queueName: OrbitaQueueBindingSettings.PatientsToProcessQueueName, Connection = OrbitaQueueBindingSettings.StorageAccountConnection)] IAsyncCollector<PatientToOrbitaObject> patientToOrbitaQueue,
            ILogger log,
            CancellationToken ct)
        {
            var patients = await _astuteSoapProxy.GetPatientsAsync().ConfigureAwait(false);
            log.LogInformation($"Retrieved {patients.Count} records from Astute");
            foreach (var patient in patients)
            {
                var patientValidator = new PatientValidator();
                if (patientValidator.Validate(patient).IsValid)
                {
                    var patientToOrbita = _mapper.Map<PatientToOrbitaObject>(patient);
                    await patientToOrbitaQueue.AddAsync(patientToOrbita, ct).ConfigureAwait(false);
                    await _astuteSoapProxy.UpdatePatientAsync(patient.AddressId).ConfigureAwait(false);
                }
                else
                {
                    await _astuteSoapProxy.UpdatePatientAsync(patient.AddressId).ConfigureAwait(false);
                    log.LogError($"Transaction ID {patient.AddressId} is missing one or more required fields");
                    throw new AstuteException($"Transaction {patient.AddressId} is missing one or more required fields. Check app insights for details.");
                    //TODO: create incident (bicep)
                }
            }
        }
    }
}