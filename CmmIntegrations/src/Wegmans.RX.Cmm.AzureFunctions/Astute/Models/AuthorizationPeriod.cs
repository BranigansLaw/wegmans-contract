using System;
using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    [ExcludeFromCodeCoverage]
    public class AuthorizationPeriod
    {
        public DateTimeOffset? EffectiveDate { get; set; }

        public DateTimeOffset? ExpirationDate { get; set; }
    }
}
