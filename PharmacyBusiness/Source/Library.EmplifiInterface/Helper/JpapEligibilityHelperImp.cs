using CaseServiceWrapper;
using CaseStreamServiceWrapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly;
using System.Net.Sockets;
using System.ComponentModel.Design;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Exceptions;
using AddressServiceWrapper;
using Library.LibraryUtilities.Extensions;
using System.Globalization;
using System.Linq;
using Library.DataFileInterface.VendorFileDataModels;
namespace Library.EmplifiInterface.Helper;

public class JpapEligibilityHelperImp : IJpapEligibilityHelper
{

    private readonly Dictionary<string, string> OutcomeMap = new(StringComparer.OrdinalIgnoreCase)
    {
            { "Pending Pt Contact - Alternate Support Outreach", "Pending PT Contact Alt Support Outreach" },
            { "Unable to Contact Pt - Alternate Support Outreach", "Unable to Contact PT Alt Support Outreac" },
            { "Pt Does not reside in the US or its Territories", "Pt Does not reside in US or Territories" }
    };

    private const string PatientIdType = "JPAP CarePath Patient ID";
    private const string ProgramType = "JPAP";
    private const string StatusAll = "B";
    private const string StatusOpen = "O";
    private const string TextTypeCode = "exceptions";
    private const string TextTypeDescription = "2";
    private const string ActionTypeCode = "case not found";
    private const string ReferredToUserCode = "paexceptions"; 
    private const string CompanyId = "SYS";

    private readonly ILogger<JpapEligibilityHelperImp> _logger;
    private readonly IOptions<EmplifiConfig> _config;
    private readonly ICaseService _caseService;
    private readonly IAddressService _addressService;
    private readonly ICaseStreamService _caseStreamService;
    private readonly AsyncRetryPolicy RetryPolicy;

