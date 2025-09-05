using INN.JobRunner.Commands;
using INN.JobRunner.Commands.CommandHelpers.EmplifiImportVerificationOfBenefitsFromIbmHelper;
using Library.DataFileInterface;
using Library.EmplifiInterface.Helper;
using Library.EmplifiInterface;
using NSubstitute;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;
using System.Text;

namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers.EmplifiImportVerificationOfBenefitsFromIbmHelper;

public class HelperImpTests : PharmacyCommandBaseTests
{
    private readonly HelperImp _sut;

    private readonly IVerificationOfBenefitsHelper _mockVerificationOfBenefitsHelper = Substitute.For<IVerificationOfBenefitsHelper>();
    private readonly IDataFileInterface _mockDataFileInterface = Substitute.For<IDataFileInterface>();
    private readonly IEmplifiInterface _mockEmplifiInterface = Substitute.For<IEmplifiInterface>();

    public HelperImpTests() : base()
    {
        _sut = new(_mockVerificationOfBenefitsHelper,
            _mockDataFileInterface,
            _mockEmplifiInterface);
    }

    [Theory]
    [InlineData("Something_20241206162813_RERUN.txt", "", 0)]
    [InlineData("Something_20241206162813_CONTROL.txt", "", 0)]
    public async Task ProcessFilesAndBuildReportEmailAsyncTest_IfStatementsAreReachedAsExpected(string fileName, string expectedString, int expectedInt)
    {
        // Arrange
        IEnumerable<string> files = [fileName];
        expectedString = $"IBM reconciliation reporting for {DateOnly.FromDateTime(DateTime.Now):MM/dd/yyyy}\r\n";

        // Act
        (StringBuilder, int) acutalResults = await _sut.ProcessFilesAndBuildReportEmailAsync(files, TestCancellationToken);

        // Assert
        Assert.Equal(acutalResults.Item1.ToString(), expectedString);
        Assert.Equal(acutalResults.Item2, expectedInt);
    }
}
