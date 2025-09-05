using AddressServiceWrapper;
using CaseServiceWrapper;
using CaseStreamServiceWrapper;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Exceptions;
using Library.TenTenInterface.DataModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Net.Sockets;

namespace Library.EmplifiInterface.Helper
{
    public class OncologyVoucherTriageHelperImp : IOncologyVoucherTriageHelper
    {
        private const string OncologyCarePathPatientId = "Oncology CarePath Patient ID";
        private const string DemographicIdType = "JCP ID";
        private const string ProgramType = "Janssen Oncology Voucher";
        private const string StatusOpen = "O";
        private const string StatusAll = "B";
        private const string PhoneNumberType = "Phone#";
        private const string AddressTypePatient = "Patient";
        private const string IssueSource = "Triage";
        private const string WorkflowStatus = "New Referral";
        private const string ActionTypeCodeNoPefReceived = "no PEF received";
        private const string ReferredToUserCodeTriageExceptions = "triageexceptions";
        private const string AttachmentDescriptions = "Prescription Image";
        private const string PatientStatus = "P1=New Referral";
        private const string CompanyId = "SYS";
        private const string ClinicalQolQuestions = "Clinical/Qol - Declined";

        private readonly ILogger<OncologyVoucherTriageHelperImp> _logger;
        private readonly IOptions<EmplifiConfig> _config;
        private readonly ICaseService _caseService;
        private readonly IAddressService _addressService;
        private readonly ICaseStreamService _caseStreamService;
        private readonly AsyncRetryPolicy RetryPolicy;

