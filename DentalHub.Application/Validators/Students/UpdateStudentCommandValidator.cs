using FluentValidation;
using DentalHub.Application.Commands.Students;

namespace DentalHub.Application.Validators.Students
{
    public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
    {
        public UpdateStudentCommandValidator()
        {
            RuleFor(x => x.PublicId)
                .NotEmpty().WithMessage("Student public ID is required");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .Length(3, 100).WithMessage("Full name must be between 3 and 100 characters")
                .Matches(@"^[a-zA-Z\u0621-\u064A\s]+$").WithMessage("Full name can only contain letters and spaces");

            RuleFor(x => x.University)
                .NotEmpty().WithMessage("University name is required")
                .Length(3, 200).WithMessage("University name must be between 3 and 200 characters");

            RuleFor(x => x.Level)
                .InclusiveBetween(1, 7).WithMessage("Level must be between 1 and 7");
        }
    }
}
