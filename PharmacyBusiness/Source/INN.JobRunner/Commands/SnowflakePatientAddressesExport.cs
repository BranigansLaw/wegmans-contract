using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.LibraryUtilities.DataFileWriter;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace INN.JobRunner.Commands
{
    public class SnowflakePatientAddressesExport : PharmacyCommandBase
    {
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly IDataFileWriter _dataFileWriter;
        private readonly ILogger<SnowflakeDeceasedExport> _logger;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;

        public SnowflakePatientAddressesExport(
            ISnowflakeInterface snowflakeInterface,
            IDataFileWriter dataFileWriter,
            ILogger<SnowflakeDeceasedExport> logger,
            IOptions<SnowflakeDataOutputDirectories> snowflakeDataOutputDirectoriesOptions,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _snowflakeInterface = snowflakeInterface ?? throw new ArgumentNullException(nameof(snowflakeInterface));
            _dataFileWriter = dataFileWriter ?? throw new ArgumentNullException(nameof(dataFileWriter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _snowflakeDataOutputDirectoriesOptions = snowflakeDataOutputDirectoriesOptions ?? throw new ArgumentNullException(nameof(snowflakeDataOutputDirectoriesOptions));
        }

        [Command(
            "snowflake-patient-addresses-export",
            Description = "Runs snowflake query, Patient_Addresses_YYYYMMDD.sql",
            Aliases = ["INN619"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("Gathering PatientAddresses");
            IEnumerable<PatientAddressesRow> patientAddressesData =
                await _snowflakeInterface.QuerySnowflakeAsync(new PatientAddressesQuery
                {
                    RunDate = runFor.RunFor,
                }, CancellationToken);

            _logger.LogInformation($"Returned {patientAddressesData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN619.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(patientAddressesData, new DataFileWriterConfig<PatientAddressesRow>
            {
                Header = "PatientNum|PadrKey|County|AddrCity|AddressOne|AddressTwo|AddressType|AddressUpdated|AddressUsage|AddressZipext|AddrState|AddrStatus|AddrZipcode|AreaCode|Dwaddressnum|Dwphonenum|PhoneExt|PhoneNum|PhoneUpdated",
                WriteDataLine = (PatientAddressesRow c) => $"{c.PatientNum}|{c.PadrKey}|{c.County}|{c.AddrCity}|{c.AddressOne}|{c.AddressTwo}|{c.AddressType}|{c.AddressUpdated}|{c.AddressUsage}|{c.AddressZipext}|{c.AddrState}|{c.AddrStatus}|{c.AddrZipcode}|{c.AreaCode}|{c.Dwaddressnum}|{c.Dwphonenum}|{c.PhoneExt}|{c.PhoneNum}|{c.PhoneUpdated}",
                OutputFilePath = writePath,
            });
        }
    }
}
