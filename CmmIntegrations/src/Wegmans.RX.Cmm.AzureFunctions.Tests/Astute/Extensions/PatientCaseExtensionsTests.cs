using AstuteCaseService;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Wegmans.RX.Cmm.AzureFunctions.Astute;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Extensions;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound;
using Xunit;

namespace Wegmans.RX.Cmm.AzureFunctions.Tests.Astute.Extensions
{
    public class PatientCaseExtensionsTests
    {
        [Theory]
        [MemberData(nameof(CreateCaseListUpdateRequestData))]
        public void CreateCaseListUpdateRequest(PatientCase patientCase, CaseStatus caseStatus, CaseListUpdateRequest expected)
        {
            var result = patientCase.CreateCaseListUpdateRequest(caseStatus);

            result.Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> CreateCaseListUpdateRequestData()
        {
            var patientCases = new Dictionary<string, PatientCase>
            {
                { "Minimum PatientCase", new PatientCase { PatientEvents = new List<PatientEvent> { new PatientEvent() } } }
            };

            var caseStatuses = new Dictionary<string, CaseStatus>
            {
                { "Minimum CaseStatus", new CaseStatus { Patient = new CaseStatusPatient(), PriorAuthorization = new PriorAuthorization(), AdditionalDetails = new AdditionalDetails { PharmacyBenefits = new PharmacyBenefits { PrimaryInsurance = new PharmacyBenefitsPrimaryInsurance() } } } }
            };

            var caseListUpdateRequests = new Dictionary<string, CaseListUpdateRequest>
            {
                { "Minimum CaseListUpdateRequest", new CaseListUpdateRequest
                    {
                        Case = new AstuteCaseService.Case[]
                        {
                            new AstuteCaseService.Case
                            {
                                APCWEditMode = APCWEditModeType.Modified,
                                company_id = AstuteConstants.CompanyId,
                                b07_code = AstuteConstants.ProgramType,
                                case_status = AstuteConstants.StatusOpen,
                                b27_code = AstuteConstants.EnrollmentSource,
                                b25_code = string.Empty,
                                b29_code = string.Empty,
                                b30_code = string.Empty,
                                b33_code = string.Empty,
                                b51_code = string.Empty,
                                b53_code = string.Empty,
                                b56_code = string.Empty,
                                b57_code = string.Empty,
                                b11_code = string.Empty,
                                b54_code = string.Empty,
                                b55_code = string.Empty,
                                b73_code = string.Empty,
                                b74_code = "PA-Not Submitted",
                                b75_code = string.Empty,
                                b79_code = string.Empty,
                                b24_code = string.Empty,
                                b76_code = string.Empty,
                                b28_code = DateTimeOffset.Now.ToString(),
                                b34_code = string.Empty,
                                b35_code = string.Empty,
                                b42_code = string.Empty,
                                b52_code = string.Empty,
                                case_id = "0",
                                IssueList = new IssueList()
                                {
                                    Issue = new Issue[]
                                    {
                                        new Issue()
                                        {
                                            issue_seq = "0",
                                            c78_code = "Is Approved: , Is Denied: ",
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
                                            description = "2"
                                        }
                                    }
                                }
                            }
                        },
                        ResponseFormat = (new ResponseFormat()).CreateDefaultCaseResponseFormat(),
                        UTCOffset = "0"
                    }
                }
            };

            return new List<object[]>
            {
                new object[] { patientCases["Minimum PatientCase"], caseStatuses["Minimum CaseStatus"], caseListUpdateRequests["Minimum CaseListUpdateRequest"] }
            };
        }
    }
}
