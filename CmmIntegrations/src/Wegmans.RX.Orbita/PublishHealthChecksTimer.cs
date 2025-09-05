using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wegmans.RX.Orbita
{
    public class PublishHealthChecksTimer
    {
        private readonly HealthCheckService _service;
        private readonly IHealthCheckPublisher[] _publishers;

        public PublishHealthChecksTimer(HealthCheckService service, IEnumerable<IHealthCheckPublisher> publishers)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _publishers = publishers?.ToArray() ?? Array.Empty<IHealthCheckPublisher>();
        }

        //NOTE: RunOnStartup Should not normally be used for a non Health Check TimerTrigger. We have this on so that when deployment is done a check is run out of band from the timer (i.e. good to know deploy works)
        [FunctionName(nameof(PublishHealthChecksTimer))]
        public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log, CancellationToken ct = default)
        {
            _ = myTimer ?? throw new ArgumentNullException(nameof(myTimer));
            var report = await _service.CheckHealthAsync().ConfigureAwait(false);
            await Task.WhenAll(_publishers.Select(x => x.PublishAsync(report, ct))).ConfigureAwait(false);
        }
    }
}