using AddressServiceWrapper;
using CaseServiceWrapper;
using CaseStreamServiceWrapper;
using Library.EmplifiInterface;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Exceptions;
using Library.EmplifiInterface.Helper;
using Library.TenTenInterface.DataModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace ZZZTest.Library.EmplifiInterface.Helper;

public class OncologyTriageHelperImpTest
{
    private const string CompanyId = "SYS";
    private readonly IOptions<EmplifiConfig> _config;
    private readonly ILogger<OncologyTriageHelperImp> _logger;
    private readonly ICaseService _mockCaseService;
    private readonly IAddressService _mockAddressService;
    private readonly ICaseStreamService _mockCaseStreamService;
    private readonly OncologyTriageHelperImp _helper;

    private readonly List<CompleteSpecialtyItemRow> completeSpecialtyItemRows =
    [
        new("12345678901", "ProgramHeader1", "DrugName1"),
        new("11111111111", "ProgramHeader2", "DrugName2")
    ];

    public OncologyTriageHelperImpTest()
    {
        _config = Substitute.For<IOptions<EmplifiConfig>>();
        _logger = Substitute.For<ILogger<OncologyTriageHelperImp>>();
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

        _helper = new OncologyTriageHelperImp(
            _logger,
            _config,
            _mockCaseService,
            _mockAddressService,
            _mockCaseStreamService
        );
    }

    [Theory]
    [InlineData("01012000", 2000, 1, 1)]
    [InlineData("09242015", 2015, 9, 24)]
    public void DeriveDate_ShouldReturnDateTime_WhenValidDateStringIsPassed(string input, int year, int month, int day)
    {
        // Arrange
        DateTime expected = new(year, month, day);

        // Act
        var result = _helper.DeriveDate(input);

        // Assert
        Assert.Equal(result, expected);
    }

    [Theory]
    [InlineData("01012a00")]
    [InlineData("20150924")]
    [InlineData(null)]
    [InlineData("")]
    public void DeriveDat_ShouldReturnNull_WhenInvalidDateStringIsPassed(string? input)
    {
        // Arrange

        // Act
        var result = _helper.DeriveDate(input);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(
        "12345",
        "John",
        "Doe",
        1980, 1, 1,
        "Male",
        "123 Main St",
        "Apt 4B",
        "Anytown",
        "NY",
        "12345",
        "555-1234",
        "Jane",
        "Smith",
        "9876543210",
        "456 Elm St",
        "Suite 101",
        "Othertown",
        "CA",
        "67890",
        "555-5678",
        "D12345",
        "T12345"
        )]
    [InlineData(
        null,
        null,
        null,
        null, null, null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null
        )]
    public void DeriveCaseText_ShouldReturnExpectedFormattedString(
            string? carePathPatientId,
            string? PatientFirstName,
            string? PatientLastName,
            int? year, int? month, int? day,
            string? PatientGender,
            string? PatientAddress1,
            string? PatientAddress2,
            string? PatientCity,
            string? PatientState,
            string? PatientZipCode,
            string? PatientPhoneNumber,
            string? PrescriberFirstName,
            string? PrescriberLastName,
            string? PrescriberNpi,
            string? PrescriberAddress1,
            string? PrescriberAddress2,
            string? PrescriberCity,
            string? PrescriberState,
            string? PrescriberZipCode,
            string? PrescriberPhoneNumber,
            string? PatientDemographicId,
            string? CaseId
            )
    {
        // Arrange
        DateTime? patientDateOfBirth = null;
        if (year.HasValue && month.HasValue && day.HasValue)
        {
            patientDateOfBirth = new(year.Value, month.Value, day.Value);
        }

        // Act
        var result = _helper.DeriveCaseText(
            carePathPatientId,
             PatientFirstName,
             PatientLastName,
             patientDateOfBirth,
             PatientGender,
             PatientAddress1,
             PatientAddress2,
             PatientCity,
             PatientState,
             PatientZipCode,
             PatientPhoneNumber,
             PrescriberFirstName,
             PrescriberLastName,
             PrescriberNpi,
             PrescriberAddress1,
             PrescriberAddress2,
             PrescriberCity,
             PrescriberState,
             PrescriberZipCode,
             PrescriberPhoneNumber,
             PatientDemographicId,
             CaseId
            );

        // Assert
        var expected = $"CarePath Patient ID: {carePathPatientId}{Environment.NewLine}" +
                       $"Patient First Name: {PatientFirstName}{Environment.NewLine}" +
                       $"Patient Last Name: {PatientLastName}{Environment.NewLine}" +
                       $"Patient DOB: {patientDateOfBirth}{Environment.NewLine}" +
                       $"Patient Sex: {PatientGender}{Environment.NewLine}" +
                       $"Patient Address 1: {PatientAddress1}{Environment.NewLine}" +
                       $"Patient Address 2: {PatientAddress2}{Environment.NewLine}" +
                       $"Patient City: {PatientCity}{Environment.NewLine}" +
                       $"Patient State: {PatientState}{Environment.NewLine}" +
                       $"Patient Zip: {PatientZipCode}{Environment.NewLine}" +
                       $"Patient Phone number: {PatientPhoneNumber}{Environment.NewLine}" +
                       $"Prescriber First Name: {PrescriberFirstName}{Environment.NewLine}" +
                       $"Prescriber Last Name: {PrescriberLastName}{Environment.NewLine}" +
                       $"National Physician ID: {PrescriberNpi}{Environment.NewLine}" +
                       $"Prescriber Address 1: {PrescriberAddress1}{Environment.NewLine}" +
                       $"Prescriber Address 2: {PrescriberAddress2}{Environment.NewLine}" +
                       $"Prescriber City: {PrescriberCity}{Environment.NewLine}" +
                       $"Prescriber State: {PrescriberState}{Environment.NewLine}" +
                       $"Prescriber Zip: {PrescriberZipCode}{Environment.NewLine}" +
                       $"Prescriber Phone: {PrescriberPhoneNumber}{Environment.NewLine}" +
                       $"DEMOGRAPHICID: {PatientDemographicId}{Environment.NewLine}" +
                       $"Carepath Transaction ID: {CaseId}{Environment.NewLine}";

        Assert.Equal(result, expected);
    }

