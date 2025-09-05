using FluentValidation;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation
{
    public class CaseValidator : AbstractValidator<Case>
    {
        public CaseValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.ReferralId).NotEmpty();
            RuleFor(x => x.Ndc).NotEmpty();
            RuleFor(x => x.DiagnosisCode).NotEmpty();
            RuleFor(x => x.ProgramName).NotEmpty();
        }
    }
}
