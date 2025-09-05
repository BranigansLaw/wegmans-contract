using AddressServiceWrapper;
using CaseServiceWrapper;
using CaseStreamServiceWrapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly;
using System.Net.Sockets;
using Parquet.Meta;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Exceptions;
using System.ComponentModel.Design;
using Library.McKessonDWInterface.DataModel;
using Library.LibraryUtilities.Extensions;
using System.Globalization;
using Library.DataFileInterface.VendorFileDataModels;

namespace Library.EmplifiInterface.Helper;

public class VerificationOfBenefitsHelperImp : IVerificationOfBenefitsHelper
{
    private const string ActionTypeCodeNoImageReceived = "no VOB received";
    private const string ReferredToUserCode = "vobexceptions";
    private const string TextTypeCode = "exceptions";
    private const string TextTypeDescription = "2";
    private const string AddressTypeCodePatient = "PATIENT";
    private const string CompanyId = "SYS";
    private const string StatusOpen = "O";

    private readonly ILogger<VerificationOfBenefitsHelperImp> _logger;
    private readonly IOptions<EmplifiConfig> _config;
    private readonly ICaseService _caseService;
    private readonly IAddressService _addressService;
    private readonly ICaseStreamService _caseStreamService;
    private readonly AsyncRetryPolicy RetryPolicy;

