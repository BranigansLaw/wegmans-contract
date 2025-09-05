using AddressServiceWrapper;
using CaseServiceWrapper;
using CaseStreamServiceWrapper;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Exceptions;
using Library.TenTenInterface.DataModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Net.Sockets;

namespace Library.EmplifiInterface.Helper;

public class JpapTriageHelperImp : IJpapTriageHelper
{
    private readonly IOptions<EmplifiConfig> _config;
    private readonly ILogger<JpapTriageHelperImp> _logger;
    private readonly ICaseService _caseService;
    private readonly IAddressService _addressService;
    private readonly ICaseStreamService _caseStreamService;
    private readonly AsyncRetryPolicy RetryPolicy;

    private const string CaseStatusOpen = "O";
    private const string ProgramType = "JPAP";
    private const string AddressTypePatient = "PATIENT";
    private const string PatientStatusNewReferral = "P1=New Referral";
    private const string PhoneTypeCodeJpapCarePathPatientId = "JPAP CarePath Patient ID";
    private const string PhoneTypeCodeDemographicId = "JCP ID";
    private const string PhoneTypeCodePhoneNumber = "Phone#";
    private const string CompanyId = "SYS";
    private const string PmpOptOut = "Yes, Continue Refill Reminders";
    private const string WorkflowStatusNewReferral = "New Referral";
    private const string IssueSourceTriage = "Triage";
    private const string ReferredToUserCodeTriageExceptions = "triageexceptions";
    private const string ActionTypeCodeNoPefReceived = "no PEF received";

    public JpapTriageHelperImp(
    IOptions<EmplifiConfig> config,
    ILogger<JpapTriageHelperImp> logger,
    ICaseService caseService,
    IAddressService addressService,
    ICaseStreamService caseStreamService)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

