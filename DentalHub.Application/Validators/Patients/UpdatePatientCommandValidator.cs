using FluentValidation;
using DentalHub.Application.Commands.Patient;

namespace DentalHub.Application.Validators.Patients
{
    public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidator()
        {
            RuleFor(x => x.PublicId)
                .NotEmpty().WithMessage("Patient public ID is required");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .Length(3, 100).WithMessage("Full name must be between 3 and 100 characters")
                .Matches(@"^[a-zA-Z\u0621-\u064A\s]+$").WithMessage("Full name can only contain letters and spaces");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^(010|011|012|015)\d{8}$").WithMessage("Phone must be a valid Egyptian number");

            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required")
                .Matches(@"^\d{14}$").WithMessage("National ID must be exactly 14 digits");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date is required")
                .LessThan(DateTime.Now).WithMessage("Birth date must be in the past");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender");
        }
    }
}
