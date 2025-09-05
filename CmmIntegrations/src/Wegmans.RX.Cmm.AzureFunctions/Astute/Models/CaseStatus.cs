using System;
using System.Collections.Generic;
using System.Linq;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models
{
    public class CaseStatus
    {
        private static readonly Dictionary<string, string> LinkStatusReasonMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Challenge Open", "90 Day Pending" },
            { "Challenge Closed", "90 Day Closed" },
            { "Invalid Clinical Rationale", "Invalid Clinical Rationale" },
            { "Benefits Available", "Benefits Available" },
            { "Product Not Covered", "Product Not Covered" }
        };

        private static readonly Dictionary<string, string> CaseClosureReasonMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Patient does not have coverage", "Closed - Patient does not have coverage" },
            { "Therapy unsuitable for patient", "Closed - Therapy unsuitable for patient" },
            { "Case is duplicate", "Closed" },
            { "Case is complete", "Closed" },
            { "Case was restarted", "Closed" },
            { "Off-Label", "Closed - Off-Label" },
            { "Ops Correction", "Closed" },
            { "Patient Choice", "Closed - Patient Choice" },
            { "Prescriber Choice", "Closed - Prescriber Choice" },
            { "Federal Insurance", "Closed - Federal Insurance" },
            { "Reimbursement Issue", "Closed - Reimbursement Issue" },
            { "Other", "Closed" }
        };

        public int AstuteCaseId { get; set; }

        public int AstuteAddressId { get; set; }

        public string CaseId { get; set; }

        public bool? IsCaseClosed { get; set; }

        public string CaseClosureReason { get; set; }

        public string Ndc { get; set; }

        public CaseStatusPatient Patient { get; set; }

        public PriorAuthorization PriorAuthorization { get; set; }

        public AdditionalDetails AdditionalDetails { get; set; }

        public bool? HasGovernmentInsurance { private get; set; }

        public string GeCheck 
        { 
            get
            {
                return this.HasGovernmentInsurance switch
                {
                    true => "Fail",
                    false => "Pass",
                    _ => string.Empty,
                };
            }
        }

        public string LinkStatus { get; set; }

        public IEnumerable<string> LinkStatusReasons { get; set; }

        public DateTimeOffset? LinkEligibilityEndDate { get; set; }

        public DateTimeOffset? ReverificationStartDate { get; set; }

        public string ReverificationReason { get; set; }

        public string GetLinkStatusReason(int index)
        {
            if (!(this.LinkStatusReasons is null))
            {
                var linkStatusReason = this.LinkStatusReasons.ElementAtOrDefault(index);
                if (!(linkStatusReason is null) && LinkStatusReasonMap.ContainsKey(linkStatusReason))
                {
                    return LinkStatusReasonMap[linkStatusReason];
                }
                else
                {
                    return linkStatusReason;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public string AstuteCaseClosureReason
        {
            get
            {
                string caseClosureReason = this.CaseClosureReason;
                if (!string.IsNullOrEmpty(this.CaseClosureReason) && CaseClosureReasonMap.ContainsKey(this.CaseClosureReason))
                {
                    caseClosureReason = CaseClosureReasonMap[this.CaseClosureReason];
                }
                return caseClosureReason;
            }
        }

        public string CaseStatusPayload { get; set; }

        public string FormattedVerificationOfBenefitsData
        {
            get
            {
                var vobText = string.Empty;

                vobText += $"Prior Authorization{Environment.NewLine}" +
                    $"\tAuthorization Period{Environment.NewLine}" +
                    $"\t\tEffective Date: {this.PriorAuthorization.AuthorizationPeriod?.EffectiveDate}{Environment.NewLine}" +
                    $"\t\tExpiration Date: {this.PriorAuthorization.AuthorizationPeriod?.ExpirationDate}{Environment.NewLine}" +
                    Environment.NewLine;

                vobText += $"Prior Authorization Appeals{Environment.NewLine}";
                if (!(this.AdditionalDetails.PriorAuthorizationAppeals is null))
                {
                    foreach (var appeal in this.AdditionalDetails.PriorAuthorizationAppeals)
                    {
                        vobText += $"\tOutcome Received At: {appeal.OutcomeReceivedAt}{Environment.NewLine}" +
                            $"\tAuthorization Period{Environment.NewLine}" +
                            $"\t\tEffective Date: {appeal.AuthorizationPeriod?.EffectiveDate}{Environment.NewLine}" +
                            $"\t\tExpiration Date: {appeal.AuthorizationPeriod?.ExpirationDate}{Environment.NewLine}";
                    }
                }
                vobText += Environment.NewLine;

                vobText += $"Pharmacy Benefits{Environment.NewLine}" +
                    $"\tPrimary Insurance{Environment.NewLine}" +
                    $"\t\tBin: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.Bin}{Environment.NewLine}" +
                    $"\t\tPCN: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.Pcn}{Environment.NewLine}" +
                    $"\t\tRx Group: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.RxGroup}{Environment.NewLine}" +
                    $"\t\tMember Id: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.MemberId}{Environment.NewLine}" +
                    $"\t\tStart Date: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.StartDate}{Environment.NewLine}" +
                    $"\t\tEnd Date: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.EndDate}{Environment.NewLine}" +
                    $"\t\tIs Pa Required: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.AstuteIsPaRequired}{Environment.NewLine}" +
                    $"\t\tPharmacy Npi: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.PharmacyNpi}{Environment.NewLine}" +
                    $"\t\tLine Of Business: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.LineOfBusiness}{Environment.NewLine}" +
                    $"\t\tBenefits{Environment.NewLine}" +
                    $"\t\t\tCoinsurance: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.Benefits?.Coinsurance}{Environment.NewLine}" +
                    $"\t\t\tCopay: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.Benefits?.Copay}{Environment.NewLine}" +
                    $"\t\t\tDeductible: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.Benefits?.Deductible}{Environment.NewLine}" +
                    $"\t\t\tDeductible Met: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.Benefits?.DeductibleMet}{Environment.NewLine}" +
                    $"\t\t\tDeductible Remaining: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.Benefits?.DeductibleRemaining}{Environment.NewLine}" +
                    $"\t\t\tOop Max: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.Benefits?.OopMax}{Environment.NewLine}" +
                    $"\t\t\tOop Max Met: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.Benefits?.OopMaxMet}{Environment.NewLine}" +
                    $"\t\t\tOop Max Remaining: {this.AdditionalDetails.PharmacyBenefits.PrimaryInsurance.Benefits?.OopMaxRemaining}{Environment.NewLine}" +
                    $"{Environment.NewLine}";

                vobText += $"Medical Benefits{Environment.NewLine}" +
                    $"\tPrimary Insurance{Environment.NewLine}" +
                    $"\t\tMember Id: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.MemberId}{Environment.NewLine}" +
                    $"\t\tPlan Name: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.PlanName}{Environment.NewLine}" +
                    $"\t\tGroup: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.Group}{Environment.NewLine}" +
                    $"\t\tIs Pa Required: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.AstuteIsPaRequired}{Environment.NewLine}" +
                    $"\t\tLine Of Business: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.LineOfBusiness}{Environment.NewLine}" +
                    $"\t\tStart Date: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.StartDate}{Environment.NewLine}" +
                    $"\t\tEnd Date: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.EndDate}{Environment.NewLine}" +
                    $"\t\tBenefits{Environment.NewLine}" +
                    $"\t\t\tCoinsurance: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.Benefits?.Coinsurance}{Environment.NewLine}" +
                    $"\t\t\tCopay: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.Benefits?.Copay}{Environment.NewLine}" +
                    $"\t\t\tDeductible: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.Benefits?.Deductible}{Environment.NewLine}" +
                    $"\t\t\tDeductible Met: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.Benefits?.DeductibleMet}{Environment.NewLine}" +
                    $"\t\t\tDeductible Remaining: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.Benefits?.DeductibleRemaining}{Environment.NewLine}" +
                    $"\t\t\tOop Max: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.Benefits?.OopMax}{Environment.NewLine}" +
                    $"\t\t\tOop Max Met: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.Benefits?.OopMaxMet}{Environment.NewLine}" +
                    $"\t\t\tOop Max Remaining: {this.AdditionalDetails.MedicalBenefits?.PrimaryInsurance.Benefits?.OopMaxRemaining}{Environment.NewLine}" +
                    $"{Environment.NewLine}";

                vobText += $"Copay{Environment.NewLine}" +
                    $"\tIs Enrolled: {this.AdditionalDetails.Copay?.AstuteIsEnrolled}{Environment.NewLine}" +
                    $"\tMember Id: {this.AdditionalDetails.Copay?.MemberId}{Environment.NewLine}";

                return vobText;
            }
        }
    }
}
