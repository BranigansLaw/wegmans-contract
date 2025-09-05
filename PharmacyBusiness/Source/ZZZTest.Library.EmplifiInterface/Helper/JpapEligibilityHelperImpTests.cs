using AddressServiceWrapper;
using CaseServiceWrapper;
using CaseStreamServiceWrapper;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Exceptions;
using Library.EmplifiInterface.Helper;
using Library.TenTenInterface.DataModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Polly;
using System.Reflection;
using Xunit;

namespace ZZZTest.Library.EmplifiInterface.Helper;

public class JpapEligibilityHelperImpTests
{
    private const string CompanyId = "SYS";
    private readonly IOptions<EmplifiConfig> _config;
    private readonly ILogger<JpapEligibilityHelperImp> _logger;
    private readonly ICaseService _mockCaseService;
    private readonly IAddressService _mockAddressService;
    private readonly ICaseStreamService _mockCaseStreamService;
    private readonly JpapEligibilityHelperImp _helper;

    public JpapEligibilityHelperImpTests()
    {
        _config = Substitute.For<IOptions<EmplifiConfig>>();
        _logger = Substitute.For<ILogger<JpapEligibilityHelperImp>>();
        _mockCaseService = Substitute.For<ICaseService>();
        _mockAddressService = Substitute.For<IAddressService>();
        _mockCaseStreamService = Substitute.For<ICaseStreamService>();

        _config.Value.Returns(new EmplifiConfig
        {
            Url = "https://test.com",
            Username = "testUser",
            Password = "testPassword",
            InputFileLocation = "C:\\Test\\Input",
            OutputFileLocation = "C:\\Test\\Output",
            ArchiveFileLocation = "C:\\Test\\Archive",
            RejectFileLocation = "C:\\Test\\Reject",
            NotificationEmailTo = "testEmail.com",
            EmplifiDispenseNotificationEmailTo = "testDispenseEmail.com",
            ImageFileLocation = "C:\\Test\\Image",
            EmplifiTriageNotificationEmailTo = "testTriageEmail.com"
        });

        _helper = new JpapEligibilityHelperImp(
            _logger,
            _config,
            _mockCaseService,
            _mockAddressService,
            _mockCaseStreamService
        );
    }

    [Theory]
    [InlineData("20000101", 2000, 1, 1)]
    [InlineData("20150924", 2015, 9, 24)]
    public void DerivePatientDateOfBirth_ShouldReturnDateTime_WhenValidDateStringIsPassed(string input, int year, int month, int day)
    {
        // Arrange
        DateTime expected = new(year, month, day);

        // Act
        var result = _helper.DeriveDate(input);

        // Assert
        Assert.Equal(result, expected);
    }