    [Theory]
    [InlineData("12345678901", "ProgramHeader1")]
    [InlineData("11111111111", "ProgramHeader2")]
    public void DeriveProgramHeader_ShouldReturnExpectedProgramHeader_WhenValidNdcIsPassed(string drugNdc, string expected)
    {
        // Arrange

        // Act
        var result = _helper.DeriveProgramHeader(drugNdc, completeSpecialtyItemRows);

        // Assert
        Assert.Equal(result, expected);
    }

    [Theory]
    [InlineData("99999999999")]
    [InlineData(null)]
    public void DeriveProgramHeader_ShouldReturnEmptyString_WhenInvalidNdcIsPassed(string? drugNdc)
    {
        // Arrange

        // Act
        var result = _helper.DeriveProgramHeader(drugNdc, completeSpecialtyItemRows);

        // Assert
        Assert.Equal(result, string.Empty);
    }

    [Theory]
    [InlineData("12345678901", "DrugName1")]
    [InlineData("11111111111", "DrugName2")]
    public void DeriveDrugName_ShouldReturnExpectedDrugName_WhenValidNdcIsPassed(string drugNdc, string expected)
    {
        // Arrange

        // Act
        var result = _helper.DeriveDrugName(drugNdc, completeSpecialtyItemRows);

        // Assert
        Assert.Equal(result, expected);
    }

    [Theory]
    [InlineData("99999999999")]
    [InlineData(null)]
    public void DeriveDrugName_ShouldReturnEmptyString_WhenInvalidNdcIsPassed(string? drugNdc)
    {
        // Arrange

        // Act
        var result = _helper.DeriveDrugName(drugNdc, completeSpecialtyItemRows);

        // Assert
        Assert.Equal(result, string.Empty);
    }

    [Theory]
    [InlineData("M", "Male")]
    [InlineData("F", "Female")]
    public void DerivePatientGender_ShouldReturnExpectedResult_WhenValidInputIsPassed(string input, string expected)
    {
        // Arrange

        // Act
        var result = _helper.DeriveCrmPatientGender(input);

        // Assert
        Assert.Equal(result, expected);
    }

