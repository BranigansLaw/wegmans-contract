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

public class JpapTriageHelperImpTests
{
    private const string CompanyId = "SYS";
    private readonly IOptions<EmplifiConfig> _config;
    private readonly ILogger<JpapTriageHelperImp> _logger;
    private readonly ICaseService _mockCaseService;
    private readonly IAddressService _mockAddressService;
    private readonly ICaseStreamService _mockCaseStreamService;
    private readonly JpapTriageHelperImp _helper;

    private readonly List<CompleteSpecialtyItemRow> completeSpecialtyItemRows =
    [
        new("12345678901", "ProgramHeader1", "DrugName1"),
        new("11111111111", "ProgramHeader2", "DrugName2")
    ];

    public JpapTriageHelperImpTests()
    {
        _config = Substitute.For<IOptions<EmplifiConfig>>();
        _logger = Substitute.For<ILogger<JpapTriageHelperImp>>();
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

        _helper = new JpapTriageHelperImp(
            _config,
            _logger,
            _mockCaseService,
            _mockAddressService,
            _mockCaseStreamService
        );
    }

    [Theory]
    [InlineData("01012000", 2000, 1, 1)]
    [InlineData("09242015", 2015, 9, 24)]
    public void DerivePatientDateOfBirth_ShouldReturnDateTime_WhenValidDateStringIsPassed(string input, int year, int month, int day)
    {
        // Arrange
        DateTime expected = new(year, month, day);

        // Act
        var result = _helper.DerivePatientDateOfBirth(input);

        // Assert
        Assert.Equal(result, expected);
    }

