using FluentValidation;
using DentalHub.Application.Commands.PatientCase;

namespace DentalHub.Application.Validators.PatientCase
{
    public class UpdatePatientCaseCommandValidator : AbstractValidator<UpdatePatientCaseCommand>
    {
        public UpdatePatientCaseCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Case public ID is required");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required");
        }
    }
}
