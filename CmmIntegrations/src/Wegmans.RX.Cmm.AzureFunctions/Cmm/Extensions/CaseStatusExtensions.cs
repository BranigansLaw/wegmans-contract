using System.Linq;
using System.Text.Json;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Extensions
{
    public static class CaseStatusExtensions
    {
        public static Astute.Models.CaseStatus CreateAstuteCaseStatus(this Cmm.Models.CaseStatus caseStatus) => new Astute.Models.CaseStatus()
        {
            CaseId = caseStatus.CaseId,
            IsCaseClosed = caseStatus.IsCaseClosed,
            CaseClosureReason = caseStatus.CaseClosureReason,
            Ndc = caseStatus.Ndc,
            Patient = new Astute.Models.CaseStatusPatient()
            {
                Id = caseStatus.Patient.Id,
                FirstName = caseStatus.Patient.FirstName,
                LastName = caseStatus.Patient.LastName,
                DateOfBirth = caseStatus.Patient.DateOfBirth,
                Gender = caseStatus.Patient.Gender,
                PrimaryPhone = caseStatus.Patient.PrimaryPhone,
                Address = new Astute.Models.Address()
                {
                    Address1 = caseStatus.Patient.Address.Address1,
                    Address2 = caseStatus.Patient.Address.Address2,
                    City = caseStatus.Patient.Address.City,
                    State = caseStatus.Patient.Address.State,
                    ZipCode = caseStatus.Patient.Address.ZipCode
                }
            },
            PriorAuthorization = new Astute.Models.PriorAuthorization()
            {
                SentToPlanAt = caseStatus.PriorAuthorization.SentToPlanAt,
                IsApproved = caseStatus.PriorAuthorization.IsApproved,
                IsDenied = caseStatus.PriorAuthorization.IsDenied,
                OutcomeReceivedAt = caseStatus.PriorAuthorization.OutcomeReceivedAt,
                AuthorizationPeriod = new Astute.Models.AuthorizationPeriod()
                {
                    EffectiveDate = caseStatus.PriorAuthorization.AuthorizationPeriod.EffectiveDate,
                    ExpirationDate = caseStatus.PriorAuthorization.AuthorizationPeriod.ExpirationDate
                },
                PlanOutcome = caseStatus.PriorAuthorization.PlanOutcome
            },
            HasGovernmentInsurance = caseStatus.PharmacyBenefits.EagleForce.HasGovernmentInsurance,
            AdditionalDetails = new Astute.Models.AdditionalDetails()
            {
                PriorAuthorizationAppeals = caseStatus.PriorAuthorizationAppeals?.Select(x => new Astute.Models.PriorAuthorization
                {
                    SentToPlanAt = x.SentToPlanAt,
                    IsApproved = x.IsApproved,
                    IsDenied = x.IsDenied,
                    OutcomeReceivedAt = x.OutcomeReceivedAt,
                    AuthorizationPeriod = new Astute.Models.AuthorizationPeriod()
                    {
                        EffectiveDate = x.AuthorizationPeriod.EffectiveDate,
                        ExpirationDate = x.AuthorizationPeriod.ExpirationDate
                    },
                    PlanOutcome = x.PlanOutcome
                }),
                PharmacyBenefits = new Astute.Models.PharmacyBenefits()
                {
                    PrimaryInsurance = new Astute.Models.PharmacyBenefitsPrimaryInsurance()
                    {
                        Bin = caseStatus.PharmacyBenefits.PrimaryInsurance.Bin,
                        Pcn = caseStatus.PharmacyBenefits.PrimaryInsurance.Pcn,
                        RxGroup = caseStatus.PharmacyBenefits.PrimaryInsurance.RxGroup,
                        MemberId = caseStatus.PharmacyBenefits.PrimaryInsurance.MemberId,
                        StartDate = caseStatus.PharmacyBenefits.PrimaryInsurance.StartDate,
                        EndDate = caseStatus.PharmacyBenefits.PrimaryInsurance.EndDate,
                        IsPaRequired = caseStatus.PharmacyBenefits.PrimaryInsurance.IsPaRequired,
                        PharmacyNpi = caseStatus.PharmacyBenefits.PrimaryInsurance.PharmacyNpi,
                        LineOfBusiness = caseStatus.PharmacyBenefits.PrimaryInsurance.LineOfBusiness,
                        Benefits = new Astute.Models.Benefits()
                        {
                            Coinsurance = caseStatus.PharmacyBenefits.PrimaryInsurance.Benefits.Coinsurance,
                            Copay = caseStatus.PharmacyBenefits.PrimaryInsurance.Benefits.Copay,
                            Deductible = caseStatus.PharmacyBenefits.PrimaryInsurance.Benefits.Deductible,
                            DeductibleMet = caseStatus.PharmacyBenefits.PrimaryInsurance.Benefits.DeductibleMet,
                            DeductibleRemaining = caseStatus.PharmacyBenefits.PrimaryInsurance.Benefits.DeductibleRemaining,
                            OopMax = caseStatus.PharmacyBenefits.PrimaryInsurance.Benefits.OopMax,
                            OopMaxMet = caseStatus.PharmacyBenefits.PrimaryInsurance.Benefits.OopMaxMet,
                            OopMaxRemaining = caseStatus.PharmacyBenefits.PrimaryInsurance.Benefits.OopMaxRemaining
                        }
                    }
                },
                MedicalBenefits = new Astute.Models.MedicalBenefits()
                {
                    PrimaryInsurance = new Astute.Models.MedicalBenefitsPrimaryInsurance()
                    {
                        MemberId = caseStatus.MedicalBenefits.PrimaryInsurance.MemberId,
                        PlanName = caseStatus.MedicalBenefits.PrimaryInsurance.PlanName,
                        Group = caseStatus.MedicalBenefits.PrimaryInsurance.Group,
                        IsPaRequired = caseStatus.MedicalBenefits.PrimaryInsurance.IsPaRequired,
                        LineOfBusiness = caseStatus.MedicalBenefits.PrimaryInsurance.LineOfBusiness,
                        StartDate = caseStatus.MedicalBenefits.PrimaryInsurance.StartDate,
                        EndDate = caseStatus.MedicalBenefits.PrimaryInsurance.EndDate,
                        Benefits = new Astute.Models.Benefits()
                        {
                            Coinsurance = caseStatus.MedicalBenefits.PrimaryInsurance.Benefits.Coinsurance,
                            Copay = caseStatus.MedicalBenefits.PrimaryInsurance.Benefits.Copay,
                            Deductible = caseStatus.MedicalBenefits.PrimaryInsurance.Benefits.Deductible,
                            DeductibleMet = caseStatus.MedicalBenefits.PrimaryInsurance.Benefits.DeductibleMet,
                            DeductibleRemaining = caseStatus.MedicalBenefits.PrimaryInsurance.Benefits.DeductibleRemaining,
                            OopMax = caseStatus.MedicalBenefits.PrimaryInsurance.Benefits.OopMax,
                            OopMaxMet = caseStatus.MedicalBenefits.PrimaryInsurance.Benefits.OopMaxMet,
                            OopMaxRemaining = caseStatus.MedicalBenefits.PrimaryInsurance.Benefits.OopMaxRemaining
                        }
                    }
                },
                Copay = new Astute.Models.Copay()
                {
                    IsEnrolled = caseStatus.Copay.IsEnrolled,
                    MemberId = caseStatus.Copay.MemberId
                }
            },
            LinkStatus = caseStatus.LinkStatus,
            LinkStatusReasons = caseStatus.LinkStatusReasons,
            LinkEligibilityEndDate = caseStatus.LinkEligibilityEndDate,
            ReverificationStartDate = caseStatus.ReverificationStartDate,
            ReverificationReason = caseStatus.ReverificationReason,
            CaseStatusPayload = JsonSerializer.Serialize(caseStatus)
        };
    }
}
