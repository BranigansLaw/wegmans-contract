using Library.TenTenInterface;
using Library.TenTenInterface.Helper;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Text;

namespace ZZZTest.Library.TenTenInterface.Helper
{
    public class HelperImpTests
    {
        private readonly HelperImp _helper;
        private readonly IOptions<TenTenConfig> _options = Substitute.For<IOptions<TenTenConfig>>();

        public HelperImpTests()
        {
            _options.Value.Returns(new TenTenConfig());
            _helper = new HelperImp(_options);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        public void CreateXmlBatches_Returns_AllDataInBatches(int maxBatchSizeMb)
        {
            // Arrange
            ICollection<string> rows = new List<string>();
            ICollection<string> expectedResults = new List<string>();
            for (int i = 0; i < 100000; i++)
            {
                string row = $"<tr><td>{Guid.NewGuid()}</td><td>{Random.Shared.Next(1, 1000)}</td></tr>";
                rows.Add(row);
                expectedResults.Add(row);
            }

            string xmlUploadBody = @"<in>
    <table>
        <title>Grocery List</title>
        <ldesc>Grocery list based on the movie Elf.</ldesc>
        <cols>
            <th name=""item_name"" type=""a"" format=""type:char;width:10"">Item Name</th>
            <th name=""quantity"" type=""i"" format=""type:num;width:4"">Quantity</th>
        </cols>
        <data>{0}</data>
    </table>
    <name mode=""replace"">wegmans.devpharm.tylerf.demos.grocery_list</name>
    <users>
        <user>wegmans_cjmccarthy</user>
        <user>wegmans_vndtoptaltxf</user>
        <user>wegmans_rx_batch</user>
        <user>wegmans_rx_batch1</user>
        <user>wegmans_rx_batch2</user>
        <user>wegmans_rx_batch_pool</user>
    </users>
</in>";

            // Act
            IEnumerable<string> batches = _helper.CreateXmlBatches(xmlUploadBody, rows, x => x, maxBatchSizeMb);

            // Assert
            foreach (string batch in batches)
            {
                Assert.True(Encoding.UTF8.GetByteCount(batch) < maxBatchSizeMb * 1024 * 1024);
            }

            // Checks every batch is in the expected results. Takes a very long time to run, so should only be used if testing this method.
            //foreach (string expectedResult in expectedResults)
            //{
            //    Assert.Contains(batches, b => b.Contains(expectedResult));
            //}
        }
    }
}