using FluentValidation;
using DentalHub.Application.Commands.Sessions;

namespace DentalHub.Application.Validators.Sessions
{
    public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
    {
        public CreateSessionCommandValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student public ID is required");

            RuleFor(x => x.PatientCaseId)
                .NotEmpty().WithMessage("Case public ID is required");

            RuleFor(x => x.SessionDate)
                .NotEmpty().WithMessage("Scheduled date is required")
                .GreaterThan(DateTime.Now).WithMessage("Scheduled date must be in the future");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required")
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");
        }
    }
}
