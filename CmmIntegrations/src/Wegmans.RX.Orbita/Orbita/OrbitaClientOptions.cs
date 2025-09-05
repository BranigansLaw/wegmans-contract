using System;

namespace Wegmans.RX.Orbita.Orbita
{
    public class OrbitaClientOptions
    {
        public static string SectionName { get; } = "OrbitaClient";

        public string ServerAddress { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Scopes { get; set; }

        public string GrantType { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        public TimeSpan RetryAttempt { get; set; } = TimeSpan.FromSeconds(10);
        public string ApiKey { get; set; }
    }
}