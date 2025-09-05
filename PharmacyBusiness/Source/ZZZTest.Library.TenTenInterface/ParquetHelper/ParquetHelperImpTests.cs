using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Library.LibraryUtilities.TelemetryWrapper;
using Library.TenTenInterface;
using Library.TenTenInterface.DataModel.UploadRow.Implementation;
using Library.TenTenInterface.ParquetFileGeneration;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ZZZTest.Library.TenTenInterface.ParquetFileGeneration
{
    public class ParquetHelperImpTests
    {
        private readonly ParquetHelperImp _sut;

        public readonly BlobContainerClient _mockBlobContainerClient;
        public readonly TenTenConfig _mockConfig = new();
        public readonly ITelemetryWrapper _mockTelemetryWrapper = Substitute.For<ITelemetryWrapper>();

        public ParquetHelperImpTests()
        {
            IOptions<TenTenConfig> mockOptions = Substitute.For<IOptions<TenTenConfig>>();
            mockOptions.Value.Returns(_mockConfig);

            BlobServiceClient blobServiceClient = Substitute.For<BlobServiceClient>();
            _mockBlobContainerClient = Substitute.For<BlobContainerClient>();
            IAzureClientFactory<BlobServiceClient> mockAzureClientFactory = Substitute.For<IAzureClientFactory<BlobServiceClient>>();
            mockAzureClientFactory
                .CreateClient(TenTenInterfaceConstants.TenTenAzureBlobServiceClientName)
                .Returns(blobServiceClient);
            blobServiceClient
                .GetBlobContainerClient(_mockConfig.AzureBlobEnvironmentFolderName)
                .Returns(_mockBlobContainerClient);

            _sut = new ParquetHelperImp(mockAzureClientFactory, _mockTelemetryWrapper, mockOptions);
        }

        /// <summary>
        /// Tests that <see cref="ParquetHelperImp.GenerateParquetFileAsync{T}(IEnumerable{T}, CancellationToken)"/> returns a non-empty <see cref="Stream"/>
        /// </summary>
        [Fact]
        public async Task GenerateParquetFileAsync_Returns_NonEmptyStream()
        {
            // Arrange
            IEnumerable<TestTenTenRow> testData = Enumerable.Range(0, 1000).Select(i =>
                new TestTenTenRow
                {
                    Id = i,
                    DontWrite = "I won't be written to file",
                    Price = 12.54f,
                    Message = "Hello, TenTen",
                });
            MemoryStream ms = new();

            // Act
            await _sut.WriteParquetToStreamAsync(testData, ms, CancellationToken.None);

            // Assert
            ms.Position = 0;
            string? content = null;
            using (StreamReader sr = new(ms))
            {
                content = await sr.ReadToEndAsync();
            }

            Assert.NotNull(content);
            Assert.NotEmpty(content);

            _mockTelemetryWrapper
                .Received(1)
                .LogTenTenAzureBlobUploadTelemetry(Arg.Any<string>(), Arg.Any<DateTimeOffset>(), Arg.Any<TimeSpan>(), true);
        }

        /// <summary>
        /// Tests that <see cref="ParquetHelperImp.GenerateParquetFileAsync{T}(IEnumerable{T}, CancellationToken)"/> logs the error in <see cref="ITelemetryWrapper"/> when an exception is thrown
        /// </summary>
        [Fact]
        public async Task GenerateParquetFileAsync_LogsError_WhenExceptionThrown()
        {
            // Arrange
            IEnumerable<TestTenTenRow> testData = Enumerable.Range(0, 1000).Select(i =>
                new TestTenTenRow
                {
                    Id = i,
                    DontWrite = "I won't be written to file",
                    Price = 12.54f,
                    Message = "Hello, TenTen",
                });
            MemoryStream ms = new();
            ms.Close();

            // Act
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.WriteParquetToStreamAsync(testData, ms, CancellationToken.None));

            // Assert
            _mockTelemetryWrapper
                .Received(1)
                .LogTenTenAzureBlobUploadTelemetry(Arg.Any<string>(), Arg.Any<DateTimeOffset>(), Arg.Any<TimeSpan>(), false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(null, true)]
        public async Task GetParquetUploadStreamAsync_Returns_CorrectStreamBasedOnConfigSetting(bool? developerMode, bool sendToAzure)
        {
            // Arrange
            _mockConfig.DeveloperMode = developerMode;
            string feedName = Guid.NewGuid().ToString();
            string fileName = Guid.NewGuid().ToString();
            CancellationToken cancellationToken = new();

            BlockBlobClient mockBlockBlobClient = Substitute.For<BlockBlobClient>();
            Stream azureReturnedStream = new MemoryStream();
            if (developerMode != true)
            {
                _mockBlobContainerClient.GetBlockBlobClient($"{feedName.ToUpper()}/{fileName}").Returns(mockBlockBlobClient);
                mockBlockBlobClient.OpenWriteAsync(true, cancellationToken: cancellationToken).Returns(azureReturnedStream);
            }

            // Act
            Stream ret = await _sut.GetParquetUploadStreamAsync(feedName, fileName, cancellationToken);

            // Assert
            if (sendToAzure)
            {
                _mockBlobContainerClient.Received(1).GetBlockBlobClient($"{feedName.ToUpper()}/{fileName}");
                await mockBlockBlobClient.Received(1).OpenWriteAsync(true, cancellationToken: cancellationToken);
                Assert.Equal(azureReturnedStream, ret);
            }
            else
            {
                _mockBlobContainerClient.Received(0).GetBlockBlobClient(Arg.Any<string>());
                await mockBlockBlobClient.Received(0).OpenWriteAsync(Arg.Any<bool>());
                Assert.IsType<BufferedStream>(ret);
            }
        }
    }
}
