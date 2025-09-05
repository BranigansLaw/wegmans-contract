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
using Parquet.Meta;
using Xunit;

namespace ZZZTest.Library.EmplifiInterface.Helper;

public class VerificationOfBenefitsHelperImpTests
{
    private const string CompanyId = "SYS";
    private readonly IOptions<EmplifiConfig> _config;
    private readonly ILogger<VerificationOfBenefitsHelperImp> _logger;
    private readonly ICaseService _mockCaseService;
    private readonly IAddressService _mockAddressService;
    private readonly ICaseStreamService _mockCaseStreamService;
    private readonly IVerificationOfBenefitsHelper _helper;

    public VerificationOfBenefitsHelperImpTests()
    {
        _config = Substitute.For<IOptions<EmplifiConfig>>();
        _logger = Substitute.For<ILogger<VerificationOfBenefitsHelperImp>>();
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

        _helper = new VerificationOfBenefitsHelperImp(
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
        var result = _helper.DeriveNormalDate(input);

        // Assert
        Assert.Equal(result, expected);
    }

    [Theory]
    [InlineData("01012a00")]
    [InlineData("09242015")]
    [InlineData(null)]
    [InlineData("")]
    public void DerivePatientDateOfBirth_ShouldReturnNull_WhenInvalidDateStringIsPassed(string? input)
    {
        // Arrange

        // Act
        var result = _helper.DeriveNormalDate(input);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(
     1980, 1, 1
     )]
    [InlineData(
     null, null, null
     )]
    public void DeriveCaseText_ShouldReturnExpectedFormattedString(
         int? year, int? month, int? day
         )
    {
        // Arrange
        DateTime? recordTimestamp = null;
        if (year.HasValue && month.HasValue && day.HasValue)
        {
            recordTimestamp = new(year.Value, month.Value, day.Value);
        }

        DateTime? patientEnrollmentForm = null;
        if (year.HasValue && month.HasValue && day.HasValue)
        {
            patientEnrollmentForm = new(year.Value, month.Value, day.Value);
        }

        var ibmVerificationOfBenefitsRow = new IbmVerificationOfBenefitsRow()
        {
            CarePathSpecialtyPharmacyName = "CarePathSpecialtyPharmacyName",
            CarePathPatientId = "CarePathPatientId",
            PatientBirthYear = "PatientBirthYear",
            PayerType = "PayerType",
            SpecialtyPharmacyName = "SpecialtyPharmacyName",
            SpecialtyPharmacyPhone = "SpecialtyPharmacyPhone",
            ImageExists = "ImageExists",
            CarePathCaseId = "CarePathCaseId",
            ExternalPatientId = "ExternalPatientId",
            ProductName = "ProductName",
            DemographicId = "DemographicId"
        };

        // Act
        var result = _helper.DeriveCaseText(
            ibmVerificationOfBenefitsRow,
            recordTimestamp,
            patientEnrollmentForm
        );

        // Assert
        var expected = $"RecordTimestamp:{recordTimestamp}{Environment.NewLine}" +
               $"CarePathSpecialtyPharmacyName:{ibmVerificationOfBenefitsRow.CarePathSpecialtyPharmacyName}{Environment.NewLine}" +
               $"CarePathPatientId:{ibmVerificationOfBenefitsRow.CarePathPatientId}{Environment.NewLine}" +
               $"PatientBirthYear:{ibmVerificationOfBenefitsRow.PatientBirthYear}{Environment.NewLine}" +
               $"PayerType:{ibmVerificationOfBenefitsRow.PayerType}{Environment.NewLine}" +
               $"SpecialtyPharmacyName:{ibmVerificationOfBenefitsRow.SpecialtyPharmacyName}{Environment.NewLine}" +
               $"SpecialtyPharmacyPhone:{ibmVerificationOfBenefitsRow.SpecialtyPharmacyPhone}{Environment.NewLine}" +
               $"ImageExists:{ibmVerificationOfBenefitsRow.ImageExists}{Environment.NewLine}" +
               $"CarePathCaseId:{ibmVerificationOfBenefitsRow.CarePathCaseId}{Environment.NewLine}" +
               $"PatientEnrollmentFormReceived:{patientEnrollmentForm}{Environment.NewLine}" +
               $"ExternalPatientId:{ibmVerificationOfBenefitsRow.ExternalPatientId}{Environment.NewLine}" +
               $"ProductName:{ibmVerificationOfBenefitsRow.ProductName}{Environment.NewLine}" +
               $"DemographicId:{ibmVerificationOfBenefitsRow.DemographicId}{Environment.NewLine}";

        Assert.Equal(result, expected);
    }

    [Fact]
    public void DeriveImageFileNamePattern_ShouldThrowArgumentNullException_WhenImageFileNamePatternIsNullOrEmpty()
    {
        // Arrange
        string imageFileNamePattern = string.Empty;
        string patientDemographicId = "12345";
        string carePathTransactionId = "67890";
        string imageFileNameExtension = ".jpg";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _helper.DeriveImageFileNamePattern(
            imageFileNamePattern,
            patientDemographicId,
            carePathTransactionId,
            imageFileNameExtension
        ));
    }