    [Theory]
    [InlineData("Test")]
    [InlineData("")]
    [InlineData(null)]
    public void DerivePatientGender_ShouldReturnEmptyString_WhenInvalidInputIsPassed(string? input)
    {
        // Arrange

        // Act
        var result = _helper.DeriveCrmPatientGender(input);

        // Assert
        Assert.Equal(result, string.Empty);
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
    [InlineData("Pattern_", "12345", "67890", ".tiff", "Pattern_12345_67890.tiff")]
    [InlineData("Test_", "12345", "67890", ".jpg", "Test_12345_67890.jpg")]
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
    public void CreateAddressListSearchRequest_ReturnsCorrectAddressListSearch()
    {
        // Act
        var result = _helper.CreateAddressListSearchRequest();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testUser", result.UserName);
        Assert.Equal("testPassword", result.Password);
        Assert.Equal(CompanyId, result.company_id);
        Assert.Equal(RequestType.Get, result.Type);
        Assert.NotNull(result.Address);
        Assert.Equal(CompanyId, result.Address.company_id);
        Assert.Equal(SearchYesNo.B, result.Address.allow_survey);
        Assert.NotNull(result.ResponseFormat);
        Assert.NotNull(result.ResponseFormat.AddressList);
        Assert.NotNull(result.ResponseFormat.AddressList.Address);
        Assert.Equal(AddressServiceWrapper.TrueFalseType.Item1, result.ResponseFormat.AddressList.Address.AllAttributes);
        Assert.NotNull(result.ResponseFormat.AddressList.Address.PhoneList);
        Assert.Single(result.ResponseFormat.AddressList.Address.PhoneList);
        Assert.Equal(AddressServiceWrapper.TrueFalseType.Item1, result.ResponseFormat.AddressList.Address.PhoneList[0].AllAttributes);
    }

    [Fact]
    public async Task FindAddressUsingPatientIdAndDateOfBirth_ShouldReturnAddresses_WhenInputIsValid()
    {
        // Arrange
        var patientIdType = "type";
        var patientId = "123";
        var dateOfBirth = new DateTime(2000, 1, 1);

        var mockResponse = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = [new AddressServiceWrapper.Address()]
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponse));

