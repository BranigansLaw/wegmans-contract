using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Utility;
using Library.LibraryUtilities;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenImportVaccineAppointmentBookingFromOmnicellFile : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly ILibraryUtilitiesInterface _libraryInterface;
        private readonly IUtility _utility;
        private readonly ILogger<TenTenImportVaccineAppointmentBookingFromOmnicellFile> _logger;

        public TenTenImportVaccineAppointmentBookingFromOmnicellFile(
            ITenTenInterface tenTenInterface,
            ILibraryUtilitiesInterface libraryInterface,
            IUtility utility,
            ILogger<TenTenImportVaccineAppointmentBookingFromOmnicellFile> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _libraryInterface = libraryInterface ?? throw new ArgumentNullException(nameof(libraryInterface));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "import-vaccine-appointment-booking-from-omnicell-file", 
            Description = "Import Vaccine Appointment Booking from Omnicell file and send to TenTen. Control-M job INN521", 
            Aliases = ["INN521"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("UNDER CONSTRUCTION");
            await Task.CompletedTask; //To be deleted after implementation.
        }
    }
}