    [Theory]
    [InlineData("2a000101")]
    [InlineData("09242015")]
    [InlineData(null)]
    [InlineData("")]
    public void DerivePatientDateOfBirth_ShouldReturnNull_WhenInvalidDateStringIsPassed(string? input)
    {
        // Arrange

        // Act
        var result = _helper.DeriveDate(input);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(
        2003, 10, 01
        )]
    [InlineData(
        null, null, null
        )]
    public void DeriveCaseText_ShouldReturnExpectedFormattedString(
        int? year, int? month, int? day)
    {
        // Arrange
        DateTime? patientDateOfBirth = null;
        if (year.HasValue && month.HasValue && day.HasValue)
        {
            patientDateOfBirth = new(year.Value, month.Value, day.Value);
        }

        var ibmJpapEligibilityRow = new IbmJpapEligibilityRow()
        {
            RecordTimestamp = "RecordTimestamp",
            PatientProgramEnrollmentName = "PatientProgramEnrollmentName",
            Status = "Status",
            EnrollmentStatus = "EnrollmentStatus",
            Outcome = "Outcome",
            Product = "Product",
            Gender = "Gender",
            StartDate = "StartDate",
            EndDate = "EndDate",
            PatientId = "PatientId"
        };

        // Act
        var result = _helper.DeriveCaseText(
            ibmJpapEligibilityRow,
            patientDateOfBirth
        );

        // Assert
        var expected = $"Record Timestamp:{ibmJpapEligibilityRow.RecordTimestamp}{Environment.NewLine}" +
                   $"Patient Program Enrollment Name:{ibmJpapEligibilityRow.PatientProgramEnrollmentName}{Environment.NewLine}" +
                   $"Status:{ibmJpapEligibilityRow.Status}{Environment.NewLine}" +
                   $"Enrollment Status:{ibmJpapEligibilityRow.EnrollmentStatus}{Environment.NewLine}" +
                   $"Outcome:{ibmJpapEligibilityRow.Outcome}{Environment.NewLine}" +
                   $"Product:{ibmJpapEligibilityRow.Product}{Environment.NewLine}" +
                   $"Date of Birth:{patientDateOfBirth}{Environment.NewLine}" +
                   $"Gender:{ibmJpapEligibilityRow.Gender}{Environment.NewLine}" +
                   $"Start Date:{ibmJpapEligibilityRow.StartDate}{Environment.NewLine}" +
                   $"End Date:{ibmJpapEligibilityRow.EndDate}{Environment.NewLine}" +
                   $"Patient Id:{ibmJpapEligibilityRow.PatientId}";

        Assert.Equal(result, expected);
    }


    [Fact]
    public async Task ReleaseCaseAsync_ShouldSucceed_WhenApiResponseIsValid()
    {
        // Arrange
        var caseId = 123;
        var cancellationToken = CancellationToken.None;

        var mockResponse = new CaseUserListResponse
        {
            Valid = CaseServiceWrapper.ValidState.Ok
        };

        _mockCaseService.ReleaseCaseAsync(Arg.Any<CaseUserListRequest>())
            .Returns(Task.FromResult(mockResponse));

        // Act
        await _helper.ReleaseCaseAsync(caseId, cancellationToken);

        // Assert
        await _mockCaseService.Received(1).ReleaseCaseAsync(Arg.Is<CaseUserListRequest>(request =>
            request.CaseUser.case_id == caseId &&
            request.CaseUser.company_id == CompanyId &&
            request.CaseUser.system_user_id == "testUser" &&
            request.UserName == "testUser" &&
            request.Password == "testPassword"
        ));
    }

