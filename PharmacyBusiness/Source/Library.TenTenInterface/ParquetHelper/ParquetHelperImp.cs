using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Library.LibraryUtilities.TelemetryWrapper;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Parquet;
using Parquet.Serialization;
using System.IO.Compression;

namespace Library.TenTenInterface.ParquetFileGeneration
{
    public class ParquetHelperImp : IParquetHelper
    {
        private readonly IAzureClientFactory<BlobServiceClient> _azureClientFactory;
        private readonly ITelemetryWrapper _telemetryWrapper;
        private readonly IOptions<TenTenConfig> _config;

        public ParquetHelperImp(
            IAzureClientFactory<BlobServiceClient> blobClientFactory,
            ITelemetryWrapper telemetryWrapper,
            IOptions<TenTenConfig> config)
        {
            _telemetryWrapper = telemetryWrapper ?? throw new ArgumentNullException(nameof(telemetryWrapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _azureClientFactory = blobClientFactory ?? throw new ArgumentNullException(nameof(blobClientFactory));
        }

        /// <inheritdoc />
        public async Task<Stream> GetParquetUploadStreamAsync(string feedName, string fileName, CancellationToken c)
        {
            if (_config.Value.DeveloperMode == true)
            {
                return new BufferedStream(File.Create(fileName));
            }
            else
            {
                BlockBlobClient client = _azureClientFactory
                    .CreateClient(TenTenInterfaceConstants.TenTenAzureBlobServiceClientName)
                    .GetBlobContainerClient(_config.Value.AzureBlobEnvironmentFolderName)
                    .GetBlockBlobClient($"{feedName.ToUpper()}/{fileName}");

                return await client.OpenWriteAsync(true, cancellationToken: c).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task WriteParquetToStreamAsync<T>(IEnumerable<T> toSerialize, Stream writeSteam, CancellationToken c)
        {
            bool success = false;
            DateTimeOffset queryStartTime = DateTimeOffset.Now;
            try
            {
                await ParquetSerializer.SerializeAsync(
                    objectInstances: toSerialize,
                    destination: writeSteam,
                    options: new ParquetSerializerOptions
                    {
                        CompressionLevel = CompressionLevel.NoCompression,
                        PropertyNameCaseInsensitive = true,
                        CompressionMethod = CompressionMethod.None,
                    },
                    cancellationToken: c).ConfigureAwait(false);

                success = true;
            }
            finally
            {
                _telemetryWrapper.LogTenTenAzureBlobUploadTelemetry($"Uploading {toSerialize.Count()} of type {typeof(T).FullName}", queryStartTime, DateTimeOffset.Now - queryStartTime, success);
            }
        }
    }
}
