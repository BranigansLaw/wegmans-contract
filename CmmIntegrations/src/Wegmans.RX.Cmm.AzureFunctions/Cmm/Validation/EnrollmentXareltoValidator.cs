using FluentValidation;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation
{
    /// <summary>
    /// Fluent validator for Xarelto 
    /// </summary>
    public class EnrollmentXareltoValidator : AbstractValidator<EnrollmentXarelto>
    {
        public EnrollmentXareltoValidator()
        {
            RuleFor(x => x.Patient).SetValidator(new EnrollmentPatientXareltoValidator());
        }
    }
}