        // Act
        var result = await _helper.FindAddressUsingPatientIdAndDateOfBirth(patientIdType, patientId, dateOfBirth);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Theory]
    [InlineData(AddressServiceWrapper.ValidState.Error)]
    [InlineData(AddressServiceWrapper.ValidState.Unknown)]
    public async Task FindAddressUsingPatientIdAndDateOfBirth_ShouldThrowException_WhenResponseIsInvalid(
        AddressServiceWrapper.ValidState validState)
    {
        // Arrange
        var patientIdType = "type";
        var patientId = "123";
        var dateOfBirth = new DateTime(2000, 1, 1);

        var mockResponse = new AddressListResponse
        {
            Valid = validState
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponse));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.FindAddressUsingPatientIdAndDateOfBirth(patientIdType, patientId, dateOfBirth));
    }

    [Fact]
    public async Task FindAddressUsingPatientIdAndDateOfBirth_ShouldThrowException_WhenResponseIsNull()
    {
        // Arrange
        var patientIdType = "type";
        var patientId = "123";
        var dateOfBirth = new DateTime(2000, 1, 1);

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult<AddressListResponse?>(null));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.FindAddressUsingPatientIdAndDateOfBirth(patientIdType, patientId, dateOfBirth));
    }

    [Theory]
    [InlineData(null, null, null, null, null)]
    [InlineData("type", "123", null, null, null)]
    [InlineData(null, "123", 2000, 1, 1)]
    [InlineData("type", null, 2000, 1, 1)]
    [InlineData("", "123", 2000, 1, 1)]
    [InlineData("type", "", 2000, 1, 1)]
    public async Task FindAddressUsingPatientIdAndDateOfBirth_ShouldReturnEmpty_WhenInputIsNullOrEmpty(
        string? patientIdType,
        string? patientId,
        int? year, int? month, int? day)
    {
        // Arrange
        DateTime? patientDateOfBirth = null;
        if (year.HasValue && month.HasValue && day.HasValue)
        {
            patientDateOfBirth = new(year.Value, month.Value, day.Value);
        }

        // Act
        var result = await _helper.FindAddressUsingPatientIdAndDateOfBirth(patientIdType, patientId, patientDateOfBirth);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindAddressUsingLastNameFirstNameAndDateOfBirth_ShouldReturnAddresses_WhenInputIsValid()
    {
        // Arrange
        var lastName = "Doe";
        var firstName = "John";
        var dateOfBirth = new DateTime(2000, 1, 1);

        var mockResponse = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = [new AddressServiceWrapper.Address()]
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponse));

        // Act
        var result = await _helper.FindAddressUsingLastNameFirstNameAndDateOfBirth(lastName, firstName, dateOfBirth);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Theory]
    [InlineData(AddressServiceWrapper.ValidState.Error)]
    [InlineData(AddressServiceWrapper.ValidState.Unknown)]
    public async Task FindAddressUsingLastNameFirstNameAndDateOfBirth_ShouldThrowException_WhenResponseIsInvalid(
        AddressServiceWrapper.ValidState validState)
    {
        // Arrange
        var lastName = "Doe";
        var firstName = "John";
        var dateOfBirth = new DateTime(2000, 1, 1);

        var mockResponse = new AddressListResponse
        {
            Valid = validState
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponse));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.FindAddressUsingLastNameFirstNameAndDateOfBirth(lastName, firstName, dateOfBirth));
    }

    [Fact]
    public async Task FindAddressUsingLastNameFirstNameAndDateOfBirth_ShouldThrowException_WhenResponseIsNull()
    {
        // Arrange
        var lastName = "Doe";
        var firstName = "John";
        var dateOfBirth = new DateTime(2000, 1, 1);

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult<AddressListResponse?>(null));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.FindAddressUsingLastNameFirstNameAndDateOfBirth(lastName, firstName, dateOfBirth));
    }

    [Theory]
    [InlineData(null, null, null, null, null)]
    [InlineData("last", "first", null, null, null)]
    [InlineData(null, "first", 2000, 1, 1)]
    [InlineData("last", null, 2000, 1, 1)]
    [InlineData("", "first", 2000, 1, 1)]
    [InlineData("last", "", 2000, 1, 1)]
    public async Task FindAddressUsingLastNameFirstNameStateAndDateOfBirth_ShouldReturnEmpty_WhenInputIsNullOrEmpty(
        string? lastName,
        string? firstName,
        int? year, int? month, int? day)
    {
        // Arrange
        DateTime? dateOfBirth = null;
        if (year.HasValue && month.HasValue && day.HasValue)
        {
            dateOfBirth = new(year.Value, month.Value, day.Value);
        }

        // Act
        var result = await _helper.FindAddressUsingLastNameFirstNameAndDateOfBirth(lastName, firstName, dateOfBirth);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindAddressesForOncologyTriageAsync_ShouldReturnAddresses_WhenPatientDemographicIdIsValid()
    {
        // Arrange
        var patientDemographicId = "123";
        var carePathPatientId = "456";
        var dateOfBirth = new DateTime(2000, 1, 1);
        var lastName = "Doe";
        var firstName = "John";

        var mockResponse = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = [new AddressServiceWrapper.Address()]
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponse));

        // Act
        var result = await _helper.FindAddressesForOncologyTriageAsync(
            patientDemographicId, carePathPatientId, dateOfBirth, lastName, firstName);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task FindAddressesForOncologyTriageAsync_ShouldReturnAddresses_WhenCarePathPatientIdIsValid()
    {
        // Arrange
        var patientDemographicId = "123";
        var carePathPatientId = "456";
        var dateOfBirth = new DateTime(2000, 1, 1);
        var lastName = "Doe";
        var firstName = "John";

        var mockResponseEmpty = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = []
        };

        var mockResponse = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = [new AddressServiceWrapper.Address()]
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponseEmpty), Task.FromResult(mockResponse));

        // Act
        var result = await _helper.FindAddressesForOncologyTriageAsync(
            patientDemographicId, carePathPatientId, dateOfBirth, lastName, firstName);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task FindAddressesForOncologyTriageAsync_ShouldReturnAddresses_WhenLastNameFirstNameStateIsValid()
    {
        // Arrange
        var patientDemographicId = "123";
        var carePathPatientId = "456";
        var dateOfBirth = new DateTime(2000, 1, 1);
        var lastName = "Doe";
        var firstName = "John";

        var mockResponseEmpty = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = []
        };

        var mockResponse = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = [new AddressServiceWrapper.Address()]
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponseEmpty), Task.FromResult(mockResponseEmpty), Task.FromResult(mockResponse));

        // Act
        var result = await _helper.FindAddressesForOncologyTriageAsync(
            patientDemographicId, carePathPatientId, dateOfBirth, lastName, firstName);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task FindAddressesForOncologyTriageAsync_ShouldReturnEmpty_WhenNoValidAddresses()
    {
        // Arrange
        var patientDemographicId = "123";
        var carePathPatientId = "456";
        var dateOfBirth = new DateTime(2000, 1, 1);
        var lastName = "Doe";
        var firstName = "John";

        var mockResponseEmpty = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = []
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponseEmpty));

        // Act
        var result = await _helper.FindAddressesForOncologyTriageAsync(
            patientDemographicId, carePathPatientId, dateOfBirth, lastName, firstName);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
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
    public async Task CreateAddressFromOncologyTriageAsync_ShouldReturnAddressId_WhenInputIsValid()
    {
        // Arrange
        var oncologyTriage = new OncologyTriage
        {
            RecordTimestamp = DateTime.Now,
            CarePathSpecialtyPharmacyName = "PharmacyName",
            CarePathPatientId = "CP123",
            PatientFirstName = "John",
            PatientLastName = "Doe",
            PatientDateOfBirth = DateTime.Now,
            PatientGender = "M",
            ShipToLocation = "Location",
            PatientAddress1 = "123 Main St",
            PatientAddress2 = "Apt 4",
            PatientCity = "City",
            PatientState = "State",
            PatientZipCode = "12345",
            ProductName = "Product Name",
            PriorAuthReceivedFromPayerDate = DateTime.Now,
            PrescriberFirstName = "Dr",
            PrescriberLastName = "Smith",
            PrescriberAddress1 = "456 Elm St",
            PrescriberAddress2 = "Suite 5",
            PrescriberCity = "City",
            PrescriberState = "State",
            PrescriberZipCode = "67890",
            PrescriberPhoneNumber = "123-456-7890",
            PrescriberNpi = "1234567890",
            CarePathPrescriberId = "CPP456",
            PatientPhoneNumber = "555-555-5555",
            TreatmentCenterName = "Treatment Center",
            TreatmentCenterContactFirstName = "First",
            TreatmentCenterContactLastName = "Last",
            TreatmentCenterAddress1 = "456 Address Ln",
            TreatmentCenterAddress2 = "Office #5",
            TreatmentCenterCity = "City",
            TreatmentCenterState = "State",
            TreatmentCenterZipCode = "11111",
            TreatmentCenterPhoneNumber = "222-222-222",
            TreatmentCenterFaxNumber = "333-333-3333",
            TreatmentCenterNpi = "NpiTreatmentCenter",
            TreatmentCenterDea = "DeaTreatmentCenter",
            DoseType = "Dose101",
            ImageAvailable = "ImageAvailable",
            CaseId = "Case78910",
            NdcCode = "NDC12345678",
            PrimaryDiagnosisCode = "PrimaryDiagnosisCode",
            PatientDemographicId = "PD123",
        };

        var response = new AddressListResponse
        {
            Address = [new AddressServiceWrapper.Address { address_id = "1" }],
            Valid = AddressServiceWrapper.ValidState.Ok
        };

        _mockAddressService.UpdateAddressAsync(Arg.Any<AddressListUpdate>()).Returns(Task.FromResult(response));

        // Act
        var result = await _helper.CreateAddressFromOncologyTriageAsync(oncologyTriage, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }

    [Theory]
    [InlineData(AddressServiceWrapper.ValidState.Error)]
    [InlineData(AddressServiceWrapper.ValidState.Unknown)]
    public async Task CreateAddressFromOncologyTriageAsync_ShouldThrowException_WhenResponseIsInvalid(AddressServiceWrapper.ValidState validState)
    {
        // Arrange
        var oncologyTriage = new OncologyTriage();
        var response = new AddressListResponse
        {
            Valid = validState
        };

        _mockAddressService.UpdateAddressAsync(Arg.Any<AddressListUpdate>()).Returns(Task.FromResult(response));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.CreateAddressFromOncologyTriageAsync(oncologyTriage, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAddressFromOncologyTriageAsync_ShouldThrowException_WhenResponseIsNull()
    {
        // Arrange
        var oncologyTriage = new OncologyTriage();
        AddressListResponse? response = null;

        _mockAddressService.UpdateAddressAsync(Arg.Any<AddressListUpdate>()).Returns(Task.FromResult(response));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.CreateAddressFromOncologyTriageAsync(oncologyTriage, CancellationToken.None));
    }

    [Fact]
    public async Task UploadFileAsync_ShouldReturnFileName_WhenUploadIsSuccessful()
    {
        // Arrange
        var sourceFileName = "test.jpg";
        var fileByteStream = new MemoryStream();
        var caseId = 123;
        var expectedFileName = "uploaded_test.jpg";

        var fileResponse = new FileResponse
        {
            Valid = CaseStreamServiceWrapper.ValidState.Ok,
            FileName = expectedFileName
        };

        _mockCaseStreamService.UploadFileAsync(Arg.Any<FileRequest>())
            .Returns(Task.FromResult(fileResponse));

        // Act
        var result = await _helper.UploadFileAsync(sourceFileName, fileByteStream, caseId);

        // Assert
        Assert.Equal(expectedFileName, result);
    }

    [Fact]
    public async Task UploadFileAsync_ShouldThrowGeneralApiException_WhenResponseIsNull()
    {
        // Arrange
        var sourceFileName = "testFile.txt";
        var fileByteStream = new MemoryStream();
        var caseId = 123;

        _mockCaseStreamService.UploadFileAsync(Arg.Any<FileRequest>()).Returns((FileResponse?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<GeneralApiException>(() => _helper.UploadFileAsync(sourceFileName, fileByteStream, caseId));
        Assert.Equal("UploadFileAsync failed with API status [].", exception.Message);
    }

    [Theory]
    [InlineData(CaseStreamServiceWrapper.ValidState.Error)]
    [InlineData(CaseStreamServiceWrapper.ValidState.Unknown)]
    public async Task UploadFileAsync_ShouldThrowGeneralApiException_WhenResponseIsInvalid(CaseStreamServiceWrapper.ValidState validState)
    {
        // Arrange
        var sourceFileName = "testFile.txt";
        var fileByteStream = new MemoryStream();
        var caseId = 123;
        var fileResponse = new FileResponse
        {
            Valid = validState
        };

        _mockCaseStreamService.UploadFileAsync(Arg.Any<FileRequest>()).Returns(fileResponse);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<GeneralApiException>(() => _helper.UploadFileAsync(sourceFileName, fileByteStream, caseId));
        Assert.Equal($"UploadFileAsync failed with API status [{validState}].", exception.Message);
    }

    [Fact]
    public async Task UploadFileAsync_ShouldThrowFileUploadException_WhenApiThrowsAnException()
    {
        // Arrange
        var sourceFileName = "testFile.txt";
        var fileByteStream = new MemoryStream();
        var caseId = 123;

        _mockCaseStreamService.UploadFileAsync(Arg.Any<FileRequest>()).Throws(new Exception("Test exception"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<FileUploadException>(() => _helper.UploadFileAsync(sourceFileName, fileByteStream, caseId));
        Assert.Equal("Test exception", exception.Message);
        Assert.Equal(sourceFileName, exception.FileName);
    }

    [Fact]
    public async Task AddAttachmentToCaseAsync_ShouldCallUpdateCaseAsync()
    {
        // Arrange
        var caseId = 123;
        var attachmentName = "testAttachment";
        var attachmentDescription = "testDescription";
        var cancellationToken = CancellationToken.None;

        var response = new CaseListResponse
        {
            Valid = CaseServiceWrapper.ValidState.Ok
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>())
            .Returns(Task.FromResult(response));
        _mockCaseService.ReleaseCaseAsync(Arg.Any<CaseUserListRequest>())
            .Returns(Task.FromResult(new CaseUserListResponse { Valid = CaseServiceWrapper.ValidState.Ok }));

        // Act
        await _helper.AddAttachmentToCaseAsync(caseId, attachmentName, attachmentDescription, cancellationToken);

        // Assert
        await _mockCaseService.Received(1).UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>());
    }

    [Theory]
    [InlineData(CaseServiceWrapper.ValidState.Error)]
    [InlineData(CaseServiceWrapper.ValidState.Unknown)]
    public async Task AddAttachmentToCaseAsync_ShouldThrowException_WhenUpdateCaseAsyncFails(CaseServiceWrapper.ValidState validState)
    {
        // Arrange
        var caseId = 123;
        var attachmentName = "testAttachment";
        var attachmentDescription = "testDescription";
        var cancellationToken = CancellationToken.None;

        var response = new CaseListResponse
        {
            Valid = validState
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>())
            .Returns(Task.FromResult(response));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.AddAttachmentToCaseAsync(caseId, attachmentName, attachmentDescription, cancellationToken));
    }

    [Fact]
    public async Task FindCasesByAddressIdProgramAndStatusAsync_ShouldReturnEmptyList_WhenAddressIdIsInvalid()
    {
        // Arrange
        var addressId = 0;
        var programType = "testProgram";
        var caseStatus = "testStatus";

        // Act
        var result = await _helper.FindCasesByAddressIdProgramAndStatusAsync(addressId, programType, caseStatus);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindCasesByAddressIdProgramAndStatusAsync_ShouldReturnEmptyList_WhenProgramTypeIsNullOrEmpty()
    {
        // Arrange
        var addressId = 123;
        var programType = string.Empty;
        var caseStatus = "testStatus";

        // Act
        var result = await _helper.FindCasesByAddressIdProgramAndStatusAsync(addressId, programType, caseStatus);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindCasesByAddressIdProgramAndStatusAsync_ShouldReturnEmptyList_WhenCaseStatusIsNullOrEmpty()
    {
        // Arrange
        var addressId = 123;
        var programType = "testProgram";
        var caseStatus = string.Empty;

        // Act
        var result = await _helper.FindCasesByAddressIdProgramAndStatusAsync(addressId, programType, caseStatus);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindCasesByAddressIdProgramAndStatusAsync_ShouldThrowException_WhenGetCaseAsyncReturnsNull()
    {
        // Arrange
        var addressId = 123;
        var programType = "testProgram";
        var caseStatus = "testStatus";

        _mockCaseService.GetCaseAsync(Arg.Any<CaseGetRequest>()).Returns(Task.FromResult<CaseListResponse?>(null));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.FindCasesByAddressIdProgramAndStatusAsync(addressId, programType, caseStatus));
    }

    [Fact]
    public async Task FindCasesByAddressIdProgramAndStatusAsync_ShouldReturnCaseList_WhenGetCaseAsyncSucceeds()
    {
        // Arrange
        var addressId = 123;
        var programType = "testProgram";
        var caseStatus = "testStatus";

        var caseListResponse = new CaseListResponse
        {
            Valid = CaseServiceWrapper.ValidState.Ok,
            Case = [new Case()]
        };

        _mockCaseService.GetCaseAsync(Arg.Any<CaseGetRequest>()).Returns(Task.FromResult(caseListResponse));

        // Act
        var result = await _helper.FindCasesByAddressIdProgramAndStatusAsync(addressId, programType, caseStatus);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task UpdateAddressFromOncologyTriageAsync_ShouldUpdateAddressSuccessfully()
    {
        // Arrange
        var address = new AddressServiceWrapper.Address
        {
            address_id = "1",
            PhoneList = new AddressServiceWrapper.PhoneList
            {
                Phone = new List<AddressServiceWrapper.Phone>().ToArray()
            }
        };
        var oncologyTriage = new OncologyTriage
        {
            RecordTimestamp = DateTime.Now,
            CarePathSpecialtyPharmacyName = "PharmacyName",
            CarePathPatientId = "CP123",
            PatientFirstName = "John",
            PatientLastName = "Doe",
            PatientDateOfBirth = DateTime.Now,
            PatientGender = "M",
            ShipToLocation = "Location",
            PatientAddress1 = "123 Main St",
            PatientAddress2 = "Apt 4",
            PatientCity = "City",
            PatientState = "State",
            PatientZipCode = "12345",
            ProductName = "Product Name",
            PriorAuthReceivedFromPayerDate = DateTime.Now,
            PrescriberFirstName = "Dr",
            PrescriberLastName = "Smith",
            PrescriberAddress1 = "456 Elm St",
            PrescriberAddress2 = "Suite 5",
            PrescriberCity = "City",
            PrescriberState = "State",
            PrescriberZipCode = "67890",
            PrescriberPhoneNumber = "123-456-7890",
            PrescriberNpi = "1234567890",
            CarePathPrescriberId = "CPP456",
            PatientPhoneNumber = "555-555-5555",
            TreatmentCenterName = "Treatment Center",
            TreatmentCenterContactFirstName = "First",
            TreatmentCenterContactLastName = "Last",
            TreatmentCenterAddress1 = "456 Address Ln",
            TreatmentCenterAddress2 = "Office #5",
            TreatmentCenterCity = "City",
            TreatmentCenterState = "State",
            TreatmentCenterZipCode = "11111",
            TreatmentCenterPhoneNumber = "222-222-222",
            TreatmentCenterFaxNumber = "333-333-3333",
            TreatmentCenterNpi = "NpiTreatmentCenter",
            TreatmentCenterDea = "DeaTreatmentCenter",
            DoseType = "Dose101",
            ImageAvailable = "ImageAvailable",
            CaseId = "Case78910",
            NdcCode = "NDC12345678",
            PrimaryDiagnosisCode = "PrimaryDiagnosisCode",
            PatientDemographicId = "PD123"
        };
        var cancellationToken = new CancellationToken();

        _mockAddressService.UpdateAddressAsync(Arg.Any<AddressListUpdate>())
            .Returns(new AddressListResponse
            {
                Valid = AddressServiceWrapper.ValidState.Ok
            });

        // Act
        await _helper.UpdateAddressFromOncologyTriageAsync(address, oncologyTriage, cancellationToken);

        // Assert
        await _mockAddressService.Received(1).UpdateAddressAsync(Arg.Any<AddressListUpdate>());
    }

    [Fact]
    public async Task UpdateAddressFromOncologyTriageAsync_ShouldThrowException_WhenApiResponseIsNull()
    {
        // Arrange
        var address = new AddressServiceWrapper.Address
        {
            address_id = "1",
            PhoneList = new AddressServiceWrapper.PhoneList
            {
                Phone = new List<AddressServiceWrapper.Phone>().ToArray()
            }
        };
        var oncologyTriage = new OncologyTriage();
        var cancellationToken = new CancellationToken();

        _mockAddressService.UpdateAddressAsync(Arg.Any<AddressListUpdate>())
            .Returns((AddressListResponse?)null);

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.UpdateAddressFromOncologyTriageAsync(address, oncologyTriage, cancellationToken));
    }

    [Fact]
    public async Task CreateCaseFromOncologyTriageAsync_ShouldCreateCaseSuccessfully()
    {
        // Arrange
        var addressId = 1;
        var oncologyTriage = new OncologyTriage
        {
            RecordTimestamp = DateTime.Now,
            CarePathSpecialtyPharmacyName = "PharmacyName",
            CarePathPatientId = "CP123",
            PatientFirstName = "John",
            PatientLastName = "Doe",
            PatientDateOfBirth = DateTime.Now,
            PatientGender = "M",
            ShipToLocation = "Location",
            PatientAddress1 = "123 Main St",
            PatientAddress2 = "Apt 4",
            PatientCity = "City",
            PatientState = "State",
            PatientZipCode = "12345",
            ProductName = "Product Name",
            PriorAuthReceivedFromPayerDate = DateTime.Now,
            PrescriberFirstName = "Dr",
            PrescriberLastName = "Smith",
            PrescriberAddress1 = "456 Elm St",
            PrescriberAddress2 = "Suite 5",
            PrescriberCity = "City",
            PrescriberState = "State",
            PrescriberZipCode = "67890",
            PrescriberPhoneNumber = "123-456-7890",
            PrescriberNpi = "1234567890",
            CarePathPrescriberId = "CPP456",
            PatientPhoneNumber = "555-555-5555",
            TreatmentCenterName = "Treatment Center",
            TreatmentCenterContactFirstName = "First",
            TreatmentCenterContactLastName = "Last",
            TreatmentCenterAddress1 = "456 Address Ln",
            TreatmentCenterAddress2 = "Office #5",
            TreatmentCenterCity = "City",
            TreatmentCenterState = "State",
            TreatmentCenterZipCode = "11111",
            TreatmentCenterPhoneNumber = "222-222-222",
            TreatmentCenterFaxNumber = "333-333-3333",
            TreatmentCenterNpi = "NpiTreatmentCenter",
            TreatmentCenterDea = "DeaTreatmentCenter",
            DoseType = "Dose101",
            ImageAvailable = "ImageAvailable",
            CaseId = "Case78910",
            NdcCode = "NDC12345678",
            PrimaryDiagnosisCode = "PrimaryDiagnosisCode",
            PatientDemographicId = "PD123"
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
        var result = await _helper.CreateCaseFromOncologyTriageAsync(addressId, oncologyTriage, cancellationToken);

        // Assert
        Assert.Equal(1, result);
        await _mockCaseService.Received(1).UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>());
    }

    [Theory]
    [InlineData(CaseServiceWrapper.ValidState.Error)]
    [InlineData(CaseServiceWrapper.ValidState.Unknown)]
    public async Task CreateCaseFromOncologyTriageAsync_ShouldThrowException_WhenApiResponseIsInvalid(CaseServiceWrapper.ValidState validState)
    {
        // Arrange
        var addressId = 1;
        var oncologyTriage = new OncologyTriage();
        var cancellationToken = new CancellationToken();

        var caseListResponse = new CaseListResponse
        {
            Case = null,
            Valid = validState
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>())
            .Returns(caseListResponse);

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.CreateCaseFromOncologyTriageAsync(addressId, oncologyTriage, cancellationToken));
    }

    [Fact]
    public async Task UpdateCaseFromOncologyTriageAsync_ShouldUpdateCaseSuccessfully()
    {
        // Arrange
        var caseId = 123;
        var oncologyTriage = new OncologyTriage { };
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
        await _helper.UpdateCaseFromOncologyTriageAsync(caseId, oncologyTriage, cancellationToken);

        // Assert
        await _mockCaseService.Received(1).UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>());
    }

    [Theory]
    [InlineData(CaseServiceWrapper.ValidState.Error)]
    [InlineData(CaseServiceWrapper.ValidState.Unknown)]
    public async Task UpdateCaseFromOncologyTriageAsync_ShouldThrowException_WhenApiReturnsInvalidResponse(
        CaseServiceWrapper.ValidState validState)
    {
        // Arrange
        var caseId = 123;
        var oncologyTriage = new OncologyTriage { };
        var cancellationToken = CancellationToken.None;

        var caseListResponse = new CaseListResponse
        {
            Case = null,
            Valid = validState
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>()).Returns(caseListResponse);

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.UpdateCaseFromOncologyTriageAsync(caseId, oncologyTriage, cancellationToken));
    }
}

