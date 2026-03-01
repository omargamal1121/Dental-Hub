using FluentValidation;
using DentalHub.Application.Commands.CaseTypes;

namespace DentalHub.Application.Validators.CaseTypes
{
    public class CreateCaseTypeCommandValidator : AbstractValidator<CreateCaseTypeCommand>
    {
        public CreateCaseTypeCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }

    public class UpdateCaseTypeCommandValidator : AbstractValidator<UpdateCaseTypeCommand>
    {
        public UpdateCaseTypeCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID is required");

            RuleFor(x => x.Name)
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Name)).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Description)).WithMessage("Description cannot exceed 500 characters");
        }
    }
}
