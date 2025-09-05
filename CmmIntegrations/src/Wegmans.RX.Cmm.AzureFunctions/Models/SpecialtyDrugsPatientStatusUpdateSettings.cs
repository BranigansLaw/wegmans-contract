using System;

namespace Wegmans.RX.Cmm.AzureFunctions.AstuteProxy.Models
{
    public class SpecialtyDrugsPatientStatusUpdateSettings
    {
        public DateTimeOffset LastRunDateTimeOffset { get; set; }

        public string ActiveRegion { get; set; }
    }
}
