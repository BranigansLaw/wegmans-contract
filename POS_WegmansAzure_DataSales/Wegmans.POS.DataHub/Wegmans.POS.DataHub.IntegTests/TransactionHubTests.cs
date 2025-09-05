using Wegmans.POS.DataHub.IntegTests.Helpers;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Newtonsoft.Json;

namespace Wegmans.POS.DataHub.IntegTests
{
    public class TransactionHubTests
    {
        [Fact]
        public async Task WhenFileIsAddedToRawTLog_ItIsProcessedIntoTransactionContainerAsync()
        {
            BlobUtility.DeleteFile("transactions", "2023/10/09/12/63_0500_0001.json");

            await BlobUtility.AddBlobToRawTlog("0063_0500_0001_1986867926664_transaction.xml");

            bool fileIsProcessed = await BlobUtility.WaitUntilBlobsAreProccessed("transactions", "v1/2023/10/09/12/63_0500_0001.json");

            fileIsProcessed.Should().BeTrue("Transaction log was not processed");
        }

        [Fact]
        public async Task WhenTransactionHasARefund_ItShowsANegativeValueInProcessedJson()
        {
            BlobUtility.DeleteFile("transactions", "v1/2023/09/22/13/104_0019_0039.json");

            await BlobUtility.AddBlobToRawTlog("0104_0019_0039_1695403041001_transaction.xml"); //File contains refund in TransactionRecord
            await BlobUtility.WaitUntilBlobsAreProccessed("transactions", "v1/2023/09/22/13/104_0019_0039.json");

            string transactionJson = await BlobUtility.ReadBlob("transactions", "v1/2023/09/22/13/104_0019_0039.json");
            Transaction transaction = JsonConvert.DeserializeObject<Transaction>(transactionJson);

            List<TenderExchange> tenderExchange = transaction.TenderExchanges.ToList();

            tenderExchange.Should().HaveCount(2).And.ContainEquivalentOf(new
            {
                TenderType = "Cash",
                TenderAmount = -11.52
            });
        }

        [Fact]
        public async Task WhenTransactionHasA0Amount_ItShowsA0ValueInProcessedJson()
        {
            BlobUtility.DeleteFile("transactions", "v1/2023/09/04/08/18_0046_0010.json");

            await BlobUtility.AddBlobToRawTlog("0018_0046_0010_1693830764660_transaction.xml"); //File contains 0 tender amount in TransactionRecord
            await BlobUtility.WaitUntilBlobsAreProccessed("transactions", "v1/2023/09/04/08/");

            string transactionJson = await BlobUtility.ReadBlob("transactions", "v1/2023/09/04/08/18_0046_0010.json");
            Transaction transaction = JsonConvert.DeserializeObject<Transaction>(transactionJson);

            List<TenderExchange> tenderExchange = transaction.TenderExchanges.ToList();

            tenderExchange.Should().HaveCount(2).And.ContainEquivalentOf(new
            {
                TenderType = "Cash",
                TenderAmount = 0
            });
        }
    }
}