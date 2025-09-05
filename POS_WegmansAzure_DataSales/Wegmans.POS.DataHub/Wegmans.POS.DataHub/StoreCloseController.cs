using AutoMapper;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.StoreClose.v1;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub
{
    public class StoreCloseController
    {
        private IMapper mapper;

        public StoreCloseController(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public StoreClose GetHubStoreCloseData(XDocument doc, string url)
        {
            string fileName = Path.GetFileNameWithoutExtension(url);
            var outputStoreClose = new StoreClose { Uri = url };
            outputStoreClose.StoreNumber = fileName.getStoreNumber();
            var storeCloseRecords = doc.Root.Elements();

            var source = SerializationUtil<TransactionRecord21>.Deserialize(storeCloseRecords.FirstOrDefault());
            return mapper.Map<TransactionRecord21, StoreClose>(source, outputStoreClose);
        }
    }
}