    [Theory]
    [InlineData("01012a00")]
    [InlineData("20150924")]
    [InlineData(null)]
    [InlineData("")]
    public void DerivePatientDateOfBirth_ShouldReturnNull_WhenInvalidDateStringIsPassed(string? input)
    {
        // Arrange

        // Act
        var result = _helper.DerivePatientDateOfBirth(input);

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
        "AB1234567",
        "456 Elm St",
        "Suite 101",
        "Othertown",
        "CA",
        "67890",
        "555-5678",
        "555-8765",
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
        null,
        null,
        null
        )]
    public void DeriveCaseText_ShouldReturnExpectedFormattedString(
        string? carePathPatientId,
        string? patientFirstName,
        string? patientLastName,
        int? year, int? month, int? day,
        string? patientGender,
        string? patientAddress1,
        string? patientAddress2,
        string? patientCity,
        string? patientState,
        string? patientZip,
        string? patientPhoneNumber,
        string? prescriberFirstName,
        string? prescriberLastName,
        string? prescriberNpi,
        string? prescriberDea,
        string? prescriberAddress1,
        string? prescriberAddress2,
        string? prescriberCity,
        string? prescriberState,
        string? prescriberZip,
        string? prescriberPhone,
        string? prescriberFax,
        string? patientDemographicId,
        string? carePathTransactionId
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
            patientFirstName,
            patientLastName,
            patientDateOfBirth,
            patientGender,
            patientAddress1,
            patientAddress2,
            patientCity,
            patientState,
            patientZip,
            patientPhoneNumber,
            prescriberFirstName,
            prescriberLastName,
            prescriberNpi,
            prescriberDea,
            prescriberAddress1,
            prescriberAddress2,
            prescriberCity,
            prescriberState,
            prescriberZip,
            prescriberPhone,
            prescriberFax,
            patientDemographicId,
            carePathTransactionId
        );

        // Assert
        var expected = $"CarePath Patient ID: {carePathPatientId}{Environment.NewLine}" +
                       $"Patient First Name: {patientFirstName}{Environment.NewLine}" +
                       $"Patient Last Name: {patientLastName}{Environment.NewLine}" +
                       $"Patient DOB: {patientDateOfBirth}{Environment.NewLine}" +
                       $"Patient Sex: {patientGender}{Environment.NewLine}" +
                       $"Patient Address 1: {patientAddress1}{Environment.NewLine}" +
                       $"Patient Address 2: {patientAddress2}{Environment.NewLine}" +
                       $"Patient City: {patientCity}{Environment.NewLine}" +
                       $"Patient State: {patientState}{Environment.NewLine}" +
                       $"Patient Zip: {patientZip}{Environment.NewLine}" +
                       $"Patient Phone Number: {patientPhoneNumber}{Environment.NewLine}" +
                       $"Prescriber First Name: {prescriberFirstName}{Environment.NewLine}" +
                       $"Prescriber Last Name: {prescriberLastName}{Environment.NewLine}" +
                       $"National Physician ID: {prescriberNpi}{Environment.NewLine}" +
                       $"Prescriber DEA: {prescriberDea}{Environment.NewLine}" +
                       $"Prescriber Address 1: {prescriberAddress1}{Environment.NewLine}" +
                       $"Prescriber Address 2: {prescriberAddress2}{Environment.NewLine}" +
                       $"Prescriber City: {prescriberCity}{Environment.NewLine}" +
                       $"Prescriber State: {prescriberState}{Environment.NewLine}" +
                       $"Prescriber Zip: {prescriberZip}{Environment.NewLine}" +
                       $"Prescriber Phone: {prescriberPhone}{Environment.NewLine}" +
                       $"Prescriber Fax: {prescriberFax}{Environment.NewLine}" +
                       $"Demographic ID: {patientDemographicId}{Environment.NewLine}" +
                       $"CarePath Transaction ID: {carePathTransactionId}{Environment.NewLine}";

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
        var result = _helper.DerivePatientGender(input);

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
        var result = _helper.DerivePatientGender(input);

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
    [InlineData("Pattern_", "12345", "67890", ".tiff", "Pattern_12345_67890_*.tiff")]
    [InlineData("Test_", "12345", "67890", ".jpg", "Test_12345_67890_*.jpg")]
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
    public async Task FindAddressUsingLastNameFirstNameStateAndDateOfBirth_ShouldReturnAddresses_WhenInputIsValid()
    {
        // Arrange
        var lastName = "Doe";
        var firstName = "John";
        var state = "NY";
        var dateOfBirth = new DateTime(2000, 1, 1);

        var mockResponse = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = [new AddressServiceWrapper.Address()]
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponse));

        // Act
        var result = await _helper.FindAddressUsingLastNameFirstNameStateAndDateOfBirth(lastName, firstName, state, dateOfBirth);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Theory]
    [InlineData(AddressServiceWrapper.ValidState.Error)]
    [InlineData(AddressServiceWrapper.ValidState.Unknown)]
    public async Task FindAddressUsingLastNameFirstNameStateAndDateOfBirth_ShouldThrowException_WhenResponseIsInvalid(
        AddressServiceWrapper.ValidState validState)
    {
        // Arrange
        var lastName = "Doe";
        var firstName = "John";
        var state = "NY";
        var dateOfBirth = new DateTime(2000, 1, 1);

        var mockResponse = new AddressListResponse
        {
            Valid = validState
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponse));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.FindAddressUsingLastNameFirstNameStateAndDateOfBirth(lastName, firstName, state, dateOfBirth));
    }

    [Fact]
    public async Task FindAddressUsingLastNameFirstNameStateAndDateOfBirth_ShouldThrowException_WhenResponseIsNull()
    {
        // Arrange
        var lastName = "Doe";
        var firstName = "John";
        var state = "NY";
        var dateOfBirth = new DateTime(2000, 1, 1);

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult<AddressListResponse?>(null));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() =>
            _helper.FindAddressUsingLastNameFirstNameStateAndDateOfBirth(lastName, firstName, state, dateOfBirth));
    }

    [Theory]
    [InlineData(null, null, null, null, null, null)]
    [InlineData("last", "first", "state", null, null, null)]
    [InlineData(null, "first", "state", 2000, 1, 1)]
    [InlineData("last", null, "state", 2000, 1, 1)]
    [InlineData("last", "first", null, 2000, 1, 1)]
    [InlineData("", "first", "state", 2000, 1, 1)]
    [InlineData("last", "", "state", 2000, 1, 1)]
    [InlineData("last", "first", "", 2000, 1, 1)]
    public async Task FindAddressUsingLastNameFirstNameStateAndDateOfBirth_ShouldReturnEmpty_WhenInputIsNullOrEmpty(
        string? lastName,
        string? firstName,
        string? state,
        int? year, int? month, int? day)
    {
        // Arrange
        DateTime? dateOfBirth = null;
        if (year.HasValue && month.HasValue && day.HasValue)
        {
            dateOfBirth = new(year.Value, month.Value, day.Value);
        }

        // Act
        var result = await _helper.FindAddressUsingLastNameFirstNameStateAndDateOfBirth(lastName, firstName, state, dateOfBirth);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindAddressesForJpapTriageAsync_ShouldReturnAddresses_WhenPatientDemographicIdIsValid()
    {
        // Arrange
        var patientDemographicId = "123";
        var carePathPatientId = "456";
        var dateOfBirth = new DateTime(2000, 1, 1);
        var lastName = "Doe";
        var firstName = "John";
        var state = "NY";

        var mockResponse = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = [new AddressServiceWrapper.Address()]
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponse));

        // Act
        var result = await _helper.FindAddressesForJpapTriageAsync(
            patientDemographicId, carePathPatientId, dateOfBirth, lastName, firstName, state);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task FindAddressesForJpapTriageAsync_ShouldReturnAddresses_WhenCarePathPatientIdIsValid()
    {
        // Arrange
        var patientDemographicId = "123";
        var carePathPatientId = "456";
        var dateOfBirth = new DateTime(2000, 1, 1);
        var lastName = "Doe";
        var firstName = "John";
        var state = "NY";

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
        var result = await _helper.FindAddressesForJpapTriageAsync(
            patientDemographicId, carePathPatientId, dateOfBirth, lastName, firstName, state);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task FindAddressesForJpapTriageAsync_ShouldReturnAddresses_WhenLastNameFirstNameStateIsValid()
    {
        // Arrange
        var patientDemographicId = "123";
        var carePathPatientId = "456";
        var dateOfBirth = new DateTime(2000, 1, 1);
        var lastName = "Doe";
        var firstName = "John";
        var state = "NY";

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
        var result = await _helper.FindAddressesForJpapTriageAsync(
            patientDemographicId, carePathPatientId, dateOfBirth, lastName, firstName, state);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task FindAddressesForJpapTriageAsync_ShouldReturnEmpty_WhenNoValidAddresses()
    {
        // Arrange
        var patientDemographicId = "123";
        var carePathPatientId = "456";
        var dateOfBirth = new DateTime(2000, 1, 1);
        var lastName = "Doe";
        var firstName = "John";
        var state = "NY";

        var mockResponseEmpty = new AddressListResponse
        {
            Valid = AddressServiceWrapper.ValidState.Ok,
            Address = []
        };

        _mockAddressService.SearchAddressAsync(Arg.Any<AddressListSearch>())
            .Returns(Task.FromResult(mockResponseEmpty));

        // Act
        var result = await _helper.FindAddressesForJpapTriageAsync(
            patientDemographicId, carePathPatientId, dateOfBirth, lastName, firstName, state);

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
    public async Task CreateAddressFromJpapTriageAsync_ShouldReturnAddressId_WhenInputIsValid()
    {
        // Arrange
        var jpapTriage = new JpapTriage
        {
            PatientFirstName = "John",
            PatientLastName = "Doe",
            DerivedPatientDateOfBirth = DateTime.Now,
            DerivedPatientGender = "M",
            ShipToLocation = "Location",
            PatientAddress1 = "123 Main St",
            PatientAddress2 = "Apt 4",
            PatientCity = "City",
            PatientState = "State",
            PatientZip = "12345",
            PrescriberFirstName = "Dr",
            PrescriberLastName = "Smith",
            PrescriberAddress1 = "456 Elm St",
            PrescriberAddress2 = "Suite 5",
            PrescriberCity = "City",
            PrescriberState = "State",
            PrescriberZip = "67890",
            PrescriberPhone = "123-456-7890",
            PrescriberFax = "098-765-4321",
            PrescriberNpi = "1234567890",
            PrescriberDea = "DEA1234567",
            CarePathPatientId = "CP123",
            PatientPhoneNumber = "555-555-5555",
            PatientDemographicId = "PD123"
        };

        var response = new AddressListResponse
        {
            Address = [new AddressServiceWrapper.Address { address_id = "1" }],
            Valid = AddressServiceWrapper.ValidState.Ok
        };

        _mockAddressService.UpdateAddressAsync(Arg.Any<AddressListUpdate>()).Returns(Task.FromResult(response));

        // Act
        var result = await _helper.CreateAddressFromJpapTriageAsync(jpapTriage, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }

    [Theory]
    [InlineData(AddressServiceWrapper.ValidState.Error)]
    [InlineData(AddressServiceWrapper.ValidState.Unknown)]
    public async Task CreateAddressFromJpapTriageAsync_ShouldThrowException_WhenResponseIsInvalid(AddressServiceWrapper.ValidState validState)
    {
        // Arrange
        var jpapTriage = new JpapTriage();
        var response = new AddressListResponse
        {
            Valid = validState
        };

        _mockAddressService.UpdateAddressAsync(Arg.Any<AddressListUpdate>()).Returns(Task.FromResult(response));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.CreateAddressFromJpapTriageAsync(jpapTriage, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAddressFromJpapTriageAsync_ShouldThrowException_WhenResponseIsNull()
    {
        // Arrange
        var jpapTriage = new JpapTriage();
        AddressListResponse? response = null;

        _mockAddressService.UpdateAddressAsync(Arg.Any<AddressListUpdate>()).Returns(Task.FromResult(response));

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.CreateAddressFromJpapTriageAsync(jpapTriage, CancellationToken.None));
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
    public async Task UpdateAddressFromJpapTriageAsync_ShouldUpdateAddressSuccessfully()
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
        var jpapTriage = new JpapTriage
        {
            PrescriberFirstName = "John",
            PrescriberLastName = "Doe",
            PrescriberAddress1 = "123 Main St",
            PrescriberCity = "Anytown",
            PrescriberState = "NY",
            PrescriberZip = "12345",
            PrescriberPhone = "555-1234",
            PrescriberFax = "555-5678",
            PrescriberNpi = "1234567890",
            PrescriberDea = "AB1234567",
            ShipToLocation = "Location1",
            PatientAddress1 = "456 Elm St",
            PatientCity = "Othertown",
            PatientState = "CA",
            PatientZip = "67890",
            CarePathPatientId = "CP123",
            PatientDemographicId = "PD456",
            PatientPhoneNumber = "555-6789"
        };
        var cancellationToken = new CancellationToken();

        _mockAddressService.UpdateAddressAsync(Arg.Any<AddressListUpdate>())
            .Returns(new AddressListResponse
            {
                Valid = AddressServiceWrapper.ValidState.Ok
            });

        // Act
        await _helper.UpdateAddressFromJpapTriageAsync(address, jpapTriage, cancellationToken);

        // Assert
        await _mockAddressService.Received(1).UpdateAddressAsync(Arg.Any<AddressListUpdate>());
    }

    [Fact]
    public async Task UpdateAddressFromJpapTriageAsync_ShouldThrowException_WhenApiResponseIsNull()
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
        var jpapTriage = new JpapTriage();
        var cancellationToken = new CancellationToken();

        _mockAddressService.UpdateAddressAsync(Arg.Any<AddressListUpdate>())
            .Returns((AddressListResponse?)null);

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => 
            _helper.UpdateAddressFromJpapTriageAsync(address, jpapTriage, cancellationToken));
    }

    [Fact]
    public async Task CreateCaseFromJpapTriageAsync_ShouldCreateCaseSuccessfully()
    {
        // Arrange
        var addressId = 1;
        var jpapTriage = new JpapTriage
        {
            PrimaryDiagnosisCode = "D123",
            CarePathTransactionId = "CP123",
            DerivedCaseText = "Case Text",
            DerivedProgramHeader = "Program Header",
            DerivedDrugName = "Drug Name",
            TreatmentCenterName = "Center Name",
            TreatmentCenterContactFirstName = "First Name",
            TreatmentCenterContactLastName = "Last Name",
            TreatmentCenterPhone = "1234567890",
            TreatmentCenterFax = "0987654321",
            TreatmentCenterAddress1 = "Address 1",
            TreatmentCenterAddress2 = "Address 2",
            TreatmentCenterCity = "City",
            TreatmentCenterState = "State",
            TreatmentCenterZip = "12345",
            TreatmentCenterNpi = "NPI123",
            TreatmentCenterDea = "DEA123",
            NdcCode = "NDC123"
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
        var result = await _helper.CreateCaseFromJpapTriageAsync(addressId, jpapTriage, cancellationToken);

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
        var addressId = 1;
        var jpapTriage = new JpapTriage();
        var cancellationToken = new CancellationToken();

        var caseListResponse = new CaseListResponse
        {
            Case = null,
            Valid = validState
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>())
            .Returns(caseListResponse);

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.CreateCaseFromJpapTriageAsync(addressId, jpapTriage, cancellationToken));
    }

    [Fact]
    public async Task UpdateCaseFromJpapTriageAsync_ShouldUpdateCaseSuccessfully()
    {
        // Arrange
        var caseId = 123;
        var jpapTriage = new JpapTriage { };
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
        await _helper.UpdateCaseFromJpapTriageAsync(caseId, jpapTriage, cancellationToken);

        // Assert
        await _mockCaseService.Received(1).UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>());
    }

    [Theory]
    [InlineData(CaseServiceWrapper.ValidState.Error)]
    [InlineData(CaseServiceWrapper.ValidState.Unknown)]
    public async Task UpdateCaseFromJpapTriageAsync_ShouldThrowException_WhenApiReturnsInvalidResponse(
        CaseServiceWrapper.ValidState validState)
    {
        // Arrange
        var caseId = 123;
        var jpapTriage = new JpapTriage { };
        var cancellationToken = CancellationToken.None;

        var caseListResponse = new CaseListResponse
        {
            Case = null,
            Valid = validState
        };

        _mockCaseService.UpdateCaseAsync(Arg.Any<CaseListUpdateRequest>()).Returns(caseListResponse);

        // Act & Assert
        await Assert.ThrowsAsync<GeneralApiException>(() => _helper.UpdateCaseFromJpapTriageAsync(caseId, jpapTriage, cancellationToken));
    }
}
