using FluentValidation;
using DentalHub.Application.Commands.CaseRequests;

namespace DentalHub.Application.Validators.CaseRequests
{
    public class CreateCaseRequestCommandValidator : AbstractValidator<CreateCaseRequestCommand>
    {
        public CreateCaseRequestCommandValidator()
        {
            RuleFor(x => x.PatientCasePublicId)
                .NotEmpty().WithMessage("Patient case public ID is required");

            RuleFor(x => x.StudentPublicId)
                .NotEmpty().WithMessage("Student public ID is required");

            RuleFor(x => x.DoctorPublicId)
                .NotEmpty().WithMessage("Doctor public ID is required");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .Length(10, 1000).WithMessage("Description must be between 10 and 1000 characters");
        }
    }
}
