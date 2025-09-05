using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wegmans.RX.Cmm.AzureFunctions
{
    public class CustomTelemetryProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        // Link processors to each other in a chain.
        public CustomTelemetryProcessor(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        public void Process(ITelemetry item)
        {
            // Exclude Health check
            var request = item as RequestTelemetry;
            if (request != null)
            {
                if (request != null && request.Name == "HealthProbe")
                {
                    return;
                }
            }
            Next.Process(item);
        }
    }
}
