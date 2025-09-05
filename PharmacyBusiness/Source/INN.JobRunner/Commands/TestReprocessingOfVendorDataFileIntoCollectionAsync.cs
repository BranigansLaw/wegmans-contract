using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.DataFileInterface;
using Library.DataFileInterface.EmailSender;
using Library.DataFileInterface.VendorFileDataModels;
using Microsoft.Extensions.Logging;
using System.Net.Mail;

namespace INN.JobRunner.Commands
{
    public class TestReprocessingOfVendorDataFileIntoCollectionAsync : PharmacyCommandBase
    {
        private readonly IDataFileInterface _dataFileInterface;
        private readonly ILogger<IntegrationTests> _logger;

        public TestReprocessingOfVendorDataFileIntoCollectionAsync(
            IDataFileInterface dataFileInterface,
            ILogger<IntegrationTests> logger,
            IGenericHelper services,
            ICoconaContextWrapper coconaContextWrapper
        ) : base(services, coconaContextWrapper)
        {
            _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// This test covers a scenario where:
        /// (1) The initial job run found data integrity issues with a vendor file so the original data file got moved into the Rejects folder.
        /// (2) Then, lets suppose someone (anyone - could be business team, support team, development team, or Operations) then reran the same job yet again...so the job should find the file in the Rejects folder and rerun with the same data integrity issues.
        /// (3) Then the business team requested a revised file from the vendor.
        /// (4) The vendor provided a corrected file which was then uploaded to the Imports folder.
        /// (5) The job reran and successfully processed the corrected file and the revised file got moved into the Archives folder and the original file in the Rejects folder got deleted.
        /// (6) Then, lets suppose someone (anyone - could be business team, support team, development team, or Operations) then reran the same job yet again...so the job should find the file in the Archives folder and rerun successfully without any issues.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [Command(
            "test-reprocessing-of-vendor-data-file",
            Description = "Run test of reprocessing vendor data file."
        )]
        public async Task RunAsync(RunForParameter r)
        {
            DateOnly runFor = new DateOnly(2024, 4, 1);
            string importPath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileImports");
            string batchJobName = "INN502";
            string testDataFileName = $"IVROutboundReport_{runFor.ToString("yyyyMMdd")}.txt";
            string testDataFileValidContents = @"5555555555|2024-03-31|12:57:38|INVTN| |11111111|0|049|Pickup Reminder
5555555555|2024-03-31|10:02:08|INVTN| |11111112|0|054|Pickup Reminder
5555555555|2024-01-01|12:55:27|INVTN| |11111113|0|049|Pickup Reminder
5555555555|2024-02-02|09:42:18|INVTN| |11111114|0|037|Pickup Reminder
1234567890|2024-03-03|11:36:06|INVTN| |11111115|0|067|Pickup Reminder
0987654321|2024-03-31|11:38:14|INVTN| |11111116|0|075|Pickup Reminder
";
            _logger.LogDebug($"Create original vendor file with some data integrity issues in file named [{testDataFileName}] having [{testDataFileValidContents.Split("\n").Count()}] rows of mock data.");
            using (StreamWriter sw = File.CreateText($"{importPath}/{testDataFileName}"))
            {
                //NOTE: Fudge some data integrity issues.
                sw.Write(testDataFileValidContents
                    .Replace("2024-01-01", "Jan first 2024")
                    .Replace("2024-02-02", "")
                    .Replace("2024-03-03", "")
                    .Replace("067", "")
                    );
            }
            _logger.LogDebug($"Initial job run should find data integrity issues and move original file into the Rejects folder.");
            await TestReadVendorDataFileIntoCollectionAsync(
                batchJobName,
                testDataFileName,
                runFor,
                CancellationToken).ConfigureAwait(false);

            _logger.LogDebug($"Restart job even though the vendor data file has not changed, and see if job grabs vendor data file from Rejects folder.");
            await TestReadVendorDataFileIntoCollectionAsync(
                batchJobName,
                testDataFileName,
                runFor,
                CancellationToken).ConfigureAwait(false);

            _logger.LogDebug($"Create mock revised and corrected vendor file with only good data contents in file named the same as originally [{testDataFileName}] having [{testDataFileValidContents.Split("\n").Count()}] rows of mock data.");
            using (StreamWriter sw = File.CreateText($"{importPath}/{testDataFileName}"))
            {
                sw.Write(testDataFileValidContents);
            }

            _logger.LogDebug($"Restart job with new revised and corrected vendor data file, and see if job grabs vendor new data file from Imports folder rather than the Rejects folder.");
            await TestReadVendorDataFileIntoCollectionAsync(
                batchJobName,
                testDataFileName,
                runFor,
                CancellationToken).ConfigureAwait(false);

            _logger.LogDebug($"Restart job yet again even though it already just reran successfully...see if job grabs vendor data file from Archives folder, then puts it into the Imports folder, runs successfully, then puts same file back into Archives folder.");
            await TestReadVendorDataFileIntoCollectionAsync(
                batchJobName,
                testDataFileName,
                runFor,
                CancellationToken).ConfigureAwait(false);

            _logger.LogDebug($"Finished vendor file reprocessing scenarios.");
        }

        private async Task TestReadVendorDataFileIntoCollectionAsync(
            string batchJobName,
            string testDataFileName,
            DateOnly runFor,
            CancellationToken c)
        {
            _logger.LogInformation($"Starting to read in IVR Outbound Calls file [{testDataFileName}] from Omnicell file.");
            IEnumerable<IvrOutboundCallsRow> ivrOutboundCallsRows = await _dataFileInterface.ReadFileToListAndNotifyExceptionsAsync<IvrOutboundCallsRow>(
                new EmailExceptionComposerImp("INTEGRATION-TESTS", runFor, MailPriority.Low),
                testDataFileName,
                "\n",
                "|",
                true,
                runFor,
                CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Finished reading data file into collection IvrOutboundCallsRow with: {ivrOutboundCallsRows.Count()} rows.");
        }
    }
}
