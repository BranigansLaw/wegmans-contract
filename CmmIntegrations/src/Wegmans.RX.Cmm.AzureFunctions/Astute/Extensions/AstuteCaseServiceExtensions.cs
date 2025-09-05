using AstuteCaseService;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Extensions
{
    public static class AstuteCaseServiceExtensions
    {
        [ExcludeFromCodeCoverage]
        public static ResponseFormat CreateDefaultCaseResponseFormat(this ResponseFormat responseFormat) => new ResponseFormat()
        {
            CaseList = new CaseListFormat()
            {
                Case = new CaseFormat()
                {
                    AllAttributes = TrueFalseType.Item1,
                    AddressList = new AddressListFormat()
                    {
                        Address = new AddressFormat()
                        {
                            AllAttributes = TrueFalseType.Item1,
                            PhoneList = new PhoneFormat[]
                                    {
                                        new PhoneFormat()
                                        {
                                            AllAttributes = TrueFalseType.Item1
                                        }
                                    }
                        }
                    },
                    IssueList = new IssueListFormat()
                    {
                        Issue = new IssueFormat()
                        {
                            AllAttributes = TrueFalseType.Item1
                        }
                    }
                }
            }
        };

        public static PatientStatus CreateAstutePatientStatus(this AstuteCaseService.Case astuteCase, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (astuteCase?.IssueList?.Issue == null || astuteCase.IssueList?.Issue?
                .Where(x => !(x.c16_code is null) && (x.c47_code == AstuteConstants.SoSimple || x.c47_code == AstuteConstants.Link)).Count() == 0)
            {
                return null;
            }

            var validIssues = astuteCase.IssueList.Issue
                .Where(x => !(x.c16_code is null) && (x.c47_code == AstuteConstants.SoSimple || x.c47_code == AstuteConstants.Link))
                .OrderBy(s => Convert.ToInt32(s.issue_seq));

            var issue = validIssues.Last();

            var updatedIssues = validIssues
                .Where(x => DateTime.TryParse(x.c26_code, out DateTime patientStatusChangeDate) && patientStatusChangeDate >= startDate && patientStatusChangeDate < endDate);

            if (updatedIssues.Count() > 0)
            {
                issue = updatedIssues.Last();
            }

            DateTime? shippedDate = null;
            DateTime? linkEnrollmentDate = null;
            DateTime? reverificationStartDate = null;

            if (DateTime.TryParse(issue.c12_code, out DateTime parsedShipDate))
            {
                shippedDate = parsedShipDate;
            }

            if (DateTime.TryParse(astuteCase.b44_code, out DateTime parsedLinkEnrollmentDate))
            {
                linkEnrollmentDate = parsedLinkEnrollmentDate;
            }

            if (DateTime.TryParse(astuteCase.b33_code, out DateTime parsedReverificationStartDate))
            {
                reverificationStartDate = parsedReverificationStartDate;
            }

            var primaryPbmName = string.Empty;
            var primaryPbmBin = string.Empty;
            var primaryPbmPcn = string.Empty;
            var primaryPbmGroupId = string.Empty;
            var primaryPbmPlanId = string.Empty;

            var patientAddresses = astuteCase.AddressList?.Address?.Where(x => x.address_type_code == AstuteConstants.AddressTypeCode);

            if (patientAddresses.Count() > 0)
            {
                var patientAddress = patientAddresses.First();
                primaryPbmName = patientAddress.a07_code;
                primaryPbmBin = patientAddress.a09_code;
                primaryPbmPcn = patientAddress.a10_code;
                primaryPbmGroupId = patientAddress.a11_code;
                primaryPbmPlanId = patientAddress.a08_code;
            }

            return new PatientStatus
            {
                AstuteCaseId = astuteCase.case_id,
                SourceIdentifier = astuteCase.b49_code, // CMM Referral ID
                TransferPharmacyName = astuteCase.b15_code,
                TransferPharmacyPhoneNumber = astuteCase.b16_code,
                TransferPharmacyNpi = (string.IsNullOrEmpty(astuteCase.b15_code) && string.IsNullOrEmpty(astuteCase.b16_code)) ? astuteCase.b18_code : string.Empty,
                AstuteIssueSequence = issue.issue_seq,
                PatientStatusCode = !string.IsNullOrEmpty(issue.c16_code) && issue.c16_code.Contains("=") ? issue.c16_code.Substring(0, issue.c16_code.IndexOf("=")) : issue.c16_code,
                LastShipmentDate = shippedDate,
                LinkEnrollmentFlag = string.IsNullOrEmpty(astuteCase.b38_code) ? string.Empty : astuteCase.b38_code.Substring(0, 1).ToLower(),
                LinkEnrollmentDate = linkEnrollmentDate,
                ReverificationStartDate = reverificationStartDate,
                PrimaryPbmName = primaryPbmName,
                PrimaryPbmBin = primaryPbmBin,
                PrimaryPbmPcn = primaryPbmPcn,
                PrimaryPbmGroupId = primaryPbmGroupId,
                PrimaryPbmPlanId = primaryPbmPlanId
            };
        }

        public static PatientCase CreatePatientCase(this AstuteCaseService.Case astuteCase)
        {
            DateTime? caseChangedDate = null;
            DateTime? outcomeReceivedAt = null;
            DateTime? appealSentToPlanAt = null;
            DateTime? dateOfBirth = null;
            DateTime? reverificationStartDate = null;

            if (DateTime.TryParse(astuteCase.b43_code, out DateTime parseCaseChangedDate))
            {
                caseChangedDate = parseCaseChangedDate;
            }
            if (DateTime.TryParse(astuteCase.b54_code, out DateTime parseOutcomeReceivedAt))
            {
                outcomeReceivedAt = parseOutcomeReceivedAt;
            }
            if (DateTime.TryParse(astuteCase.b55_code, out DateTime parseAppealSentToPlanAt))
            {
                appealSentToPlanAt = parseAppealSentToPlanAt;
            }
            if (DateTime.TryParse(astuteCase.b33_code, out DateTime parseReverificationStartDate))
            {
                reverificationStartDate = parseReverificationStartDate;
            }

            var address = astuteCase.AddressList?.Address?.FirstOrDefault(x => x.address_type_code == AstuteConstants.AddressTypeCode);

            if (!(address is null) && DateTime.TryParse(address.a05_code, out DateTime parseDateOfBirth))
            {
                dateOfBirth = parseDateOfBirth;
            }

            return new PatientCase()
            {
                CaseId = Convert.ToInt32(astuteCase.case_id),
                ProgramType = astuteCase.b07_code,
                PriorAuthSource = astuteCase.b27_code,
                CaseChangedDate = caseChangedDate,
                ExceptionReason = astuteCase.case_status_code,
                TriageSource = astuteCase.b09_code,
                CmmCaseId = astuteCase.b48_code,
                CmmReferralId = astuteCase.b49_code,
                DiagnosisCode = astuteCase.b36_code,
                LinkEnrollment = astuteCase.b38_code,
                IsCaseClosed = astuteCase.b51_code,
                CaseClosureReason = astuteCase.b11_code,
                SentToPlanAt = astuteCase.b29_code,
                OutcomeReceivedAt = outcomeReceivedAt,
                AppealSentToPlanAt = appealSentToPlanAt,
                AppealCmmPriorAuthStatus = astuteCase.b56_code,
                AppealPriorAuthStatus = astuteCase.b57_code,
                SpecialtyPharmacyNpi = astuteCase.b18_code,
                SpecialtyPharmacyName = astuteCase.b15_code,
                SpecialtyPharmacyPhone = astuteCase.b16_code,
                PayerType = astuteCase.b76_code,
                ShipFirstDoseToPrescriber = astuteCase.b08_code,
                GeCheck = astuteCase.b73_code,
                PriorAuthStatus = astuteCase.b74_code,
                LinkStatus = astuteCase.b75_code,
                LinkStatusReason1 = astuteCase.b79_code,
                LinkStatusReason2 = astuteCase.b24_code,
                PatientId = address is null ? default : Convert.ToInt32(address.address_id),
                CmmPatientId = address?.PhoneList?.Phone?.FirstOrDefault(x => x.phone_type_code == AstuteConstants.CmmPatientType)?.phone,
                FirstName = address?.given_names,
                LastName = address?.last_name,
                DateOfBirth = dateOfBirth,
                Gender = address?.a20_code,
                Address1 = address?.address1,
                Address2 = address?.address2,
                City = address?.city,
                State = address?.state,
                ZipCode = address?.postal_code,
                PrescriberFirstName = address?.a22_code,
                PrescriberLastName = address?.a23_code,
                PrescriberNpi = address?.a36_code,
                PrescriberPhone = address?.a30_code,
                PrescriberFax = address?.a31_code,
                PrescriberAddress1 = address?.a24_code,
                PrescriberAddress2 = address?.a25_code,
                PrescriberCity = address?.a27_code,
                PrescriberState = address?.a28_code,
                PrescriberZipCode = address?.a29_code,
                PrimaryPhone = address?.PhoneList?.Phone?.FirstOrDefault(x => x.phone_type_code == AstuteConstants.PrimaryPhone)?.phone,
                PatientEvents = astuteCase.IssueList?.Issue?.Select(x => new PatientEvent
                {
                    IssueSequence = Convert.ToInt32(x.issue_seq),
                    CmmDataExtractedDate = DateTime.TryParse(x.c76_code, out DateTime cmmDataExtractedDate) ? cmmDataExtractedDate : (DateTime?)null,
                    CmmDataProcessedDate = DateTime.TryParse(x.c73_code, out DateTime cmmDataProcessedDate) ? cmmDataProcessedDate : (DateTime?)null,
                    PatientStatusChangeDate = DateTime.TryParse(x.c26_code, out DateTime patientStatusChangeDate) ? patientStatusChangeDate : (DateTime?)null,
                    DrugName = x.product_code,
                    Ndc = x.c39_code,
                    ProgramHeader = x.c47_code,
                    TriageReceivedDate = DateTime.TryParse(x.c28_code, out DateTime triageReceivedDate) ? triageReceivedDate : (DateTime?)null,
                    FollowUpDate = DateTime.TryParse(x.c35_code, out DateTime followUpDate) ? followUpDate : (DateTime?)null,
                    PatientStatus = x.c16_code,
                    WorkflowStatus = x.c19_code,
                    ShippedDate = DateTime.TryParse(x.c12_code, out DateTime shippedDate) ? shippedDate : (DateTime?)null,
                    DateAdded = DateTime.TryParse(x.date_added, out DateTime addedDate) ? addedDate : (DateTime?)null,
                    IssueStatus = x.c88_code,
                    CmmPriorAuthStatus = x.c78_code,
                }
                )?.OrderBy(s => Convert.ToInt32(s.IssueSequence)),
                ReverificationStartDate = reverificationStartDate
            };
        }
    }
}
