using FluentValidation;
using DentalHub.Application.Commands.Patient;

namespace DentalHub.Application.Validators.Patients
{
    public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
    {
        public CreatePatientCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .Length(3, 100).WithMessage("Full name must be between 3 and 100 characters")
                .Matches(@"^[a-zA-Z\u0621-\u064A\s]+$").WithMessage("Full name can only contain letters and spaces");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .Length(8, 100).WithMessage("Password must be between 8 and 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number");

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

            RuleFor(x => x.City)
                .IsInEnum().WithMessage("Invalid city");
        }
    }
}
