using FluentValidation;
using DentalHub.Application.Commands.PatientCase;

namespace DentalHub.Application.Validators.PatientCase
{
    public class CreatePatientCaseCommandValidator : AbstractValidator<CreatePatientCaseCommand>
    {
        public CreatePatientCaseCommandValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient public ID is required");

            RuleFor(x => x.CaseTypeId)
                .NotEmpty().WithMessage("Case type public ID is required");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .Length(10, 1000).WithMessage("Description must be between 10 and 1000 characters");
        }
    }
}
