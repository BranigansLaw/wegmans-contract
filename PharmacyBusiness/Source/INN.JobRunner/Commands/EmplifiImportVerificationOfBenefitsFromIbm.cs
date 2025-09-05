using Cocona;
using INN.JobRunner.Commands.CommandHelpers.EmplifiImportVerificationOfBenefitsFromIbmHelper;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.DataFileInterface;
using Library.EmplifiInterface;
using Library.EmplifiInterface.EmailSender;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Text;

namespace INN.JobRunner.Commands;

public class EmplifiImportVerificationOfBenefitsFromIbm : PharmacyCommandBase
{
    private readonly IEmplifiInterface _emplifiInterface;
    private readonly IDataFileInterface _dataFileInterface;
    private readonly ILogger<EmplifiImportVerificationOfBenefitsFromIbm> _logger;
    private readonly IHelper _helper;

    public EmplifiImportVerificationOfBenefitsFromIbm(
        IEmplifiInterface emplifiInterface,
        IDataFileInterface dataFileInterface,
        ILogger<EmplifiImportVerificationOfBenefitsFromIbm> logger,
        IHelper helper,
        IGenericHelper genericHelper,
        ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
    {
        _emplifiInterface = emplifiInterface ?? throw new ArgumentNullException(nameof(emplifiInterface));
        _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _helper = helper ?? throw new ArgumentNullException(nameof(helper));
    }

    private const string BatchJobName = "INN623";
    [Command(
        "emplifi-import-verification-of-benefits-from-ibm",
        Description = "Import Verification of Benefits from IBM. Control-M job INN623",
        Aliases = [BatchJobName]
    )]
    public async Task RunAsync(RunForFileNameParameter runForFileName)
    {
        _logger.LogInformation($"Starting to import Verification of Benefits from IBM data file to Emplifi.");

        

        DateOnly runFor = DateOnly.FromDateTime(DateTime.Now);
        string emailSubject = $"{BatchJobName} IBM CarePath VOB Reconciliation";

        // If a file name is specified, only process that file. Otherwise, process all files in the input directory matching the specified file name pattern
        IEnumerable<string> fileNames;
        if (string.IsNullOrWhiteSpace(runForFileName.RunFor))
        {
            fileNames = _dataFileInterface.GetFileNames("CarePath_VOB_*.txt");
        }
        else
        {
            fileNames = [runForFileName.RunFor];
        }

        var businessLogicValues = await _helper.ProcessFilesAndBuildReportEmailAsync(fileNames, CancellationToken).ConfigureAwait(false);
        StringBuilder? emailBody = businessLogicValues.Item1;
        int dataFileCount = businessLogicValues.Item2;

        if (emailBody is null)
        {
            emailBody = new();
        }
        
        if (dataFileCount == 0)
        {
            emailBody.AppendLine($"<br /><br />File: No files found.");
        }

        _emplifiInterface.SendVobNotification(
            new EmailExceptionComposerImp(BatchJobName, DateTime.Now, MailPriority.Normal),
            emailSubject,
            emailBody.ToString());
        
        _logger.LogInformation($"Finished importing Verification of Benefits triage data to Emplifi");
    }
}