    public JpapEligibilityHelperImp(
        ILogger<JpapEligibilityHelperImp> logger,
        IOptions<EmplifiConfig> config,
        ICaseService caseService,
        IAddressService addressService,
        ICaseStreamService caseStreamService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _caseService = caseService ?? throw new ArgumentNullException(nameof(caseService));
        _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
        _caseStreamService = caseStreamService ?? throw new ArgumentNullException(nameof(caseStreamService));

        RetryPolicy = Policy
            .Handle<HttpRequestException>(e => e.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            .Or<SocketException>() // Network error
            .WaitAndRetryAsync(
                retryCount: 10,
                retryAttempt => TimeSpan.FromSeconds(30 * retryAttempt), // retry after 30s, 60s, ... 5 minutes
                onRetry: (ex, waitTime) => _logger.LogWarning(ex, $"Retrying in {waitTime.Seconds} seconds")
            );
    }

    /// <inheritdoc/>
    public string DeriveCaseText(
        IbmJpapEligibilityRow ibmJpapEligibilityRow,
        DateTime? DateOfBirth
        )
    {
        return $"Record Timestamp:{ibmJpapEligibilityRow.RecordTimestamp}{Environment.NewLine}" +
                   $"Patient Program Enrollment Name:{ibmJpapEligibilityRow.PatientProgramEnrollmentName}{Environment.NewLine}" +
                   $"Status:{ibmJpapEligibilityRow.Status}{Environment.NewLine}" +
                   $"Enrollment Status:{ibmJpapEligibilityRow.EnrollmentStatus}{Environment.NewLine}" +
                   $"Outcome:{ibmJpapEligibilityRow.Outcome}{Environment.NewLine}" +
                   $"Product:{ibmJpapEligibilityRow.Product}{Environment.NewLine}" +
                   $"Date of Birth:{DateOfBirth}{Environment.NewLine}" +
                   $"Gender:{ibmJpapEligibilityRow.Gender}{Environment.NewLine}" +
                   $"Start Date:{ibmJpapEligibilityRow.StartDate}{Environment.NewLine}" +
                   $"End Date:{ibmJpapEligibilityRow.EndDate}{Environment.NewLine}" +
                   $"Patient Id:{ibmJpapEligibilityRow.PatientId}";
    }

    /// <inheritdoc />
    public DateTime? DeriveDate(
        string? date)
    {
        if (DateTime.TryParseExact(date, "yyyyMMdd", null, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate;
        }
        return null;
    }

    /// <inheritdoc/>
    public async Task<List<Case>> FindCasesByProgramTypePatientIdAndDob(
        string programType,
        string? patientId,
        DateTime? dateOfBirth,
        string caseStatus)
    {
        var caseList = new List<Case>();

        if (string.IsNullOrEmpty(programType) || string.IsNullOrEmpty(patientId) || string.IsNullOrEmpty(caseStatus) || !dateOfBirth.HasValue)
        {
            return caseList;
        }

        var request = CreateCaseGetRequest(programType, caseStatus);

        request.Case.AddressList = new AddressList()
        {
            Address = new CaseServiceWrapper.Address[]
            {
                new CaseServiceWrapper.Address()
                {
                    a05_code = dateOfBirth.Value.ToString("MM/dd/yyyy"),
                    PhoneList = new CaseServiceWrapper.PhoneList()
                    {
                        Phone = new CaseServiceWrapper.Phone[]
                        {
                            new CaseServiceWrapper.Phone()
                            {
                                phone_type_code = PatientIdType,
                                phone = string.Format("*{0}*", patientId) // Use asterisks to do an exact match in the phone number field used for patient ID's, otherwise a partial match happens
                            }
                        }
                    }
                }
            }
        };

        var response = await RetryPolicy.ExecuteAsync(() =>
            _caseService.GetCaseAsync(request)).ConfigureAwait(false) ??
                throw new GeneralApiException("GetCaseAsync failed with no response from the API");

        if (response.Valid != CaseServiceWrapper.ValidState.Ok)
        {
            var apiException = new ApiResponse(
                response.Valid.ToString(),
                response.MessageList?.Select(x => new ApiResponseMessage
                {
                    Message = x.Text,
                    Substitutions = x.Substitution?.Select(y => y.Text).ToList()
                }).ToList()).BuildExceptionMessage();

            throw new GeneralApiException($"GetCaseAsync failed with {apiException}");
        }

        if (response.Case is not null)
        {
            caseList.AddRange(response.Case);
        }

        return caseList;
    }

    /// <inheritdoc />
    public CaseGetRequest CreateCaseGetRequest(
        string programType,
        string caseStatus)
    {
        if (string.IsNullOrEmpty(programType))
        {
            throw new ArgumentNullException(nameof(programType));
        }

        if (string.IsNullOrEmpty(caseStatus))
        {
            throw new ArgumentNullException(nameof(caseStatus));
        }

        return new CaseGetRequest()
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = CaseGetType.Get,
            Case = new Case()
            {
                company_id = CompanyId,
                b07_code = programType,
                case_status = caseStatus
            },
            ResponseFormat = new ResponseFormat()
            {
                CaseList = new CaseListFormat()
                {
                    Case = new CaseFormat()
                    {
                        AllAttributes = CaseServiceWrapper.TrueFalseType.Item1,
                        AddressList = new CaseServiceWrapper.AddressListFormat()
                        {
                            Address = new CaseServiceWrapper.AddressFormat()
                            {
                                AllAttributes = CaseServiceWrapper.TrueFalseType.Item1,
                                PhoneList =
                                [
                                    new CaseServiceWrapper.PhoneFormat()
                                        {
                                            AllAttributes = CaseServiceWrapper.TrueFalseType.Item1
                                        }
                                ]
                            }
                        },
                        IssueList = new IssueListFormat()
                        {
                            Issue = new IssueFormat()
                            {
                                AllAttributes = CaseServiceWrapper.TrueFalseType.Item1
                            }
                        }
                    }
                }
            }
        };
    }

    /// <inheritdoc/>
    public async Task CreateBlankCase(
        JpapEligibilityRow jpapEligibilityRow,
        CancellationToken c)
    {
        var request = new CaseListUpdateRequest
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = CaseListUpdateType.Update,
            Case =
            [
                new Case()
                {
                    APCWEditMode = CaseServiceWrapper.APCWEditModeType.New,
                    company_id = CompanyId,
                    b07_code = ProgramType,
                    case_status = StatusOpen,
                    CaseTextList = new CaseTextList()
                    {
                        CaseText =
                        [
                            new CaseText()
                            {
                                text_type_code = TextTypeCode,
                                case_text = jpapEligibilityRow.DerivedCaseText
                            }
                        ]
                    }
                }
            ],
            ResponseFormat = new ResponseFormat()
            {
                CaseList = new CaseListFormat()
                {
                    Case = new CaseFormat()
                    {
                        AllAttributes = CaseServiceWrapper.TrueFalseType.Item1,
                    }
                }
            }
        };

        var response = await RetryPolicy.ExecuteAsync(() =>
            _caseService.UpdateCaseAsync(request)).ConfigureAwait(false) ??
                throw new GeneralApiException("UpdateCaseAsync failed with no response from the API");

        if (response.Case is null ||
            response.Valid == CaseServiceWrapper.ValidState.Error ||
            response.Valid == CaseServiceWrapper.ValidState.Unknown)
        {
            var apiException = new ApiResponse(
                response.Valid.ToString(),
                response.MessageList?.Select(x => new ApiResponseMessage
                {
                    Message = x.Text,
                    Substitutions = x.Substitution?.Select(y => y.Text).ToList()
                }).ToList()).BuildExceptionMessage();

            throw new GeneralApiException($"UpdateCaseAsync failed with {apiException}");
        }

        var caseId = Convert.ToInt32(response.Case[0].case_id);

        await ReleaseCaseAsync(caseId, c).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ReleaseCaseAsync(
        int caseId,
        CancellationToken c)
    {
        var request = new CaseUserListRequest()
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = CaseUserListType.Update,
            CaseUser = new CaseUser()
            {
                APCWEditMode = CaseServiceWrapper.APCWEditModeType.Delete,
                case_id = caseId,
                company_id = CompanyId,
                system_user_id = _config.Value.Username
            }
        };

        var response = await RetryPolicy.ExecuteAsync(() =>
            _caseService.ReleaseCaseAsync(request)).ConfigureAwait(false) ??
                throw new GeneralApiException($"ReleaseCaseAsync failed for case [{caseId}] with no response from the API");

        if (response.Valid != CaseServiceWrapper.ValidState.Ok)
        {
            var apiException = new ApiResponse(
                response.Valid.ToString(),
                response.MessageList?.Select(x => new ApiResponseMessage
                {
                    Message = x.Text,
                    Substitutions = x.Substitution?.Select(y => y.Text).ToList()
                }).ToList()).BuildExceptionMessage();

            throw new GeneralApiException($"ReleaseCaseAsync failed for case [{caseId}] with {apiException}");
        }
    }

