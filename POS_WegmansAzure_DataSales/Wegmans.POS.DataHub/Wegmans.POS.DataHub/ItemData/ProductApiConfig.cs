using System;

namespace Wegmans.POS.DataHub.ItemData
{
    public class ProductApiConfig
    {
        public const string Category = "ProductApi";

        public string BaseAddress { get; set; }
        public string ApiKey { get; set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        public int MaxRetryCount { get; set; } = 3;
        public int RetryBackOffPower { get; set; } = 2;
    }
}
