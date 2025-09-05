using FluentValidation;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation
{
    public class PriorAuthorizationValidator : AbstractValidator<PriorAuthorization>
    {
        public PriorAuthorizationValidator()
        {
            RuleFor(x => x.PlanOutcome).Must(BeAValidPAOutcome).When(x => !(x.PlanOutcome is null)).WithMessage("'{PropertyName}' value is invalid");
        }

        private bool BeAValidPAOutcome(string paOutcome)
        {
            return paOutcome switch
            {
                var x when
                        x == "Favorable" ||
                        x == "Unfavorable" ||
                        x == "Cancelled" ||
                        x == "Pending" ||
                        x == "Unknown" ||
                        x == "Unsent" ||
                        x == "N/A" => true,
                _ => false
            };
        }
    }

}