    [Fact]
    public void DeriveImageFileNamePattern_ShouldThrowArgumentNullException_WhenImageFileNameExtensionIsNullOrEmpty()
    {
        // Arrange
        string imageFileNamePattern = "Pattern_";
        string patientDemographicId = "12345";
        string carePathTransactionId = "67890";
        string imageFileNameExtension = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _helper.DeriveImageFileNamePattern(
            imageFileNamePattern,
            patientDemographicId,
            carePathTransactionId,
            imageFileNameExtension
        ));
    }

    [Theory]
    [InlineData("Pattern_", "", "67890", ".tiff")]
    [InlineData("Pattern_", "12345", "", ".tiff")]
    public void DeriveImageFileNamePattern_ShouldReturnEmptyString_WhenPatientDemographicIdOrCarePathTransactionIdIsNullOrEmpty(
        string imageFileNamePattern,
        string patientDemographicId,
        string carePathTransactionId,
        string imageFileNameExtension
        )
    {
        // Arrange

        // Act
        var result = _helper.DeriveImageFileNamePattern(
            imageFileNamePattern,
            patientDemographicId,
            carePathTransactionId,
            imageFileNameExtension
        );

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("Pattern_", "12345", "67890", ".tiff", "Pattern_12345_67890*.tiff")]
    [InlineData("Test_", "12345", "67890", ".jpg", "Test_12345_67890*.jpg")]
    public void DeriveImageFileNamePattern_ShouldReturnExpectedResult_WhenAllParametersArePopulated(
    string imageFileNamePattern,
    string patientDemographicId,
    string carePathTransactionId,
    string imageFileNameExtension,
    string expected
    )
    {
        // Arrange

        // Act
        var result = _helper.DeriveImageFileNamePattern(
            imageFileNamePattern,
            patientDemographicId,
            carePathTransactionId,
            imageFileNameExtension
        );

        // Assert
        Assert.Equal(result, expected);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("abc!@#", "")]
    [InlineData("a1b2c3", "123")]
    [InlineData("123456", "123456")]
    [InlineData(" 1 2 3 ", "123")]
    public void GetDigits_ShouldReturnExpectedResult(string? input, string? expected)
    {
        // Arrange

        // Act
        var result = _helper.GetDigits(input);

        // Assert
        Assert.Equal(result, expected);
    }

    [Fact]
    public async Task PostActionAsync_ShouldSucceed_WhenApiResponseIsValid()
    {
        // Arrange
        var actionTypeCode = "ActionType";
        var referredToUserCode = "UserCode";
        var caseId = 123;
        var responseDue = DateTime.Now;
        var cancellationToken = CancellationToken.None;

        var mockResponse = new ActionListResponse
        {
            Valid = CaseServiceWrapper.ValidState.Ok
        };

        _mockCaseService.PostActionAsync(Arg.Any<PostActionListRequest>())
            .Returns(Task.FromResult(mockResponse));
        _mockCaseService.ReleaseCaseAsync(Arg.Any<CaseUserListRequest>())
            .Returns(Task.FromResult(new CaseUserListResponse { Valid = CaseServiceWrapper.ValidState.Ok }));

        // Act
        await _helper.PostActionAsync(actionTypeCode, referredToUserCode, caseId, responseDue, cancellationToken);

        // Assert
        await _mockCaseService.Received(1).PostActionAsync(Arg.Is<PostActionListRequest>(request =>
            request.PostAction.action_type_code == actionTypeCode &&
            request.PostAction.referred_to_user_code == referredToUserCode &&
            request.PostAction.case_id == caseId &&
            request.PostAction.response_due == responseDue.ToString()
        ));
    }

    [Fact]
    public async Task PostActionAsync_ShouldThrowException_WhenApiResponseIsNull()
    {
        // Arrange
        var actionTypeCode = "ActionType";
        var referredToUserCode = "UserCode";
        var caseId = 123;
        var responseDue = DateTime.Now;
        var cancellationToken = CancellationToken.None;

        _mockCaseService.PostActionAsync(Arg.Any<PostActionListRequest>())
            .Returns(Task.FromResult<ActionListResponse?>(null));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.PostActionAsync(actionTypeCode, referredToUserCode, caseId, responseDue, cancellationToken));
    }

    [Theory]
    [InlineData(CaseServiceWrapper.ValidState.Error)]
    [InlineData(CaseServiceWrapper.ValidState.Unknown)]
    public async Task PostActionAsync_ShouldThrowException_WhenApiResponseIsInvalid(CaseServiceWrapper.ValidState validState)
    {
        // Arrange
        var actionTypeCode = "ActionType";
        var referredToUserCode = "UserCode";
        var caseId = 123;
        var responseDue = DateTime.Now;
        var cancellationToken = CancellationToken.None;

        var mockResponse = new ActionListResponse
        {
            Valid = validState
        };

        _mockCaseService.PostActionAsync(Arg.Any<PostActionListRequest>())
            .Returns(Task.FromResult(mockResponse));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.PostActionAsync(actionTypeCode, referredToUserCode, caseId, responseDue, cancellationToken));
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
    public async Task FindCasesByAddressIdProgramAndStatusAsync_ShouldReturnEmptyList_WhenPatientIdentifiersAreEmpty()
    {
        // Arrange
        var programType = "testProgram";
        var caseStatus = "testStatus";
        var birthYear = "2003";
        List<PatientIdentifier> patientIdentifers = [];

        // Act
        var result = await _helper.FindCasesByProgramTypePatientIdAndDob(programType, birthYear, caseStatus, patientIdentifers);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindCasesByAddressIdProgramAndStatusAsync_ShouldReturnEmptyList_WhenProgramTypeIsNullOrEmpty()
    {
        // Arrange
        var programType = string.Empty;
        var caseStatus = "testStatus";
        var birthYear = "2003";
        List<PatientIdentifier> patientIdentifers = [new PatientIdentifier(){Type = "Type", Value = "Value"},
                                                     new PatientIdentifier(){Type = "Type2", Value = "Value2"}];

        // Act
        var result = await _helper.FindCasesByProgramTypePatientIdAndDob(programType, birthYear, caseStatus, patientIdentifers);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindCasesByAddressIdProgramAndStatusAsync_ShouldReturnEmptyList_WhenCaseStatusIsNullOrEmpty()
    {
        // Arrange
        var programType = "testProgram";
        var caseStatus = string.Empty;
        var birthYear = "2003";
        List<PatientIdentifier> patientIdentifers = [new PatientIdentifier(){Type = "Type", Value = "Value"},
                                                     new PatientIdentifier(){Type = "Type2", Value = "Value2"}];

        // Act
        var result = await _helper.FindCasesByProgramTypePatientIdAndDob(programType, birthYear, caseStatus, patientIdentifers);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindCasesByAddressIdProgramAndStatusAsync_ShouldThrowException_WhenGetCaseAsyncReturnsNull()
    {
        // Arrange
        var programType = "testProgram";
        var caseStatus = "testStatus";
        var birthYear = "2003";
        List<PatientIdentifier> patientIdentifers = [new PatientIdentifier(){Type = "Type", Value = "Value"},
                                                     new PatientIdentifier(){Type = "Type2", Value = "Value2"}];

        _mockCaseService.GetCaseAsync(Arg.Any<CaseGetRequest>()).Returns(Task.FromResult<CaseListResponse?>(null));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.FindCasesByProgramTypePatientIdAndDob(programType, birthYear, caseStatus, patientIdentifers));
    }

    [Fact]
    public async Task FindCasesByAddressIdProgramAndStatusAsync_ShouldReturnCaseList_WhenGetCaseAsyncSucceeds()
    {
        // Arrange
        var programType = "testProgram";
        var birthYear = "2003";
        var caseStatus = "testStatus";
        List<PatientIdentifier> patientIdentifers = [new PatientIdentifier(){Type = "Type", Value = "Value"}];

        var caseListResponse = new CaseListResponse
        {
            Valid = CaseServiceWrapper.ValidState.Ok,
            Case = [new Case() { AddressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                    new CaseServiceWrapper.Address()
                    {
                        address_type_code = "PATIENT",
                        a05_code = new DateTime(2003, 10, 01).ToString("MM/dd/yyyy"),
                        PhoneList = new CaseServiceWrapper.PhoneList()
                        {
                            Phone = new CaseServiceWrapper.Phone[]
                            {
                                new CaseServiceWrapper.Phone()
                                {
                                    phone_type_code = "Type",
                                    phone = string.Format("*{0}*", "Value") // Use asterisks to do an exact match in the phone number field used for patient ID's, otherwise a partial match happens
                                }
                            }
                        }
                    }
                }
            }}],
        };

        _mockCaseService.GetCaseAsync(Arg.Any<CaseGetRequest>()).Returns(Task.FromResult(caseListResponse));

        // Act
        var result = await _helper.FindCasesByProgramTypePatientIdAndDob(programType, birthYear, caseStatus, patientIdentifers);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task CreateCaseFromJpapTriageAsync_ShouldCreateCaseSuccessfully()
    {
        // Arrange
        var verificationOfBenefits = new VerificationOfBenefits();
       
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
        var result = await _helper.CreateCaseFromVerificationOfBenefitsAsync(verificationOfBenefits, cancellationToken);

        // Assert
        Assert.Equal(1, result);
        await _mockCaseService.Received(1).UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>());
    }

    [Theory]
    [InlineData(CaseServiceWrapper.ValidState.Error)]
    [InlineData(CaseServiceWrapper.ValidState.Unknown)]
    public async Task CreateCaseFromJpapTriageAsync_ShouldThrowException_WhenApiResponseIsInvalid(CaseServiceWrapper.ValidState validState)
    {
        // Arrange
        var verificationOfBenefits = new VerificationOfBenefits();
        var cancellationToken = new CancellationToken();

        var caseListResponse = new CaseListResponse
        {
            Case = null,
            Valid = validState
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>())
            .Returns(caseListResponse);

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.CreateCaseFromVerificationOfBenefitsAsync(verificationOfBenefits, cancellationToken));
    }

    [Fact]
    public async Task UpdateCaseFromVerificationOfBenefitsAsync_ShouldUpdateCaseSuccessfully()
    {
        // Arrange
        var caseId = 123;
        var verificationOfBenefits = new VerificationOfBenefits 
        {
            PatientIdentifiers = [new PatientIdentifier { Type = "Type", Value = "Value"}]
        };
        var cancellationToken = CancellationToken.None;

        var caseListResponse = new CaseListResponse
        {
            Valid = CaseServiceWrapper.ValidState.Ok,
            Case = [new Case() { AddressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                    new CaseServiceWrapper.Address()
                    {
                        address_type_code = "PATIENT",
                        a05_code = new DateTime(2003, 10, 01).ToString("MM/dd/yyyy"),
                        PhoneList = new CaseServiceWrapper.PhoneList()
                        {
                            Phone = new CaseServiceWrapper.Phone[]
                            {
                                new CaseServiceWrapper.Phone()
                                {
                                    phone_type_code = "Type",
                                    phone = string.Format("*{0}*", "Value") // Use asterisks to do an exact match in the phone number field used for patient ID's, otherwise a partial match happens
                                }
                            }
                        }
                    }
                }
            },
            IssueList = new IssueList()
            {
                Issue = new CaseServiceWrapper.Issue[]
                {
                    new CaseServiceWrapper.Issue()
                    {
                        issue_seq = "1234567"
                    }
                }
            }
            }],
        };

        var caseRecord = new Case
        {
            AddressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                    new CaseServiceWrapper.Address()
                    {
                        address_type_code = "PATIENT",
                        address_id = "123456",
                        PhoneList = new CaseServiceWrapper.PhoneList()
                        {
                            Phone = new CaseServiceWrapper.Phone[]
                            {
                                new CaseServiceWrapper.Phone()
                                {
                                    phone_type_code = "Type",
                                    phone = string.Format("*{0}*", "Value") // Use asterisks to do an exact match in the phone number field used for patient ID's, otherwise a partial match happens
                                }
                            }
                        }
                    }
                }
            },
            IssueList = new IssueList()
            {
                Issue = new CaseServiceWrapper.Issue[]
                {
                    new CaseServiceWrapper.Issue()
                    {
                        issue_seq = "1234567"
                    }
                }
            }
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>()).Returns(caseListResponse);
        _mockCaseService.ReleaseCaseAsync(Arg.Any<CaseUserListRequest>())
            .Returns(Task.FromResult(new CaseUserListResponse { Valid = CaseServiceWrapper.ValidState.Ok }));

        // Act
        await _helper.UpdateCaseFromVerificationOfBenefitsAsync(caseRecord, caseId, verificationOfBenefits, cancellationToken);

        // Assert
        await _mockCaseService.Received(1).UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>());
    }

    [Theory]
    [InlineData(CaseServiceWrapper.ValidState.Error)]
    [InlineData(CaseServiceWrapper.ValidState.Unknown)]
    public async Task UpdateCaseFromVerificationOfBenefitsAsync_ShouldThrowException_WhenApiReturnsInvalidResponse(
        CaseServiceWrapper.ValidState validState)
    {
        // Arrange
        var caseId = 123;
        var verificationOfBenefits = new VerificationOfBenefits { };
        var cancellationToken = CancellationToken.None;

        var caseListResponse = new CaseListResponse
        {
            Case = null,
            Valid = validState
        };

        var caseRecord = new Case
        {
            AddressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                    new CaseServiceWrapper.Address()
                    {
                        address_type_code = "PATIENT"
                    }
                }
            },
            IssueList = new IssueList()
            {
                Issue = new CaseServiceWrapper.Issue[]
                {
                    new CaseServiceWrapper.Issue()
                    {
                        issue_seq = "1234567"
                    }
                }
            }
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>()).Returns(caseListResponse);

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.UpdateCaseFromVerificationOfBenefitsAsync(caseRecord, caseId, verificationOfBenefits, cancellationToken));
    }
}