    public VerificationOfBenefitsHelperImp(
        ILogger<VerificationOfBenefitsHelperImp> logger,
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

    public string DeriveCaseText(
        IbmVerificationOfBenefitsRow ibmVerificationOfBenefitsRow,
        DateTime? recordTimestamp,
        DateTime? patientEnrollmentFormReceived
        )
    {
        return $"RecordTimestamp:{recordTimestamp}{Environment.NewLine}" +
               $"CarePathSpecialtyPharmacyName:{ibmVerificationOfBenefitsRow.CarePathSpecialtyPharmacyName}{Environment.NewLine}" +
               $"CarePathPatientId:{ibmVerificationOfBenefitsRow.CarePathPatientId}{Environment.NewLine}" +
               $"PatientBirthYear:{ibmVerificationOfBenefitsRow.PatientBirthYear}{Environment.NewLine}" +
               $"PayerType:{ibmVerificationOfBenefitsRow.PayerType}{Environment.NewLine}" +
               $"SpecialtyPharmacyName:{ibmVerificationOfBenefitsRow.SpecialtyPharmacyName}{Environment.NewLine}" +
               $"SpecialtyPharmacyPhone:{ibmVerificationOfBenefitsRow.SpecialtyPharmacyPhone}{Environment.NewLine}" +
               $"ImageExists:{ibmVerificationOfBenefitsRow.ImageExists}{Environment.NewLine}" +
               $"CarePathCaseId:{ibmVerificationOfBenefitsRow.CarePathCaseId}{Environment.NewLine}" +
               $"PatientEnrollmentFormReceived:{patientEnrollmentFormReceived}{Environment.NewLine}" +
               $"ExternalPatientId:{ibmVerificationOfBenefitsRow.ExternalPatientId}{Environment.NewLine}" +
               $"ProductName:{ibmVerificationOfBenefitsRow.ProductName}{Environment.NewLine}" +
               $"DemographicId:{ibmVerificationOfBenefitsRow.DemographicId}{Environment.NewLine}";
    }

    /// <inheritdoc />
    public DateTime? DeriveLongDate(
        string? date)
    {
        if (DateTime.TryParseExact(date, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate;
        }
        return null;
    }

    /// <inheritdoc />
    public DateTime? DeriveNormalDate(
        string? date)
    {
        if (DateTime.TryParseExact(date, "yyyyMMdd", null, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate;
        }
        return null;
    }

    public string DeriveImageFileNamePattern(
    string imageFileNamePattern,
    string? patientDemographicId,
    string? caseId,
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

        if (string.IsNullOrWhiteSpace(patientDemographicId) || string.IsNullOrWhiteSpace(caseId))
        {
            return string.Empty;
        }


        return $"{imageFileNamePattern}{patientDemographicId}_{caseId}*{imageFileNameExtension}";
    }

    /// <inheritdoc />
    public string GetDigits(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        return new string(input.Where(char.IsDigit).ToArray());
    }

    /// <inheritdoc />
    private async Task AddAttachmentToCaseAsync(
        int caseId,
        string attachmentName,
        string attachmentDescription,
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

    public async Task AppendDataToNewFile(
        string file,
        string newFile)
    {
        try
        {
            string? inputFilePath = Path.Combine(_config.Value.InputFileLocation, file);
            string? archiveFilePath = Path.Combine(_config.Value.ArchiveFileLocation, file);
            string filePath = string.Empty;

            if (File.Exists(inputFilePath))
            {
                _logger.LogInformation($"File [{file}] found in DataFileImports");
                filePath = inputFilePath;
            }
            else if (File.Exists(archiveFilePath))
            {
                _logger.LogInformation($"File [{file}] found in DataFileArchives");
                filePath = archiveFilePath;
            }
            else
            {
                _logger.LogInformation($"File [{file}] does not exist in DataFileImports and DataFileArchives");
                return; 
            }
            
            await File.AppendAllTextAsync(newFile, await File.ReadAllTextAsync(filePath)).ConfigureAwait(false);
            string? originalPath = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(originalPath))
            {
                string? newPath = Path.Combine(originalPath, Path.GetFileName(newFile));
                File.Move(newFile, newPath, true);
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($"Error moving RERUN file. {e.Message}");
        }
    }

    /// <inheritdoc />
    public CaseGetRequest CreateCaseGetRequest(
        string? programType,
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

    /// <inheritdoc />
    public async Task<int> CreateCaseFromVerificationOfBenefitsAsync(
        VerificationOfBenefits verificationOfBenefits,
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
                    CaseTextList = new CaseTextList()
                    {
                        CaseText =
                        [
                            new CaseText()
                            {
                                text_type_code = TextTypeCode,
                                case_text = verificationOfBenefits.DerivedCaseText
                            }
                        ]
                    },
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

    private AddressList CreateAddressList(
        CaseServiceWrapper.Address existingAddress,
        List<PatientIdentifier>? patientIdentifiers)
    {
        var addressList = new AddressList();
        var phoneList = new List<CaseServiceWrapper.Phone>();
        var alternatePatientIds = existingAddress.Extended is null ? string.Empty : existingAddress.Extended.p03_code;
        bool alternatePatientIdsModified = false;

        if (patientIdentifiers is null)
        {
            return addressList;
        }

        foreach (var pi in patientIdentifiers)
        {
            var existingEntries = existingAddress.PhoneList.Phone.Where(x => x.phone_type_code == pi.Type);

            if (!existingEntries.Any())
            {
                phoneList.Add(new CaseServiceWrapper.Phone()
                {
                    phone_type_code = pi.Type,
                    phone = pi.Value
                });
            }
            else if (!existingEntries.Any(x => x.phone.Equals(pi.Value)) &&
                    (pi.Value is not null &&
                    !alternatePatientIds.Contains(pi.Value)))
            {
                if (!string.IsNullOrEmpty(alternatePatientIds))
                {
                    alternatePatientIds += ",";
                }
                alternatePatientIds += $"{pi.Type}:{pi.Value}";
                alternatePatientIdsModified = true;
            }
        }

        if (phoneList.Count > 0 || alternatePatientIdsModified)
        {
            addressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                        new CaseServiceWrapper.Address()
                        {
                            address_id = existingAddress.address_id,
                            APCWEditMode = CaseServiceWrapper.APCWEditModeType.Modified,
                            company_id = CompanyId,
                            active = CaseServiceWrapper.YesNo.Y
                        }
                }
            };

            if (phoneList.Count > 0)
            {
                addressList.Address[0].PhoneList = new CaseServiceWrapper.PhoneList()
                {
                    Phone = phoneList.ToArray()
                };
            }

            if (alternatePatientIdsModified)
            {
                addressList.Address[0].Extended = new CaseServiceWrapper.AddressExtended()
                {
                    p03_code = alternatePatientIds
                };
            }
        }
        return addressList;
    }

    public async Task<List<Case>> FindCasesByProgramTypePatientIdAndDob(
        string programType,
        string? birthYear,
        string caseStatus,
        List<PatientIdentifier>? patientIndentifiers)
    {
        var caseList = new List<Case>();

        if (patientIndentifiers is null || string.IsNullOrEmpty(caseStatus) || string.IsNullOrEmpty(birthYear) || string.IsNullOrEmpty(programType))
        {
            return caseList;
        }

        foreach (var pi in patientIndentifiers)
        {
            var request = CreateCaseGetRequest(programType, caseStatus);

            request.Case.AddressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                new CaseServiceWrapper.Address()
                {
                    PhoneList = new CaseServiceWrapper.PhoneList()
                    {
                        Phone = new CaseServiceWrapper.Phone[]
                        {
                            new CaseServiceWrapper.Phone()
                            {
                                phone_type_code = pi.Type,
                                phone = string.Format("*{0}*", pi.Value) // Use asterisks to do an exact match in the phone number field used for patient ID's, otherwise a partial match happens
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
                caseList.AddRange(response.Case
                                .Where(x => x.AddressList.Address.Any(y => y.address_type_code == AddressTypeCodePatient
                                    && !string.IsNullOrEmpty(y.a05_code)
                                    && DateTime.ParseExact(y.a05_code, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("yyyy") == birthYear)));

                if (caseList.Count > 0)
                {
                    return caseList;
                }
            }
        }
        return caseList;
    }

    /// <inheritdoc />
    public async Task PostActionAsync(
        string actionTypeCode,
        string referredToUserCode,
        int caseId,
        DateTime responseDue,
        CancellationToken c)
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
    private async Task ReleaseCaseAsync(
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

    public async Task UpdateCaseFromVerificationOfBenefitsAsync(
       Case caseRecord,
       int caseId,
       VerificationOfBenefits verificationOfBenefits,
       CancellationToken c)
    {
        var issueSequence = caseRecord.IssueList.Issue.OrderBy(s => Convert.ToInt32(s.issue_seq)).Last().issue_seq; // Get the last sequential issue to update

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
                    case_status = StatusOpen,
                    b12_code = DateTime.Now.ToString("MM/dd/yyyy"),
                    b13_code = verificationOfBenefits.SpecialtyPharmacyName,
                    b14_code = verificationOfBenefits.SpecialtyPharmacyPhone,
                    b76_code = verificationOfBenefits.PayerType,
                    IssueList = new IssueList()
                    {
                        Issue =
                        [
                            new Issue()
                            {
                                APCWEditMode = CaseServiceWrapper.APCWEditModeType.Modified,
                                issue_seq = issueSequence,
                                c14_code = verificationOfBenefits.RecordTimestamp.HasValue ? verificationOfBenefits.RecordTimestamp.Value.ToString("yyyyMMddHHmmss") : string.Empty,
                                c51_code = verificationOfBenefits.ImageExists,
                                c22_code = verificationOfBenefits.PatientEnrollmentFormReceived.HasValue ? verificationOfBenefits.PatientEnrollmentFormReceived.Value.ToString("MM/dd/yyyy") : string.Empty,
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

        if (verificationOfBenefits.PatientIdentifiers?.Count > 0 && caseRecord.AddressList.Address.Any(x => x.address_type_code == AddressTypeCodePatient))
        {
            request.Case[0].AddressList = CreateAddressList(caseRecord.AddressList.Address
                .Where(x => x.address_type_code == AddressTypeCodePatient)
                .OrderBy(s => Convert.ToInt32(s.address_id)).Last(), verificationOfBenefits.PatientIdentifiers);
        }

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


    /// <inheritdoc />
    public async Task UploadVerificationOfBenefitsImageFilesAndAttachToCaseAsync(
        int caseId,
        List<TriageImage>? verificationOfBenefitsImages,
        CancellationToken c)
    {
        int imagesLoaded = 0;

        if (verificationOfBenefitsImages is not null)
        {
            var attachmentDescription = $"Verification of Benefits {DateTime.Today:MMddyyyy}";
            foreach (var verificationOfBenefitsImage in verificationOfBenefitsImages)
            {
                var attachmentName = await UploadFileAsync(verificationOfBenefitsImage.ImageFileName, verificationOfBenefitsImage.Image, caseId).ConfigureAwait(false);
                await AddAttachmentToCaseAsync(caseId, attachmentName, attachmentDescription, c).ConfigureAwait(false);
                imagesLoaded++;
            }
        }

        // If there are no images, post an action to the case to let the team know we did not receive any images from the vendor
        if (imagesLoaded == 0)
        {
            await PostActionAsync(ActionTypeCodeNoImageReceived, ReferredToUserCode, caseId, DateTime.Now, c).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    private async Task<string> UploadFileAsync(
        string sourceFileName,
        Stream fileByteStream,
        int caseId)
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
}