        public OncologyVoucherTriageHelperImp(
            ILogger<OncologyVoucherTriageHelperImp> logger,
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

        /// <inheritdoc/>
        public string DeriveCrmPatientGender(
            string? patientGender)
        {
            return patientGender?.ToUpper() switch
            {
                "M" => "Male",
                "F" => "Female",
                _ => string.Empty
            };
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public string DeriveCaseText(
            IbmOncologyVoucherTriageRow? ibmOncologyVoucherTriageRow,
            DateTime? PatientDateOfBirth,
            string? PatientGender
            )
        {
            return $"CarePath Patient ID: {ibmOncologyVoucherTriageRow?.CarePathPatientId}{Environment.NewLine}" +
                    $"Patient First Name: {ibmOncologyVoucherTriageRow?.PatientFirstName}{Environment.NewLine}" +
                    $"Patient Last Name: {ibmOncologyVoucherTriageRow?.PatientLastName}{Environment.NewLine}" +
                    $"Patient DOB: {PatientDateOfBirth}{Environment.NewLine}" +
                    $"Patient Sex: {PatientGender}{Environment.NewLine}" +
                    $"Patient Address 1: {ibmOncologyVoucherTriageRow?.PatientAddress1}{Environment.NewLine}" +
                    $"Patient Address 2: {ibmOncologyVoucherTriageRow?.PatientAddress2}{Environment.NewLine}" +
                    $"Patient City: {ibmOncologyVoucherTriageRow?.PatientCity}{Environment.NewLine}" +
                    $"Patient State: {ibmOncologyVoucherTriageRow?.PatientState}{Environment.NewLine}" +
                    $"Patient Zip: {ibmOncologyVoucherTriageRow?.PatientZipCode}{Environment.NewLine}" +
                    $"Patient Phone number: {ibmOncologyVoucherTriageRow?.PatientPhoneNumber}{Environment.NewLine}" +
                    $"Prescriber First Name: {ibmOncologyVoucherTriageRow?.PrescriberFirstName}{Environment.NewLine}" +
                    $"Prescriber Last Name: {ibmOncologyVoucherTriageRow?.PrescriberLastName}{Environment.NewLine}" +
                    $"National Physician ID: {ibmOncologyVoucherTriageRow?.PrescriberNpi}{Environment.NewLine}" +
                    $"Prescriber DEA: {ibmOncologyVoucherTriageRow?.PrescriberDea}{Environment.NewLine}" +
                    $"Prescriber Address 1: {ibmOncologyVoucherTriageRow?.PrescriberAddress1}{Environment.NewLine}" +
                    $"Prescriber Address 2: {ibmOncologyVoucherTriageRow?.PrescriberAddress2}{Environment.NewLine}" +
                    $"Prescriber City: {ibmOncologyVoucherTriageRow?.PrescriberCity}{Environment.NewLine}" +
                    $"Prescriber State: {ibmOncologyVoucherTriageRow?.PrescriberState}{Environment.NewLine}" +
                    $"Prescriber Zip: {ibmOncologyVoucherTriageRow?.PrescriberZipCode}{Environment.NewLine}" +
                    $"Prescriber Phone: {ibmOncologyVoucherTriageRow?.PrescriberPhoneNumber}{Environment.NewLine}" +
                    $"Prescriber Fax: {ibmOncologyVoucherTriageRow?.PrescriberFaxNumber}{Environment.NewLine}" +
                    $"DEMOGRAPHICID: {ibmOncologyVoucherTriageRow?.PatientDemographicId}{Environment.NewLine}" +
                    $"Carepath Transaction ID: {ibmOncologyVoucherTriageRow?.CarePathTransactionId}{Environment.NewLine}";
        }

        /// <inheritdoc />
        public DateTime? DeriveDate(
            string? date)
        {
            if (DateTime.TryParseExact(date, "MMddyyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            return null;
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

        /// <inheritdoc/>
        public async Task<IEnumerable<AddressServiceWrapper.Address>> FindAddressesForOncologyVoucherTriageAsync(
            string? carePathPatientId,
            string? demographicId,
            string? state,
            DateTime? dateOfBirth,
            string? lastName,
            string? firstName)
        {
            IEnumerable<AddressServiceWrapper.Address> addressList;

            addressList = await FindAddressUsingPatientIdAndDateOfBirth(OncologyCarePathPatientId, carePathPatientId, dateOfBirth).ConfigureAwait(false);

            if (addressList.Any())
            {
                return addressList;
            }

            addressList = await FindAddressUsingPatientIdAndDateOfBirth(DemographicIdType, demographicId, dateOfBirth).ConfigureAwait(false);

            if (addressList.Any())
            {
                return addressList;
            }

            addressList = await FindAddressUsingLastNameFirstNameStateAndDateOfBirth(lastName, firstName, state, dateOfBirth).ConfigureAwait(false);

            return addressList;
        }

        /// <inheritdoc />
        public async Task<int> CreateAddressFromOncologyVoucherTriageAsync(
            OncologyVoucherTriage oncologyVoucherTriage,
            CancellationToken c)
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
                    given_names = oncologyVoucherTriage.PatientFirstName,
                    last_name = oncologyVoucherTriage.PatientLastName,
                    a05_code = oncologyVoucherTriage.PatientDateOfBirth.ToString(),
                    a20_code = oncologyVoucherTriage.DerivedCrmPatientGender,
                    address1 = oncologyVoucherTriage.PatientAddress1,
                    address2 = oncologyVoucherTriage.PatientAddress2,
                    city = oncologyVoucherTriage.PatientCity,
                    state = oncologyVoucherTriage.PatientState,
                    postal_code = oncologyVoucherTriage.PatientZipCode,
                    a22_code = oncologyVoucherTriage.PrescriberFirstName,
                    a23_code = oncologyVoucherTriage.PrescriberLastName,
                    a24_code = oncologyVoucherTriage.PrescriberAddress1,
                    a25_code = oncologyVoucherTriage.PrescriberAddress2,
                    a27_code = oncologyVoucherTriage.PrescriberCity,
                    a28_code = oncologyVoucherTriage.PrescriberState,
                    a29_code = oncologyVoucherTriage.PrescriberZipCode,
                    a30_code = oncologyVoucherTriage.PrescriberPhoneNumber,
                    a31_code = oncologyVoucherTriage.PrescriberFaxNumber,
                    a34_code = oncologyVoucherTriage.PrescriberDea,
                    a36_code = oncologyVoucherTriage.PrescriberNpi
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

            if (!string.IsNullOrEmpty(oncologyVoucherTriage.CarePathPatientId))
            {
                phoneList.Add(new AddressServiceWrapper.Phone()
                {
                    phone_type_code = OncologyCarePathPatientId,
                    phone = oncologyVoucherTriage.CarePathPatientId
                });
            }

            if (!string.IsNullOrEmpty(oncologyVoucherTriage.PatientPhoneNumber))
            {
                phoneList.Add(new AddressServiceWrapper.Phone()
                {
                    phone_type_code = PhoneNumberType,
                    phone = oncologyVoucherTriage.PatientPhoneNumber
                });
            }

            if (!string.IsNullOrEmpty(oncologyVoucherTriage.PatientDemographicId))
            {
                phoneList.Add(new AddressServiceWrapper.Phone()
                {
                    phone_type_code = DemographicIdType,
                    phone = oncologyVoucherTriage.PatientDemographicId
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
        public async Task<int> CreateCaseFromOncologyVoucherTriageAsync(
            int addressId,
            OncologyVoucherTriage oncologyVoucherTriage,
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
                        address_id = addressId.ToString(),
                        b07_code = ProgramType,
                        case_status = StatusOpen,
                        b36_code = oncologyVoucherTriage.PrimaryDiagnosisCode,
                        b47_code = oncologyVoucherTriage.CarePathTransactionId,
                        CaseTextList = new CaseTextList()
                        {
                            CaseText =
                            [
                                new CaseText()
                                {
                                    text_type_code = WorkflowStatus,
                                    case_text = oncologyVoucherTriage.DerivedCaseText
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
                                    c88_code = StatusOpen,
                                    c28_code = DateTime.Now.ToString(), // Triage Received Date
                                    c89_code = IssueSource, // Issue Source
                                    c47_code = oncologyVoucherTriage.DerivedProgramHeader,
                                    product_code = oncologyVoucherTriage.DerivedDrugName,
                                    c19_code = WorkflowStatus, // Workflow Status
                                    c35_code = DateTime.Today.ToString("d"),
                                    c16_code = PatientStatus,
                                    c26_code = DateTime.Now.ToString(), // Patient Status Changed Date
                                    c39_code = oncologyVoucherTriage.NdcCode,
                                    c93_code = oncologyVoucherTriage.ShipToLocation,
                                    cc0_code = oncologyVoucherTriage.TreatmentCenterName,
                                    cb0_code = oncologyVoucherTriage.TreatmentCenterContactFirstName,
                                    cb1_code = oncologyVoucherTriage.TreatmentCenterContactLastName,
                                    cb2_code = oncologyVoucherTriage.TreatmentCenterPhoneNumber,
                                    cb3_code = oncologyVoucherTriage.TreatmentCenterFaxNumber,
                                    cb4_code = oncologyVoucherTriage.TreatmentCenterAddress1,
                                    cb5_code = oncologyVoucherTriage.TreatmentCenterAddress2,
                                    cb6_code = oncologyVoucherTriage.TreatmentCenterCity,
                                    cb7_code = oncologyVoucherTriage.TreatmentCenterState,
                                    cb8_code = oncologyVoucherTriage.TreatmentCenterZipCode,
                                    cc1_code = oncologyVoucherTriage.TreatmentCenterNpi,
                                    cb9_code = oncologyVoucherTriage.TreatmentCenterDea,
                                    c97_code = oncologyVoucherTriage.CarePathTransactionId
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

        /// <inheritdoc />
        public async Task UploadOncologyVoucherTriageImageFilesAndAttachToCaseAsync(
            int caseId,
            List<TriageImage>? oncologyVoucherTriageImages,
            CancellationToken c)
        {
            int imagesLoaded = 0;

            if (oncologyVoucherTriageImages is not null)
            {
                var attachmentDescription = $"Patient Enrollment Form {DateTime.Today:MMddyyyy}";
                foreach (var oncologyTriageImage in oncologyVoucherTriageImages)
                {
                    var attachmentName = await UploadFileAsync(oncologyTriageImage.ImageFileName, oncologyTriageImage.Image, caseId).ConfigureAwait(false);
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
        public async Task<string> UploadFileAsync(
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

        /// <inheritdoc />
        public async Task AddAttachmentToCaseAsync(
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
        public async Task UpdateAddressFromOncologyVoucherTriageAsync(
            AddressServiceWrapper.Address address,
            OncologyVoucherTriage oncologyVoucherTriage,
            CancellationToken c)
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
                    address1 = oncologyVoucherTriage.PatientAddress1 ?? string.Empty,
                    address2 = oncologyVoucherTriage.PatientAddress2 ?? string.Empty,
                    city = oncologyVoucherTriage.PatientCity ?? string.Empty,
                    state = oncologyVoucherTriage.PatientState ?? string.Empty,
                    postal_code = oncologyVoucherTriage.PatientZipCode ?? string.Empty,
                    a58_code = string.Empty, // TODO - Ask Melissa about this field
                    a22_code = oncologyVoucherTriage.PrescriberFirstName ?? string.Empty,
                    a23_code = oncologyVoucherTriage.PrescriberLastName ?? string.Empty,
                    a24_code = oncologyVoucherTriage.PrescriberAddress1 ?? string.Empty,
                    a25_code = oncologyVoucherTriage.PrescriberAddress2 ?? string.Empty,
                    a27_code = oncologyVoucherTriage.PrescriberCity ?? string.Empty,
                    a28_code = oncologyVoucherTriage.PrescriberState ?? string.Empty,
                    a29_code = oncologyVoucherTriage.PrescriberZipCode ?? string.Empty,
                    a30_code = oncologyVoucherTriage.PrescriberPhoneNumber ?? string.Empty,
                    a31_code = oncologyVoucherTriage.PrescriberFaxNumber ?? string.Empty,
                    a36_code = oncologyVoucherTriage.PrescriberNpi ?? string.Empty,
                    a34_code = oncologyVoucherTriage.PrescriberDea ?? string.Empty
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

            if (!string.IsNullOrEmpty(oncologyVoucherTriage.CarePathPatientId))
            {
                var existingEntries = address.PhoneList.Phone.Where(x => x.phone_type_code == OncologyCarePathPatientId);

                if (!existingEntries.Any())
                {
                    phoneList.Add(new AddressServiceWrapper.Phone()
                    {
                        phone_type_code = OncologyCarePathPatientId,
                        phone = oncologyVoucherTriage.CarePathPatientId
                    });
                }
            }

            if (!string.IsNullOrEmpty(oncologyVoucherTriage.PatientPhoneNumber))
            {
                var existingEntries = address.PhoneList.Phone
                    .Where(x => x.phone_type_code == PhoneNumberType &&
                        !string.IsNullOrEmpty(x.phone) &&
                        GetDigits(x.phone) == GetDigits(oncologyVoucherTriage.PatientPhoneNumber));

                if (!existingEntries.Any())
                {
                    phoneList.Add(new AddressServiceWrapper.Phone()
                    {
                        phone_type_code = PhoneNumberType,
                        phone = oncologyVoucherTriage.PatientPhoneNumber
                    });
                }
            }

            if (!string.IsNullOrEmpty(oncologyVoucherTriage.PatientDemographicId))
            {
                var existingEntries = address.PhoneList.Phone.Where(x => x.phone_type_code == DemographicIdType);

                if (!existingEntries.Any())
                {
                    phoneList.Add(new AddressServiceWrapper.Phone()
                    {
                        phone_type_code = DemographicIdType,
                        phone = oncologyVoucherTriage.PatientDemographicId
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
        public async Task<List<Case>> FindCasesByAddressIdProgramAndStatusAsync(
            int addressId,
            string programType,
            string caseStatus)
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

        /// <inheritdoc />
        public async Task UpdateCaseFromOncologyVoucherTriageAsync(
            int caseId,
            OncologyVoucherTriage oncologyVoucherTriage,
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
                        case_status = StatusOpen,
                        b36_code = oncologyVoucherTriage.PrimaryDiagnosisCode ?? string.Empty,
                        b47_code = oncologyVoucherTriage.CarePathTransactionId,
                        CaseTextList = new CaseTextList()
                        {
                            CaseText =
                            [
                                new CaseText()
                                {
                                    text_type_code = WorkflowStatus,
                                    case_text = oncologyVoucherTriage.DerivedCaseText
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
                                    c88_code = StatusOpen,
                                    c28_code = DateTime.Now.ToString(), // Triage Received Date
                                    c89_code = IssueSource, // Issue Source
                                    c47_code = oncologyVoucherTriage.DerivedProgramHeader,
                                    product_code = oncologyVoucherTriage.DerivedDrugName,
                                    c19_code = WorkflowStatus, // Workflow Status
                                    c35_code = DateTime.Today.ToString("d"),
                                    c16_code = PatientStatus,
                                    c26_code = DateTime.Now.ToString(), // Patient Status Changed Date
                                    c39_code = oncologyVoucherTriage.NdcCode,
                                    c93_code = oncologyVoucherTriage.ShipToLocation,
                                    cc0_code = oncologyVoucherTriage.TreatmentCenterName,
                                    cb0_code = oncologyVoucherTriage.TreatmentCenterContactFirstName,
                                    cb1_code = oncologyVoucherTriage.TreatmentCenterContactLastName,
                                    cb2_code = oncologyVoucherTriage.TreatmentCenterPhoneNumber,
                                    cb3_code = oncologyVoucherTriage.TreatmentCenterFaxNumber,
                                    cb4_code = oncologyVoucherTriage.TreatmentCenterAddress1,
                                    cb5_code = oncologyVoucherTriage.TreatmentCenterAddress2,
                                    cb6_code = oncologyVoucherTriage.TreatmentCenterCity,
                                    cb7_code = oncologyVoucherTriage.TreatmentCenterState,
                                    cb8_code = oncologyVoucherTriage.TreatmentCenterZipCode,
                                    cc1_code = oncologyVoucherTriage.TreatmentCenterNpi,
                                    cb9_code = oncologyVoucherTriage.TreatmentCenterDea,
                                    c97_code = oncologyVoucherTriage.CarePathTransactionId
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
}
