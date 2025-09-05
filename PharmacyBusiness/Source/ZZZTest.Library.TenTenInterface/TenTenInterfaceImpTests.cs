using Library.LibraryUtilities.EmailSender;
using Library.LibraryUtilities.Extensions;
using Library.LibraryUtilities.GetNow;
using Library.TenTenInterface;
using Library.TenTenInterface.DataModel.UploadRow.Implementation;
using Library.TenTenInterface.Helper;
using Library.TenTenInterface.ParquetFileGeneration;
using Library.TenTenInterface.TenTenApiCallWrapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ZZZTest.Library.TenTenInterface
{
    public class TenTenInterfaceImpTests
    {
        public readonly TenTenInterfaceImp _sut;

        public readonly IParquetHelper _mockParquetHelper = Substitute.For<IParquetHelper>();
        public readonly IEmailNotificationHelper _mockEmailNotificationHelper = Substitute.For<IEmailNotificationHelper>();
        public readonly IHelper _mockHelper = Substitute.For<IHelper>();
        public readonly TenTenConfig _mockConfig = new();
        public readonly ITenTenApiCallWrapper _mockTenTenApiWrapper = Substitute.For<ITenTenApiCallWrapper>();
        public readonly IGetNow _mockGetNow = Substitute.For<IGetNow>();
        public readonly ILogger<TenTenInterfaceImp> _mockLogger = Substitute.For<ILogger<TenTenInterfaceImp>>();

        public TenTenInterfaceImpTests()
        {
            IOptions<TenTenConfig> mockOptions = Substitute.For<IOptions<TenTenConfig>>();
            mockOptions.Value.Returns(_mockConfig);
            _mockConfig.AzureBlobEnvironmentFolderName = "unit_test";
            _mockConfig.ParquetUploadNotificationEmailTo = "sponge_bob";

            _sut = new TenTenInterfaceImp(
                _mockParquetHelper,
                _mockEmailNotificationHelper,
                _mockHelper,
                mockOptions,
                _mockTenTenApiWrapper,
                _mockGetNow,
                _mockLogger);
        }

        [Theory]
        [ClassData(typeof(UploadDataAsyncTestCases))]
        public async Task UploadDataAsync_Calls_AllRelevantDependencies(DateOnly? runDate)
        {
            // Arrange
            IEnumerable<TestTenTenRow> testData = Enumerable.Range(0, 10).Select(i =>
                new TestTenTenRow
                {
                    Id = i,
                    DontWrite = "I won't be written to file",
                    Price = 12.54f,
                    Message = "Hello, TenTen",
                });

            DateTimeOffset now = new(2025, 1, 10, 9, 52, 34, TimeSpan.Zero);
            _mockGetNow.GetNowEasternStandardTime().Returns(now);

            string feedName = testData.First().FeedName;
            string runDatePortionOfFeedName = "";
            if (runDate.HasValue)
            {
                runDatePortionOfFeedName = runDate.Value.ToString("yyyyMMdd");
            }
            else
            {
                runDatePortionOfFeedName = now.ToDateOnly().ToString("yyyyMMdd");
            }

            string expectedFileName = $"{feedName}_{runDatePortionOfFeedName}_{now:yyyyMMdd}_{now:HHmmss}.parquet";
            CancellationToken c = new();

            Stream returnedStreamFromParquetHelper = new MemoryStream();
            _mockParquetHelper.GetParquetUploadStreamAsync(feedName, expectedFileName, c).Returns(returnedStreamFromParquetHelper);

            // Act
            await _sut.UploadDataAsync(testData, c, runDate);

            // Assert
            await _mockParquetHelper.Received(1).GetParquetUploadStreamAsync(feedName, expectedFileName, c);
            await _mockParquetHelper.Received(1).WriteParquetToStreamAsync(testData, returnedStreamFromParquetHelper, c);

            await _mockEmailNotificationHelper.Received(1).NotifyOfParquetUploadAsync(
                _mockConfig.AzureBlobEnvironmentFolderName,
                feedName,
                DateOnly.ParseExact(runDatePortionOfFeedName, "yyyyMMdd"),
                testData.Count(),
                _mockConfig.ParquetUploadNotificationEmailTo,
                c);
        }

        public class UploadDataAsyncTestCases : TheoryData<DateOnly?>
        {
            public UploadDataAsyncTestCases()
            {
                AddRow(new DateOnly(2025, 1, 9));
                AddRow((DateOnly?)null);
            }
        }
    }
}
