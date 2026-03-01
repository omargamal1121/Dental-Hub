using FluentValidation;
using DentalHub.Application.Commands.Doctor;

namespace DentalHub.Application.Validators.Doctors
{
    public class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
    {
        public CreateDoctorCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Full name is required")
                .Length(3, 100).WithMessage("Full name must be between 3 and 100 characters")
                .Matches(@"^[a-zA-Z\u0621-\u064A\s]+$").WithMessage("Full name can only contain letters and spaces");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .Length(8, 100).WithMessage("Password must be between 8 and 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")
                .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number");

            RuleFor(x => x.Specialty)
                .NotEmpty().WithMessage("Specialty is required")
                .Length(3, 100).WithMessage("Specialty must be between 3 and 100 characters");

            RuleFor(x => x.UniversityId)
                .NotEmpty().WithMessage("University ID is required");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("UserName required")
                .Matches(@"^(?=.{3,20}$)(?!.*__)[a-z][a-z0-9_]*[a-z0-9]$").WithMessage("Invalid username format");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?[1-9]\d{7,14}$").WithMessage("Invalid phone number format");
        }
    }
}
