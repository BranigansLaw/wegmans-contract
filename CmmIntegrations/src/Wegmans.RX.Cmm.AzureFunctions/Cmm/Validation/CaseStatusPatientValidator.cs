using FluentValidation;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation
{
    public class CaseStatusPatientValidator : AbstractValidator<CaseStatusPatient>
    {
        public CaseStatusPatientValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
            RuleFor(x => x.PrimaryPhone).NotEmpty();

            RuleFor(x => x.Address).SetValidator(new AddressValidator());
        }
    }
}
