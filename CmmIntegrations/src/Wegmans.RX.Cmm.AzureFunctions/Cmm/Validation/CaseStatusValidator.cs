using FluentValidation;
using System.Collections.Generic;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation
{
    public class CaseStatusValidator : AbstractValidator<CaseStatus>
    {
        public CaseStatusValidator()
        {
            RuleFor(x => x.CaseId).NotEmpty();

            RuleFor(x => x.IsCaseClosed).NotNull();

            RuleFor(x => x.Ndc).NotEmpty();

            RuleFor(x => x.CaseClosureReason)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .When(x => x.IsCaseClosed == true)
                .Must(BeAValidCaseClosureReason)
                .When(x => x.IsCaseClosed == true)
                .WithMessage("'{PropertyName}' value is invalid.");

            RuleFor(x => x.LinkStatus)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(BeAValidLinkStatus)
                .WithMessage("'{PropertyName}' value is invalid.");

            RuleFor(x => x.LinkStatusReasons)
                .Must(BeAValidLinkStatusReasonCollection)
                .When(x => !(x.LinkStatusReasons is null))
                .WithMessage("'{PropertyName}' collection has invalid value(s).");

            RuleFor(x => x.ReverificationReason)
                .Cascade(CascadeMode.Stop)
                .Must(BeAValidReverificationReason)
                .When(x => !string.IsNullOrEmpty(x.ReverificationReason))
                .WithMessage("'{PropertyName}' value is invalid.");

            RuleFor(x => x.PriorAuthorization).SetValidator(new PriorAuthorizationValidator());
        }

        private bool BeAValidCaseClosureReason(string caseClosureReason)
        {
            return caseClosureReason switch
            {
                var x when
                    x == "Patient does not have coverage" ||
                    x == "Therapy unsuitable for patient" ||
                    x == "Case is duplicate" ||
                    x == "Case is complete" ||
                    x == "Case was restarted" ||
                    x == "Off-Label" ||
                    x == "Ops Correction" ||
                    x == "Patient Choice" ||
                    x == "Prescriber Choice" ||
                    x == "Federal Insurance" ||
                    x == "Reimbursement Issue" ||
                    x == "Other" => true,
                _ => false
            };
        }

        private bool BeAValidLinkStatus(string linkStatus)
        {
            return linkStatus switch
            {
                var x when
                    x == "Link Eligible" ||
                    x == "Link Pending" ||
                    x == "Not Link Eligible" => true,
                _ => false
            };
        }

        private bool BeAValidLinkStatusReasonCollection(IEnumerable<string> linkStatusReasons)
        {
            bool result = true;

            foreach (var item in linkStatusReasons)
            {
                result = item switch
                {
                    var linkStatusReason when
                        linkStatusReason == "Challenge Open" ||
                        linkStatusReason == "Challenge Closed" ||
                        linkStatusReason == "Insurance Update Needed" => true,
                    _ => false
                };

                if (!result)
                {
                    break;
                }
            }

            return result;
        }

        private bool BeAValidReverificationReason(string reverificationReason)
        {
            return reverificationReason switch
            {
                var x when
                x == "Formulary Win" ||
                x == "Change in Insurance" ||
                x == "Eligibility Anniversary Review" => true,
                _ => false
            };
        }
    }
}
