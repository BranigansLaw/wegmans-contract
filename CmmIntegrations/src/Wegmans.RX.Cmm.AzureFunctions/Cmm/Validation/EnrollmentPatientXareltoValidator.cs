using FluentValidation;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation
{
    /// <summary>
    /// Fluent patient validator for Xarelto
    /// </summary>
    public class EnrollmentPatientXareltoValidator : AbstractValidator<EnrollmentXareltoPatient>
    {
        public EnrollmentPatientXareltoValidator()
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
