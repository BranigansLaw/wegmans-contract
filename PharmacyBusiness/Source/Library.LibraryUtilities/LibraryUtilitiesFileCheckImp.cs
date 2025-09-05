using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Library.LibraryUtilities
{
    public class LibraryUtilitiesFileCheckImp : ILibraryUtilitiesFileCheckInterface
    {
        private readonly IOptions<LibraryUtilitiesConfig> _config;
        private readonly ILogger<LibraryUtilitiesFileCheckImp> _logger;

        public LibraryUtilitiesFileCheckImp(
            IOptions<LibraryUtilitiesConfig> config,
            ILogger<LibraryUtilitiesFileCheckImp> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public void CopyFileToArchiveForQA(string sourceNameAndPath)
        {
            File.Copy(Path.Combine(_config.Value.OutputFileLocation, sourceNameAndPath), Path.Combine(_config.Value.ArchiveForQaFileLocation, sourceNameAndPath), true);
        }

        /// <inheritdoc />
        public void MoveFileToArchiveForQA(string sourceNameAndPath)
        {
            File.Move(Path.Combine(_config.Value.OutputFileLocation, sourceNameAndPath), Path.Combine(_config.Value.ArchiveForQaFileLocation, sourceNameAndPath), true);
        }

    }
}