    [Fact]
    public async Task ReleaseCaseAsync_ShouldThrowException_WhenApiResponseIsNull()
    {
        // Arrange
        var caseId = 123;
        var cancellationToken = CancellationToken.None;

        _mockCaseService.ReleaseCaseAsync(Arg.Any<CaseUserListRequest>())
            .Returns(Task.FromResult<CaseUserListResponse?>(null));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.ReleaseCaseAsync(caseId, cancellationToken));
    }

    [Theory]
    [InlineData(CaseServiceWrapper.ValidState.Error)]
    [InlineData(CaseServiceWrapper.ValidState.Unknown)]
    public async Task ReleaseCaseAsync_ShouldThrowException_WhenApiResponseIsInvalid(CaseServiceWrapper.ValidState validState)
    {
        // Arrange
        var caseId = 123;
        var cancellationToken = CancellationToken.None;

        var mockResponse = new CaseUserListResponse
        {
            Valid = validState
        };

        _mockCaseService.ReleaseCaseAsync(Arg.Any<CaseUserListRequest>())
            .Returns(Task.FromResult(mockResponse));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.ReleaseCaseAsync(caseId, cancellationToken));
    }

    [Fact]
    public void CreateCaseGetRequest_ShouldReturnCorrectRequest_WhenParametersAreValid()
    {
        // Arrange
        var programType = "ProgramType";
        var caseStatus = "CaseStatus";

        // Act
        var result = _helper.CreateCaseGetRequest(programType, caseStatus);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testUser", result.UserName);
        Assert.Equal("testPassword", result.Password);
        Assert.Equal(CaseGetType.Get, result.Type);
        Assert.Equal(CompanyId, result.Case.company_id);
        Assert.Equal(programType, result.Case.b07_code);
        Assert.Equal(caseStatus, result.Case.case_status);
        Assert.Equal(CaseServiceWrapper.TrueFalseType.Item1, result.ResponseFormat.CaseList.Case.AllAttributes);
        Assert.Equal(CaseServiceWrapper.TrueFalseType.Item1, result.ResponseFormat.CaseList.Case.AddressList.Address.AllAttributes);
        Assert.Single(result.ResponseFormat.CaseList.Case.AddressList.Address.PhoneList);
        Assert.Equal(CaseServiceWrapper.TrueFalseType.Item1, result.ResponseFormat.CaseList.Case.AddressList.Address.PhoneList[0].AllAttributes);
        Assert.Equal(CaseServiceWrapper.TrueFalseType.Item1, result.ResponseFormat.CaseList.Case.IssueList.Issue.AllAttributes);
    }

    [Fact]
    public void CreateCaseGetRequest_ShouldThrowArgumentNullException_WhenProgramTypeIsEmpty()
    {
        // Arrange
        var programType = string.Empty;
        var caseStatus = "CaseStatus";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _helper.CreateCaseGetRequest(programType, caseStatus));
        Assert.Equal("programType", exception.ParamName);
    }

    [Fact]
    public void CreateCaseGetRequest_ShouldThrowArgumentNullException_WhenCaseStatusIsEmpty()
    {
        // Arrange
        var programType = "ProgramType";
        var caseStatus = string.Empty;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _helper.CreateCaseGetRequest(programType, caseStatus));
        Assert.Equal("caseStatus", exception.ParamName);
    }

    [Fact]
    public async Task FindCasesByProgramTypePatientIdAndDob_ShouldReturnEmptyList_WhenProgramTypeIsNullOrEmpty()
    {
        // Arrange
        var patientDob = DateTime.Now;
        var patientId = "testPatientId";
        var programType = string.Empty;
        var caseStatus = "testStatus";

        // Act
        var result = await _helper.FindCasesByProgramTypePatientIdAndDob(programType, patientId, patientDob, caseStatus);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindCasesByProgramTypePatientIdAndDob_ShouldReturnEmptyList_WhenCaseStatusIsNullOrEmpty()
    {
        // Arrange
        var patientDob = DateTime.Now;
        var patientId = "testPatientId";
        var programType = "testProgram";
        var caseStatus = string.Empty;

        // Act
        var result = await _helper.FindCasesByProgramTypePatientIdAndDob(programType, patientId, patientDob, caseStatus);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindCasesByProgramTypePatientIdAndDob_ShouldThrowException_WhenGetCaseAsyncReturnsNull()
    {
        // Arrange
        var patientDob = DateTime.Now;
        var patientId = "testPatientId";
        var programType = "testProgram";
        var caseStatus = "testStatus";

        _mockCaseService.GetCaseAsync(Arg.Any<CaseGetRequest>()).Returns(Task.FromResult<CaseListResponse?>(null));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.FindCasesByProgramTypePatientIdAndDob(programType, patientId, patientDob, caseStatus));
    }

    [Fact]
    public async Task FindCasesByProgramTypePatientIdAndDob_ShouldReturnCaseList_WhenGetCaseAsyncSucceeds()
    {
        // Arrange
        var patientDob = DateTime.Now;
        var patientId = "testPatientId";
        var programType = "testProgram";
        var caseStatus = "testStatus";

        var caseListResponse = new CaseListResponse
        {
            Valid = CaseServiceWrapper.ValidState.Ok,
            Case = [new Case()]
        };

        _mockCaseService.GetCaseAsync(Arg.Any<CaseGetRequest>()).Returns(Task.FromResult(caseListResponse));

        // Act
        var result = await _helper.FindCasesByProgramTypePatientIdAndDob(programType, patientId, patientDob, caseStatus);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task CreateBlankCaseFromJpapEligibilityAsync_ShouldCreateCaseSuccessfully()
    {
        // Arrange
        var jpapEligibility = new JpapEligibilityRow
        {
             RecordTimestamp = "RecordTimeStamp",
             PatientProgramEnrollmentName = "PatientProgramEnrollmentName",
             Status = "Status",
             EnrollmentStatus = "EnrollmentStatus",
             Outcome = "Outcome",
             Product = "Product",
             DateOfBirth = DateTime.Now,
             Gender = "Gender",
             StartDate = "StartDate",
             EndDate = "EndDate",
             PatientId = "PatientId",
        };
        var cancellationToken = new CancellationToken();

        var caseListResponse = new CaseListResponse
        {
            Case = new List<Case> { new() { case_id = "1" } }.ToArray(),
            Valid = CaseServiceWrapper.ValidState.Ok
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>())
            .Returns(caseListResponse);
        _mockCaseService.ReleaseCaseAsync(Arg.Any<CaseUserListRequest>())
            .Returns(Task.FromResult(new CaseUserListResponse { Valid = CaseServiceWrapper.ValidState.Ok }));

        // Act
        await _helper.CreateBlankCase(jpapEligibility, cancellationToken);

        // Assert
        await _mockCaseService.Received(1).UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>());
    }

    [Theory]
    [InlineData(CaseServiceWrapper.ValidState.Error)]
    [InlineData(CaseServiceWrapper.ValidState.Unknown)]
    public async Task CreateCaseFromJpapEligibilityAsync_ShouldThrowException_WhenApiResponseIsInvalid(CaseServiceWrapper.ValidState validState)
    {
        // Arrange
        var jpapEligibility = new JpapEligibilityRow();
        var cancellationToken = new CancellationToken();

        var caseListResponse = new CaseListResponse
        {
            Case = null,
            Valid = validState
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>())
            .Returns(caseListResponse);

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.CreateBlankCase(jpapEligibility, cancellationToken));
    }

    [Fact]
    public async Task UpdateCaseFromJpapEligibilityAsync_ShouldUpdateCaseSuccessfully()
    {
        // Arrange
        var caseId = 123;
        var jpapEligibility = new JpapEligibilityRow { };
        var cancellationToken = CancellationToken.None;

        var caseListResponse = new CaseListResponse
        {
            Case = [new Case()],
            Valid = CaseServiceWrapper.ValidState.Ok
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>()).Returns(caseListResponse);
        _mockCaseService.ReleaseCaseAsync(Arg.Any<CaseUserListRequest>())
            .Returns(Task.FromResult(new CaseUserListResponse { Valid = CaseServiceWrapper.ValidState.Ok }));

        // Act
        await _helper.UpdateCaseFromJpapEligibilityAsync(caseId, jpapEligibility, cancellationToken);

        // Assert
        await _mockCaseService.Received(1).UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>());
    }

    [Theory]
    [InlineData(CaseServiceWrapper.ValidState.Error)]
    [InlineData(CaseServiceWrapper.ValidState.Unknown)]
    public async Task UpdateCaseFromJpapEligibilityAsync_ShouldThrowException_WhenApiReturnsInvalidResponse(
        CaseServiceWrapper.ValidState validState)
    {
        // Arrange
        var caseId = 123;
        var jpapEligibility = new JpapEligibilityRow { };
        var cancellationToken = CancellationToken.None;

        var caseListResponse = new CaseListResponse
        {
            Case = null,
            Valid = validState
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>()).Returns(caseListResponse);

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.UpdateCaseFromJpapEligibilityAsync(caseId, jpapEligibility, cancellationToken));
    }
}
