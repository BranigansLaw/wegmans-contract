using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class AuthorizationOptions
    {
        public List<string> Audience { get; set; }

        public string TenantId { get; set; } = "1318d57f-757b-45b3-b1b0-9b3c3842774f";

        public string Authority { get { return $"https://sts.windows.net/{TenantId}/"; } }
        
        public List<string> ValidIssuers { get { return new List<string> { Authority }; } }
    }
}
