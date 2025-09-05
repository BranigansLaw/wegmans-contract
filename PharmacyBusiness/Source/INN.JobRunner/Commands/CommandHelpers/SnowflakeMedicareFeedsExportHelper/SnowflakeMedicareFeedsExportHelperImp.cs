using Library.LibraryUtilities.DataFileWriter;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeMedicareFeedsExportHelper
{
    public class SnowflakeMedicareFeedsExportHelperImp : ISnowflakeMedicareFeedsExportHelper
    {
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly IDataFileWriter _dataFileWriter;
        private readonly ILogger<SnowflakeMedicareFeedsExportHelperImp> _logger;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;

        public SnowflakeMedicareFeedsExportHelperImp(
            ISnowflakeInterface snowflakeInterface,
            IDataFileWriter dataFileWriter,
            ILogger<SnowflakeMedicareFeedsExportHelperImp> logger,
            IOptions<SnowflakeDataOutputDirectories> snowflakeDataOutputDirectoriesOptions)
        {
            _snowflakeInterface = snowflakeInterface ?? throw new ArgumentNullException(nameof(snowflakeInterface));
            _dataFileWriter = dataFileWriter ?? throw new ArgumentNullException(nameof(dataFileWriter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _snowflakeDataOutputDirectoriesOptions = snowflakeDataOutputDirectoriesOptions ?? throw new ArgumentNullException(nameof(snowflakeDataOutputDirectoriesOptions));
        }

        /// <inheritdoc />
        public async Task ExportFdsPrescriptionsAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogInformation("Gathering Snowflake FDS Prescriptions data");
            IEnumerable<FdsPrescriptionsRow> fdsPharmaciesData = await _snowflakeInterface.QuerySnowflakeAsync(
                new FdsPrescriptionsQuery
                {
                    RunDate = runFor
                }, c);

            _logger.LogInformation($"Returned {fdsPharmaciesData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN545_FdsPrescriptions.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(fdsPharmaciesData, new DataFileWriterConfig<FdsPrescriptionsRow>
            {
                Header = "Prescribernpi|Patientmbi|Npi|Ndc|Firstname|Lastname|Addressline1|Addressline2|City|State|Zipcode|Authorizedrefills|Bin|Clientpatientid|Copay|Dayssupply|Dob|Email|Filldate|Fillnum|Gender|Language|Groupid|Pcn|Phone|Phonetype|Plan|Productname|Qty|Reimbursement|Version",
                WriteDataLine = (FdsPrescriptionsRow c) => $"{c.Prescribernpi}|{c.Patientmbi}|{c.Npi}|{c.Ndc}|{c.Firstname}|{c.Lastname}|{c.Addressline1}|{c.Addressline2}|{c.City}|{c.State}|{c.Zipcode}|{c.Authorizedrefills}|{c.Bin}|{c.Clientpatientid}|{c.Copay}|{c.Dayssupply}|{c.Dob}|{c.Email}|{c.Filldate}|{c.Fillnum}|{c.Gender}|{c.Language}|{c.Groupid}|{c.Pcn}|{c.Phone}|{c.Phonetype}|{c.Plan}|{c.Productname}|{c.Qty}|{c.Reimbursement}|{c.Version}",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc />
        public async Task ExportFdsPharmaciesAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogInformation("Gathering Snowflake FDS Pharmacies data");
            IEnumerable<FdsPharmaciesRow> fdsPharmaciesData = await _snowflakeInterface.QuerySnowflakeAsync(
                new FdsPharmaciesQuery
                {
                    RunDate = runFor
                }, c);

            _logger.LogInformation($"Returned {fdsPharmaciesData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN545_FdsPharmacies.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(fdsPharmaciesData, new DataFileWriterConfig<FdsPharmaciesRow>
            {
                Header = "NcpdpId|Npi|PharmacyName|AddressLine1|AddressLine2|City|State|ZipCode|StorePhoneNumber|FaxPhoneNumber|Banner",
                WriteDataLine = (FdsPharmaciesRow c) => $"{c.NcpdpId}|{c.Npi}|{c.PharmacyName}|{c.AddressLine1}|{c.AddressLine2}|{c.City}|{c.State}|{c.ZipCode}|{c.StorePhoneNumber}|{c.FaxPhoneNumber}|{c.Banner}",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc />
        public async Task ExportHpOnePharmaciesAsync(CancellationToken c)
        {
            _logger.LogInformation("Gathering Snowflake Hp One Pharmacy data");
            IEnumerable<HpOnePharmaciesRow> HpOnePharmData = await _snowflakeInterface.QuerySnowflakeAsync(
                new HpOnePharmaciesQuery(), c);

            _logger.LogInformation($"Returned {HpOnePharmData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN545_HpOnePharmacies.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(HpOnePharmData, new DataFileWriterConfig<HpOnePharmaciesRow>
            {
                Header = "Npi|PharmacyName|AddressLine1|AddressLine2|City|State|ZipCode|StorePhoneNumber|FaxPhoneNumber|PaidSearchRadius|Banner",
                WriteDataLine = (HpOnePharmaciesRow c) => $"{c.Npi}|{c.PharmacyName}|{c.AddressLine1}|{c.AddressLine2}|{c.City}|{c.State}|{c.ZipCode}|{c.StorePhoneNumber}|{c.FaxPhoneNumber}|{c.PaidSearchRadius}|{c.Banner}",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc />
        public async Task ExportHpOnePrescriptionsExport(DateOnly runFor, CancellationToken c)
        {
            _logger.LogInformation("Gathering Snowflake DurConflict data");
            IEnumerable<WegmansHPOnePrescriptionsRow> hpOnePercriptionsData = await _snowflakeInterface.QuerySnowflakeAsync(
                new WegmansHPOnePrescriptionsQuery
                {
                    RunDate = runFor
                }, c);

            _logger.LogInformation($"Returned {hpOnePercriptionsData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN545_HpOnePerscriptions.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(hpOnePercriptionsData, new DataFileWriterConfig<WegmansHPOnePrescriptionsRow>
            {
                Header = "AddressLine1|AddressLine2|AuthorizedRefills|Bin|City|ClientPatientId|DaysSupply|Dob|Email|FillDate|FillNum|FirstName|Gender|GroupId|Language|LastName|MedicareContractId|Medicared|MedicareId|MedicarePlanId|Ndc|Npi|Pcn|Phone|PhoneType|Plan|PrescriberNpi|ProductName|Qty|State|Version|Zipcode",
                WriteDataLine = (WegmansHPOnePrescriptionsRow c) => $"{c.AddressLine1}|{c.AddressLine2}|{c.AuthorizedRefills}|{c.Bin}|{c.City}|{c.ClientPatientId}|{c.DaysSupply}|{c.Dob}|{c.Email}|{c.FillDate}|{c.FillNum}|{c.FirstName}|{c.Gender}|{c.GroupId}|{c.Language}|{c.LastName}|{c.MedicareContractId}|{c.Medicared}|{c.MedicareId}|{c.MedicarePlanId}|{c.Ndc}|{c.Npi}|{c.Pcn}|{c.Phone}|{c.PhoneType}|{c.Plan}|{c.PrescriberNpi}|{c.ProductName}|{c.Qty}|{c.State}|{c.Version}|{c.Zipcode}",
                OutputFilePath = writePath,
            });
        }
    }
}
