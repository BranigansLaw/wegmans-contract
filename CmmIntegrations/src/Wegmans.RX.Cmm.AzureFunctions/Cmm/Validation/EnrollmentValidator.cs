using FluentValidation;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation
{
    public class EnrollmentValidator : AbstractValidator<Enrollment>
    {
        public EnrollmentValidator()
        {
            RuleFor(x => x.Patient).SetValidator(new EnrollmentPatientValidator());
        }            
    }
}
