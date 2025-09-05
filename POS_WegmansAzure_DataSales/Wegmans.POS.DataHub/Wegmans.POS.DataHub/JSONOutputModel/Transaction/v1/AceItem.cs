using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1
{
    public class AceItem : Item
    {
        [JsonIgnore]
        public HashSet<string> ProcessedStrings { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
    }
}