    /// <inheritdoc/>
    public async Task UpdateCaseFromJpapEligibilityAsync(
            int caseId,
            JpapEligibilityRow jpapEligibilityRow,
            CancellationToken c)
    {
        var request = new CaseListUpdateRequest
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = CaseListUpdateType.Update,
            Case =
            [
                new Case()
                {
                    APCWEditMode = CaseServiceWrapper.APCWEditModeType.Modified,
                    company_id = CompanyId,
                    case_id = caseId.ToString() ?? string.Empty,
                    b11_code = jpapEligibilityRow.Status ?? string.Empty,
                    b75_code = jpapEligibilityRow.EnrollmentStatus ?? string.Empty,
                    b79_code = (!string.IsNullOrEmpty(jpapEligibilityRow.Outcome) 
                            && OutcomeMap.TryGetValue(jpapEligibilityRow.Outcome, out string? value)) ? value : jpapEligibilityRow.Outcome,
                    b24_code = jpapEligibilityRow.Product ?? string.Empty,
                    b42_code = DateTime.TryParseExact(jpapEligibilityRow.EndDate, "yyyyMMddHHmmss", 
                                                      CultureInfo.InvariantCulture, 
                                                      DateTimeStyles.None,
                                                      out DateTime result) ? result.ToString("MM/dd/yyyy") : string.Empty,
                }
            ],
            ResponseFormat = new ResponseFormat()
            {
                CaseList = new CaseListFormat()
                {
                    Case = new CaseFormat()
                    {
                        AllAttributes = CaseServiceWrapper.TrueFalseType.Item1
                    }
                }
            }
        };

        var response = await RetryPolicy.ExecuteAsync(() =>
            _caseService.UpdateCaseAsync(request)).ConfigureAwait(false) ??
                throw new GeneralApiException($"UpdateCaseAsync failed for case ID [{caseId}] with no response from the API");

        if (response.Case is null ||
            response.Valid == CaseServiceWrapper.ValidState.Error ||
            response.Valid == CaseServiceWrapper.ValidState.Unknown)
        {
            var apiException = new ApiResponse(
                response.Valid.ToString(),
                response.MessageList?.Select(x => new ApiResponseMessage
                {
                    Message = x.Text,
                    Substitutions = x.Substitution?.Select(y => y.Text).ToList()
                }).ToList()).BuildExceptionMessage();

            throw new GeneralApiException($"UpdateCaseAsync failed for case ID [{caseId}] with {apiException}");
        }

        await ReleaseCaseAsync(caseId, c).ConfigureAwait(false);
    }
}
