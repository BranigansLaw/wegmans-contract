using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Wegmans.RX.Cmm.AzureFunctions
{
    public class HealthProbe
    {
        [FunctionName(nameof(HealthProbe))]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "head")] HttpRequest req,
            ILogger log)
        {
            return new OkResult();
        }
    }
}
