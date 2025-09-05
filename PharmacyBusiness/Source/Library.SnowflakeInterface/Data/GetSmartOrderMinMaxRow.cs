using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.SnowflakeInterface.Data
{
    public class GetSmartOrderMinMaxRow
    {
        public required string? StoreNumber { get; set; }
        public string? NdcWo { get; set; }
        public long? MinQtyOverride { get; set; }
        public long? MaxQtyOverride { get; set; }
        public string? PurchasePlan { get; set; }
        public long? LastUpdated { get; set; }

    }
}
