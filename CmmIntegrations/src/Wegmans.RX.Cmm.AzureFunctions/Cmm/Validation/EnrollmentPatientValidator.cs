using FluentValidation;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation
{
    public class EnrollmentPatientValidator : AbstractValidator<EnrollmentPatient>
    {
        public EnrollmentPatientValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
            RuleFor(x => x.PrimaryPhone).NotEmpty();

            RuleFor(x => x.Address).SetValidator(new AddressValidator());
            RuleFor(x => x.Case).SetValidator(new CaseValidator());
        }
    }
}
