using AstuteCaseService;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Extensions
{
    public static class PatientCaseExtensions
    {
        [ExcludeFromCodeCoverage]
        public static CaseListUpdateRequest CreateCaseListReopenRequest(this PatientCase patientCase, Models.Case caseRecord)
        {
            return new CaseListUpdateRequest()
            {
                UTCOffset = "0",
                Type = CaseListUpdateType.Update,
                Case = new AstuteCaseService.Case[]
                {
                    new AstuteCaseService.Case()
                    {
                        APCWEditMode = AstuteCaseService.APCWEditModeType.Modified,
                        company_id = AstuteConstants.CompanyId,
                        case_id = patientCase.CaseId.ToString(),
                        address_id = caseRecord.AstuteAddressId.ToString(),
                        case_status = AstuteConstants.StatusOpen,
                        b09_code = AstuteConstants.EnrollmentSource, // Triage (enrollment) source
                        b48_code = caseRecord.Id,
                        b49_code = caseRecord.ReferralId,
                        b36_code = (!string.IsNullOrEmpty(caseRecord.DiagnosisCode) && caseRecord.DiagnosisCode.Length > 40) ? caseRecord.DiagnosisCode.Substring(0, 40) : caseRecord.DiagnosisCode,
                        b37_code = caseRecord.ProgramName,
                        b08_code = caseRecord.AstuteShipFirstDoseToPrescriber ?? string.Empty,
                        b06_code = string.Empty,
                        b20_code = string.Empty,
                        b19_code = string.Empty,
                        b59_code = string.Empty,
                        b60_code = string.Empty,                       
                        AddressList = new AddressList()
                        {
                            Address = new AstuteCaseService.Address[]
                            {
                                new AstuteCaseService.Address()
                                {
                                    APCWEditMode = APCWEditModeType.Modified,
                                    company_id = AstuteConstants.CompanyId,
                                    address_id = caseRecord.AstuteAddressId.ToString(),   
                                    active = YesNo.Y,
                                    a24_code = caseRecord.Provider.Address.Address1 ?? string.Empty,   
                                    a25_code = caseRecord.Provider.Address.Address2 ?? string.Empty,                   
                                    a27_code = caseRecord.Provider.Address.City ?? string.Empty,                  
                                    a28_code = caseRecord.Provider.Address.State ?? string.Empty,                  
                                    a29_code = caseRecord.Provider.Address.ZipCode ?? string.Empty,                              
                                    a34_code = string.Empty,
                                    a31_code = caseRecord.Provider.Fax ?? string.Empty,
                                    a22_code = caseRecord.Provider.FirstName ?? string.Empty,                        
                                    a23_code = caseRecord.Provider.LastName ?? string.Empty,                            
                                    a36_code = caseRecord.Provider.Npi ?? string.Empty,                         
                                    a30_code = caseRecord.Provider.Phone ?? string.Empty,                        
                                }
                            }
                        },
                        IssueList = new IssueList()
                        {
                            Issue = new Issue[]
                            {
                                new Issue()
                                {
                                    APCWEditMode = AstuteCaseService.APCWEditModeType.New,
                                    c88_code = AstuteConstants.StatusOpen,
                                    product_code = caseRecord.AstuteProductName,
                                    c39_code = caseRecord.Ndc,
                                    c47_code = caseRecord.AstuteProgramHeader,
                                    c26_code = DateTimeOffset.Now.ToString(), // Patient status change date
                                    c28_code = DateTimeOffset.Now.ToString(), // Triage (enrollment) received date
                                    c35_code = DateTimeOffset.Now.ToString(), // Follow-Up Date
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
                                }
                            }
                        },
                    }
                },
                ResponseFormat = (new ResponseFormat()).CreateDefaultCaseResponseFormat()
            };
        }

        public static CaseListUpdateRequest CreateCaseListUpdateRequest(this PatientCase patientCase, CaseStatus caseStatus)
        {
            _ = patientCase.PatientEvents ?? throw new ArgumentNullException(nameof(patientCase.PatientEvents));

            if (patientCase.PatientEvents.Count() == 0)
            {
                throw new ArgumentException("Collection must contain at least one item", nameof(patientCase.PatientEvents));
            }

            var issueSequence = patientCase.PatientEvents.Where(x => x.IssueSequence == patientCase.PatientEvents.Max(z => z.IssueSequence)).First().IssueSequence;

            var request = new CaseListUpdateRequest()
            {
                UTCOffset = "0",
                Type = CaseListUpdateType.Update,
                Case = new AstuteCaseService.Case[]
                {
                    new AstuteCaseService.Case()
                    {
                        APCWEditMode = AstuteCaseService.APCWEditModeType.Modified,
                        company_id = AstuteConstants.CompanyId,
                        b07_code = AstuteConstants.ProgramType,
                        case_status = AstuteConstants.StatusOpen,
                        case_id = patientCase.CaseId.ToString(),
                        b25_code = string.Empty,
                        b27_code = AstuteConstants.EnrollmentSource, // Prior Auth source
                        b51_code = caseStatus.IsCaseClosed.ToString() ?? string.Empty,
                        b11_code = caseStatus.AstuteCaseClosureReason ?? string.Empty,
                        b29_code = caseStatus.PriorAuthorization.SentToPlanAt.ToString() ?? string.Empty,
                        b30_code = string.Empty,
                        b33_code = caseStatus.ReverificationStartDate.HasValue ? caseStatus.ReverificationStartDate.Value.ToString("d", CultureInfo.InvariantCulture) : string.Empty,
                        b53_code = string.Empty,
                        b54_code = caseStatus.PriorAuthorization.OutcomeReceivedAt.HasValue ? caseStatus.PriorAuthorization.OutcomeReceivedAt.Value.ToString("G", CultureInfo.InvariantCulture) : string.Empty,
                        b55_code = caseStatus.AdditionalDetails.AppealSentToPlanAt.HasValue ? caseStatus.AdditionalDetails.AppealSentToPlanAt.Value.ToString("G", CultureInfo.InvariantCulture) : string.Empty,
                        b56_code = caseStatus.AdditionalDetails.AppealCmmPaStatus ?? string.Empty,
                        b57_code = caseStatus.AdditionalDetails.AppealPaStatus ?? string.Empty,
                        b18_code = caseStatus.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.PharmacyNpi,
                        b76_code = caseStatus.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.LineOfBusiness ?? string.Empty,
                        b73_code = caseStatus.GeCheck ?? string.Empty,
                        b74_code = caseStatus.PriorAuthorization.PaStatus ?? string.Empty,
                        b75_code = caseStatus.LinkStatus ?? string.Empty,
                        b79_code = caseStatus.GetLinkStatusReason(0) ?? string.Empty,
                        b24_code = !string.IsNullOrEmpty(caseStatus.GetLinkStatusReason(1)) && !string.IsNullOrEmpty(caseStatus.GetLinkStatusReason(2)) ?
                            $"{caseStatus.GetLinkStatusReason(1)},{caseStatus.GetLinkStatusReason(2)}" : caseStatus.GetLinkStatusReason(1) ?? string.Empty,
                        b28_code = DateTimeOffset.Now.ToString(), // Prior Auth Received Date
                        b34_code = caseStatus.AdditionalDetails.Copay?.AstuteIsEnrolled ?? string.Empty,
                        b35_code = caseStatus.AdditionalDetails.Copay?.MemberId ?? string.Empty,
                        b42_code = caseStatus.LinkEligibilityEndDate.HasValue ? caseStatus.LinkEligibilityEndDate.Value.ToString("d", CultureInfo.InvariantCulture) : string.Empty,
                        b52_code = caseStatus.ReverificationReason ?? string.Empty,
                        CaseTextList = new CaseTextList()
                        {
                            CaseText = new AstuteCaseService.CaseText[]
                            {
                                new AstuteCaseService.CaseText()
                                {
                                    text_type_code = "AdminOnly-DoNotUse",
                                    description = "2",
                                    case_text = caseStatus.CaseStatusPayload
                                }
                            }
                        }
                    }
                },
                ResponseFormat = (new ResponseFormat()).CreateDefaultCaseResponseFormat()
            };

            var issueList = new List<Issue>()
            {
                new Issue()
                {
                    issue_seq = issueSequence.ToString(),
                    c78_code = caseStatus.PriorAuthorization.CmmPaStatus,
                }
            };

            if (caseStatus.LinkStatusReasons?.Count() > 0 && caseStatus.LinkStatusReasons.Contains(AstuteConstants.InsuranceUpdateNeeded))
            {
                issueList.Add(new Issue()
                {
                    APCWEditMode = APCWEditModeType.New,
                    c19_code = AstuteConstants.InsuranceUpdateWorkflowStatus,
                    c35_code = DateTimeOffset.Now.Date.ToString(),
                    c47_code = AstuteConstants.PatientOutreach
                });
            }

            request.Case[0].IssueList = new IssueList()
            {
                Issue = issueList.ToArray()
            };

            if (caseStatus.ReverificationStartDate.HasValue)
            {
                if (!patientCase.ReverificationStartDate.HasValue || patientCase.ReverificationStartDate.Value < caseStatus.ReverificationStartDate.Value)
                {
                    request.Case[0].b38_code = AstuteConstants.LinkEnrollmentDefault;
                }
            }

            var updateExtendedPatientIdList = false;
            var phoneList = new List<Phone>();
            var extendedPatientIdList = patientCase.ExtendedPatientIdList ?? string.Empty;

            if (!string.IsNullOrEmpty(caseStatus.Patient.Id))
            {
                if (string.IsNullOrEmpty(patientCase.CmmPatientId))
                {
                    phoneList.Add(new Phone()
                    {
                        phone_type_code = AstuteConstants.CmmPatientType,
                        phone = caseStatus.Patient.Id
                    });
                }
                else
                {
                    if (patientCase.CmmPatientId != caseStatus.Patient.Id && !extendedPatientIdList.Contains(caseStatus.Patient.Id))
                    {
                        if (!string.IsNullOrEmpty(extendedPatientIdList))
                        {
                            extendedPatientIdList += ",";
                        }
                        extendedPatientIdList += caseStatus.Patient.Id;
                        updateExtendedPatientIdList = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(caseStatus.Patient.JcpId))
            {
                if (string.IsNullOrEmpty(patientCase.JcpId))
                {
                    phoneList.Add(new Phone()
                    {
                        phone_type_code = AstuteConstants.JcpPatientId,
                        phone = caseStatus.Patient.JcpId
                    });
                }
                else
                {
                    if (patientCase.JcpId != caseStatus.Patient.JcpId && !extendedPatientIdList.Contains(caseStatus.Patient.JcpId))
                    {
                        if (!string.IsNullOrEmpty(extendedPatientIdList))
                        {
                            extendedPatientIdList += ",";
                        }
                        extendedPatientIdList += caseStatus.Patient.JcpId;
                        updateExtendedPatientIdList = true;
                    }
                }
            }

            if (phoneList.Count > 0 || updateExtendedPatientIdList)
            {
                var addressList = new AddressList()
                {
                    Address = new AstuteCaseService.Address[]
                    {
                        new AstuteCaseService.Address()
                        {
                            address_id = caseStatus.AstuteAddressId.ToString(),
                            APCWEditMode = APCWEditModeType.Modified,
                            company_id = AstuteConstants.CompanyId,
                            active = YesNo.Y
                        }
                    }
                };

                if (phoneList.Count > 0)
                {
                    addressList.Address[0].PhoneList = new PhoneList()
                    {
                        Phone = phoneList.ToArray()
                    };
                }

                if (updateExtendedPatientIdList)
                {
                    addressList.Address[0].Extended = new AddressExtended()
                    {
                        p03_code = extendedPatientIdList
                    };
                }

                request.Case[0].AddressList = addressList;
            }

            return request;
        }
    }
}
