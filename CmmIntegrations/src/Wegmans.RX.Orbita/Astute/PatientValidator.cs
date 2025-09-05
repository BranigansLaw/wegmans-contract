using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wegmans.RX.Orbita.Astute
{
    public class PatientValidator : AbstractValidator<Models.Patient>
    {
        public PatientValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
        }
    }
}