    /// <inheritdoc />
    public DateTime? DerivePatientDateOfBirth(
        string? patientDateOfBirth)
    {
        if (DateTime.TryParseExact(patientDateOfBirth, "MMddyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedPatientDateOfBirth))
        {
            return parsedPatientDateOfBirth;
        }

        return null;
    }

    /// <inheritdoc />
    public string? DeriveCaseText(
        string? carePathPatientId,
        string? patientFirstName,
        string? patientLastName,
        DateTime? patientDateOfBirth,
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
        string? carePathTransactionId)
    {
        return $"CarePath Patient ID: {carePathPatientId}{Environment.NewLine}" +
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
    }

    /// <inheritdoc />
    public string DeriveProgramHeader(
        string? drugNdc,
        IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows)
    {
        var completeSpecialtyItemRow = completeSpecialtyItemRows.Where(r => r.NdcWo == drugNdc);

        if (completeSpecialtyItemRow is not null && completeSpecialtyItemRow.Any())
        {
            return completeSpecialtyItemRow.First().ProgramHeader ?? string.Empty;
        }

        return string.Empty;
    }

    /// <inheritdoc />
    public string DeriveDrugName(
        string? drugNdc,
        IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows)
    {
        var completeSpecialtyItemRow = completeSpecialtyItemRows.Where(r => r.NdcWo == drugNdc);

        if (completeSpecialtyItemRow is not null && completeSpecialtyItemRow.Any())
        {
            return completeSpecialtyItemRow.First().ActualDrugName ?? string.Empty;
        }

        return string.Empty;
    }

    /// <inheritdoc />
    public string DerivePatientGender(
        string? patientGender)
    {
        string genderIn = string.Empty;

        if (!string.IsNullOrWhiteSpace(patientGender))
        {
            genderIn = patientGender.ToUpper();
        }

        return genderIn switch
        {
            "M" => "Male",
            "F" => "Female",
            _ => string.Empty,
        };
    }

    /// <inheritdoc />
    public string DeriveImageFileNamePattern(
        string imageFileNamePattern,
        string? patientDemographicId,
        string? carePathTransactionId,
        string imageFileNameExtension)
    {
        if (string.IsNullOrWhiteSpace(imageFileNamePattern))
        {
            throw new ArgumentNullException(nameof(imageFileNamePattern));
        }

        if (string.IsNullOrWhiteSpace(imageFileNameExtension))
        {
            throw new ArgumentNullException(nameof(imageFileNameExtension));
        }

        if (string.IsNullOrWhiteSpace(patientDemographicId) || string.IsNullOrWhiteSpace(carePathTransactionId))
        {
            return string.Empty;
        }

        return $"{imageFileNamePattern}{patientDemographicId}_{carePathTransactionId}_*{imageFileNameExtension}";
    }

    /// <inheritdoc />
    public string GetDigits(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }

    /// <inheritdoc />
    public AddressListSearch CreateAddressListSearchRequest()
    {
        return new AddressListSearch
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            company_id = CompanyId,
            Type = RequestType.Get,
            Address = new AddressServiceWrapper.SearchAddress
            {
                company_id = CompanyId,
                allow_survey = SearchYesNo.B
            },
            ResponseFormat = new AddressResponseFormat()
            {
                AddressList = new AddressServiceWrapper.AddressListFormat
                {
                    Address = new AddressServiceWrapper.AddressFormat()
                    {
                        AllAttributes = AddressServiceWrapper.TrueFalseType.Item1,
                        PhoneList =
                        [
                            new AddressServiceWrapper.PhoneFormat()
                            {
                                AllAttributes = AddressServiceWrapper.TrueFalseType.Item1
                            }
                        ]
                    }
                }
            }
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressUsingPatientIdAndDateOfBirth(
        string? patientIdType,
        string? patientId,
        DateTime? dateOfBirth)
    {
        if (!string.IsNullOrWhiteSpace(patientIdType) &&
            !string.IsNullOrWhiteSpace(patientId) &&
            dateOfBirth.HasValue)
        {
            var request = CreateAddressListSearchRequest();

            request.Address.PhoneList = new AddressServiceWrapper.PhoneList()
            {
                Phone =
                [
                    new AddressServiceWrapper.Phone()
                    {
                        phone_type_code = patientIdType,
                        // Use asterisks to do an exact match in the phone number field used for patient ID's, otherwise a partial match happens
                        phone = string.Format("*{0}*", patientId)
                    }
                ]
            };

            request.Address.a05_code = Convert.ToDateTime(dateOfBirth).ToString("MM/dd/yyyy");

            var response = await RetryPolicy.ExecuteAsync(() =>
                _addressService.SearchAddressAsync(request)).ConfigureAwait(false) ?? 
                throw new GeneralApiException("SearchAddressAsync failed with no response from the API");

            if (response.Valid != AddressServiceWrapper.ValidState.Ok)
            {
                var apiException = new ApiResponse(
                    response.Valid.ToString(),
                    response.MessageList?.Select(x => new ApiResponseMessage
                    {
                        Message = x.Text,
                        Substitutions = x.Substitution?.Select(y => y.Text).ToList()
                    }).ToList()).BuildExceptionMessage();

                throw new GeneralApiException($"SearchAddressAsync failed with {apiException}");
            }

            if (response.Address is not null)
            {
                return response.Address;
            }
        }

        return [];
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressUsingLastNameFirstNameStateAndDateOfBirth(
        string? lastName,
        string? firstName,
        string? state,
        DateTime? dateOfBirth)
    {
        if (!string.IsNullOrWhiteSpace(lastName) &&
            !string.IsNullOrWhiteSpace(firstName) &&
            !string.IsNullOrWhiteSpace(state) &&
            dateOfBirth.HasValue)
        {
            var request = CreateAddressListSearchRequest();

            request.Address.last_name = lastName;
            request.Address.given_names = firstName;
            request.Address.a05_code = dateOfBirth.Value.ToString("MM/dd/yyyy");
            request.Address.state = state;

            var response = await RetryPolicy.ExecuteAsync(() =>
                _addressService.SearchAddressAsync(request)).ConfigureAwait(false) ?? 
                throw new GeneralApiException("SearchAddressAsync failed with no response from the API");

            if (response.Valid != AddressServiceWrapper.ValidState.Ok)
            {
                var apiException = new ApiResponse(
                    response.Valid.ToString(),
                    response.MessageList?.Select(x => new ApiResponseMessage
                    {
                        Message = x.Text,
                        Substitutions = x.Substitution?.Select(y => y.Text).ToList()
                    }).ToList()).BuildExceptionMessage();

                throw new GeneralApiException($"SearchAddressAsync failed with {apiException}");
            }

            if (response.Address is not null)
            {
                return response.Address;
            }
        }

        return [];
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressesForJpapTriageAsync(
        string? patientDemographicId,
        string? carePathPatientId,
        DateTime? dateOfBirth,
        string? lastName,
        string? firstName,
        string? state)
    {
        IEnumerable<AddressServiceWrapper.Address> addressList;

        addressList = await FindAddressUsingPatientIdAndDateOfBirth(PhoneTypeCodeDemographicId, patientDemographicId, dateOfBirth).ConfigureAwait(false);

        if (addressList.Any())
        {
            return addressList;
        }

        addressList = await FindAddressUsingPatientIdAndDateOfBirth(PhoneTypeCodeJpapCarePathPatientId, carePathPatientId, dateOfBirth).ConfigureAwait(false);

        if (addressList.Any())
        {
            return addressList;
        }

        addressList = await FindAddressUsingLastNameFirstNameStateAndDateOfBirth(lastName, firstName, state, dateOfBirth).ConfigureAwait(false);

        return addressList;
    }

    /// <inheritdoc />
    public async Task PostActionAsync(String actionTypeCode, string referredToUserCode, int caseId, DateTime responseDue, CancellationToken c)
    {
        var request = new PostActionListRequest()
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = PostActionListType.Update,
            PostAction = new PostAction()
            {
                APCWEditMode = CaseServiceWrapper.APCWEditModeType.New,
                company_id = CompanyId,
                case_id = caseId,
                action_type_code = actionTypeCode,
                referred_to_user_code = referredToUserCode,
                response_due = responseDue.ToString()
            },
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
            _caseService.PostActionAsync(request)).ConfigureAwait(false) ?? 
            throw new GeneralApiException($"PostActionAsync for case [{caseId}] failed with with no response from the API");

        if (response.Valid != CaseServiceWrapper.ValidState.Ok)
        {
            var apiException = new ApiResponse(
                response.Valid.ToString(),
                response.MessageList?.Select(x => new ApiResponseMessage
                {
                    Message = x.Text,
                    Substitutions = x.Substitution?.Select(y => y.Text).ToList()
                }).ToList()).BuildExceptionMessage();

            throw new GeneralApiException($"PostActionAsync failed for case [{caseId}] with {apiException}");
        }

        await ReleaseCaseAsync(caseId, c).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ReleaseCaseAsync(int caseId, CancellationToken c)
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

    /// <inheritdoc />
    public CaseGetRequest CreateCaseGetRequest(string programType, string caseStatus)
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

    /// <inheritdoc />
    public async Task<int> CreateAddressFromJpapTriageAsync(JpapTriage jpapTriage, CancellationToken c)
    {
        var request = new AddressListUpdate
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = RequestType.Update,
            Address = new AddressServiceWrapper.Address()
            {
                APCWEditMode = AddressServiceWrapper.APCWEditModeType.New,
                company_id = CompanyId,
                active = AddressServiceWrapper.YesNo.Y,
                address_type_code = AddressTypePatient,
                given_names = jpapTriage.PatientFirstName,
                last_name = jpapTriage.PatientLastName,
                a05_code = jpapTriage.DerivedPatientDateOfBirth.ToString(),
                a20_code = jpapTriage.DerivedPatientGender,
                a53_code = jpapTriage.ShipToLocation,
                address1 = jpapTriage.PatientAddress1,
                address2 = jpapTriage.PatientAddress2,
                city = jpapTriage.PatientCity,
                state = jpapTriage.PatientState,
                postal_code = jpapTriage.PatientZip,
                a22_code = jpapTriage.PrescriberFirstName,
                a23_code = jpapTriage.PrescriberLastName,
                a24_code = jpapTriage.PrescriberAddress1,
                a25_code = jpapTriage.PrescriberAddress2,
                a27_code = jpapTriage.PrescriberCity,
                a28_code = jpapTriage.PrescriberState,
                a29_code = jpapTriage.PrescriberZip,
                a30_code = jpapTriage.PrescriberPhone,
                a31_code = jpapTriage.PrescriberFax,
                a36_code = jpapTriage.PrescriberNpi,
                a34_code = jpapTriage.PrescriberDea
            },
            ResponseFormat = new AddressResponseFormat()
            {
                AddressList = new AddressServiceWrapper.AddressListFormat()
                {
                    Address = new AddressServiceWrapper.AddressFormat()
                    {
                        AllAttributes = AddressServiceWrapper.TrueFalseType.Item1
                    }
                }
            }
        };

        var phoneList = new List<AddressServiceWrapper.Phone>();

        if (!string.IsNullOrEmpty(jpapTriage.CarePathPatientId))
        {
            phoneList.Add(new AddressServiceWrapper.Phone()
            {
                phone_type_code = PhoneTypeCodeJpapCarePathPatientId,
                phone = jpapTriage.CarePathPatientId
            });
        }

        if (!string.IsNullOrEmpty(jpapTriage.PatientPhoneNumber))
        {
            phoneList.Add(new AddressServiceWrapper.Phone()
            {
                phone_type_code = PhoneTypeCodePhoneNumber,
                phone = jpapTriage.PatientPhoneNumber
            });
        }

        if (!string.IsNullOrEmpty(jpapTriage.PatientDemographicId))
        {
            phoneList.Add(new AddressServiceWrapper.Phone()
            {
                phone_type_code = PhoneTypeCodeDemographicId,
                phone = jpapTriage.PatientDemographicId
            });
        }

        request.Address.PhoneList = new AddressServiceWrapper.PhoneList()
        {
            Phone = phoneList.ToArray()
        };

        var response = await RetryPolicy.ExecuteAsync(() =>
            _addressService.UpdateAddressAsync(request)).ConfigureAwait(false) ??
                throw new GeneralApiException("UpdateAddressAsync failed with no response from the API");

        if (response.Address is null ||
            response.Valid == AddressServiceWrapper.ValidState.Error ||
            response.Valid == AddressServiceWrapper.ValidState.Unknown)
        {
            var apiException = new ApiResponse(
                response.Valid.ToString(),
                response.MessageList?.Select(x => new ApiResponseMessage
                {
                    Message = x.Text,
                    Substitutions = x.Substitution?.Select(y => y.Text).ToList()
                }).ToList()).BuildExceptionMessage();

            throw new GeneralApiException($"UpdateAddressAsync failed with {apiException}");
        }

        return Convert.ToInt32(response.Address[0].address_id);
    }

    /// <inheritdoc />
    public async Task UploadJpapTriageImageFilesAndAttachToCaseAsync(int caseId, List<TriageImage>? jpapTriageImages, CancellationToken c)
    {
        int imagesLoaded = 0;

        if (jpapTriageImages is not null)
        {
            var attachmentDescription = $"Patient Enrollment Form {DateTime.Today:MMddyyyy}";
            foreach (var jpapTriageImage in jpapTriageImages)
            {
                var attachmentName = await UploadFileAsync(jpapTriageImage.ImageFileName, jpapTriageImage.Image, caseId).ConfigureAwait(false);
                await AddAttachmentToCaseAsync(caseId, attachmentName, attachmentDescription, c).ConfigureAwait(false);
                imagesLoaded++;
            }
        }

        // If there are no images, post an action to the case to let the team know we did not receive any images from the vendor
        if (imagesLoaded == 0)
        {
            await PostActionAsync(ActionTypeCodeNoPefReceived, ReferredToUserCodeTriageExceptions, caseId, DateTime.Now, c).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<string> UploadFileAsync(string sourceFileName, Stream fileByteStream, int caseId)
    {
        var request = new FileRequest()
        {
            FileSize = string.Empty,
            ID = caseId.ToString(),
            Method = FileMethodType.UploadAutoName,
            Overwrite = "false",
            Password = _config.Value.Password,
            Prefix = string.Empty,
            ServerPath = FilePathType.Attachments,
            SourceFileName = sourceFileName,
            UserLanguageID = "en",
            UserName = _config.Value.Username,
            company_id = CompanyId,
            FileByteStream = fileByteStream
        };

        FileResponse response;

        try
        {
            response = await RetryPolicy.ExecuteAsync(() =>
                _caseStreamService.UploadFileAsync(request)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new FileUploadException(ex.Message, sourceFileName);
        }

        if (response is not null && response.Valid == CaseStreamServiceWrapper.ValidState.Ok)
        {
            return response.FileName;
        }
        else
        {
            var status = response is null ? string.Empty : response.Valid.ToString();
            throw new GeneralApiException($"UploadFileAsync failed with API status [{status}].");
        }
    }

    /// <inheritdoc />
    public async Task AddAttachmentToCaseAsync(int caseId, string attachmentName, string attachmentDescription, CancellationToken c)
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
                    case_id = caseId.ToString(),
                    CaseAttachmentList = new CaseAttachmentList()
                    {
                        CaseAttachment =
                        [
                            new CaseAttachment()
                            {
                                description = attachmentDescription,
                                file_name = attachmentName,
                                file_type = CaseAttachmentFileType.Standard,
                                APCWEditMode = CaseServiceWrapper.APCWEditModeType.New
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
                        AllAttributes = CaseServiceWrapper.TrueFalseType.Item1
                    }
                }
            }
        };

        var response = await RetryPolicy.ExecuteAsync(() =>
            _caseService.UpdateCaseAsync(request)).ConfigureAwait(false) ??
                throw new GeneralApiException($"UpdateCaseAsync failed for case ID [{caseId}] with no response from the API");

        if (response.Valid == CaseServiceWrapper.ValidState.Error ||
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

    /// <inheritdoc />
    public async Task<List<Case>> FindCasesByAddressIdProgramAndStatusAsync(int addressId, string programType, string caseStatus)
    {
        var caseList = new List<Case>();

        if (addressId <= 0 || string.IsNullOrEmpty(programType) || string.IsNullOrEmpty(caseStatus))
        {
            return caseList;
        }

        var request = CreateCaseGetRequest(programType, caseStatus);

        request.Case.address_id = addressId.ToString();

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
    public async Task UpdateAddressFromJpapTriageAsync(AddressServiceWrapper.Address address, JpapTriage jpapTriage, CancellationToken c)
    {
        var request = new AddressListUpdate
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = RequestType.Update,
            Address = new AddressServiceWrapper.Address()
            {
                APCWEditMode = AddressServiceWrapper.APCWEditModeType.Modified,
                company_id = CompanyId,
                address_id = address.address_id,
                active = AddressServiceWrapper.YesNo.Y,
                a22_code = jpapTriage.PrescriberFirstName ?? string.Empty,
                a23_code = jpapTriage.PrescriberLastName ?? string.Empty,
                a24_code = jpapTriage.PrescriberAddress1 ?? string.Empty,
                a25_code = jpapTriage.PrescriberAddress2 ?? string.Empty,
                a27_code = jpapTriage.PrescriberCity ?? string.Empty,
                a28_code = jpapTriage.PrescriberState ?? string.Empty,
                a29_code = jpapTriage.PrescriberZip ?? string.Empty,
                a30_code = jpapTriage.PrescriberPhone ?? string.Empty,
                a31_code = jpapTriage.PrescriberFax ?? string.Empty,
                a36_code = jpapTriage.PrescriberNpi ?? string.Empty,
                a34_code = jpapTriage.PrescriberDea ?? string.Empty,
                a53_code = jpapTriage.ShipToLocation ?? string.Empty,
                address1 = jpapTriage.PatientAddress1 ?? string.Empty,
                address2 = jpapTriage.PatientAddress2 ?? string.Empty,
                city = jpapTriage.PatientCity ?? string.Empty,
                state = jpapTriage.PatientState ?? string.Empty,
                postal_code = jpapTriage.PatientZip ?? string.Empty
            },
            ResponseFormat = new AddressResponseFormat()
            {
                AddressList = new AddressServiceWrapper.AddressListFormat()
                {
                    Address = new AddressServiceWrapper.AddressFormat()
                    {
                        AllAttributes = AddressServiceWrapper.TrueFalseType.Item1
                    }
                }
            }
        };

        var phoneList = new List<AddressServiceWrapper.Phone>();

        if (!string.IsNullOrEmpty(jpapTriage.CarePathPatientId))
        {
            var existingEntries = address.PhoneList.Phone.Where(x => x.phone_type_code == PhoneTypeCodeJpapCarePathPatientId);

            if (!existingEntries.Any())
            {
                phoneList.Add(new AddressServiceWrapper.Phone()
                {
                    phone_type_code = PhoneTypeCodeJpapCarePathPatientId,
                    phone = jpapTriage.CarePathPatientId
                });
            }
        }

        if (!string.IsNullOrEmpty(jpapTriage.PatientDemographicId))
        {
            var existingEntries = address.PhoneList.Phone.Where(x => x.phone_type_code == PhoneTypeCodeDemographicId);

            if (!existingEntries.Any())
            {
                phoneList.Add(new AddressServiceWrapper.Phone()
                {
                    phone_type_code = PhoneTypeCodeDemographicId,
                    phone = jpapTriage.PatientDemographicId
                });
            }
        }

        if (!string.IsNullOrEmpty(jpapTriage.PatientPhoneNumber))
        {
            var existingEntries = address.PhoneList.Phone
                .Where(x => x.phone_type_code == PhoneTypeCodePhoneNumber &&
                    !string.IsNullOrEmpty(x.phone) &&
                    GetDigits(x.phone) == GetDigits(jpapTriage.PatientPhoneNumber));

            if (!existingEntries.Any())
            {
                phoneList.Add(new AddressServiceWrapper.Phone()
                {
                    phone_type_code = PhoneTypeCodePhoneNumber,
                    phone = jpapTriage.PatientPhoneNumber
                });
            }
        }

        if (phoneList.Count > 0)
        {
            request.Address.PhoneList = new AddressServiceWrapper.PhoneList()
            {
                Phone = phoneList.ToArray()
            };
        }

        var response = await RetryPolicy.ExecuteAsync(() =>
            _addressService.UpdateAddressAsync(request)).ConfigureAwait(false) ??
                throw new GeneralApiException("UpdateAddressAsync failed with no response from the API");

        if (response.Valid == AddressServiceWrapper.ValidState.Error ||
            response.Valid == AddressServiceWrapper.ValidState.Unknown)
        {
            var apiException = new ApiResponse(
                response.Valid.ToString(),
                response.MessageList?.Select(x => new ApiResponseMessage
                {
                    Message = x.Text,
                    Substitutions = x.Substitution?.Select(y => y.Text).ToList()
                }).ToList()).BuildExceptionMessage();

            throw new GeneralApiException($"UpdateAddressAsync failed with {apiException}");
        }
    }

    /// <inheritdoc />
    public async Task<int> CreateCaseFromJpapTriageAsync(int addressId, JpapTriage jpapTriage, CancellationToken c)
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
                    address_id = addressId.ToString(),
                    b07_code = ProgramType,
                    case_status = CaseStatusOpen,
                    b41_code = PmpOptOut,
                    b36_code = jpapTriage.PrimaryDiagnosisCode,
                    b47_code = jpapTriage.CarePathTransactionId,
                    CaseTextList = new CaseTextList()
                    {
                        CaseText =
                        [
                            new CaseText()
                            {
                                text_type_code = WorkflowStatusNewReferral,
                                case_text = jpapTriage.DerivedCaseText
                            }
                        ]
                    },
                    IssueList = new IssueList()
                    {
                        Issue =
                        [
                            new Issue()
                            {
                                APCWEditMode = CaseServiceWrapper.APCWEditModeType.New,
                                c88_code = CaseStatusOpen,
                                c28_code = DateTime.Now.ToString(), // Triage Received Date
                                c89_code = IssueSourceTriage, // Issue Source
                                c47_code = jpapTriage.DerivedProgramHeader,
                                product_code = jpapTriage.DerivedDrugName,
                                c19_code = WorkflowStatusNewReferral, // Workflow Status
                                c35_code = DateTime.Now.ToString(), // Follow-Up Date
                                c16_code = PatientStatusNewReferral,
                                c26_code = DateTime.Now.ToString(), // Patient Status Changed Date
                                cc0_code = jpapTriage.TreatmentCenterName,
                                cb0_code = jpapTriage.TreatmentCenterContactFirstName,
                                cb1_code = jpapTriage.TreatmentCenterContactLastName,
                                cb2_code = jpapTriage.TreatmentCenterPhone,
                                cb3_code = jpapTriage.TreatmentCenterFax,
                                cb4_code = jpapTriage.TreatmentCenterAddress1,
                                cb5_code = jpapTriage.TreatmentCenterAddress2,
                                cb6_code = jpapTriage.TreatmentCenterCity,
                                cb7_code = jpapTriage.TreatmentCenterState,
                                cb8_code = jpapTriage.TreatmentCenterZip,
                                cc1_code = jpapTriage.TreatmentCenterNpi,
                                cb9_code = jpapTriage.TreatmentCenterDea,
                                c39_code = jpapTriage.NdcCode,
                                c97_code = jpapTriage.CarePathTransactionId
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

        return caseId;
    }

    /// <inheritdoc />
    public async Task UpdateCaseFromJpapTriageAsync(int caseId, JpapTriage jpapTriage, CancellationToken c)
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
                    case_id = caseId.ToString(),
                    case_status = CaseStatusOpen,
                    b36_code = jpapTriage.PrimaryDiagnosisCode ?? string.Empty,
                    b47_code = jpapTriage.CarePathTransactionId,
                    CaseTextList = new CaseTextList()
                    {
                        CaseText =
                        [
                            new CaseText()
                            {
                                text_type_code = WorkflowStatusNewReferral,
                                case_text = jpapTriage.DerivedCaseText
                            }
                        ]
                    },
                    IssueList = new IssueList()
                    {
                        Issue =
                        [
                            new Issue()
                            {
                                APCWEditMode = CaseServiceWrapper.APCWEditModeType.New,
                                c88_code = CaseStatusOpen,
                                c28_code = DateTime.Now.ToString(), // Triage Received Date
                                c89_code = IssueSourceTriage, // Issue Source
                                c47_code = jpapTriage.DerivedProgramHeader,
                                product_code = jpapTriage.DerivedDrugName,
                                c19_code = WorkflowStatusNewReferral, // Workflow Status
                                c35_code = DateTime.Now.ToString(), // Follow-Up Date
                                c16_code = PatientStatusNewReferral,
                                c26_code = DateTime.Now.ToString(), // Patient Status Changed Date
                                cc0_code = jpapTriage.TreatmentCenterName,
                                cb0_code = jpapTriage.TreatmentCenterContactFirstName,
                                cb1_code = jpapTriage.TreatmentCenterContactLastName,
                                cb2_code = jpapTriage.TreatmentCenterPhone,
                                cb3_code = jpapTriage.TreatmentCenterFax,
                                cb4_code = jpapTriage.TreatmentCenterAddress1,
                                cb5_code = jpapTriage.TreatmentCenterAddress2,
                                cb6_code = jpapTriage.TreatmentCenterCity,
                                cb7_code = jpapTriage.TreatmentCenterState,
                                cb8_code = jpapTriage.TreatmentCenterZip,
                                cc1_code = jpapTriage.TreatmentCenterNpi,
                                cb9_code = jpapTriage.TreatmentCenterDea,
                                c39_code = jpapTriage.NdcCode,
                                c97_code = jpapTriage.CarePathTransactionId
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
