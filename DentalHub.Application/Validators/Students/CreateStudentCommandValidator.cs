using FluentValidation;
using DentalHub.Application.Commands.Students;

namespace DentalHub.Application.Validators.Students
{
    public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentCommandValidator()
        {
            RuleFor(x => x.FullName)
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

            RuleFor(x => x.UniversityId)
                .NotEmpty().WithMessage("University ID is required")
                .MinimumLength(4).WithMessage("Invalid University ID Must Be More than 4 char ");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("UserName required")
                .Matches(@"^(?=.{3,20}$)(?!.*__)[a-z][a-z0-9_]*[a-z0-9]$").WithMessage("Invalid username format");

            RuleFor(x => x.Level)
                .InclusiveBetween(1, 7).WithMessage("Level must be between 1 and 7");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^(010|011|012|015)\d{8}$").WithMessage("Phone must be a valid Egyptian number");
        }
    }
}
