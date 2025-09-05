using System.Text.Json.Serialization;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1
{
    public class AceCustomArrangementItem : CustomArrangementItem
    {
        [JsonIgnore]
        public string UniversalProductCode { get; set; }
        
    }
}