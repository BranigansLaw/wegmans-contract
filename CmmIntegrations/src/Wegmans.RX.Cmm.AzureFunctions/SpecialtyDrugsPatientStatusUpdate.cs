using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Wegmans.RX.Cmm.AzureFunctions.Astute;
using Wegmans.RX.Cmm.AzureFunctions.AstuteProxy.Models;
using Wegmans.RX.Cmm.AzureFunctions.Cmm;

namespace Wegmans.RX.Cmm.AzureFunctions.AstuteProxy
{
    public class SpecialtyDrugsPatientStatusUpdate
    {
        //The maximum allowed days in the past that the LastRunDateTimeOffset can acceptably be
        private int daysBack = 4;
        private readonly CmmHttpClient _client;
        private AstuteSoapProxy _astuteSoapProxy;

        public SpecialtyDrugsPatientStatusUpdate(CmmHttpClient client, IAstuteSoapProxy astuteSoapProxy)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _astuteSoapProxy = (AstuteSoapProxy)astuteSoapProxy ?? throw new ArgumentNullException(nameof(astuteSoapProxy));
        }

        [FunctionName(nameof(SpecialtyDrugsPatientStatusUpdate))]
        public async Task Run(
            [TimerTrigger("%SpecialtyDrugsPatientStatusUpdateTimerTrigger%")] TimerInfo myTimer,
            ILogger log,
            [Blob("cmmastuteproxy/specialtyDrugsPatientStatusUpdateSettings.json", System.IO.FileAccess.ReadWrite, Connection = "CmmStorageAccount")] CloudBlockBlob cloudBlockBlob)
        {
            if (!cloudBlockBlob.Exists())
            {
                throw new ArgumentNullException("cloudBlockBlob", "specialtyDrugsPatientStatusUpdateSettings.json does not exist in the storage account. Please create it with today's date as the value for 'LastRunDateTimeOffset'");
            }

            var t = await cloudBlockBlob.DownloadTextAsync().ConfigureAwait(false);

            SpecialtyDrugsPatientStatusUpdateSettings settings = JsonSerializer.Deserialize<SpecialtyDrugsPatientStatusUpdateSettings>(t);

            _ = settings.ActiveRegion ?? throw new ArgumentNullException(nameof(SpecialtyDrugsPatientStatusUpdateSettings.ActiveRegion), "The Active Region must be set in specialtyDrugsPatientStatusUpdateSettings.json");

            if (Environment.GetEnvironmentVariable("REGION_NAME") != settings.ActiveRegion)
            {
                return;
            }

            if (DateTimeOffset.Now.AddDays(-daysBack) > settings.LastRunDateTimeOffset.ToLocalTime())
            {
                throw new InvalidOperationException("specialtyDrugsPatientStatusUpdateSettings.js has a date that it is too far in the past. Please update to last run datetime from control-m.");
            }

            bool shouldUpdateLastRun = true;

            var patientStatusUpdates = await _astuteSoapProxy.GetStatusesAsync(settings.LastRunDateTimeOffset.AddMinutes(-1)).ConfigureAwait(false);

            foreach (var patientStatusUpdate in patientStatusUpdates)
            {
                CancellationToken cancellationToken = new CancellationToken();

                try
                {
                    //call cmm with data
                    await _client.SpecialtyDrugsPatientStatusUpdate(new Cmm.Models.PatientStatus
                    {
                        ServiceRequestEvent = new Cmm.Models.ServiceRequestEvent
                        {
                            SourceIdentifier = patientStatusUpdate.SourceIdentifier,
                            SourceType = patientStatusUpdate.SourceType,
                            TypeName = patientStatusUpdate.TypeName,
                            Data = new Cmm.Models.PatientStatusData
                            {
                                PatientStatus = patientStatusUpdate.PatientStatusCode,
                                LastShipmentDate = patientStatusUpdate.LastShipmentDate,
                                TransferPharmacyNpi = patientStatusUpdate.TransferPharmacyNpi,
                                TransferPharmacyName = patientStatusUpdate.TransferPharmacyName,
                                TransferPharmacyPhoneNumber = patientStatusUpdate.TransferPharmacyPhoneNumber,
                                LinkEnrollmentFlag = patientStatusUpdate.LinkEnrollmentFlag,
                                LinkEnrollmentDate = patientStatusUpdate.LinkEnrollmentDate,
                                ReverificationStartDate = patientStatusUpdate.ReverificationStartDate,
                                PrimaryPbmName = patientStatusUpdate.PrimaryPbmName,
                                PrimaryPbmBin = patientStatusUpdate.PrimaryPbmBin,
                                PrimaryPbmPcn = patientStatusUpdate.PrimaryPbmPcn,
                                PrimaryPbmGroupId = patientStatusUpdate.PrimaryPbmGroupId,
                                PrimaryPbmPlanId = patientStatusUpdate.PrimaryPbmPlanId
                            }
                        }
                    }, cancellationToken).ConfigureAwait(false);
                }
                catch (CmmException ex)
                {
                    log.LogError(ex.Message);
                    shouldUpdateLastRun = false;
                    throw ex;
                }
                catch (TaskCanceledException)
                {
                    log.LogError("Call to CMM Patient Status Update was cancelled (took too long).");
                    shouldUpdateLastRun = false;
                }
            }

            if (shouldUpdateLastRun)
            {
                settings.LastRunDateTimeOffset = DateTimeOffset.Now;

                await cloudBlockBlob.UploadTextAsync(JsonSerializer.Serialize(settings)).ConfigureAwait(false);
            }
        }
    }
}
