using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Library.LibraryUtilities
{
    public class LibraryUtilitiesInterfaceImp : ILibraryUtilitiesInterface
    {
        private readonly IOptions<LibraryUtilitiesConfig> _config;
        private readonly ILogger<LibraryUtilitiesInterfaceImp> _logger;

        public LibraryUtilitiesInterfaceImp(
            IOptions<LibraryUtilitiesConfig> config,
            ILogger<LibraryUtilitiesInterfaceImp> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
