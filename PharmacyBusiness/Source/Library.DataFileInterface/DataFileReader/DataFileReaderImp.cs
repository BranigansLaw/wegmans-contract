using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Net.Sockets;

namespace Library.DataFileInterface.DataFileReader
{
    public class DataFileReaderImp : IDataFileReader
    {
        private readonly IOptions<DataFileConfig> _config;
        private readonly ILogger<DataFileInterfaceImp> _logger;

        /// <summary>
        /// Retry logic
        /// </summary>
        private readonly AsyncRetryPolicy RetryPolicy;

        public DataFileReaderImp(
            IOptions<DataFileConfig> config,
            ILogger<DataFileInterfaceImp> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            RetryPolicy = Policy
                .Handle<IOException>()
                .Or<SocketException>() // Network error
                .WaitAndRetryAsync(
                    retryCount: 3,
                    retryAttempt => TimeSpan.FromSeconds(30 * retryAttempt), // retry after 30s, 60s, 90s
                    onRetry: (ex, waitTime) => _logger.LogWarning(ex, $"Retrying in {waitTime.Seconds} seconds")
                );
        }

        /// <inheritdoc />
        public Task<string> ReadDataFileAsync(
            string dataFileName,
            CancellationToken c)
        {
            return RetryPolicy.ExecuteAsync(() =>
            {
                if (File.Exists($"{_config.Value.InputFileLocation}/{dataFileName}") == false)
                {
                    if (File.Exists($"{_config.Value.ArchiveFileLocation}/{dataFileName}") == true)
                    {
                        File.Copy(Path.Combine(_config.Value.ArchiveFileLocation, dataFileName), Path.Combine(_config.Value.InputFileLocation, dataFileName));
                        _logger.LogDebug($"File {dataFileName} not found in imports folder, but was found in archive folder, so copied file from archive to imports folder for this rerun.");
                    }
                    else
                    {
                        if (File.Exists($"{_config.Value.RejectFileLocation}/{dataFileName}") == true)
                        {
                            File.Copy(Path.Combine(_config.Value.RejectFileLocation, dataFileName), Path.Combine(_config.Value.InputFileLocation, dataFileName));
                            _logger.LogDebug($"File {dataFileName} not found in imports folder, and not found in archive folder, but was found in rejects folder, so copied file from rejects to imports folder for this rerun.");
                        }
                    }
                }

                if (File.Exists($"{_config.Value.InputFileLocation}/{dataFileName}") == true)
                    return File.ReadAllTextAsync($"{_config.Value.InputFileLocation}/{dataFileName}", c);
                else
                    throw new FileNotFoundException($"File not found: {_config.Value.InputFileLocation}/{dataFileName}");
            });
        }
    }
}
