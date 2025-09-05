using INN.JobRunner;
using INN.JobRunner.Commands;
using INN.JobRunner.Commands.CommandHelpers.EmplifiImportVerificationOfBenefitsFromIbmHelper;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.DataFileInterface;
using Library.EmplifiInterface;
using Library.EmplifiInterface.DataModel;
using Library.LibraryUtilities.DataFileWriter;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Text;
using System.Linq;
using ZZZTest.INN.JobRunner.Commands.CommandHelpers;
using Library.EmplifiInterface.EmailSender;
namespace ZZZTest.INN.JobRunner.Commands;

public class EmplifiImportVerificationsOfBenefitsFromIbmTests : PharmacyCommandBaseTests
{
    private readonly EmplifiImportVerificationOfBenefitsFromIbm _sut;

    private readonly ILogger<EmplifiImportVerificationOfBenefitsFromIbm> _mockLogger = Substitute.For<ILogger<EmplifiImportVerificationOfBenefitsFromIbm>>();
    private readonly IDataFileInterface _mockDataFileInterface = Substitute.For<IDataFileInterface>();
    private readonly IEmplifiInterface _mockEmplifiInterface = Substitute.For<IEmplifiInterface>();
    private readonly IHelper _mockHelper = Substitute.For<IHelper>(); 

    public EmplifiImportVerificationsOfBenefitsFromIbmTests() : base()
    {
        _sut = new(_mockEmplifiInterface,
            _mockDataFileInterface,
            _mockLogger,
            _mockHelper,
            _mockGenericHelper,
            _mockCoconaContextWrapper);
    }

    [Fact]
    public async Task RunAsyncTest_CallsAllDependencies_WhenSuccessful()
    {
        // Arrange
        string mockFileName = "fileName.txt";
        RunForFileNameParameter runFor = new RunForFileNameParameter
        {
            RunFor = mockFileName,
        };

        string emailSubject = "INN623 IBM CarePath VOB Reconciliation";
        StringBuilder stringBody = new($"IBM reconciliation reporting for {DateTime.Now:MM/dd/yyyy}");

        IEnumerable<string> mockFileNames = [mockFileName];
        int number = 0;
        _mockHelper.ProcessFilesAndBuildReportEmailAsync(Arg.Is<IEnumerable<string>>(a => a.SequenceEqual(mockFileNames)), TestCancellationToken)
            .Returns(Task.FromResult((stringBody, number)));

        DateTime testTime = DateTime.Now;
        EmailExceptionComposerImp emailExceptionComposerImp = new("INN623", testTime, System.Net.Mail.MailPriority.Normal);
        _mockEmplifiInterface.SendVobNotification(
            Arg.Any<EmailExceptionComposerImp>(),
            Arg.Is<string>(a => a.Equals(emailSubject)),
            Arg.Is<string>(a => a.Equals(stringBody.ToString())));

        // Act
        await _sut.RunAsync(runFor);

        // Assert
        await _mockHelper.Received(1).ProcessFilesAndBuildReportEmailAsync(Arg.Is<IEnumerable<string>>(a => a.SequenceEqual(mockFileNames)), TestCancellationToken);
        _mockEmplifiInterface.Received(1).SendVobNotification(
            Arg.Any<EmailExceptionComposerImp>(),
            Arg.Is<string>(a => a.Equals(emailSubject)),
            Arg.Is<string>(a => a.Equals(stringBody.ToString())));
    }
}
