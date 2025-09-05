using AstuteAttachmentService;
using AstuteCaseService;
using AstutePatientService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Configuration;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Extensions;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound;
using System.Net.Mail;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute
{
    public class AstuteSoapProxy : IAstuteSoapProxy
    {
        private struct LoggingStruct
        {
            public string Text { get; set; }
            public IEnumerable<string> Substitutions { get; set; }
            public bool isError { get; set; }
        };

        private readonly ICaseService _caseServiceClient;
        private readonly IAddressService _addressServiceClient;
        private readonly ICaseStreamService _caseStreamServiceClient;
        private readonly ILogger _log;
        private readonly AppAstuteClientSecurity _clientSecurity;     

        public AstuteSoapProxy(ICaseService caseServiceClient, IAddressService addressServiceClient, ICaseStreamService caseStreamServiceClient,
            ILogger<AstuteSoapProxy> log, IOptions<AppAstuteClientSecurity> clientSecurity)
        {
            _caseServiceClient = caseServiceClient ?? throw new ArgumentNullException(nameof(caseServiceClient));
            _addressServiceClient = addressServiceClient ?? throw new ArgumentNullException(nameof(addressServiceClient));
            _caseStreamServiceClient = caseStreamServiceClient ?? throw new ArgumentNullException(nameof(caseStreamServiceClient));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _clientSecurity = clientSecurity.Value ?? throw new ArgumentNullException(nameof(clientSecurity));
        }

        [ExcludeFromCodeCoverage]
        public async Task<bool> SetProcessedDateAsync(string caseId, string issueSequence, DateTimeOffset processedDate)
        {
            if (string.IsNullOrEmpty(caseId) || string.IsNullOrEmpty(issueSequence))
            {
                return false;
            }

            var request = new CaseListUpdateRequest
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                UTCOffset = "0",
                Type = CaseListUpdateType.Update,
                Case = new AstuteCaseService.Case[]
                {
                    new AstuteCaseService.Case()
                    {
                        APCWEditMode = AstuteCaseService.APCWEditModeType.Modified,
                        company_id = AstuteConstants.CompanyId,
                        case_id = caseId,
                        IssueList = new IssueList()
                        {
                            Issue = new Issue[]
                            {
                                new Issue()
                                {
                                    APCWEditMode = AstuteCaseService.APCWEditModeType.Modified,
                                    issue_seq = issueSequence,
                                    c73_code = processedDate.ToString(), // Data Processed Time CMM
                                }
                            }
                        }
                    }
                },
                ResponseFormat = new ResponseFormat()
                {
                    CaseList = new CaseListFormat()
                    {
                        Case = new CaseFormat()
                        {
                            AllAttributes = AstuteCaseService.TrueFalseType.Item1,
                        }
                    }
                }
            };

            var response = await this._caseServiceClient.UpdateCaseAsync(request).ConfigureAwait(false);

            if (!(response is null))
            {
                if (response.Valid != AstuteCaseService.ValidState.Ok)
                {
                    _log.LogWarning("API response status:{0}", response.Valid);
                    this.LogAstuteError(response.MessageList?.Select(x => new LoggingStruct { Text = x.Text, Substitutions = x.Substitution?.Select(y => y.Text), isError = (response.Valid == AstuteCaseService.ValidState.Error) }));
                }

                if (response.Valid == AstuteCaseService.ValidState.Ok)
                {
                    return true;
                }
            }

            return false;
        }

        [ExcludeFromCodeCoverage]
        public async Task<bool> SetExtractedDateAsync(string caseId, string issueSequence)
        {
            if (string.IsNullOrEmpty(caseId) || string.IsNullOrEmpty(issueSequence))
            {
                return false;
            }

            var request = new CaseListUpdateRequest
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                UTCOffset = "0",
                Type = CaseListUpdateType.Update,
                Case = new AstuteCaseService.Case[]
                {
                    new AstuteCaseService.Case()
                    {
                        APCWEditMode = AstuteCaseService.APCWEditModeType.Modified,
                        company_id = AstuteConstants.CompanyId,
                        case_id = caseId,
                        IssueList = new IssueList()
                        {
                            Issue = new Issue[]
                            {
                                new Issue()
                                {
                                    APCWEditMode = AstuteCaseService.APCWEditModeType.Modified,
                                    issue_seq = issueSequence,
                                    c76_code = DateTimeOffset.Now.ToString(), // Data Extracted Time CMM
                                }
                            }
                        }
                    }
                },
                ResponseFormat = new ResponseFormat()
                {
                    CaseList = new CaseListFormat()
                    {
                        Case = new CaseFormat()
                        {
                            AllAttributes = AstuteCaseService.TrueFalseType.Item1,
                        }
                    }
                }
            };

            var response = await this._caseServiceClient.UpdateCaseAsync(request).ConfigureAwait(false);

            if (!(response is null))
            {
                if (response.Valid != AstuteCaseService.ValidState.Ok)
                {
                    _log.LogWarning("API response status:{0}", response.Valid);
                    this.LogAstuteError(response.MessageList?.Select(x => new LoggingStruct { Text = x.Text, Substitutions = x.Substitution?.Select(y => y.Text), isError = (response.Valid == AstuteCaseService.ValidState.Error) }));
                }

                if (response.Valid == AstuteCaseService.ValidState.Ok)
                {
                    return true;
                }
            }

            return false;
        }

        [ExcludeFromCodeCoverage]
        public async Task<List<PatientStatus>> GetStatusesAsync(DateTimeOffset lastRunDateTime)
        {
            var runDate = DateTimeOffset.Now;

            var request = new CaseSearchRequest()
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                UTCOffset = "0",
                Type = CaseSearchType.Get,
                Case = new SearchCase()
                {
                    company_id = AstuteConstants.CompanyId,
                    b07_code = AstuteConstants.ProgramType,
                    case_status = AstuteConstants.StatusOpen,
                },
                FilterList = new FilterList()
                {
                    Filter = new Filter[]
                    {
                        new Filter()
                        {
                            filter_seq = 1,
                            selection_operator = SelectionOperator.EQUAL,
                            selection_category_id = "B27", // PA Source
                            selection_code = AstuteConstants.EnrollmentSource
                        },
                        new Filter()
                        {
                            filter_seq = 2,
                            selection_operator = SelectionOperator.GREATERTHANOREQUALTO,
                            selection_category_id = "B43", // Case Date Changed
                            selection_code = lastRunDateTime.ToString()
                        },
                        new Filter()
                        {
                            filter_seq = 3,
                            selection_operator = SelectionOperator.ISNULL,
                            selection_category_id = "C73", // Data Processed Time CMM
                            selection_code = runDate.ToString()
                        },
                    }
                },
                ResponseFormat = (new ResponseFormat()).CreateDefaultCaseResponseFormat()
            };

            List<string> caseIdList = (await this.SearchStatusesAsync(request, runDate).ConfigureAwait(false)).ToList();

            request.FilterList.Filter[2].selection_operator = SelectionOperator.LESSTHAN;
            caseIdList.AddRange(await this.SearchStatusesAsync(request, runDate).ConfigureAwait(false));

            request.FilterList.Filter[1].selection_category_id = "C26"; // Patient Status Change Date
            request.FilterList.Filter[2].selection_operator = SelectionOperator.ISNULL;
            caseIdList.AddRange(await this.SearchStatusesAsync(request, runDate).ConfigureAwait(false));

            request.FilterList.Filter[2].selection_operator = SelectionOperator.LESSTHAN;
            caseIdList.AddRange(await this.SearchStatusesAsync(request, runDate).ConfigureAwait(false));

            request.FilterList.Filter[1].selection_category_id = "A38"; // Address Changed Date
            request.FilterList.Filter[2].selection_operator = SelectionOperator.ISNULL;
            caseIdList.AddRange(await this.SearchStatusesAsync(request, runDate).ConfigureAwait(false));

            request.FilterList.Filter[2].selection_operator = SelectionOperator.LESSTHAN;
            caseIdList.AddRange(await this.SearchStatusesAsync(request, runDate).ConfigureAwait(false));

            var uniqueCaseIdList = caseIdList.Distinct();

            List<PatientStatus> patientStatuses = new List<PatientStatus>();

            foreach (var caseId in uniqueCaseIdList)
            {
                var caseRecord = await this.GetCaseAsync(caseId).ConfigureAwait(false);
                var patientStatus = caseRecord.CreateAstutePatientStatus(lastRunDateTime, runDate);
                if (!(patientStatus is null))
                {
                    patientStatuses.Add(patientStatus);
                    await this.SetExtractedDateAsync(caseRecord.case_id, patientStatus.AstuteIssueSequence).ConfigureAwait(false);
                    await this.ReleaseCaseAsync(Convert.ToInt32(caseRecord.case_id)).ConfigureAwait(false);
                }
            }

            return patientStatuses;
        }

        public async Task<PatientCase> FindPatientAsync(string demographicId, string patientId, string firstName, string lastName, DateTimeOffset? dateOfBirth)
        {
            AstutePatientService.Address address = null;

            var addressSearch = new AddressListSearch
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password
            };

            if (!string.IsNullOrEmpty(demographicId))
            {
                addressSearch.CreatePatientIdSearch(demographicId, AstuteConstants.JcpPatientId);

                var response = await _addressServiceClient.SearchAddressAsync(addressSearch).ConfigureAwait(false);

                if (!(response is null) && !(response.Address is null))
                {
                    address = response.Address[0];
                }
            }

            if (address is null && !string.IsNullOrEmpty(patientId))
            {
                addressSearch.CreatePatientIdSearch(patientId, AstuteConstants.CmmPatientType);

                var response = await _addressServiceClient.SearchAddressAsync(addressSearch).ConfigureAwait(false);

                if (!(response is null) && !(response.Address is null))
                {
                    address = response.Address[0];
                }
            }

            if (address is null && !(string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || dateOfBirth == null || dateOfBirth == DateTimeOffset.MinValue))
            {
                addressSearch.CreateFirstNameLastNameDobSearch(firstName, lastName, dateOfBirth);

                var response = await _addressServiceClient.SearchAddressAsync(addressSearch).ConfigureAwait(false);

                if (!(response is null) && !(response.Address is null))
                {
                    address = response.Address[0];
                }
            }

            return address is null ? null : address.CreatePatientCase();
        }

        [ExcludeFromCodeCoverage]
        public async Task<PatientCase> CreatePatientAsync(Patient patient)
        {
            var request = new AddressListUpdate()
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                UTCOffset = "0",
                Type = RequestType.Update,
                Address = new AstutePatientService.Address()
                {
                    APCWEditMode = AstutePatientService.APCWEditModeType.New,
                    company_id = AstuteConstants.CompanyId,
                    address_id = AstuteConstants.AddressIdDefault,
                    active = AstutePatientService.YesNo.Y,
                    address_type_code = AstuteConstants.AddressTypeCode,
                    given_names = patient.FirstName,
                    last_name = patient.LastName,
                    a05_code = patient.DateOfBirth.HasValue ? patient.DateOfBirth.Value.ToString("d") : string.Empty,
                    a20_code = patient.Gender,
                    address1 = patient.Address.Address1,
                    address2 = patient.Address.Address2,
                    city = patient.Address.City,
                    state = patient.Address.State,
                    postal_code = patient.Address.ZipCode,
                    a22_code = patient.Provider.FirstName,
                    a23_code = patient.Provider.LastName,
                    a36_code = patient.Provider.Npi,
                    a30_code = patient.Provider.Phone,
                    a31_code = patient.Provider.Fax,
                    a24_code = patient.Provider.Address.Address1,
                    a25_code = patient.Provider.Address.Address2,
                    a27_code = patient.Provider.Address.City,
                    a28_code = patient.Provider.Address.State,
                    a29_code = patient.Provider.Address.ZipCode,
                    a34_code = string.Empty,
                    PhoneList = new AstutePatientService.PhoneList()
                    {
                        Phone = new AstutePatientService.Phone[]
                        {
                            new AstutePatientService.Phone()
                            {
                                phone_type_code = AstuteConstants.CmmPatientType,
                                phone = patient.Id
                            },
                            new AstutePatientService.Phone()
                            {
                                phone_type_code = AstuteConstants.JcpPatientId,
                                phone = patient.JcpId
                            },
                            new AstutePatientService.Phone()
                            {
                                phone_type_code = AstuteConstants.PrimaryPhone,
                                phone = patient.Phone
                            }
                        }
                    },
                },
                ResponseFormat = (new AddressResponseFormat()).CreateDefaultAddressResponseFormat()
            };

            var response = await _addressServiceClient.UpdateAddressAsync(request).ConfigureAwait(false);

            if (response is null || response.Address is null)
            {
                throw new AstuteException("Patient could not be created");
            }

            if (response.Valid != AstutePatientService.ValidState.Ok)
            {
                this.LogAstuteError(response.MessageList?.Select(x => new LoggingStruct { Text = x.Text, Substitutions = x.Substitution?.Select(y => y.Text), isError = (response.Valid == AstutePatientService.ValidState.Error) }));
            }

            return response.Address[0].CreatePatientCase();
        }

        [ExcludeFromCodeCoverage]
        public async Task UpdatePatientAsync(PatientCase existingPatient, Patient patient)
        {
            var updateExtendedPatientIdList = false;
            var phoneList = new List<AstutePatientService.Phone>();
            var extendedPatientIdList = existingPatient.ExtendedPatientIdList ?? string.Empty;

            if (!string.IsNullOrEmpty(patient.Id))
            {
                if (string.IsNullOrEmpty(existingPatient.CmmPatientId))
                {
                    phoneList.Add(new AstutePatientService.Phone()
                    {
                        phone_type_code = AstuteConstants.CmmPatientType,
                        phone = patient.Id
                    });
                }
                else
                {
                    if (existingPatient.CmmPatientId != patient.Id && !extendedPatientIdList.Contains(patient.Id))
                    {
                        if (!string.IsNullOrEmpty(extendedPatientIdList))
                        {
                            extendedPatientIdList += ",";
                        }
                        extendedPatientIdList += patient.Id;
                        updateExtendedPatientIdList = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(patient.JcpId))
            {
                if (string.IsNullOrEmpty(existingPatient.JcpId))
                {
                    phoneList.Add(new AstutePatientService.Phone()
                    {
                        phone_type_code = AstuteConstants.JcpPatientId,
                        phone = patient.JcpId
                    });
                }
                else
                {
                    if (existingPatient.JcpId != patient.JcpId && !extendedPatientIdList.Contains(patient.JcpId))
                    {
                        if (!string.IsNullOrEmpty(extendedPatientIdList))
                        {
                            extendedPatientIdList += ",";
                        }
                        extendedPatientIdList += patient.JcpId;
                        updateExtendedPatientIdList = true;
                    }
                }
            }

            if (phoneList.Count > 0 || updateExtendedPatientIdList)
            {
                var request = new AddressListUpdate()
                {
                    UserName = _clientSecurity.Username,
                    Password = _clientSecurity.Password,
                    UTCOffset = "0",
                    Type = RequestType.Update,
                    Address = new AstutePatientService.Address()
                    {
                        APCWEditMode = AstutePatientService.APCWEditModeType.Modified,
                        company_id = AstuteConstants.CompanyId,
                        address_id = existingPatient.PatientId.ToString(),
                        active = AstutePatientService.YesNo.Y
                    },
                    ResponseFormat = (new AddressResponseFormat()).CreateDefaultAddressResponseFormat()
                };

                if (phoneList.Count > 0)
                {
                    request.Address.PhoneList = new AstutePatientService.PhoneList()
                    {
                        Phone = phoneList.ToArray()
                    };
                }

                if (updateExtendedPatientIdList)
                {
                    request.Address.Extended = new AstutePatientService.AddressExtended()
                    {
                        p03_code = extendedPatientIdList
                    };
                }

                var response = await _addressServiceClient.UpdateAddressAsync(request).ConfigureAwait(false);

                if (response is null || response.Address is null)
                {
                    throw new AstuteException("Patient could not be updated");
                }

                if (response.Valid != AstutePatientService.ValidState.Ok)
                {
                    this.LogAstuteError(response.MessageList?.Select(x => new LoggingStruct { Text = x.Text, Substitutions = x.Substitution?.Select(y => y.Text), isError = (response.Valid == AstutePatientService.ValidState.Error) }));
                }
            }
        }

        [ExcludeFromCodeCoverage]
        public async Task<PatientCase> CreateCaseAsync(Models.Case caseRecord)
        {
            var request = new CaseListUpdateRequest()
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                UTCOffset = "0",
                Type = CaseListUpdateType.Update,
                Case = new AstuteCaseService.Case[]
                {
                    new AstuteCaseService.Case()
                    {
                        APCWEditMode = AstuteCaseService.APCWEditModeType.New,
                        company_id = AstuteConstants.CompanyId,
                        case_id = caseRecord.AstuteCaseId.ToString(),
                        address_id = caseRecord.AstuteAddressId.ToString(),
                        case_status = caseRecord.CaseStatus,
                        case_status_code = caseRecord.ExceptionReason,
                        b09_code = AstuteConstants.EnrollmentSource, // Triage (enrollment) source
                        b27_code = AstuteConstants.EnrollmentSource, // Prior Auth source - pre-populate from enrollment
                        b48_code = caseRecord.Id,
                        b49_code = caseRecord.ReferralId,
                        b36_code = (!string.IsNullOrEmpty(caseRecord.DiagnosisCode) && caseRecord.DiagnosisCode.Length > 40) ? caseRecord.DiagnosisCode.Substring(0, 40) : caseRecord.DiagnosisCode,
                        b07_code = AstuteConstants.ProgramType,
                        b38_code = AstuteConstants.LinkEnrollmentDefault,
                        b32_code = DateTimeOffset.Now.Year.ToString(), // Program Year
                        b37_code = caseRecord.ProgramName,
                        b08_code = caseRecord.AstuteShipFirstDoseToPrescriber ?? string.Empty,
                        b06_code = string.Empty,
                        b20_code = string.Empty,
                        b19_code = string.Empty,
                        b59_code = string.Empty,
                        b60_code = string.Empty,
                        IssueList = new IssueList()
                        {
                            Issue = new Issue[]
                            {
                                new Issue()
                                {
                                    APCWEditMode = AstuteCaseService.APCWEditModeType.New,
                                    c88_code = caseRecord.CaseStatus,
                                    product_code = caseRecord.AstuteProductName,
                                    c39_code = caseRecord.Ndc,
                                    c47_code = caseRecord.AstuteProgramHeader,
                                    c26_code = DateTimeOffset.Now.ToString(), // Patient status change date
                                    c28_code = DateTimeOffset.Now.ToString(), // Triage (enrollment) received date
                                    c35_code = caseRecord.CaseStatus == AstuteConstants.StatusOpen ? DateTimeOffset.Now.ToString() : string.Empty, // Follow-Up Date
                                    c16_code = AstuteConstants.PatientStatusDefault,
                                    c19_code = caseRecord.WorkflowStatus,
                                    c60_code = caseRecord.ProgramName,
                                    c89_code = AstuteConstants.IssueSourceTriage,
                                    c75_code = AstuteConstants.ClincalDeclined
                                }
                            }
                        },
                        CaseTextList = new CaseTextList()
                        {
                            CaseText = new AstuteCaseService.CaseText[]
                            {
                                new AstuteCaseService.CaseText()
                                {
                                    text_type_code = "AdminOnly-DoNotUse",
                                    description = "2",
                                    case_text = caseRecord.EnrollmentPayload
                                },
                                new AstuteCaseService.CaseText()
                                {
                                    text_type_code = "CMM Enrollment",
                                    description = "2",
                                    case_text = $"*{caseRecord.AstuteProgramHeader.ToLower()}triage*{Environment.NewLine}Referral Id: {caseRecord.ReferralId}{Environment.NewLine}"
                                }
                            }
                        }
                    }
                },
                ResponseFormat = (new ResponseFormat()).CreateDefaultCaseResponseFormat()
            };

            var response = await _caseServiceClient.UpdateCaseAsync(request).ConfigureAwait(false);

            if (response is null || response.Case is null)
            {
                throw new AstuteException("Case could not be created");
            }

            if (response.Valid != AstuteCaseService.ValidState.Ok)
            {
                this.LogAstuteError(response.MessageList?.Select(x => new LoggingStruct { Text = x.Text, Substitutions = x.Substitution?.Select(y => y.Text), isError = (response.Valid == AstuteCaseService.ValidState.Error) }));
            }

            return response.Case[0].CreatePatientCase();
        }

        [ExcludeFromCodeCoverage]
        public async Task ReleaseCaseAsync(int caseId)
        {
            var request = new CaseUserListRequest()
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                UTCOffset = "0",
                Type = CaseUserListType.Update,
                CaseUser = new CaseUser()
                {
                    APCWEditMode = AstuteCaseService.APCWEditModeType.Delete,
                    case_id = caseId,
                    company_id = AstuteConstants.CompanyId,
                    system_user_id = _clientSecurity.Username
                }
            };

            await _caseServiceClient.ReleaseCaseAsync(request).ConfigureAwait(false);
        }

        [ExcludeFromCodeCoverage]
        public async Task<IEnumerable<PatientCase>> FindOpenCasesAsync(int addressId, string cmmCaseId)
        {
            if (addressId <= 0)
            {
                return null;
            }

            var request = new CaseGetRequest()
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                UTCOffset = "0",
                Type = CaseGetType.Get,
                Case = new AstuteCaseService.Case()
                {
                    company_id = AstuteConstants.CompanyId,
                    b07_code = AstuteConstants.ProgramType,
                    case_status = AstuteConstants.StatusOpen,
                    address_id = addressId.ToString(),
                    b48_code = cmmCaseId
                },
                ResponseFormat = (new ResponseFormat()).CreateDefaultCaseResponseFormat()
            };

            var response = await this._caseServiceClient.GetCaseAsync(request).ConfigureAwait(false);

            if (!(response is null) && !(response.Case is null))
            {
                return response.Case.Select(x => x.CreatePatientCase());
            }

            return null;
        }

        public async Task<PatientCase> UpdateCaseAsync(PatientCase patientCase, CaseStatus caseStatus)
        {
            var request = patientCase.CreateCaseListUpdateRequest(caseStatus);

            request.UserName = _clientSecurity.Username;
            request.Password = _clientSecurity.Password;

            var response = await this._caseServiceClient.UpdateCaseAsync(request).ConfigureAwait(false);

            if (response is null || response.Case is null)
            {
                throw new AstuteException("Case update failed");
            }

            if (response.Valid != AstuteCaseService.ValidState.Ok)
            {
                this.LogAstuteError(response.MessageList?.Select(x => new LoggingStruct { Text = x.Text, Substitutions = x.Substitution?.Select(y => y.Text), isError = (response.Valid == AstuteCaseService.ValidState.Error) }));
            }

            return response.Case[0].CreatePatientCase();
        }

        [ExcludeFromCodeCoverage]
        private async Task ReopenCaseAsync(PatientCase patientCase, Models.Case caseRecord)
        {
            var request = patientCase.CreateCaseListReopenRequest(caseRecord);

            request.UserName = _clientSecurity.Username;
            request.Password = _clientSecurity.Password;

            var response = await _caseServiceClient.UpdateCaseAsync(request).ConfigureAwait(false);

            if (response is null)
            {
                throw new AstuteException("Case update failed");
            }

            if (response.Valid != AstuteCaseService.ValidState.Ok)
            {
                this.LogAstuteError(response.MessageList?.Select(x => new LoggingStruct { Text = x.Text, Substitutions = x.Substitution?.Select(y => y.Text), isError = (response.Valid == AstuteCaseService.ValidState.Error) }));
            }
        }

        [ExcludeFromCodeCoverage]
        public async Task<IEnumerable<string>> SearchStatusesAsync(CaseSearchRequest caseSearchRequest, DateTimeOffset runDate)
        {
            int recordCount = 0;
            int errorCount = 0;
            List<string> caseIds = new List<string>();
            do
            {
                var caseListResponse = await this._caseServiceClient.SearchCaseAsync(caseSearchRequest).ConfigureAwait(false);
                if ((caseListResponse is null) || (caseListResponse.Case is null))
                {
                    return new List<string>();
                }
                recordCount = caseListResponse.Case.Count();
                caseIds.AddRange(caseListResponse.Case.Select(x => x.case_id));

                foreach (var caseRecord in caseListResponse.Case)
                {
                    foreach (var issue in caseRecord.IssueList.Issue)
                    {
                        if (!(await this.SetProcessedDateAsync(caseRecord.case_id, issue.issue_seq, runDate).ConfigureAwait(false)))
                        {
                            errorCount++;
                        }
                    }
                    await this.ReleaseCaseAsync(Convert.ToInt32(caseRecord.case_id)).ConfigureAwait(false);
                }
            } while (recordCount >= 250 && errorCount == 0);

            if (errorCount > 0)
            {
                _log.LogError($"SetProcessedDateAsync returned [{errorCount}] failures.");
            }
            return caseIds;
        }

        [ExcludeFromCodeCoverage]
        public async Task<AstuteCaseService.Case> GetCaseAsync(string caseId)
        {
            if (string.IsNullOrEmpty(caseId))
            {
                return null;
            }

            var request = new CaseGetRequest()
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                UTCOffset = "0",
                Type = CaseGetType.GetUnique,
                Case = new AstuteCaseService.Case()
                {
                    company_id = AstuteConstants.CompanyId,
                    case_id = caseId
                },
                ResponseFormat = (new ResponseFormat()).CreateDefaultCaseResponseFormat()
            };

            var response = await this._caseServiceClient.GetCaseAsync(request).ConfigureAwait(false);

            if (!(response is null) && !(response.Case is null))
            {
                return response.Case[0];
            }

            return null;
        }

        [ExcludeFromCodeCoverage]
        public async Task<string> UploadFileAsync(string sourceFileName, string attachmentData, int caseId)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(attachmentData);
            writer.Flush();
            stream.Position = 0;

            var request = new FileRequest()
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                FileByteStream = stream,
                SourceFileName = sourceFileName,
                ID = caseId.ToString(),
                Method = FileMethodType.UploadAutoName,
                ServerPath = FilePathType.Attachments,
                UserLanguageID = "en",
                company_id = AstuteConstants.CompanyId,
                Overwrite = "false",
            };

            var response = await this._caseStreamServiceClient.UploadFileAsync(request).ConfigureAwait(false);

            if (!(response is null) && !string.IsNullOrEmpty(response.FileName))
            {
                return response.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        [ExcludeFromCodeCoverage]
        public async Task AddAttachmentAsync(int caseId, string attachmentName, string attachmentDescription)
        {
            CaseListUpdateRequest request = new CaseListUpdateRequest
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                Type = CaseListUpdateType.Update,
                Case = new AstuteCaseService.Case[]
                {
                    new AstuteCaseService.Case()
                    {
                        APCWEditMode = AstuteCaseService.APCWEditModeType.Modified,
                        company_id = AstuteConstants.CompanyId,
                        case_id = caseId.ToString(),
                        CaseAttachmentList = new CaseAttachmentList()
                        {
                            CaseAttachment = new CaseAttachment[]
                            {
                                new CaseAttachment()
                                {
                                    case_attachment_seq = "-1",
                                    description = attachmentDescription,
                                    file_name = attachmentName,
                                    file_type = CaseAttachmentFileType.Standard,
                                    APCWEditMode = AstuteCaseService.APCWEditModeType.New
                                }
                            }
                        }
                    }
                },
                ResponseFormat = new ResponseFormat()
                {
                    CaseList = new CaseListFormat()
                    {
                        Case = new CaseFormat()
                        {
                            AllAttributes = AstuteCaseService.TrueFalseType.Item1,
                        }
                    }
                }
            };

            var response = await this._caseServiceClient.UpdateCaseAsync(request).ConfigureAwait(false);

            if (response is null || response.Case is null)
            {
                throw new AstuteException("Add attachment failed");
            }

            if (response.Valid != AstuteCaseService.ValidState.Ok)
            {
                this.LogAstuteError(response.MessageList?.Select(x => new LoggingStruct { Text = x.Text, Substitutions = x.Substitution?.Select(y => y.Text), isError = (response.Valid == AstuteCaseService.ValidState.Error) }));
            }
        }

        public async Task<(string CaseStatus, string ExceptionReason)> CreateNewCaseStatus(Models.Case caseRecord, IEnumerable<PatientCase> existingCasesForPatient)
        {
            string caseStatus = AstuteConstants.StatusOpen;
            string exceptionReason = null;

            if (!(existingCasesForPatient is null))
            {
                var casesWithMatchingDrugs = existingCasesForPatient.Where(x => x.PatientEvents.Count(y => !(y.DrugName is null) && y.DrugName.Contains(caseRecord.BaseProductName)) > 0);

                if (casesWithMatchingDrugs.Count() > 0)
                {
                    var mostRecentCaseWithMatchingDrugs = casesWithMatchingDrugs.FirstOrDefault(x => x.PatientEvents.Last().DateAdded == casesWithMatchingDrugs.Max(y => y.PatientEvents.Last().DateAdded));

                    if (mostRecentCaseWithMatchingDrugs.CaseId != 0)
                    {
                        var mostRecentProgramEvent = mostRecentCaseWithMatchingDrugs.PatientEvents.Where(x => x.ProgramHeader == AstuteConstants.SoSimple || x.ProgramHeader == AstuteConstants.Link).Last();

                        switch (caseRecord.AstuteProgramHeader)
                        {
                            case AstuteConstants.SoSimple:
                                if (mostRecentProgramEvent.IssueStatus == AstuteConstants.StatusOpen)
                                {
                                    exceptionReason = AstuteConstants.CaseStatusDuplicateCase;
                                }
                                else
                                {
                                    await this.ReopenCaseAsync(mostRecentCaseWithMatchingDrugs, caseRecord).ConfigureAwait(false);
                                    await this.ReleaseCaseAsync(mostRecentCaseWithMatchingDrugs.CaseId).ConfigureAwait(false);
                                    caseStatus = AstuteConstants.StatusClosed;
                                }
                                break;
                            case AstuteConstants.Link:
                                if (mostRecentProgramEvent.IssueStatus == AstuteConstants.StatusOpen)
                                {
                                    if (mostRecentProgramEvent.ProgramHeader == AstuteConstants.Link)
                                    {
                                        exceptionReason = AstuteConstants.CaseStatusDuplicateCase;
                                    }
                                    else
                                    {
                                        exceptionReason = AstuteConstants.CaseStatusLinkReview;
                                    }
                                }
                                else
                                {
                                    await this.ReopenCaseAsync(mostRecentCaseWithMatchingDrugs, caseRecord).ConfigureAwait(false);
                                    await this.ReleaseCaseAsync(mostRecentCaseWithMatchingDrugs.CaseId).ConfigureAwait(false);
                                    caseStatus = AstuteConstants.StatusClosed;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (caseStatus, exceptionReason);
        }

        private void LogAstuteError(IEnumerable<LoggingStruct> messageCollection)
        {
            string logMessage = string.Empty;
            bool throwException = false;

            foreach (var message in messageCollection)
            {
                logMessage = "Astute Error: ";
                if (!(message.Text is null))
                {
                    logMessage += message.Text.Replace('{', '(').Replace('}', ')');
                }
                else
                {
                    logMessage += "message has no Text object";
                }
                if (!(message.Substitutions is null))
                {
                    foreach (var substitution in message.Substitutions)
                    {
                        logMessage += " : " + substitution;
                    }
                }

                _log.LogError(logMessage);

                if (message.isError == true)
                    throwException = true;
            }

            if (throwException == true)
                throw new AstuteException(logMessage);
        }
    }
}
